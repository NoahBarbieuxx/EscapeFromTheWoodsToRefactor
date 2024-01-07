using Escape.DL.Models;
using MongoDB.Driver;

namespace Escape.DL.Repositories
{
    public class MongoDBrepository
    {
        private IMongoClient _dbClient;
        private IMongoDatabase _database;
        private string _connectionString;

        public MongoDBrepository(string connectionString)
        {
            _connectionString = connectionString;
            _dbClient = new MongoClient(connectionString);
            _database = _dbClient.GetDatabase("EscapeFromTheWoods");
        }

        public Task WriteWoodRecords(List<WoodRecords> woodRecords)
        {
            var collection = _database.GetCollection<WoodRecords>("WoodRecords");
            return collection.InsertManyAsync(woodRecords);
        }

        public Task WriteMonkeyRecords(List<MonkeyRecords> monkeyRecords)
        {
            var collection = _database.GetCollection<MonkeyRecords>("MonkeyRecords");
            return collection.InsertManyAsync(monkeyRecords);
        }

        public Task WriteMonkeyLogs(List<Logs> logs)
        {
            var collection = _database.GetCollection<Logs>("Logs");
            return collection.InsertManyAsync(logs);
        }
    }
}