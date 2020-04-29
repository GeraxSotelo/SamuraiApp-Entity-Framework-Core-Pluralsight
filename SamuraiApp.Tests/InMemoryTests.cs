using Microsoft.EntityFrameworkCore;
using SamuraiApp.Data;
using SamuraiApp.Domain;
using Xunit;

namespace SamuraiApp.Tests
{
    public class InMemoryTests
    {
        [Fact]
        public void CanInsertDirectoryIntoDatabase()
        {
            var builder = new DbContextOptionsBuilder();
            //UsInMemoryDatabase() needs a string to be passed in to give a name to that instance
            //It's how EF Core keeps track of different instances of the provider in memory.
            builder.UseInMemoryDatabase("CanInsertSamurai");

            using (var context = new SamuraiContext(builder.Options))
            {
                var samurai = new Samurai();

                //The InMemory provider is just lists in memory & even though it knows how to increment IDs, it does it as the entity is being introduced into the change tracker (which is different than what happens if you're waiting for a db to generate that ID
                context.Samurais.Add(samurai);

                //In this case, SaveChanges() doesn't really impact the ID beyond what calling Add has done.
                //context.SaveChanges();

                //This Assertion only makes sense when testing with a real db
                //Assert.NotEqual(0, dir.Id);

                //Make sure the SamuraiContext knows what to do when asked to Add()
                //Check the state of the entity is added.
                Assert.Equal(EntityState.Added, context.Entry(samurai).State);
            }
        }
    }
}
