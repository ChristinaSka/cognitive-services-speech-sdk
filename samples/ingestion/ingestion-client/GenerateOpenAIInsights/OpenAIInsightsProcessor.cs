// <copyright file="OpenAIInsightsProcessor.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
// </copyright>

namespace GenerateOpenAIInsightsFunction
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;

    using Connector;
    using Connector.Serializable.OpenAIInsights;

    using Microsoft.Extensions.Logging;

    using Newtonsoft.Json;

    using Polly;

    public class OpenAIInsightsProcessor
    {
        private static readonly double OpenAICharactersPerToken = 3.5;

        private static readonly StorageConnector StorageConnectorInstance = new StorageConnector(GenerateOpenAIInsightsEnvironmentVariables.AzureWebJobsStorage);

        private readonly IServiceProvider serviceProvider;

        public OpenAIInsightsProcessor(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public async Task GetOpenAIInsightsAsync(string transcriptionFileUrl, ILogger log)
        {
            if (string.IsNullOrEmpty(transcriptionFileUrl))
            {
                throw new ArgumentNullException(nameof(transcriptionFileUrl));
            }

            // Step 01 - Read the file, extract the transcription text, and deserialize
            var blobSas = StorageConnectorInstance.CreateSas(new Uri(transcriptionFileUrl));

            var transcriptionBytes = await StorageConnector.DownloadFileFromSAS(blobSas).ConfigureAwait(false);
            log.LogInformation($"Read transcript, len = {transcriptionBytes.Length}");

            var utfString = Encoding.UTF8.GetString(transcriptionBytes, 0, transcriptionBytes.Length);
            var transcription = JsonConvert.DeserializeObject<SpeechTranscript>(utfString);

            // Step 02 - Extract the relevant data to translate, in "dialogue" format with alternating speakers. This assumes text is being captured in the
            // right way.
            var text = ExtractDialogueStrings(transcription);

            // Step 03 - If the text is not in english, it needs to be translated, as OpenAI may underperform with other languages
            var localePrefix = GenerateOpenAIInsightsEnvironmentVariables.Locale.Split("-")[0];

            TranslationConnector translationConnector = null;
            if (localePrefix != "en")
            {
                translationConnector = new TranslationConnector(GenerateOpenAIInsightsEnvironmentVariables.AzureSpeechServicesKey, GenerateOpenAIInsightsEnvironmentVariables.AzureSpeechServicesRegion, log);
                var translatedText = await translationConnector.Translate(text, localePrefix, "en").ConfigureAwait(false);
                text = translatedText;
            }

            // Step 04 - Build a string with the pre-defined prompts depending on the flags in the configuration
            var predefinedPrompts = BuildOpenAIDefaultPrompts();

            // Step 05 - Do the Azure OpenAI Call, both for the predefined prompts and optional user-defined prompts
            OpenAIInsightsMessage openAIResponses = null;
            try
            {
                openAIResponses = await CallAzureOpenAI<OpenAIInsightsMessage>(text, predefinedPrompts, GenerateOpenAIInsightsEnvironmentVariables.AzureOpenAIDefaultDeploymentName, log);

                if (localePrefix != "en")
                {
                    await TranslateOpenAIDefaultResponsesToOriginalTextLanguage(log, localePrefix, translationConnector, openAIResponses).ConfigureAwait(false);
                }

                // Step 06 - Do the Azure OpenAI Call for the user-defined prompts
                // one request per user-defined prompt. Each UDP can have its own deployment name, and we can't just group them by deployment name
                // because we don't know what the user will configure. Ex -- will json format be asked for?
                var udprompts = new OpenAIUserDefinedPrompts(GenerateOpenAIInsightsEnvironmentVariables.AzureOpenAIUserDefinedPrompts);
                foreach (var prompt in udprompts.UserDefinedPrompts)
                {
                    var response = await CallAzureOpenAI<string>(text, prompt.Prompt, prompt.DeploymentName, log);

                    if (localePrefix != "en")
                    {
                        var translatedText = await translationConnector.Translate(response, "en", localePrefix).ConfigureAwait(false);
                        response = translatedText;
                    }

                    openAIResponses.UserDefinedPromptsResponses.Add(prompt.FieldName, response);
                }

                // Step 07 - Write out the combined file with all the responses
                var filename = Path.GetFileNameWithoutExtension(new Uri(transcriptionFileUrl).AbsolutePath);
                filename = filename + ".openai.json";

                await StorageConnectorInstance.WriteTextFileToBlobAsync(JsonConvert.SerializeObject(openAIResponses), GenerateOpenAIInsightsEnvironmentVariables.OutputContainer, filename, log);

                log.LogInformation($"Wrote output to file {filename}");
            }
            catch (Exception ex)
            {
                log.LogError("Exception while doing Azure OpenAI call (retries can have been exceeded, configuration can be wrong, etc.): " + ex.Message, ex);
            }
        }

        private static async Task TranslateOpenAIDefaultResponsesToOriginalTextLanguage(ILogger log, string localePrefix, TranslationConnector translationConnector, OpenAIInsightsMessage openAIResponses)
        {
            openAIResponses.Summary = await translationConnector.Translate(openAIResponses.Summary, "en", localePrefix).ConfigureAwait(false);
            openAIResponses.Sentiment = await translationConnector.Translate(openAIResponses.Sentiment, "en", localePrefix).ConfigureAwait(false);
            openAIResponses.Category = await translationConnector.Translate(openAIResponses.Category, "en", localePrefix).ConfigureAwait(false);

            // translate topics
            var translatedTopics = new List<string>();
            foreach (var original in openAIResponses.Topics)
            {
                var translated = await translationConnector.Translate(original, "en", localePrefix).ConfigureAwait(false);
                translatedTopics.Add(translated);
            }

            openAIResponses.Topics = translatedTopics;

            // translate key phrases
            var translatedKeyPhrases = new List<string>();
            foreach (var original in openAIResponses.KeyPhrases)
            {
                var translated = await translationConnector.Translate(original, "en", localePrefix).ConfigureAwait(false);
                translatedKeyPhrases.Add(translated);
            }

            openAIResponses.KeyPhrases = translatedKeyPhrases;

            // translate company names (this can risky -- does the translation translated their names originally?)
            var companies = new List<string>();
            foreach (var original in openAIResponses.Companies)
            {
                var translated = await translationConnector.Translate(original, "en", localePrefix).ConfigureAwait(false);
                translatedKeyPhrases.Add(translated);
            }

            openAIResponses.Companies = companies;

            // translate company names (this can risky -- does the translation translated their names originally?)
            var peopleAndTitles = new List<string>();
            foreach (var original in openAIResponses.People)
            {
                var translated = await translationConnector.Translate(original, "en", localePrefix).ConfigureAwait(false);
                translatedKeyPhrases.Add(translated);
            }

            openAIResponses.People = peopleAndTitles;
        }

        /// <summary>
        /// Convert text into a dialogue format
        /// TODO -- needs replicating logic and configurations from ingestion\ingestion-client\FetchTranscription\Language\AnalyzeConversationsProvider.cs
        /// Implies refactoring of said code
        /// </summary>
        /// <param name="transcription"></param>
        /// <returns>Transcription in dialogue format</returns>
        private static string ExtractDialogueStrings(SpeechTranscript transcription)
        {
            // simplistic implementation
            var text = transcription.CombinedRecognizedPhrases.First().Display; // lexical has no punctuation or initials
            return text;
        }

        /// <summary>
        /// Estimate the number of tokens in a string of text.
        /// Important note: on average, for english, each token is 4-5 characters long. As there doesn't seem to be reliable libraries
        /// to count tokens in c#, this method does a conservative estimate.
        /// </summary>
        /// <param name="text">English language text</param>
        /// <returns>Estimated number of OpenAI tokens in a string</returns>
        private static int CountTokens(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return 0;
            }

            return (int)(text.Length / OpenAICharactersPerToken);
        }

        private static string TrimToAvailableTokens(string text, int maxTokens)
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }

            if (CountTokens(text) > maxTokens)
            {
                return text.Substring(0, (int)(maxTokens * 3.5));
            }

            return text;
        }

        private static string BuildOpenAIDefaultPrompts()
        {
            var questions = new StringBuilder();
            var jsonFormatSpec = new StringBuilder();

            questions.AppendLine("Extract using the above Conversation only the following information:");
            jsonFormatSpec.AppendLine("In the following JSON format:").AppendLine("{");

            var questionCount = 1;

            if (GenerateOpenAIInsightsEnvironmentVariables.AddAzureOpenAISummarization)
            {
                questions.Append(questionCount.ToString(CultureInfo.CurrentCulture)).AppendLine(". The summary of the Conversation;");
                questionCount++;
                jsonFormatSpec.AppendLine("\"summary\": \"Summary of the Conversation\",");
            }

            if (GenerateOpenAIInsightsEnvironmentVariables.AddAzureOpenAIKeywordsExtraction)
            {
                questions.Append(questionCount.ToString(CultureInfo.CurrentCulture)).AppendLine(". The topics of the Conversation;");
                questionCount++;
                jsonFormatSpec.AppendLine("\"topics\": [ <comma-separated list of up to 5 main topics mentioned> ],");
            }

            if (GenerateOpenAIInsightsEnvironmentVariables.AddAzureOpenAIKeyPhraseExtraction)
            {
                questions.Append(questionCount.ToString(CultureInfo.CurrentCulture)).AppendLine(". The list of keyphrases of the Conversation;");
                questionCount++;

                jsonFormatSpec.AppendLine("\"keyphrases\": [ <comma-separated list of key phrases mentioned> ],");
            }

            if (GenerateOpenAIInsightsEnvironmentVariables.AddAzureOpenAIEntityExtraction)
            {
                questions.Append(questionCount.ToString(CultureInfo.CurrentCulture)).AppendLine(". All the Companies, People and their titles (Entities) mentioned in the Conversation;");
                questionCount++;

                jsonFormatSpec.AppendLine("\"Companies and Organizations\": [ <comma-separated list of companies and organizations mentioned> ],");
                jsonFormatSpec.AppendLine("\"People & titles\": [ <comma-separated list of people mentioned (with their titles or roles appended in parentheses)> ],");
            }

            if (GenerateOpenAIInsightsEnvironmentVariables.AddAzureOpenAISentimentAnalysis)
            {
                questions.Append(questionCount.ToString(CultureInfo.CurrentCulture)).AppendLine(". Classify the Sentiment of the Conversation as Positive, Neutral or Negative;");
                questionCount++;
                jsonFormatSpec.AppendLine("\"sentiment\": \"Sentiment\",");
            }

            if (GenerateOpenAIInsightsEnvironmentVariables.AddAzureOpenAIClassification)
            {
                questions.Append(questionCount.ToString(CultureInfo.CurrentCulture)).AppendLine(". Classify the Conversation into the following categories : [Health, Insurance, Finance, Business, Technology, Agriculture, Mining, Pharmaceutical, Retail, Transportation].");
                questionCount++;
                jsonFormatSpec.AppendLine("\"category\": \"Category\",");
            }

            if (questionCount == 1)
            {
                return string.Empty; // all the flags were false, so there's no prompts
            }

            // final tweak -- remove extra comma from the last json example
            jsonFormatSpec.Remove(jsonFormatSpec.Length - 3, 1);
            jsonFormatSpec.AppendLine("}");

            return questions.AppendLine(jsonFormatSpec.ToString()).ToString();
        }

        /// <summary>
        /// Make an API call to AzureOpenAI's service and return the literal un-deserialized response for upstream parsing.
        /// </summary>
        /// <param name="text">The context, the text on which the prompt will act upon</param>
        /// <param name="prompt">The prompt</param>
        /// <param name="modelDeploymentName">The name of the Azure OpenAI deployment name</param>
        /// <param name="log">Instance of logger</param>
        /// <returns>Response from OpenAI</returns>
        private static async Task<TOpenAIPromptResponse> CallAzureOpenAI<TOpenAIPromptResponse>(string text, string prompt, string modelDeploymentName, ILogger log)
        {
            // The code below should use https://github.com/Azure/azure-sdk-for-net/tree/main/sdk/openai/Azure.AI.OpenAI but this is not yet ready for use.
            var apiVersion = "2022-12-01";
            var url = $"https://{GenerateOpenAIInsightsEnvironmentVariables.AzureOpenAIResourceName}.openai.azure.com/openai/deployments/{modelDeploymentName}/completions?api-version={apiVersion}";

            // check if request is too long, token-wise. Assuming 750 tokens left for the response
            // TO-DO: if modelDeploymentName is for a non-DaVinci model, then the max tokens will be different. This will imply asking OpenAI what the
            // model is, and then using the appropriate max tokens.
            if (CountTokens(text + Environment.NewLine + Environment.NewLine + prompt) > 4096 - 750)
            {
                var availableTokensForText = 4096 - CountTokens(prompt) - 750;

                log.LogWarning($"OpenAI text is too long for DaVinci 03, token-wise. Trimming to {availableTokensForText * OpenAICharactersPerToken} chars.");

                text = TrimToAvailableTokens(text, availableTokensForText);
            }

            // create http request body
            object body = new
            {
                prompt = text + Environment.NewLine + Environment.NewLine + prompt,
                max_tokens = 750, // The maximum number of tokens to generate in the completion (arbitrary value)
                temperature = 0
            };

            // IMPORTANT: THIS LOG HAS TO BE LEFT COMMENTED AS IT PRINTS OUT POTENTIALLY CONFIDENTIAL INFORMATION 
            // log.LogInformation(text + Environment.NewLine + Environment.NewLine + prompt);
            var requestBody = JsonConvert.SerializeObject(body);

            using (var client = new HttpClient())
            {
                var pollyContext = new Context("Retry Transient Errors");
                var retryPolicy = Policy
                    .HandleResult<HttpResponseMessage>(r => r.StatusCode == HttpStatusCode.TooManyRequests)
                    .OrResult(r => r.StatusCode == HttpStatusCode.GatewayTimeout)
                    .OrResult(r => r.StatusCode == HttpStatusCode.RequestTimeout)
                    .WaitAndRetryAsync(
                        new[]
                        {
                            TimeSpan.FromMilliseconds(100),
                            TimeSpan.FromMilliseconds(250),
                            TimeSpan.FromMilliseconds(500),
                            TimeSpan.FromMilliseconds(1000),
                            TimeSpan.FromMilliseconds(5000)
                        },
                        (result, timespan, retryNo, context) =>
                        {
                            log.LogWarning($"Retrying call to Azure OpenAI. Retry number {retryNo} within {timespan.TotalMilliseconds}ms. Status Code: {result.Result.StatusCode} - {result.Result.ReasonPhrase}");
                        });

                var response = await retryPolicy.ExecuteAsync(
                    async ctx =>
                {
                    // HttpRequestMessages can't be reused as per documentation
                    var request = new HttpRequestMessage
                    {
                        Method = HttpMethod.Post,
                        RequestUri = new Uri(url),
                        Content = new StringContent(requestBody, Encoding.UTF8, "application/json"),
                    };

                    request.Headers.Add("api-key", GenerateOpenAIInsightsEnvironmentVariables.AzureOpenAIServiceKey);

                    // Make the HTTP request to Azure OpenAI
                    var response = await client.SendAsync(request).ConfigureAwait(false);

                    return response;
                }, pollyContext);

                // And read response as a string (containing Json)
                var completions = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                // if all the retries fail, the next line will ensure an exception is generated.
                // The same happens if there's an applicational error
                try
                {
                    // do this just to be able to log the exact response from OpenAI (there has to be a better way)
                    response.EnsureSuccessStatusCode();
                }
                catch (Exception)
                {
                    if (completions == null)
                    {
                        completions = "(completions is null)";
                    }

                    log.LogError("Error response from OpenAI Http call: " + completions);
                    throw; // rethrow
                }

                log.LogInformation(completions);

                var successResponse = JsonConvert.DeserializeObject<OpenAISuccessResponse>(completions);

                var openAiResponseText = successResponse.Choices[0].Text; // this has the response from the completions API

                // if we got here, it means that Azure OpenAI did not return an applicational error, so let's continue processing
                if (typeof(TOpenAIPromptResponse) != typeof(string))
                {
                    return JsonConvert.DeserializeObject<TOpenAIPromptResponse>(openAiResponseText);
                }
                else
                {
                    return (TOpenAIPromptResponse)Convert.ChangeType(openAiResponseText.Trim(), typeof(TOpenAIPromptResponse));
                }
            }
        }
    }
}