using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Escape.DL.Models
{
    public class MonkeyRecords
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string RecordId { get; set; }
        public int MonkeyID { get; set; }
        public string MonkeyName { get; set; }
        public int WoodID { get; set; }
        public int SeqNr { get; set; }
        public int TreeID { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
    }
}