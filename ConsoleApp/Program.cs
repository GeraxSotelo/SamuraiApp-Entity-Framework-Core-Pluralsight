using Microsoft.EntityFrameworkCore;
using SamuraiApp.Data;
using SamuraiApp.Domain;
using System;
using System.Linq;

namespace ConsoleApp
{
    internal class Program
    {
        private static SamuraiContext _context = new SamuraiContext();
        static void Main(string[] args)
        {
            //This is just a quick hack. DO NOT use this in real software
            _context.Database.EnsureCreated(); //This will cause EF Core to read the provider & connection string defined in the Context class, and then go look to see it the db exists
            //This is just a quick hack. DO NOT use this in real software
            //GetSamurais("Before Add:");
            //AddSamurai();
            //GetSamurais("After Add:");
            //InsertMultipleSamurais();
            //RetrieveAndUpdateSamurai();
            //RetrieveAndUpdateMultipleSamurais();
            //MultipleDatabaseOperations();
            GetSamurais("Done");
            Console.WriteLine("Press any key...");
            Console.ReadKey();
        }

        private static void InsertMultipleSamurais()
        {
            //DbSet AddRange() method
            var samurai = new Samurai { Name = "Samurai-1" };
            var samurai2 = new Samurai { Name = "Samurai-2" };
            var samurai3 = new Samurai { Name = "Samurai-3" };
            var samurai4 = new Samurai { Name = "Samurai-4" };
            //Add to context.Samurais DbSet, so it's an in-memory collection of samurais that the context keeps track of
            _context.AddRange(samurai, samurai2, samurai3, samurai4);
            //Save the data that the context is tracking back to the db
            _context.SaveChanges();
        }

        private static void InsertVariousTypes()
        {
            var samurai = new Samurai { Name = "Samurai-5" };
            var clan = new Clan { ClanName = "Imperial Clan" };
            _context.AddRange(samurai, clan);
            _context.SaveChanges();
        }

        private static void AddSamurai()
        {
            //DbSet Add() method
            var samurai = new Samurai { Name = "Sampson" };
            //Add to context.Samurais DbSet, so it's an in-memory collection of samurais that the context keeps track of
            _context.Samurais.Add(samurai);
            //Save the data that the context is tracking back to the db
            _context.SaveChanges();
        }

        private static void GetSamurais(string text)
        {
            var samurais = _context.Samurais.ToList();//LINQ query to retrieve all the samurais from the db as objects.
            //ToList() LINQ method forces the query to execute and returns a list of the objects
            Console.WriteLine($"{text}: Samurai count is {samurais.Count}");
            foreach (var samurai in samurais)
            {
                Console.WriteLine(samurai.Name);
            }
        }

        private static void QueryFilters()
        {
            var name = "Sampson";
            //var samurais = _context.Samurais.Where(s => s.Name == name).ToList();

            //% is a wildcard
            //var samurais = _context.Samurais.Where(s => EF.Functions.Like(s.Name, "J%")).ToList();

            //var samurais = _context.Samurais.FirstOrDefault(s => s.Name == name);

            //Find() is not a LINQ method. It's a DbSet method that will execute right away
            //var samurais = _context.Samurais.Find(2);

            //Last() or LastOrDefault() methods will only work if you first sort the query using the OrderBy LINQ method.
            var samurais = _context.Samurais.OrderBy(s => s.Id).LastOrDefault(s => s.Name == name);
        }

        private static void RetrieveAndUpdateSamurai()
        {
            //LINQ FirstOrDefault() method
            var samurai = _context.Samurais.FirstOrDefault();
            samurai.Name += "San";
            _context.SaveChanges();
        }

        private static void RetrieveAndUpdateMultipleSamurais()
        {
            //Can be used for pagination
            var samurais = _context.Samurais.Skip(1).Take(3).ToList();
            samurais.ForEach(s => s.Name += "San");
            _context.SaveChanges();
        }

        private static void MultipleDatabaseOperations()
        {
            var samurai = _context.Samurais.FirstOrDefault();
            samurai.Name += "San";
            _context.Samurais.Add(new Samurai { Name = "Kikuchiyo" });
            _context.SaveChanges();
        }

        private static void RetrieveAndDeleteSamurai()
        {
            //DbSet Remove() method
            //Context needs to track the entity => Set its state to "Deleted" => Sends relevant SQL to db on SaveChanges
            var samurai = _context.Samurais.Find(3);
            _context.Samurais.Remove(samurai);
            _context.SaveChanges();
        }

        private static void InsertBattle()
        {
            _context.Battles.Add(new Battle { Name="Battle of Okehazama", StartDate=new DateTime(1560, 05, 02), EndDate=new DateTime(1560, 06, 15) });
            _context.SaveChanges();
        }

        private static void QueryAndUpdateBattle_Disconnected()
        {
            //In a disconnected scenario, requesting a battle in one context, modifying it, and then using a brand-new context to push the changes to the db.
            var battle = _context.Battles.AsNoTracking().FirstOrDefault();
            battle.EndDate = new DateTime(1560, 06, 30);

            using (var newContextInstance = new SamuraiContext())
            {
                //DbSet Update() method. Context will start tracking the object then it will tell EF Core to mark the object as modified
                //In a disconnected scenario, EF Core handles updates by passing in the values of all the object's properties
                newContextInstance.Battles.Update(battle);
                newContextInstance.SaveChanges();
            }
        }
    }
}
