global using FineCollection.Worker;
global using FineCollection.Worker.Configs;
global using FineCollection.Worker.Interfaces;
global using System.Text.Json.Serialization;
global using Microsoft.Extensions.Options;
global using RabbitMQ.Client;
global using RabbitMQ.Client.Events;
global using FineCollection.Worker.Infra.Brokers.RabbitMq;
global using FineCollection.Worker.Schemas.Documents;
global using MongoDB.Driver;
global using FineCollection.Worker.Infra.Data.Repositories;
global using FineCollection.Worker.Schemas.Responses;
global using FineCollection.Worker.Infra.ExternalServices.VehicleRegistrationApi;
global using FineCollection.Worker.Schemas.Messages;
global using FineCollection.Worker.Services;
global using FineCollection.Worker.Services.Impl;
global using MongoDB.Bson;
global using MongoDB.Bson.Serialization.Attributes;
global using System.Text;
global using System.Text.Json;
global using System.Net.Http.Json;