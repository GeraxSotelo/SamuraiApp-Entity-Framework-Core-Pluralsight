using Microsoft.EntityFrameworkCore;
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
        //EF Core will presume that the table names match these DbSet names.

        //The optionsBuilder can be used to configure options for the DbContext
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder expects a parameter that's the connection string
            optionsBuilder.UseSqlServer("Data Source = (localdb)\\MSSQLLocalDB; Initial Catalog = SamuraiAppData");
        }
        //The first time EF Core instantiates the SamuraiContext at runtime, it will trigger the OnConfiguring method, learn that it should be using the SQL Server provider, and be aware of the connection string. So it will be able to find the database and do its work.
    }
}
