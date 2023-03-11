// <copyright file="OpenAIUserDefinedPrompts.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
// </copyright>

namespace Connector.Serializable.OpenAIInsights
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class OpenAIUserDefinedPrompts
    {
        public OpenAIUserDefinedPrompts(string configuration)
        {
            this.UserDefinedPrompts = ParseConfiguraton(configuration);
        }

        public IEnumerable<OpenAIUserDefinedPrompt> UserDefinedPrompts { get; set; }

        /// <summary>
        /// Read triplets of values in the format [['fieldname1', 'model deployment name1', 'custom prompt1'], ['fieldname2', 'model deployment name2', 'custom prompt2']] and add to a list
        /// </summary>
        /// <param name="configuration">A string read from the configuration</param>
        /// <returns>Collection of custom Azure OpenAI prompts, including an a fieldname, the model deployment name, and the custom prompt.</returns>
        private static IEnumerable<OpenAIUserDefinedPrompt> ParseConfiguraton(string configuration)
        {
            var prompts = new List<OpenAIUserDefinedPrompt>();

            if (!string.IsNullOrEmpty(configuration))
            {
                configuration = configuration.Trim();

                List<List<string>> customPrompts = null;

                try
                {
                    customPrompts = JsonConvert.DeserializeObject<List<List<string>>>(configuration);
                }
                catch (JsonException je)
                {
                    throw new OpenAICustomPromptsConfigurationException("OpenAI custom prompt configuration contains invalid Json. Use a list of lists, for example [['fieldname1', 'model deployment name1', 'custom prompt1']].", je);
                }

                foreach (List<string> customPrompt in customPrompts)
                {
                    if (customPrompt.Count != 3)
                    {
                        throw new OpenAICustomPromptsConfigurationException("OpenAI custom prompt configuration contains an invalid configuration. Use a list of lists, for example [['fieldname1', 'model deployment name1', 'custom prompt1']].");
                    }

                    OpenAIUserDefinedPrompt prompt = new OpenAIUserDefinedPrompt
                    {
                        FieldName = customPrompt[0],
                        DeploymentName = customPrompt[1],
                        Prompt = customPrompt[2]
                    };

                    prompts.Add(prompt);
                }
            }

            return prompts;
        }
    }
}
