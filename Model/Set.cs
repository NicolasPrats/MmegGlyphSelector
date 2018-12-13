using System;
using System.Collections.Generic;
using System.Text;

namespace Mmeg.Model
{
    public static class Set
    {
        public static Dictionary<string, Glyph> Bonus = new Dictionary<string, Glyph>() { { "haste", new Glyph () { Speed = 10 } }, { "defense", new Glyph () { DefenseMult = 0.2m } }, { "endurance", new Glyph () { Resistance = 20 } }, { "vitality", new Glyph () { HPMult = 0.2m } }, { "immunity", new Glyph () { } }, { "strength", new Glyph () { AttackMult = 0.2m } }, { "life steal", new Glyph () { } }, { "precision", new Glyph () { Accuracy = 20 } }, { "appeasement", new Glyph () { } }, { "meditation", new Glyph () { } }, { "frenzy", new Glyph () { CriticalChance = 10 } }, { "destruction", new Glyph () { CriticalDamage = 30 } }
        };

    }
}
