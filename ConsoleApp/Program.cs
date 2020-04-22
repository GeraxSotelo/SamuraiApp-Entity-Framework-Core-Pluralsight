using SamuraiApp.Data;
using SamuraiApp.Domain;
using System;
using System.Linq;

namespace ConsoleApp
{
    internal class Program
    {
        private static SamuraiContext context = new SamuraiContext();
        static void Main(string[] args)
        {
            //This is just a quick hack. DO NOT use this in real software
            context.Database.EnsureCreated(); //This will cause EF Core to read the provider & connection string defined in the Context class, and then go look to see it the db exists
            //This is just a quick hack. DO NOT use this in real software
            //GetSamurais("Before Add:");
            //AddSamurai();
            //GetSamurais("After Add:");
            InsertMultipleSamurais();
            Console.WriteLine("Press any key...");
            Console.ReadKey();
        }

        private static void InsertMultipleSamurais()
        {
            var samurai = new Samurai { Name = "Samurai-1" };
            var samurai2 = new Samurai { Name = "Samurai-2" };
            var samurai3 = new Samurai { Name = "Samurai-3" };
            var samurai4 = new Samurai { Name = "Samurai-4" };
            context.AddRange(samurai, samurai2, samurai3, samurai4);
            context.SaveChanges();
        }

        private static void AddSamurai()
        {
            var samurai = new Samurai { Name = "Sampson" };
            context.Samurais.Add(samurai); //Add to context.Samurais DbSet, so it's an in-memory collection of samurais that the context keeps track of
            context.SaveChanges();//Save the data that the context is tracking back to the db
        }

        private static void GetSamurais(string text)
        {
            var samurais = context.Samurais.ToList();//LINQ query to retrieve all the samurais from the db as objects.
            //ToList() LINQ method forces the query to execute and returns a list of the objects
            Console.WriteLine($"{text}: Samurai count is {samurais.Count}");
            foreach (var samurai in samurais)
            {
                Console.WriteLine(samurai.Name);
            }
        }
    }
}
