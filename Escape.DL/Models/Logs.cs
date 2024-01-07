using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Escape.DL.Models
{
    public class Logs
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string RecordId { get; set; }
        public int LogId { get; set; }
        public int WoodID { get; set; }
        public int MonkeyId { get; set; }
        public string Message { get; set; }
    }
}