using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson;

namespace week2
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync().Wait();
            Console.ReadLine();
        }

        static async Task MainAsync() 
        {
            var database = new MongoClient().GetDatabase("students");
            var collection = database.GetCollection<BsonDocument>("grades");

            List<BsonDocument> listStudents = await collection.Find(new BsonDocument("type", "homework")).ToListAsync();
            
            listStudents.GroupBy(data => data["student_id"]).Select(data => new { student_id = data.Key, score = data.Min(x => x["score"]) }).ToList().ForEach(student =>
            {
                Console.WriteLine(student + " score removed.");
                collection.DeleteOneAsync(new BsonDocument("student_id", student.student_id).Add("score", student.score).Add("type", "homework"));
            });
        }
    }
}
