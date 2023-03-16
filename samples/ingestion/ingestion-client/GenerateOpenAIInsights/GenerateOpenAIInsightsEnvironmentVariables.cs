// <copyright file="GenerateOpenAIInsightsEnvironmentVariables.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
// </copyright>

namespace GenerateOpenAIInsightsFunction
{
    using System;

    public static class GenerateOpenAIInsightsEnvironmentVariables
    {
        public static readonly string Locale = Environment.GetEnvironmentVariable(nameof(Locale), EnvironmentVariableTarget.Process);

        public static readonly bool AddAzureOpenAIClassification = bool.TryParse(Environment.GetEnvironmentVariable(nameof(AddAzureOpenAIClassification), EnvironmentVariableTarget.Process), out AddAzureOpenAIClassification) && AddAzureOpenAIClassification;

        public static readonly bool AddAzureOpenAIEntityExtraction = bool.TryParse(Environment.GetEnvironmentVariable(nameof(AddAzureOpenAIEntityExtraction), EnvironmentVariableTarget.Process), out AddAzureOpenAIEntityExtraction) && AddAzureOpenAIEntityExtraction;

        public static readonly bool AddAzureOpenAIKeyPhraseExtraction = bool.TryParse(Environment.GetEnvironmentVariable(nameof(AddAzureOpenAIKeyPhraseExtraction), EnvironmentVariableTarget.Process), out AddAzureOpenAIKeyPhraseExtraction) && AddAzureOpenAIKeyPhraseExtraction;

        public static readonly bool AddAzureOpenAIKeywordsExtraction = bool.TryParse(Environment.GetEnvironmentVariable(nameof(AddAzureOpenAIKeywordsExtraction), EnvironmentVariableTarget.Process), out AddAzureOpenAIKeywordsExtraction) && AddAzureOpenAIKeywordsExtraction;

        public static readonly bool AddAzureOpenAISentimentAnalysis = bool.TryParse(Environment.GetEnvironmentVariable(nameof(AddAzureOpenAISentimentAnalysis), EnvironmentVariableTarget.Process), out AddAzureOpenAISentimentAnalysis) && AddAzureOpenAISentimentAnalysis;

        public static readonly bool AddAzureOpenAISummarization = bool.TryParse(Environment.GetEnvironmentVariable(nameof(AddAzureOpenAISummarization), EnvironmentVariableTarget.Process), out AddAzureOpenAISummarization) && AddAzureOpenAISummarization;

        public static readonly string AzureOpenAIOutputContainer = Environment.GetEnvironmentVariable(nameof(AzureOpenAIOutputContainer), EnvironmentVariableTarget.Process);

        public static readonly string AzureOpenAIServiceKey = Environment.GetEnvironmentVariable(nameof(AzureOpenAIServiceKey), EnvironmentVariableTarget.Process);

        public static readonly string AzureOpenAIDefaultDeploymentName = Environment.GetEnvironmentVariable(nameof(AzureOpenAIDefaultDeploymentName), EnvironmentVariableTarget.Process);

        public static readonly string AzureOpenAIResourceName = Environment.GetEnvironmentVariable(nameof(AzureOpenAIResourceName), EnvironmentVariableTarget.Process);

        public static readonly string AzureOpenAIUserDefinedPrompts = Environment.GetEnvironmentVariable(nameof(AzureOpenAIUserDefinedPrompts), EnvironmentVariableTarget.Process);

        public static readonly string AzureWebJobsStorage = Environment.GetEnvironmentVariable(nameof(AzureWebJobsStorage), EnvironmentVariableTarget.Process);

        public static readonly string OutputContainer = Environment.GetEnvironmentVariable(nameof(OutputContainer), EnvironmentVariableTarget.Process);

        public static readonly string AzureSpeechServicesKey = Environment.GetEnvironmentVariable(nameof(AzureSpeechServicesKey), EnvironmentVariableTarget.Process);

        public static readonly string AzureSpeechServicesRegion = Environment.GetEnvironmentVariable(nameof(AzureSpeechServicesRegion), EnvironmentVariableTarget.Process);
    }
}
