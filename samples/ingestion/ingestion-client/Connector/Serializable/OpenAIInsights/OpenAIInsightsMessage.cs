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
        public string Classification { get; set; }

        public string Entities { get; set; }

        public string KeyPhrases { get; set; }

        public string Keywords { get; set; }

        public string Sentiment { get; set; }

        public string Summary { get; set; }

        public Dictionary<string, string> UserDefinedPrompts { get; }

        // public static OpenAIInsightsMessage DeserializeMessage(string serviceBusMessage)
        // {
        //    if (serviceBusMessage == null)
        //    {
        //        throw new ArgumentNullException(nameof(serviceBusMessage));
        //    }

        // return JsonConvert.DeserializeObject<OpenAIInsightsMessage>(serviceBusMessage);
        // }

        // public string CreateMessageString()
        // {
        //    return JsonConvert.SerializeObject(this);
        // }
    }
}
