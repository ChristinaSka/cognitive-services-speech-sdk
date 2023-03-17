// <copyright file="OpenAIUserDefinedPrompt.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
// </copyright>

namespace Connector.Serializable.OpenAIInsights
{
    public class OpenAIUserDefinedPrompt
    {
        public string FieldName { get; set; }

        public string DeploymentName { get; set; }

        public string Prompt { get; set; }
    }
}