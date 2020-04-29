using System;
using System.Diagnostics;
using SamuraiApp.Data;
using SamuraiApp.Domain;
using Xunit;

namespace SamuraiApp.Tests
{
    public class DatabaseTests
    {
        [Fact]
        public void CanInsertDirectoryIntoDatabase()
        {
            using (var context = new SamuraiContext())
            {
                //context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                var dir = new Samurai();
                context.Samurais.Add(dir);
                Debug.WriteLine($"Before save: {dir.Id}");

                context.SaveChanges();
                Debug.WriteLine($"After save: {dir.Id}");

                Assert.NotEqual(0, dir.Id);
            }
        }
    }
}
