using System.Linq.Expressions;
using MongoDB.Driver;

namespace Play.Common.MongoDB
{
    public class MongoRepository<T> : IRepository<T> where T : IEntity
    {

        // variable which holds the instance of the collection
        // readonly: the value of the field cannot be changed after it is initialized 
        private readonly IMongoCollection<T> dbCollection;

        // filter builder which queries for items in mongoDB
        private readonly FilterDefinitionBuilder<T> filterBuilder = Builders<T>.Filter;

        public MongoRepository(IMongoDatabase database, string collectionName)
        {
            dbCollection = database.GetCollection<T>(collectionName);
        }

        // calls to databases are often async because they take a long time to complete
        // this setup allows the program to do other stuff while this function is still running
        // get all entries in db
        public async Task<IReadOnlyCollection<T>> GetAllAsync()
        {
            return await dbCollection.Find(filterBuilder.Empty).ToListAsync();
        }

        public async Task<IReadOnlyCollection<T>> GetAllAsync(Expression<Func<T, bool>> filter)
        {
            return await dbCollection.Find(filter).ToListAsync();
        }

        // get entry by id
        public async Task<T> GetAsync(Guid id)
        {
            // create an equality filter
            FilterDefinition<T> filter = filterBuilder.Eq(entity => entity.Id, id);
            // call the filter and return the first hit
            return await dbCollection.Find(filter).FirstOrDefaultAsync();
        }
        
        public async Task<T> GetAsync(Expression<Func<T, bool>> filter)
        {
            return await dbCollection.Find(filter).FirstOrDefaultAsync();
        }

        // add item to db
        public async Task CreateAsync(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentException(nameof(entity));
            }

            await dbCollection.InsertOneAsync(entity);
        }

        public async Task UpdateAsync(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentException(nameof(entity));
            }

            FilterDefinition<T> filter = filterBuilder.Eq(existingEntity => existingEntity.Id, entity.Id);
            await dbCollection.ReplaceOneAsync(filter, entity);
        }

        public async Task RemoveAsync(Guid id)
        {
            FilterDefinition<T> filter = filterBuilder.Eq(entity => entity.Id, id);
            await dbCollection.DeleteOneAsync(filter);
        }
    }
}