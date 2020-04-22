using System;
using System.Collections.Generic;
using System.Text;

namespace SamuraiApp.Domain
{
    public class Samurai
    {
        public Samurai()
        {
            Quotes = new List<Quote>();
            SamuraiBattles = new List<SamuraiBattle>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Quote> Quotes { get; set; }
        public Clan Clan { get; set; }
        public List<SamuraiBattle> SamuraiBattles { get; set; }
        public Horse Horse { get; set; }
        //With the navigation property in Samurai and the foreign key in the Horse class, it's enough for EF Core to comprehend the relationship without any more information
    }
}
