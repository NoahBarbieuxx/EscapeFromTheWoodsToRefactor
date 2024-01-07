using Escape.BL.Models;
using Escape.DL.Repositories;
using System.Diagnostics;

namespace Escape.UI
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Console.WriteLine("Hello World!");
            string connectionString = "mongodb://localhost:27017";
            MongoDBrepository db = new MongoDBrepository(connectionString);

            string path = @"C:\Users\barbi\Documents\hogent\sem3\6 - Solutions\EscapeFromTheWoodsToRefactor\Escape.Bitmap";
            Map m1 = new Map(0, 500, 0, 500);
            Wood w1 = WoodBuilder.GetWood(500, m1, path, db);
            w1.PlaceMonkey("Alice", IDgenerator.GetMonkeyID());
            w1.PlaceMonkey("Janice", IDgenerator.GetMonkeyID());
            w1.PlaceMonkey("Toby", IDgenerator.GetMonkeyID());
            w1.PlaceMonkey("Mindy", IDgenerator.GetMonkeyID());
            w1.PlaceMonkey("Jos", IDgenerator.GetMonkeyID());

            Map m2 = new Map(0, 200, 0, 400);
            Wood w2 = WoodBuilder.GetWood(2500, m2, path, db);
            w2.PlaceMonkey("Tom", IDgenerator.GetMonkeyID());
            w2.PlaceMonkey("Jerry", IDgenerator.GetMonkeyID());
            w2.PlaceMonkey("Tiffany", IDgenerator.GetMonkeyID());
            w2.PlaceMonkey("Mozes", IDgenerator.GetMonkeyID());
            w2.PlaceMonkey("Jebus", IDgenerator.GetMonkeyID());

            Map m3 = new Map(0, 400, 0, 400);
            Wood w3 = WoodBuilder.GetWood(2000, m3, path, db);
            w3.PlaceMonkey("Kelly", IDgenerator.GetMonkeyID());
            w3.PlaceMonkey("Kenji", IDgenerator.GetMonkeyID());
            w3.PlaceMonkey("Kobe", IDgenerator.GetMonkeyID());
            w3.PlaceMonkey("Kendra", IDgenerator.GetMonkeyID());

            Task t1 = Task.Run(() => ProcessWood(w1));
            Task t2 = Task.Run(() => ProcessWood(w2));
            Task t3 = Task.Run(() => ProcessWood(w3));

            await Task.WhenAll(t1, t2, t3);

            stopwatch.Stop();
            Console.WriteLine("Time elapsed: {0}", stopwatch.Elapsed);
            Console.WriteLine("end");
        }

        static async Task ProcessWood(Wood wood)
        {
            await wood.WriteWoodToDBAsync();
            await wood.EscapeAsync();
        }
    }
}