﻿using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace WisdomPetMedicine.Rescue.Api.IntegrationEvents
{
    public class PetFlaggedForAdoptionIntegrationEventHandler : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<PetFlaggedForAdoptionIntegrationEventHandler> _logger;
        private readonly ServiceBusClient _client;
        private readonly ServiceBusProcessor _processor;

        public PetFlaggedForAdoptionIntegrationEventHandler(IConfiguration configuration, ILogger<PetFlaggedForAdoptionIntegrationEventHandler> logger)
        {
            _configuration = configuration;
            _logger = logger;

            _client = new ServiceBusClient(configuration["ServiceBus:ConnectionString"]);
            _processor = _client.CreateProcessor(configuration["ServiceBus:TopicName"], configuration["ServiceBus:SubscriptionName"]);

            _processor.ProcessMessageAsync += Processor_ProcessMessageAsync;
            _processor.ProcessErrorAsync += Processor_ProcessErrorAsync;
        }

        private Task Processor_ProcessErrorAsync(ProcessErrorEventArgs args)
        {
            _logger?.LogError(args.Exception.ToString());
            return Task.CompletedTask;
        }

        private async Task Processor_ProcessMessageAsync(ProcessMessageEventArgs args)
        {
            var body = args.Message.Body.ToString();
            var theEvent = JsonConvert.DeserializeObject<PetFlaggedForAdoptionIntegrationEvent>(body);
            await args.CompleteMessageAsync(args.Message);
            _logger?.LogInformation(body);
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _processor.StartProcessingAsync(stoppingToken);
        }

        public async override Task StopAsync(CancellationToken cancellationToken)
        {
            await _processor.StopProcessingAsync(cancellationToken);
        }
    }
}