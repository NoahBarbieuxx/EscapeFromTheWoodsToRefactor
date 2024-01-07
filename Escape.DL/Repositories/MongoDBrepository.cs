using Escape.DL.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public Task WriteWoodRecords(List<WoodRecords> data)
        {
            var collection = _database.GetCollection<WoodRecords>("WoodRecords");
            return collection.InsertManyAsync(data);
        }

        public Task WriteMonkeyRecords(List<MonkeyRecords> data)
        {
            var collection = _database.GetCollection<MonkeyRecords>("MonkeyRecords");
            return collection.InsertManyAsync(data);
        }

        public Task WriteMonkeyLogs(List<Logs> data)
        {
            var collection = _database.GetCollection<Logs>("Logs");
            return collection.InsertManyAsync(data);
        }
    }
}