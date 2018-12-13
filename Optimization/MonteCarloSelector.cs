using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Mmeg.Model;
using Mmeg.Utils;

namespace Mmeg.Optimization
{
    public class MonteCarloSelector
       : IGlyphSelector
    {
        private int NumberOfIterations = 10_000_000;

        private IEnumerable<Glyph> Glyphs { get; }
        public Random Rng { get; }
        private double MaxValue { get; set; } = 1;

        private string[] Sets { get; set; }
        public MonteCarloSelector(IEnumerable<Glyph> glyphs, IEnumerable<string> sets)
        {
            this.Glyphs = glyphs;
            this.Rng = new Random();
            List<string> bestSets = sets.ToList();
            bestSets.Add(null);
            this.Sets = bestSets.ToArray();
        }

        public IEnumerable<IGlyphSelection> GetGlyphCombination()
        {
            var hexGlyphs = this.Glyphs.Where(g => g.Shape == Shape.Hex).Select(g => new ValuedGlyph(g)).ToArray();
            var leafGlyphs = this.Glyphs.Where(g => g.Shape == Shape.Leaf).Select(g => new ValuedGlyph(g)).ToArray(); ;
            var squareGlyphs = this.Glyphs.Where(g => g.Shape == Shape.Square).Select(g => new ValuedGlyph(g)).ToArray(); ;

            if (hexGlyphs.Count() < 2)
            {
                throw new ApplicationException("Not enough hex glyphs");
            }

            if (leafGlyphs.Count() < 2)
            {
                throw new ApplicationException("Not enough leaf glyphs");
            }

            if (squareGlyphs.Count() < 2)
            {
                throw new ApplicationException("Not enough square glyphs");
            }

            int nbPaths = 0;
            var setsToUse = new string[6];
            var glyphPositions = new int[] { 0, 1, 2, 3, 4, 5 };
            ValuedGlyph[] result = new ValuedGlyph[6];
            IEnumerable<ValuedGlyph>[] availableGlyphs = new[]{
                hexGlyphs,
                squareGlyphs,
                leafGlyphs
                };
            while (nbPaths < NumberOfIterations)
            {
                if (nbPaths * 2 == NumberOfIterations)
                {
                    
                    // A mi chemin, on ne garde que les meilleurs glyphes trouvés
                    for (int i = 0; i < 3; i++)
                    {
                        availableGlyphs[i] = GetBestGlyphs(availableGlyphs[i], 20).ToArray();
                        foreach (var glyph in availableGlyphs[i])
                        {
                            var oldNumberOfPath = glyph.NumberOfPath;
                            if (oldNumberOfPath == 0)
                            {
                                //Si on en arrive là, c'est qu'il y a un effet de seuil quelque part
                                // il faut revoir la fonction à maximiser
                                throw new ApplicationException("Not all glyphs tested!");
                            }
                           
                            glyph.NumberOfPath = 1;
                            glyph.Value = glyph.Value / oldNumberOfPath;
                        }
                    }

                }
                RandomUtils.RandomizeSort(glyphPositions);
                string set = this.Sets[this.Rng.Next(this.Sets.Length)];
                setsToUse[glyphPositions[0]] = setsToUse[glyphPositions[1]] = setsToUse[glyphPositions[2]] = set;
                set = this.Sets[this.Rng.Next(this.Sets.Length)];
                setsToUse[glyphPositions[3]] = setsToUse[glyphPositions[4]] = setsToUse[glyphPositions[5]] = set;


                nbPaths++;
                for (int i = 0; i < 3; i++)
                {
                    IEnumerable<ValuedGlyph> glyphs = availableGlyphs[i];
                    int position1 = i * 2;
                    int position2 = position1 + 1;
                    if (setsToUse[position1] != null)
                    {
                        glyphs = glyphs.Where(g => g.Glyph.SetName == setsToUse[position1]);
                    }

                    if (glyphs.Count() == 0)
                    {
                        glyphs = availableGlyphs[i];
                    }

                    result[position1] = RandomUtils.GetNextWeightedRandomValue(glyphs, g => g.GetScore(this.MaxValue));

                    glyphs = availableGlyphs[i].Where(g => g != result[position1]);
                    IEnumerable<ValuedGlyph> glyphs2 = glyphs;
                    if (setsToUse[position2] != null)
                    {
                        glyphs2 = glyphs2.Where(g => g.Glyph.SetName == setsToUse[position2]);
                    }

                    if (glyphs2.Count() == 0)
                    {
                        glyphs2 = glyphs;
                    }

                    result[position2] = RandomUtils.GetNextWeightedRandomValue(glyphs2, g => g.GetScore(this.MaxValue));
                }

                yield return new Selection()
                {
                    Glyphs = result
                };
            }
            var bestGlyphs = GetBestGlyphs(hexGlyphs, 10)
                .Union(GetBestGlyphs(squareGlyphs, 10))
                .Union(GetBestGlyphs(leafGlyphs, 10))
                .Select(g => g.Glyph);

            var exhaustiveSearch = new ExhaustiveSearchSelector(bestGlyphs);
            foreach (var selection in exhaustiveSearch.GetGlyphCombination())
            {
                yield return selection;
            }
        }

        private IEnumerable<ValuedGlyph> GetBestGlyphs(IEnumerable<ValuedGlyph> glyphs, int number)
        {
            var bestGlyphs = new List<ValuedGlyph>();
            foreach (var glyph in glyphs.Where(g => !this.Sets.Contains(g.Glyph.SetName)).OrderByDescending(g => g.Value).Take(number))
            {
                bestGlyphs.Add(glyph);
            }

            foreach (var glyph in glyphs.Where(g => this.Sets.Contains(g.Glyph.SetName)).OrderByDescending(g => g.Value).Take(number))
            {
                bestGlyphs.Add(glyph);
            }

            return bestGlyphs;
        }

        public void PropagateValue(IGlyphSelection selection, decimal value)
        {
            var dblValue = (double)value;
            if (dblValue > this.MaxValue)
            {
                this.MaxValue = dblValue;
            }

            var valuedSelection = selection as Selection;
            if (valuedSelection != null)
            {
                foreach (var glyph in valuedSelection.Glyphs)
                {
                    glyph.NumberOfPath++;
                    if (glyph.Value <= value)
                    {
                        glyph.Value = value;
                    }
                }
            }
        }
        private class Selection :
            IGlyphSelection
        {
            public ValuedGlyph[] Glyphs { get; set; }
            public Glyph[] GetGlyphs()
            {
                return this.Glyphs.Select(g => g.Glyph).ToArray();
            }
        }

        private class ValuedGlyph
        {

            public ValuedGlyph(Glyph glyph)
            {
                this.Glyph = glyph;
            }

            public Glyph Glyph { get; set; }

            private decimal value;
            public decimal Value
            {
                get
                {
                    return this.value;
                }
                set
                {
                    this.value = value;
                    this.Score = (double)(value * value) / this.NumberOfPath;
                }
            }

            public double Score { get; private set; }

            public int NumberOfPath { get; set; }

            public double GetScore(double defaultValue)
            {

                if (this.NumberOfPath != 0)
                {
                    return this.Score;
                }
                else
                {
                    return defaultValue * defaultValue;
                }
            }
        }
    }
}
