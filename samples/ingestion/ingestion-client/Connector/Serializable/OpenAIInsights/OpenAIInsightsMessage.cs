// <copyright file="OpenAIInsightsMessage.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
// </copyright>

namespace Connector
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class OpenAIInsightsMessage
    {
        public OpenAIInsightsMessage()
        {
            this.UserDefinedPromptsResponses = new Dictionary<string, string>();
        }

        [JsonProperty("summary")]
        public string Summary { get; set; }

        [JsonProperty("topics")]
        public List<string> Topics { get; private set; }

        [JsonProperty("keyphrases")]
        public List<string> KeyPhrases { get; private set; }

        [JsonProperty("Companies and Organizations")]
        public List<string> Companies { get; private set; }

        [JsonProperty("People & titles")]
        public List<string> People { get; private set; }

        [JsonProperty("sentiment")]
        public string Sentiment { get; set; }

        [JsonProperty("category")]
        public string Category { get; set; }

        [JsonProperty("userDefinedPromptResponses")]
        public Dictionary<string, string> UserDefinedPromptsResponses { get; }
    }
}
