// <copyright file="GenerateOpenAIInsights.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
// </copyright>

namespace GenerateOpenAIInsightsFunction
{
    using System;
    using System.Threading.Tasks;

    using Azure.Messaging.EventGrid;

    using Microsoft.Azure.WebJobs;

    // using Microsoft.Azure.WebJobs.Host;

    // using Microsoft.Azure.EventGrid.Models;

    // using Microsoft.Azure.WebJobs.Extensions.EventGrid;
    using Microsoft.Extensions.Logging;

    public class GenerateOpenAIInsights
    {
        private readonly IServiceProvider serviceProvider;

        public GenerateOpenAIInsights(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        [FunctionName("GenerateOpenAIInsights")]
        public async Task Run([ServiceBusTrigger("openai_transcription_queue", Connection = "AzureServiceBus")] string message, ILogger log)
        {
            if (log == null)
            {
                throw new ArgumentNullException(nameof(log));
            }

            log.LogInformation($"C# Service bus triggered function executed at: {DateTime.Now}");

            if (message == null)
            {
                log.LogInformation($"Found invalid service bus message: {message}. Stopping execution.");
                return;
            }

            var eventGridEvent = EventGridEvent.Parse(new BinaryData(message));
            if (eventGridEvent.EventType != "Microsoft.Storage.BlobCreated")
            {
                log.LogWarning($"Found invalid EventGridType: {eventGridEvent.EventType}. Ignoring.");
                return;
            }

            // "subject": "/blobServices/default/containers/kmoaiprocessed/blobs/New York Brochure.json",
            // data."url": "https://kmoaistorage.blob.core.windows.net/kmoaiprocessed/New York Brochure.json",
            log.LogInformation($"C# ServiceBus queue trigger new blog: {eventGridEvent.Subject}");

            // var transcriptionProcessor = new TranscriptionProcessor(this.serviceProvider);
            var a = this.serviceProvider;

            // await transcriptionProcessor.ProcessTranscriptionJobAsync(serviceBusMessage, this.serviceProvider, log).ConfigureAwait(false);
        }
    }
}
