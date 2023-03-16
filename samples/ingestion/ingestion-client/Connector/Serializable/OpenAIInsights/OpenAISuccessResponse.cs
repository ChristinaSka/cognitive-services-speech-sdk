// <copyright file="OpenAISuccessResponse.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
// </copyright>

namespace Connector.Serializable.OpenAIInsights
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class OpenAISuccessResponse
    {
            [JsonProperty("id")]
            public string Id { get; set; }

            [JsonProperty("object")]
            public string Objective { get; set; }

            [JsonProperty("created")]
            public int Created { get; set; }

            [JsonProperty("model")]
            public string Model { get; set; }

            [JsonProperty("choices")]
            public List<Choice> Choices { get; private set; }

            [JsonProperty("usage")]
            public Usage TokenUsage { get; set; }
    }
}
