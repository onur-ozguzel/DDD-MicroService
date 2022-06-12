﻿using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WisdomPetMedicine.Common;
using WisdomPetMedicine.Hospital.Domain.Entities;
using WisdomPetMedicine.Hospital.Domain.Repositories;
using WisdomPetMedicine.Hospital.Domain.ValueObjects;

namespace WisdomPetMedicine.Hospital.Infrastructure
{
    public class PatientAggregateStore : IPatientAggregateStore
    {
        private readonly CosmosClient _cosmosClient;
        private readonly Container _patientContainer;

        public PatientAggregateStore(IConfiguration configuration)
        {
            var connectionString = configuration["CosmosDb:ConnectionString"];
            var databaseId = configuration["CosmosDb:DatabaseId"];
            var containerId = configuration["CosmosDb:ContainerId"];

            var clientOptions = new CosmosClientOptions()
            {
                SerializerOptions = new CosmosSerializationOptions()
                {
                    PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
                }
            };

            _cosmosClient = new CosmosClient(connectionString, clientOptions);
            _patientContainer = _cosmosClient.GetContainer(databaseId, containerId);
        }
        public async Task<Patient> LoadAsync(PatientId patientId)
        {
            if (patientId == null)
            {
                throw new ArgumentNullException(nameof(patientId));
            }

            var aggregateId = $"Patient-{patientId.Value}";
            var sqlQueryText = $"select * from c where c.aggregateId = '{aggregateId}'";
            var queryDefinition = new QueryDefinition(sqlQueryText);
            var queryResultSetIterator = _patientContainer.GetItemQueryIterator<CosmosEventData>(queryDefinition);
            var allEvents = new List<CosmosEventData>();
            while (queryResultSetIterator.HasMoreResults)
            {
                var currentResultSet = await queryResultSetIterator.ReadNextAsync();
                foreach (CosmosEventData item in currentResultSet)
                {
                    allEvents.Add(item);
                }
            }

            var domainEvents = allEvents.Select(e =>
            {
                var assemblyQualifiedName = JsonConvert.DeserializeObject<string>(e.AssemblyQualifiedName);
                var eventType = Type.GetType(assemblyQualifiedName);
                var data = JsonConvert.DeserializeObject(e.Data, eventType);
                return data as IDomainEvent;
            });

            var aggregate = new Patient();
            aggregate.Load(domainEvents);

            return aggregate;
        }

        public async Task SaveAsync(Patient patient)
        {
            if (patient == null)
            {
                throw new ArgumentNullException(nameof(patient));
            }

            var changes = patient.GetChanges()
                .Select(e => new CosmosEventData()
                {
                    Id = Guid.NewGuid(),
                    AggregateId = $"Patient-{patient.Id}",
                    EventName = e.GetType().Name,
                    Data = JsonConvert.SerializeObject(e),
                    AssemblyQualifiedName = JsonConvert.SerializeObject(e.GetType().AssemblyQualifiedName)
                }).AsEnumerable();

            if (!changes.Any()) return;

            foreach (var item in changes)
            {
                await _patientContainer.CreateItemAsync(item);
            }
            patient.ClearChanges();
        }
    }
}
