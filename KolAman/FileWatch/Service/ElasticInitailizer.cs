using Elastic.Clients.Elasticsearch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileWatch.Models;
namespace FileWatch.Service
{
    public class ElasticInitializer
    {
        private readonly ElasticsearchClient _client;

        public ElasticInitializer(ElasticsearchClient client)
        {
            _client = client;
        }

        public async Task EnsureIndexCreatedAsync(string indexName)
        {
            var existsResponse =
                await _client.Indices.ExistsAsync(indexName);

            if (existsResponse.Exists)
            {
                Console.WriteLine(
                    $"Elasticsearch index '{indexName}' already exists.");

                return;
            }

            var createResponse =
                await _client.Indices.CreateAsync(
                    indexName,
                    c => c
                        .Mappings(m => m
                            .Properties<NotificationLog>(p => p
                                .Keyword(x => x.Level)
                                .Text(x => x.Message)
                                .Date(x => x.Timestamp)
                                .Text(x => x.Exception)
                            )
                        )
                );

            if (!createResponse.IsValidResponse)
            {
                throw new Exception(
                    $"Failed to create Elasticsearch index '{indexName}'.");
            }

            Console.WriteLine(
                $"Elasticsearch index '{indexName}' created successfully.");
        }
    }
}
