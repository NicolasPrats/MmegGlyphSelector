using RestSharp.Deserializers;

namespace Mmeg.Model {
    public class Creature {
        public Creature() {
        
        }
        public string Id { get; set; }

        public string Name { get; set; }

        public string EvolvesTo { get; set; }

        public int HP { get; set; }

        public int Attack { get; set; }

        public int Defense { get; set; }

        public int CriticalChance { get; set; }

        public int CriticalDamage { get; set; }

        public decimal Accuracy { get; set; }

        public decimal Resistance { get; set; }

        public int Speed { get; set; }

    }
}