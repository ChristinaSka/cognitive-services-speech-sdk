// <copyright file="TranslationConnector.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
// </copyright>

namespace GenerateOpenAIInsightsFunction
{
    using System;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Using multi-service resource. More informatipon here:
    /// https://learn.microsoft.com/en-us/azure/cognitive-services/translator/reference/v3-0-reference#base-urls
    /// </summary>
    public class TranslationConnector
    {
        private string endpoint = "https://api.cognitive.microsofttranslator.com";
        private string multiServiceCognitiveSubscriptionKey;
        private string multiServiceCognitiveSubscriptionRegion;
        private ILogger log;

        public TranslationConnector(string serviceApiKey, string location, ILogger log)
        {
            this.multiServiceCognitiveSubscriptionKey = serviceApiKey;
            this.multiServiceCognitiveSubscriptionRegion = location;
            this.log = log;
        }

        public async Task<string> Translate(string textToTranslate, string sourceLanguage, string targetLanguage = "en")
        {
            // ignore requests to translate empty text
            if (string.IsNullOrWhiteSpace(textToTranslate.Trim()))
            {
                return string.Empty;
            }

            // Input and output languages are defined as parameters.
            var route = $"/translate?api-version=3.0&from={sourceLanguage}&to={targetLanguage}";
            var body = new object[] { new { Text = textToTranslate } };

            var requestBody = JsonConvert.SerializeObject(body);

            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage())
            {
                // Build the request.
                request.Method = HttpMethod.Post;
                request.RequestUri = new Uri(this.endpoint + route);
                request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                request.Headers.Add("Ocp-Apim-Subscription-Key", this.multiServiceCognitiveSubscriptionKey);

                // location required if you're using a multi-service or regional (not global) resource.
                request.Headers.Add("Ocp-Apim-Subscription-Region", this.multiServiceCognitiveSubscriptionRegion);
                request.Headers.Add("X-ClientTraceId", Guid.NewGuid().ToString());

                // Send the request
                var response = await client.SendAsync(request).ConfigureAwait(false);

                // And read response as a string (containn Json)
                var translatedTextJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                // needs error handling. Not pretty but avoids creating a class just to get a string out
                var json = (JArray)JsonConvert.DeserializeObject(translatedTextJson);
                return json[0]["translations"][0]["text"].Value<string>();
            }
        }
    }
}
