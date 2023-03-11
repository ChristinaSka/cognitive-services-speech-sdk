// <copyright file="GenerateOpenAIInsightsEnvironmentVariables.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
// </copyright>

namespace GenerateOpenAIInsightsFunction
{
    using System;
    using Connector;
    using Connector.Constants;
    using Connector.Enums;
    using Connector.Serializable.Language.Conversations;

    public static class GenerateOpenAIInsightsEnvironmentVariables
    {
        // public static readonly StorageAccount = Environment.GetEnvironmentVariable(nameof(AzureWebJobsStorage), EnvironmentVariableTarget.Process);
        public static readonly string Locale = Environment.GetEnvironmentVariable(nameof(Locale), EnvironmentVariableTarget.Process);

        public static readonly bool AddAzureOpenAIClassification = bool.TryParse(Environment.GetEnvironmentVariable(nameof(AddAzureOpenAIClassification), EnvironmentVariableTarget.Process), out AddAzureOpenAIClassification) && AddAzureOpenAIClassification;

        public static readonly bool AddAzureOpenAIEntityExtraction = bool.TryParse(Environment.GetEnvironmentVariable(nameof(AddAzureOpenAIEntityExtraction), EnvironmentVariableTarget.Process), out AddAzureOpenAIEntityExtraction) && AddAzureOpenAIEntityExtraction;

        public static readonly bool AddAzureOpenAIKeyPhraseExtraction = bool.TryParse(Environment.GetEnvironmentVariable(nameof(AddAzureOpenAIKeyPhraseExtraction), EnvironmentVariableTarget.Process), out AddAzureOpenAIKeyPhraseExtraction) && AddAzureOpenAIKeyPhraseExtraction;

        public static readonly bool AddAzureOpenAIKeywordsExtraction = bool.TryParse(Environment.GetEnvironmentVariable(nameof(AddAzureOpenAIKeywordsExtraction), EnvironmentVariableTarget.Process), out AddAzureOpenAIKeywordsExtraction) && AddAzureOpenAIKeywordsExtraction;

        public static readonly bool AddAzureOpenAISentimentAnalysis = bool.TryParse(Environment.GetEnvironmentVariable(nameof(AddAzureOpenAISentimentAnalysis), EnvironmentVariableTarget.Process), out AddAzureOpenAISentimentAnalysis) && AddAzureOpenAISentimentAnalysis;

        public static readonly bool AddAzureOpenAISummarization = bool.TryParse(Environment.GetEnvironmentVariable(nameof(AddAzureOpenAISummarization), EnvironmentVariableTarget.Process), out AddAzureOpenAISummarization) && AddAzureOpenAISummarization;

        public static readonly string AzureOpenaAIOutputContainer = Environment.GetEnvironmentVariable(nameof(AzureOpenaAIOutputContainer), EnvironmentVariableTarget.Process);

        public static readonly string AzureOpenAIServiceKey = Environment.GetEnvironmentVariable(nameof(AzureOpenAIServiceKey), EnvironmentVariableTarget.Process);

        public static readonly string AzureOpenAIDefaultDeploymentName = Environment.GetEnvironmentVariable(nameof(AzureOpenAIDefaultDeploymentName), EnvironmentVariableTarget.Process);

        public static readonly string AzureOpenaAIResourceName = Environment.GetEnvironmentVariable(nameof(AzureOpenaAIResourceName), EnvironmentVariableTarget.Process);

        public static readonly string AzureOpenAIUserDefinedPrompts = Environment.GetEnvironmentVariable(nameof(AzureOpenAIUserDefinedPrompts), EnvironmentVariableTarget.Process);

        /* remove following

        public static readonly int InitialPollingDelayInMinutes = int.TryParse(Environment.GetEnvironmentVariable(nameof(InitialPollingDelayInMinutes), EnvironmentVariableTarget.Process), out InitialPollingDelayInMinutes) ? InitialPollingDelayInMinutes.ClampInt(2, Constants.MaxInitialPollingDelayInMinutes) : Constants.DefaultInitialPollingDelayInMinutes;

        public static readonly int MaxPollingDelayInMinutes = int.TryParse(Environment.GetEnvironmentVariable(nameof(MaxPollingDelayInMinutes), EnvironmentVariableTarget.Process), out MaxPollingDelayInMinutes) ? MaxPollingDelayInMinutes : Constants.DefaultMaxPollingDelayInMinutes;

        public static readonly int RetryLimit = int.TryParse(Environment.GetEnvironmentVariable(nameof(RetryLimit), EnvironmentVariableTarget.Process), out RetryLimit) ? RetryLimit.ClampInt(1, Constants.MaxRetryLimit) : Constants.DefaultRetryLimit;

        public static readonly string AudioInputContainer = Environment.GetEnvironmentVariable(nameof(AudioInputContainer), EnvironmentVariableTarget.Process);

        public static readonly string AzureSpeechServicesKey = Environment.GetEnvironmentVariable(nameof(AzureSpeechServicesKey), EnvironmentVariableTarget.Process);

        public static readonly string AzureWebJobsStorage = Environment.GetEnvironmentVariable(nameof(AzureWebJobsStorage), EnvironmentVariableTarget.Process);

        public static readonly string DatabaseConnectionString = Environment.GetEnvironmentVariable(nameof(DatabaseConnectionString), EnvironmentVariableTarget.Process);

        public static readonly string ErrorFilesOutputContainer = Environment.GetEnvironmentVariable(nameof(ErrorFilesOutputContainer), EnvironmentVariableTarget.Process);

        public static readonly string ErrorReportOutputContainer = Environment.GetEnvironmentVariable(nameof(ErrorReportOutputContainer), EnvironmentVariableTarget.Process);

        public static readonly string FetchTranscriptionServiceBusConnectionString = Environment.GetEnvironmentVariable(nameof(FetchTranscriptionServiceBusConnectionString), EnvironmentVariableTarget.Process);

        public static readonly string HtmlResultOutputContainer = Environment.GetEnvironmentVariable(nameof(HtmlResultOutputContainer), EnvironmentVariableTarget.Process);

        public static readonly string JsonResultOutputContainer = Environment.GetEnvironmentVariable(nameof(JsonResultOutputContainer), EnvironmentVariableTarget.Process);

        public static readonly string StartTranscriptionServiceBusConnectionString = Environment.GetEnvironmentVariable(nameof(StartTranscriptionServiceBusConnectionString), EnvironmentVariableTarget.Process);

        public static readonly bool CreateConsolidatedOutputFiles = bool.TryParse(Environment.GetEnvironmentVariable(nameof(CreateConsolidatedOutputFiles), EnvironmentVariableTarget.Process), out CreateConsolidatedOutputFiles) && CreateConsolidatedOutputFiles;

        public static readonly string ConsolidatedFilesOutputContainer = Environment.GetEnvironmentVariable(nameof(ConsolidatedFilesOutputContainer), EnvironmentVariableTarget.Process);

        public static readonly bool CreateAudioProcessedContainer = bool.TryParse(Environment.GetEnvironmentVariable(nameof(CreateAudioProcessedContainer), EnvironmentVariableTarget.Process), out CreateAudioProcessedContainer) && CreateAudioProcessedContainer;

        public static readonly string AudioProcessedContainer = Environment.GetEnvironmentVariable(nameof(AudioProcessedContainer), EnvironmentVariableTarget.Process);
        */
    }
}
