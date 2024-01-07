using Escape.BL.Models;
using Escape.DL.Repositories;
using System.Diagnostics;
using System.Reflection.Emit;

namespace Escape.UI
{
    internal class Program
    {
        static void Main(string[] args)
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

            //Thread t1 = new Thread(() => ProcessWood(w1));
            //Thread t2 = new Thread(() => ProcessWood(w2));
            //Thread t3 = new Thread(() => ProcessWood(w3));

            //t1.Start();
            //t2.Start();
            //t3.Start();

            //t1.Join();
            //t2.Join();
            //t3.Join();

            ProcessWood(w1);
            ProcessWood(w2);
            ProcessWood(w3);

            stopwatch.Stop();
            Console.WriteLine("Time elapsed: {0}", stopwatch.Elapsed);
            Console.WriteLine("end");
        }

        static void ProcessWood(Wood wood)
        {
            wood.WriteWoodToDB();
            wood.Escape();
        }
    }
}