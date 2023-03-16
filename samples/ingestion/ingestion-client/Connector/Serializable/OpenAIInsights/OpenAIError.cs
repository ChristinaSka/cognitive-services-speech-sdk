// <copyright file="OpenAIError.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
// </copyright>

namespace Connector.Serializable.OpenAIInsights
{
    using Newtonsoft.Json;

    public class OpenAIError
    {
        // usually repeats the input prompt or has a specific message, eg: This model's maximum context length
        // is 4097 tokens, however you requested 4789 tokens (4039 in your prompt; 750 for the completion).
        // Please reduce your prompt; or completion length.",
        [JsonProperty("message")]
        public string Message { get; set; }

        // eg - invalid_request_error
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("param")]
        public object Param { get; set; }

        [JsonProperty("code")]
        public object Code { get; set; }
    }
}
