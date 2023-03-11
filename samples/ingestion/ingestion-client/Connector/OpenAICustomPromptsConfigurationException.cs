// <copyright file="OpenAICustomPromptsConfigurationException.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
// </copyright>

namespace Connector
{
    using System;

    /// <summary>
    /// Exception thrown when there is an invalid Azure OpenAI custom prompts configuration
    /// </summary>
    public sealed class OpenAICustomPromptsConfigurationException : Exception
    {
        public OpenAICustomPromptsConfigurationException()
        {
        }

        public OpenAICustomPromptsConfigurationException(string message)
            : base(message)
        {
        }

        public OpenAICustomPromptsConfigurationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
