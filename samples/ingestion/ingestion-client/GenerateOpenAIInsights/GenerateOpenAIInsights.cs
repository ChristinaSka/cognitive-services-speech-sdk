// <copyright file="GenerateOpenAIInsights.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
// </copyright>

namespace GenerateOpenAIInsightsFunction
{
    using System;
    using System.Threading.Tasks;
    using Azure.Messaging.EventGrid;
    using Azure.Messaging.EventGrid.SystemEvents;
    using Microsoft.Azure.WebJobs;
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
            log.LogInformation($"C# Service bus triggered function executed at: {DateTime.Now}");

            if (log == null)
            {
                throw new ArgumentNullException(nameof(log));
            }

            if (string.IsNullOrEmpty(message))
            {
                log.LogInformation($"Found null/empty service bus message. Stopping execution.");
                return;
            }

            // schema for this event is documented here: https://learn.microsoft.com/en-us/azure/event-grid/event-schema-blob-storage?tabs=event-grid-event-schema
            var eventGridEvent = EventGridEvent.Parse(new BinaryData(message));
            if (eventGridEvent.EventType != "Microsoft.Storage.BlobCreated")
            {
                log.LogWarning($"Found unexpected EventGridType: {eventGridEvent.EventType}. Should be Microsoft.Storage.BlobCreated. Ignoring message.");
                return;
            }

            // extract the URL part from the EventGrid paylog
            var newBlobData = eventGridEvent.Data.ToObjectFromJson<StorageBlobCreatedEventData>();

            // Example: "https://kmoaistorage.blob.core.windows.net/kmoaiprocessed/filename.json",
            log.LogInformation($"C# ServiceBus queue trigger new blob: {newBlobData.Url}");

            var openaiInsightsProcessor = new OpenAIInsightsProcessor(this.serviceProvider);

            await openaiInsightsProcessor.GetOpenAIInsightsAsync(newBlobData.Url, log).ConfigureAwait(false);
        }
    }
}
