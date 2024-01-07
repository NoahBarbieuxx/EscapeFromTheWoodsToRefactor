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

        public void WriteWoodRecords(List<WoodRecords> data)
        {
            var collection = _database.GetCollection<WoodRecords>("WoodRecords");
            collection.InsertMany(data);
        }

        public void WriteMonkeyRecords(List<MonkeyRecords> data)
        {
            var collection = _database.GetCollection<MonkeyRecords>("MonkeyRecords");
            collection.InsertMany(data);
        }

        public void WriteMonkeyLogs(List<Logs> data)
        {
            var collection = _database.GetCollection<Logs>("Logs");
            collection.InsertMany(data);
        }
    }
}