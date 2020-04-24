using Microsoft.EntityFrameworkCore;
using SamuraiApp.Data;
using SamuraiApp.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApp
{
    internal class Program
    {
        private static SamuraiContext _context = new SamuraiContext();
        static void Main(string[] args)
        {
            //This is just a quick hack. DO NOT use this in real software
            //_context.Database.EnsureCreated(); //This will cause EF Core to read the provider & connection string defined in the Context class, and then go look to see it the db exists
            //This is just a quick hack. DO NOT use this in real software
            //GetSamurais("Before Add:");
            //AddSamurai();
            //GetSamurais("After Add:");
            //InsertMultipleSamurais();
            //RetrieveAndUpdateSamurai();
            //RetrieveAndUpdateMultipleSamurais();
            //MultipleDatabaseOperations();
            //AddQuoteToExistingSamuraiWhileTracked();
            //EagerLoadSamuraiWithQuotes();
            //GetSamurais("Done");
            //Console.WriteLine("Press any key...");
            //Console.ReadKey();

            //Querying database views
            //QuerySamuraiBattleStats();

            QueryUsingRawSql();
        }

        private static void QuerySamuraiBattleStats()
        {
            var stats = _context.SamuraiBattleStats.ToList();
            var firstStat = _context.SamuraiBattleStats.FirstOrDefault();
            var sampsonStat = _context.SamuraiBattleStats.Where(s => s.Name == "Sampson").FirstOrDefault();
            //The ChangeTracker has no clue about the data that gets returned when using DbSet methods due to keyless entities

            //This makes no sense because there's no key
            var findOne = _context.SamuraiBattleStats.Find(2);
        }

        private static void QueryUsingRawSql()
        {
            //1 requirement with using FromSQL methods on the DbSet is the SQL needs to return data whose structure matches the entity represented by the DbSet
            //Because these are DbSet methods, they can only be used for known entities
            //You can't use the raw SQL query to return a type that's not known by the DbContext model

            //var samurais = _context.Samurais.FromSqlRaw("Select * from Samurais").ToList();

            var samurais = _context.Samurais.FromSqlRaw("Select Id, Name, ClanId from Samurais").Include(s => s.Quotes).ToList();
        }

        private static void QueryUsingRawSqlWithInterpolation()
        {
            string name = "Kikuchyo";
            var samurais = _context.Samurais.FromSqlInterpolated($"Select * from Samurais Where Name = {name}").ToList();
        }

        private static void DANGERDANGERQueryUsingRawSqlWithInterpolation()
        {
            //Danger of SQL Injection Attack!
            //If you pass an interpolated method into FromSqlRaw, you are in danger of SQL injection attacks!
            string name = "Kikuchyo";
            var samurais = _context.Samurais.FromSqlRaw($"Select * from Samurais Where Name = '{name}'").ToList();
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
                foreach(var quote in samurai.Quotes)
                {
                Console.WriteLine(quote.Text);
                }
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
            //DbSet AsNoTracking() method in a LINQ query will ensure the DbContext doesn't create entity entry objects to track the results of the query.
            //Useful when tracking is not needed
            var battle = _context.Battles.AsNoTracking().FirstOrDefault();
            //The AsNoTracking() method returns a query, so you can't append DbSet methods such as Find() to AsNoTracking(), but you can still use LINQ methods
            battle.EndDate = new DateTime(1560, 06, 30);

            using (var newContextInstance = new SamuraiContext())
            {
                //DbSet Update() method. Context will start tracking the object then it will tell EF Core to mark the object as modified
                //In a disconnected scenario, EF Core handles updates by passing in the values of all the object's properties
                newContextInstance.Battles.Update(battle);
                newContextInstance.SaveChanges();
            }
        }

        private static void InsertNewSamuraiWithAQuote()
        {
            var samurai = new Samurai { Name = "Kambei Shimada", 
                                        Quotes = new List<Quote> { new Quote { Text = "I've come to save you" } } };
            _context.Samurais.Add(samurai);
            _context.SaveChanges();
        }

        private static void InsertNewSamuraiWithManyQuotes()
        {
            var samurai = new Samurai
            {
                Name = "Kyuzo",
                Quotes = new List<Quote> { new Quote { Text = "Watch out for my sword!" }, new Quote { Text = "Oh well!" } }
            };
            _context.Samurais.Add(samurai);
            _context.SaveChanges();
        }

        private static void AddQuoteToExistingSamuraiWhileTracked()
        {
            var samurai = _context.Samurais.FirstOrDefault();
            samurai.Quotes.Add(new Quote { Text = "I bet you're happy I saved you" });
            _context.SaveChanges();
        }

        private static void AddQuoteToExistingSamuraiNotTracked(int samuraiId)
        {
            //New DbContext in disconnected scenario
            var samurai = _context.Samurais.Find(samuraiId); 
            samurai.Quotes.Add(new Quote { Text = "I'm hungry" });

            using (var newContext = new SamuraiContext())
            {
                //DbSet Update() method to start tracking the graph
                //But using Update() here is a performance issue because an update command gets sent even though the object's direct props were not edited
                //newContext.Samurais.Update(samurai); 

                //The Attach() method connects the object and sets its state to unmodified.
                newContext.Samurais.Attach(samurai); //The Quote still gets inserted, but there is no update command being sent
                newContext.SaveChanges();
            }
            //EF Core sees that the samurai already has an ID and determines that the quote must be new because it doesn't have an ID.
            //Its default behavior is to assume that the quote's foreign key value should be the value of the Samurai ID because they're connected.
        }

        private static void AddQuoteToExistingSamuraiNotTracked_Easy(int samuraiId)
        {
            //Easier way is to set the foreign key on the quote.
            var quote = new Quote { Text = "I'm hungry again", SamuraiId = samuraiId };

            using (var newContext = new SamuraiContext())
            {
                //Add to the Quotes DbSet
                newContext.Quotes.Add(quote);
                newContext.SaveChanges();
            }
        }

        private static void EagerLoadSamuraiWithQuotes()
        {
            //DbSet Include() method. LINQ ToList() exectution method
            //Here, Quotes is the navigation property to include.
            var samuraiWithQuotes = _context.Samurais.Include(s => s.Quotes).ToList();

            var filteredSamuraiWithQuotes = _context.Samurais.Where(s => s.Name.Contains("Kikuchiyo")).Include(s => s.Quotes).FirstOrDefault();
            //The Include() method always loads the entire set of related objects.It doesn't allow you to filter which related data is returned.
        }

        private static void ProjectSomeProperties()
        {
            //Select() method to specify which properties of an object we want returned.
            //When returning more than 1 property, they'll have to be contained within a type
            //LINQ lets you do that using the new keyword & placing the properties in curly braces. This is an "anonymous" type

            //This will return a list of anonymous types whose properties are an int & a string
            var someProperties = _context.Samurais.Select(s => new { s.Id, s.Name, s.Quotes.Count }).ToList();
            //Anonymous types can't be tracked because the context doesn't comprehend that, but if it has props that are recognized Entity objects, they will be tracked
        }

        private static void ProjectSamuraisWithQuotes()
        {
            //var somePropertiesWithQuotes = _context.Samurais.Select(s => new { s.Id, s.Name, s.Quotes.Count }).ToList();

            //filter on the Quotes
            //var somePropertiesWithQuotes = _context.Samurais
            //    .Select(s => new { s.Id, s.Name, HungryQuotes = s.Quotes.Where(q => q.Text.Contains("hungry")) })
            //    .ToList();

            //Get full samurais & a filtered full list of quotes
            var samuraisWithHungryQuotes = _context.Samurais
                .Select(s => new { Samurai = s, HungryQuotes = s.Quotes.Where(q => q.Text.Contains("hungry")) })
                .ToList();

            var firstSamurai = samuraisWithHungryQuotes[0].Samurai.Name += " The Hungriest";
        }

        private static void ExplicitLoadQuotes()
        {
            //Explicitly retrieve related data for objects already in memory.
            //You can only load from a single object, so it can't be used for a list of Samurais
            var samurai = _context.Samurais.FirstOrDefault(s => s.Name.Contains("Kikuchiyo"));
            //Selecting all of the quotes with the samurai id
            _context.Entry(samurai).Collection(s => s.Quotes).Load(); //Quotes is a collection property
            _context.Entry(samurai).Reference(s => s.Horse).Load(); //Horse is a reference property

            //can also filter
            var hungryQuotes = _context.Entry(samurai).Collection(b => b.Quotes).Query().Where(q => q.Text.Contains("hungry")).ToList();
        }

        private static void LazyLoadQuotes()
        {
            //Lazy loading is not good for performance. Lazy loading has to be enabled
            var samurai = _context.Samurais.FirstOrDefault(s => s.Name.Contains("Kikuchiyo"));

            //This is okay. It will send 1 command to retrieve all the Quotes for that samurai, then iterate through them.
            foreach(var q in samurai.Quotes)
            {
                Console.WriteLine(q.Text);
            }

            //This is not good. It will retrieve all the quote objects from the db & materialize them & then give you the count.
            var quoteCount = samurai.Quotes.Count();
        }

        private static void FilteringWithRelatedData()
        {
            //Use Where() & Any() methods to compose query where the related data is used to filter or sort the main object
            //Navigate thru the relationship in the where predicate
            //Then do a subquery of the samurai's quotes, determining if any of the quotes contain the word hungry
            var samurais = _context.Samurais.Where(s => s.Quotes.Any(q => q.Text.Contains("hungy"))).ToList();
            //This will only return the samurai, not its quotes or horse or clan
        }

        private static void ModifyingRelatedDataWhenTracked()
        {
            //Use Include() to eager load a single samurai along with all of its quotes
            var samurai = _context.Samurais.Include(s => s.Quotes).FirstOrDefault(s => s.Id == 2);
            //Modify the text of the first quote
            samurai.Quotes[0].Text = "Did you hear that?";
            _context.SaveChanges();
        }

        private static void ModifyingRelatedDataWhenNotTracked()
        {
            //Disconnected scenario
            //Use Include() to eager load a single samurai along with all of its quotes
            var samurai = _context.Samurais.Include(s => s.Quotes).FirstOrDefault(s => s.Id == 2);
            //Modify the text of the first quote
            var quote = samurai.Quotes[0];
            quote.Text = "Did you hear that again?";

            using (var newContext = new SamuraiContext())
            {
                //Using DbSet Update() to tell the new context to start tracking the quote & mark it as modified
                //newContext.Quotes.Update(quote); // This is not good because it will update everything in samurai object

                //DbContext.Entry will focus specifically on the entry that you pass in & ignore anything else that might be attached to it.
                //Entry's State prop can set the state of that entry to Modified, which is of the EntityState enums.
                newContext.Entry(quote).State = EntityState.Modified;
                //Now the change tracker is only tracking the quote
                //SaveChanges() will only send an Update command to the db for the quote
                newContext.SaveChanges();
            }
        }

        private static void JoinBattleAndSamurai()
        {
            //Samurai & Battle already exist & we have their IDs
            //There is no DbSet for SamuraiBattle in SamuraiContext.cs
            var sbJoin = new SamuraiBattle { SamuraiId = 1, BattleId = 3 };
            //You can call Add,Attach, Update or Delete for entities that don't have a DbSet defined in the DB context
            _context.Add(sbJoin);
            _context.SaveChanges(); //pushes the SamuraiBattle into the db
        }

        private static void EnlistSamuraiIntoABattle()
        {
            var battle = _context.Battles.Find(1);
            battle.SamuraiBattles.Add(new SamuraiBattle { SamuraiId = 21 });
            //If changetracker is watching this, there's no need to specify the battle id.
            //EF Core can figure that out when SaveChanges() is called
            _context.SaveChanges();
        }

        private static void RemoveJoinBetweenSamuraiAndBattleSimple()
        {
            //Samurai & Battle already exist & we have their IDs
            //Normally, retrieve a true object queried from the db to use with EF Core's Remove() method,
            //but in this case, SamuraiBattle is so simple that it's safe to create it in memory without worrying about side effects
            var join = new SamuraiBattle { BattleId = 1, SamuraiId = 2 };
            _context.Remove(join);
            _context.SaveChanges();
        }

        private static void GetSamuraiWithBattles()
        {
            //Using eager load
            //Problem: with results, will still need to drill into each 1 of the samurai battles attached to the samurai to find the battle on the other end
            var samuraiWithBattle = _context.Samurais
                .Include(s => s.SamuraiBattles)
                .ThenInclude(sb => sb.Battle)
                .FirstOrDefault(samurai => samurai.Id == 2);

            //Better solution
            //Use a pojection. Then you can project a type that has the samurai as 1 prop & all of the battles for that samurai as another prop
            var samuraiWithBattlesCleaner = _context.Samurais
                .Where(s => s.Id == 2)
                .Select(s => new { Samurai = s, Battles = s.SamuraiBattles.Select(sb => sb.Battle) })
                .FirstOrDefault();
        }

        private static void AddNewSamuraiWithHorse()
        {
            //EF Core first sends an INSERT statement from the Samurai, along with a query to return the db-generated ID.
            var samurai = new Samurai { Name = "Kenichi" };
            //EF Core takes that new ID, integrates it into an INSERT command for the new Horse, & sets it to the value of the Horse's SamuraiId foreign key
            samurai.Horse = new Horse { Name = "Silver" };
            _context.Samurais.Add(samurai);
            _context.SaveChanges();
        }

        private static void AddNewHorseToSamuraiUsingId()
        {
            //Add a new Horse to an already existing Samurai who doesn't have a horse yet

            //Easiest path if the Samurai isn't in memory yet, but you have its ID
            //Create a new Horse object and set its SamuraiId
            var horse = new Horse { Name = "Scout", SamuraiId = 2 };
            //Since there is no Horse DbSet, just use Add() to track it and save changes.
            _context.Add(horse);
            _context.SaveChanges();
        }

        private static void AddNewHorseToSamuraiObject()
        {
            //If Samurai is already in memory, just set the Horse object to the Horse navigation prop
            var samurai = _context.Samurais.Find(2);
            samurai.Horse = new Horse { Name = "Black Beauty" };
            _context.SaveChanges();
        }

        private static void AddNewHorseToDisconnectedSamuraiObject()
        {
            //This can be done if you are disconnected as well.
            var samurai = _context.Samurais.AsNoTracking().FirstOrDefault(s => s.Id == 2);
            samurai.Horse = new Horse { Name = "Mr. Ed" };
            using (var newContext = new SamuraiContext())
            {
                newContext.Attach(samurai);
                newContext.SaveChanges();
            }
            //EF Core will see that the Samurai already has an ID & will mark it as unchanged
            //It will also recognize that Horse doesn't have an ID yet. It will set the state for the Horse to 'added'
            //Then it will get inserted into the db when SaveChanges() is called
        }

        private static void ReplaceAHorse()
        {
            //If Samurai & its Horse is already in memory & its Horse is replaced with a new Horse object,
            //EF Core will delete the old one from the db & then add the new one
            //because in this project, the constraints don't allow the Horse to exist without a Samurai
            var samurai = _context.Samurais.Include(s => s.Horse).FirstOrDefault(s => s.Id == 2);
            samurai.Horse = new Horse { Name = "Trigger" };
            _context.SaveChanges();

            //If trading a Horse, you can just set the Horse's SamuraiId to the ID of the new Samurai owner.

            //If the Horse object isn't in memory, EF Core won't know to delete it,
            //and it will just send the insert to the db & if there's a conflict with the unique constraint, the db will throw an exception
        }

        private static void GetSamuraisWithHorse()
        {
            var samurai = _context.Samurais.Include(s => s.Horse).ToList();
        }

        private static void GetHorseWithSamurai()
        {
            //Since we have neither a Horse DbSet nor a Samurai property in the Horse class,
            //it won't be as easy to query for a Horse and load its Samurai

            //When there is no DbSet, you have the option to query using the DbContext Set() method to start the query
            var horseWithoutSamurai = _context.Set<Horse>().Find(3);

            //Since there is no Samurai navigation prop in the Horse class, will have to find a way to bring back the Samurai at the same time.
            //You can query for a Samurai & then drill through the relationship to filter on the Horse's ID property
            var horseWithSamurai = _context.Samurais.Include(s => s.Horse).FirstOrDefault(s => s.Horse.Id == 3);

            //Or first filter on Samurai's that have a Horse by making sure the Horse isn't null
            //then do a projection to pull back a type that has the Horse & the Samurai in it
            var horseWithSamurais = _context.Samurais.Where(s => s.Horse != null).Select(s => new { Horse = s.Horse, Samurai = s }).ToList();
        }

        private static void GetSamuraiWithClan()
        {
            var samurai = _context.Samurais.Include(s => s.Clan).FirstOrDefault();
        }

        private static void GetClanWithSamurais()
        {   
            //Can't filter on the Samurais by expressing the ClanId because that property doesn't exist

            //Can't include the Samurais on the Clan because there's no List of Samurais prop in the Clan type
            //var clan = _context.Clans.Include(c=>c.???)

            //First you need to get the Clan,
            var clan = _context.Clans.Find(3);
            //then query for the Samurais, drilling through the navigation property to the Clan ID and getting all that back
            var samuraisForClan = _context.Samurais.Where(s => s.Clan.Id == 3).ToList();
        }
    }
}
