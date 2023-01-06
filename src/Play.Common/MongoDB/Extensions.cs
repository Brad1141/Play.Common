using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Play.Common.Settings;

namespace Play.Common.MongoDB
{
    public static class Extensions
    {
        // use extension methods to add on to the "IServiceCollection" functionality
        
        //This extension method adds the mongo client to the service provider
        public static IServiceCollection AddMongo(this IServiceCollection services)
        {
                    
            // anytime we store a guid, store it as a string
            BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
            // anytime we store a DateTimeOffset, store it as a string
            BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(BsonType.String));

            // Construct the mongoDB client
            services.AddSingleton(serviceProvider => {
                var configuration = serviceProvider.GetService<IConfiguration>();
                var serviceSettings = configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();
                var mongoDBSettings = configuration.GetSection(nameof(MongoDbSettings)).Get<MongoDbSettings>();
                var mongoClient = new MongoClient(mongoDBSettings.ConnectionString);
                return mongoClient.GetDatabase(serviceSettings.ServiceName);
            });

            return services;
        }

        // This extension method adds an instance of "IRepo" as the interface and "MongoRepo" as the implementation
        public static IServiceCollection AddMongoRepository<T>(this IServiceCollection services, string collectionName) 
            where T : IEntity
        {
            //register the IItemsRepo dependency
            // uses a factory delegate to create an instance of an object "IRepo<T>"
            services.AddSingleton<IRepository<T>>(serviceProvider => 
            {
                var database = serviceProvider.GetService<IMongoDatabase>();
                return new MongoRepository<T>(database, collectionName);
            });
            return services;
        }
    }
}