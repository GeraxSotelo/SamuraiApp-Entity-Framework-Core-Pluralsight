using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SamuraiApp.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace SamuraiApp.Data
{
    //DbContext will provide all the logic that EF Core is going to use to do it's change tracking and data base interaction tasks.
    public class SamuraiContext : DbContext
    {
        //A DbContext needs to expose DbSets, which become wrappers to the different types that you'll interact with while you're using the context.
        public DbSet<Samurai> Samurais { get; set; }
        public DbSet<Quote> Quotes { get; set; }
        public DbSet<Clan> Clans { get; set; }
        public DbSet<Battle> Battles { get; set; }
        //EF Core will presume that the table names match these DbSet names.

            //filters to only show database commands and basic information detail
        public static readonly ILoggerFactory ConsoleLoggerFactory = LoggerFactory.Create(builder => {
            builder.AddFilter((category, level) => 
            category == DbLoggerCategory.Database.Command.Name && level == LogLevel.Information).AddConsole();
        });

        //The optionsBuilder can be used to configure options for the DbContext
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder expects a parameter that's the connection string
            optionsBuilder.UseLoggerFactory(ConsoleLoggerFactory).UseSqlServer("Data Source = (localdb)\\MSSQLLocalDB; Initial Catalog = SamuraiAppData");
            //The first time EF Core instantiates the SamuraiContext at runtime, it will trigger the OnConfiguring method, learn that it should be using the SQL Server provider, and be aware of the connection string. So it will be able to find the database and do its work.
        }


        //Using the Fluent API to specify the last critical detail of the many-to-many relationship.
        //he fluent mappings go into DbContext's onModelCreating method, which gets called internally at runtime when EF Core is working out what the data model looks like.
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Using the modelBuilder object that EF Core has passed into the method, it's told that the SamuraiBattle Entity has a key that's composed from its SamuraiId & BattleId properites.
            modelBuilder.Entity<SamuraiBattle>().HasKey(s => new { s.SamuraiId, s.BattleId });
            
            //Tell the modelBuilder that the Horse entity maps to a table called Horses
            modelBuilder.Entity<Horse>().ToTable("Horses");
        }
    }
}
