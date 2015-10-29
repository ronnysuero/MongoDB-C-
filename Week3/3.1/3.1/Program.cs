
using MongoDB.Bson;
using System;
using System.Threading.Tasks;
using System.Linq;
using MongoDB.Driver;
using System.Collections.Generic;

namespace _3._1
{
    class Program
    {
        public static async Task mainAsync()
        {
            var db = new MongoClient().GetDatabase("school");
            var collection = db.GetCollection<BsonDocument>("students");
            var list = await collection.Find(new BsonDocument()).ToListAsync();
            var newList = new List<BsonDocument>();

            list.ForEach(data => newList.Add(dropScore(data)));

            await db.DropCollectionAsync("students");
            await db.CreateCollectionAsync("students");
            await collection.InsertManyAsync(newList);
        }

        public static BsonDocument dropScore(BsonDocument document)
        {
            var homeworks = document["scores"].AsBsonArray.Where(value => value.ToBsonDocument()["type"] == "homework").ToList();
            var others = document["scores"].AsBsonArray.Where(value => value.ToBsonDocument()["type"] == "exam" || value.ToBsonDocument()["type"] == "quiz").ToList();
            var array = new BsonArray();

            others.Add((homeworks[0].ToBsonDocument()["score"].ToDouble() > homeworks[1].ToBsonDocument()["score"].ToDouble()) ? homeworks[0] : homeworks[1]);
            others.ForEach(data => array.Add(data.ToBsonDocument()));

            return new BsonDocument("_id", document["_id"]).Add("name", document["name"]).Add("scores", array);
        }

        static void Main(string[] args)
        {
            mainAsync().Wait();
        }
    }
}
