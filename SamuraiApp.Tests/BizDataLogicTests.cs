using Microsoft.EntityFrameworkCore;
using SamuraiApp.Data;
using SamuraiApp.Domain;
using ConsoleApp;
using Xunit;
using System.Linq;

namespace SamuraiApp.Tests
{
    public class BizDataLogicTests
    {
        [Fact]
        public void AddMultipleSamuraisReturnsCorrectNumberOfInsertedRows()
        {
            var builder = new DbContextOptionsBuilder();
            builder.UseInMemoryDatabase("AddMultipleSamurais");
            using (var context = new SamuraiContext(builder.Options))
            {
                var bizlogic = new BusinessDataLogic(context);
                var nameList = new string[] { "Kikuchiyo", "Kyuzo", "Rikchi" };
                var result = bizlogic.AddMultipleSamurais(nameList);
                //result is number of rows inserted into the db
                Assert.Equal(nameList.Count(), result);
            }
        }

        [Fact]
        public void CanInsertSingleSamurai()
        {
            var builder = new DbContextOptionsBuilder();
            builder.UseInMemoryDatabase("InsertNewSamurai");

            using(var context = new SamuraiContext(builder.Options))
            {
                var bizlogic = new BusinessDataLogic(context);
                bizlogic.InsertNewSamurai(new Samurai());
            }

            using (var context2 = new SamuraiContext(builder.Options))
            {
                Assert.Equal(1, context2.Samurais.Count());
            }
        }
    }
}
