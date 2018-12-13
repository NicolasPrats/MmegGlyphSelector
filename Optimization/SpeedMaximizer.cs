using System;
using System.Collections.Generic;
using System.Linq;
using Mmeg.Model;

namespace Mmeg.Optimization {
    public static class SpeedMaximizer {
        public static Glyph[] Optimize (IEnumerable<Glyph> glyphs) {
            int maxSpeed = 0;
            IEnumerable<Glyph> bestGlyphs = new List<Glyph> ();

            glyphs = glyphs.OrderByDescending (g => g.Speed);
            var hasteGlyphs = glyphs.Where (g => g.SetName == "haste");
            var nonHasteGlyphs = glyphs.Where (g => g.SetName != "haste");
            // // Peu importe le set -> pris en compte par le mixte
            // var anySetBestGlyphs = glyphs.Where (g => g.Shape == Shape.Square).Take (2)
            //     .Union (glyphs.Where (g => g.Shape == Shape.Leaf).Take (2))
            //     .Union (glyphs.Where (g => g.Shape == Shape.Hex).Take (2)).ToArray ();
            // var tmpSpeed = anySetBestGlyphs.Sum (g => g.Speed);
            // if (tmpSpeed > maxSpeed) {
            //     bestGlyphs = anySetBestGlyphs;
            //     maxSpeed = tmpSpeed;
            // }

            // 2 sets hâte
            var hasteSet = hasteGlyphs.Where (g => g.Shape == Shape.Square).Take (2)
                .Union (hasteGlyphs.Where (g => g.Shape == Shape.Leaf).Take (2))
                .Union (hasteGlyphs.Where (g => g.Shape == Shape.Hex).Take (2)).ToArray ();
            var tmpSpeed = hasteSet.Sum (g => g.Speed) + 20;
            if (tmpSpeed > maxSpeed) {
                bestGlyphs = hasteSet;
                maxSpeed = tmpSpeed;
            }

            // Aucun set hâte
            var nonHasteSet = nonHasteGlyphs.Where (g => g.Shape == Shape.Square).Take (2)
                .Union (nonHasteGlyphs.Where (g => g.Shape == Shape.Leaf).Take (2))
                .Union (nonHasteGlyphs.Where (g => g.Shape == Shape.Hex).Take (2)).ToArray ();
            // tmpSpeed = hasteSet.Sum (g => g.Speed);
            // if (tmpSpeed > maxSpeed) {
            //     bestGlyphs = hasteSet;
            //     maxSpeed = tmpSpeed;
            // }

var leafs = nonHasteGlyphs.Where (g => g.Shape == Shape.Leaf).ToArray();
            // mixte
            GlyphSwap[] swaps = new GlyphSwap[] {
                new GlyphSwap () { HasteIndex = 1, NonHasteIndex = 0, Benefit = nonHasteSet[0].Speed - hasteSet[1].Speed },
                new GlyphSwap () { HasteIndex = 0, NonHasteIndex = 1, Benefit = nonHasteSet[1].Speed - hasteSet[0].Speed },
                new GlyphSwap () { HasteIndex = 2, NonHasteIndex = 3, Benefit = nonHasteSet[3].Speed - hasteSet[2].Speed },
                new GlyphSwap () { HasteIndex = 3, NonHasteIndex = 2, Benefit = nonHasteSet[2].Speed - hasteSet[3].Speed },
                new GlyphSwap () { HasteIndex = 4, NonHasteIndex = 5, Benefit = nonHasteSet[5].Speed - hasteSet[4].Speed },
                new GlyphSwap () { HasteIndex = 5, NonHasteIndex = 4, Benefit = nonHasteSet[4].Speed - hasteSet[5].Speed },
            };
            var orderedSwaps = swaps.OrderByDescending (s => s.Benefit).ToArray();
            var mixedSet = (Glyph[])hasteSet.Clone ();
            for (int i = 0; i < 3; i++) {
                mixedSet[orderedSwaps[i].HasteIndex] = nonHasteSet[orderedSwaps[i].NonHasteIndex];
            }
            tmpSpeed = mixedSet.Sum (g => g.Speed) + 10;
            if (tmpSpeed > maxSpeed) {
                bestGlyphs = mixedSet;
                maxSpeed = tmpSpeed;
            }
            mixedSet = (Glyph[])mixedSet.Clone ();
            for (int i = 3; i < 6; i++) {
                mixedSet[orderedSwaps[i].HasteIndex] = nonHasteSet[orderedSwaps[i].NonHasteIndex];
            }
            tmpSpeed = mixedSet.Sum (g => g.Speed);
            if (tmpSpeed > maxSpeed) {
                bestGlyphs = mixedSet;
                maxSpeed = tmpSpeed;
            }

            return bestGlyphs.ToArray ();
        }

        private class GlyphSwap {
            public int HasteIndex { get; set; }
            public int NonHasteIndex { get; set; }

            public int Benefit { get; set; }
        }
    }

}