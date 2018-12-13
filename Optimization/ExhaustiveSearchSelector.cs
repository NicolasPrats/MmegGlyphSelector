using Mmeg.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mmeg.Optimization
{
    public class ExhaustiveSearchSelector
        : IGlyphSelector
    {
        private IEnumerable<Glyph> Glyphs { get; }
        public ExhaustiveSearchSelector(IEnumerable<Glyph> glyphs)
        {
            this.Glyphs = glyphs;
        }
        public IEnumerable<IGlyphSelection> GetGlyphCombination()
        {
            var hexGlyphs = this.Glyphs.Where(g => g.Shape == Shape.Hex).ToArray();
            var leafGlyphs = this.Glyphs.Where(g => g.Shape == Shape.Leaf).ToArray(); ;
            var squareGlyphs = this.Glyphs.Where(g => g.Shape == Shape.Square).ToArray(); ;

            if (hexGlyphs.Count() < 2)
                throw new ApplicationException("Not enough hex glyphs");
            if (leafGlyphs.Count() < 2)
                throw new ApplicationException("Not enough leaf glyphs");
            if (squareGlyphs.Count() < 2)
                throw new ApplicationException("Not enough square glyphs");

            Glyph[] glyphs = new Glyph[6];    
            for (int i = 0; i < hexGlyphs.Count() - 1; i++)
            {
                glyphs[0] = hexGlyphs[i];
                for (int j = i + 1; j < hexGlyphs.Count(); j++)
                {
                    glyphs[1] = hexGlyphs[j];
                    for (int k = 0; k < leafGlyphs.Count() - 1; k++)
                    {
                        glyphs[2] = leafGlyphs[k];
                        for (int l = k + 1; l < leafGlyphs.Count(); l++)
                        {
                            glyphs[3] = leafGlyphs[l];
                            for (int m = 0; m < squareGlyphs.Count() - 1; m++)
                            {
                                glyphs[4] = squareGlyphs[m];
                                for (int n = m + 1; n < squareGlyphs.Count(); n++)
                                {
                                    glyphs[5] = squareGlyphs[n];
                                    //Si paralellism, cloner les objets
                                    yield return new Selection() { Glyphs = (Glyph[])glyphs.Clone() };
                                }
                            }
                        }
                    }
                }
            }

        }

        public void PropagateValue(IGlyphSelection selection, decimal value)
        {
            //Ignore
        }

        private class Selection
            : IGlyphSelection
        {
            public Glyph[] Glyphs { get; set; }
            public Glyph[] GetGlyphs()
            {
                return this.Glyphs;
            }
        }

    }
}
