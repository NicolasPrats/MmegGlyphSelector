namespace Mmeg.Model {
    public class Glyph {
        public string Id { get; set; }

        public Shape Shape { get; set; }

        public string SetName { get; set; }

        public decimal HPMult { get; set; }
        public int HPFlat { get; set; }

        public decimal AttackMult { get; set; }
        public int AttackFlat { get; set; }

        public decimal DefenseMult { get; set; }
        public int DefenseFlat { get; set; }

        public int CriticalChance { get; set; }

        public decimal CriticalDamage { get; set; }

        public int Accuracy { get; set; }

        public int Resistance { get; set; }

        public int Speed { get; set; }
        public string Rarity { get; internal set; }
        public int RealSpeed { get; internal set; }
        public string Creature { get; internal set; }

        public override string ToString()
        {
            var creatureName = this.Creature != null ? "(" + this.Creature + ")" : "";
            var speed = this.RealSpeed == 0 ? this.Speed.ToString() : $"{this.Speed}/{this.RealSpeed}";
            return $"{this.Id}{creatureName} - {this.SetName} - {this.Shape} - Speed:{speed} - Accuracy:{this.Accuracy} - Atk:{this.AttackFlat} - Atk%:{this.AttackMult}";
        }
    }
}