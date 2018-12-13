using System.Collections.Generic;
using System.Linq;

namespace Mmeg.Model
{
    public class GlyphedCreature
    {

        public Creature Creature { get; }

        public Glyph[] Glyphs { get; }

        public int HP { get; private set; }
        public int Attack { get; private set; }
        public int Defense { get; private set; }
        public int CriticalChance { get; private set; }
        public decimal CriticalDamage { get; private set; }
        public decimal Accuracy { get; private set; }
        public decimal Resistance { get; private set; }
        public int Speed { get; private set; }

        public GlyphedCreature(Creature creature, Glyph[] glyphs)
        {
            this.Creature = creature;
            this.Glyphs = glyphs;

            Recalc();
        }

        public void Recalc()
        {
            this.HP = this.Creature.HP;
            this.Attack = this.Creature.Attack;
            this.Defense = this.Creature.Defense;
            this.CriticalChance = this.Creature.CriticalChance;
            this.CriticalDamage = this.Creature.CriticalDamage;
            this.Accuracy = this.Creature.Accuracy;
            this.Resistance = this.Creature.Resistance;
            this.Speed = this.Creature.Speed;

            foreach (var glyph in this.Glyphs)
            {

                AddGlyph(glyph);
            }

            var sets = this.Glyphs.GroupBy(g => g.SetName);
            foreach (var set in sets)
            {
                if (set.Key == null)
                {
                    continue;
                }

                var bonus = Set.Bonus[set.Key];
                if (set.Count() == 6)
                {
                    AddGlyph(bonus);
                    AddGlyph(bonus);
                }
                else if (set.Count() >= 3)
                {
                    AddGlyph(bonus);
                }
            }
        }

        private void AddGlyph(Glyph glyph)
        {
            this.Accuracy += glyph.Accuracy;
            this.Attack += glyph.AttackFlat + (int)(glyph.AttackMult/100 * this.Creature.Attack);
            this.CriticalChance += glyph.CriticalChance;
            this.CriticalDamage += glyph.CriticalDamage;
            this.Defense += glyph.DefenseFlat + (int)(glyph.DefenseMult/100 * this.Creature.Defense);
            this.HP += glyph.HPFlat + (int)(glyph.HPMult/100 * this.Creature.HP);
            this.Resistance += glyph.Resistance;
            this.Speed += glyph.Speed;
        }

    }
}