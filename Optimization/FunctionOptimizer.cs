using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Mmeg.Model;

namespace Mmeg.Optimization {
    public class FunctionOptimizer {

        private Creature Creature { get; set; }
        private IEnumerable<Glyph> Glyphs { get; }
        private Func<GlyphedCreature, decimal> FunctionToMaximize { get; }
        private Predicate<GlyphedCreature>[] Constraints { get; }

 
        /// <summary>
        /// Une bonne fonction doit être croissante et ne pas avoir de paliers.
        /// </summary>
        /// <param name="creature"></param>
        /// <param name="glyphs"></param>
        /// <param name="functionToMaximize"></param>
        /// <param name="constraints"></param>
        public FunctionOptimizer (Creature creature, IEnumerable<Glyph> glyphs, Func<GlyphedCreature, decimal> functionToMaximize, params Predicate<GlyphedCreature>[] constraints) {
            //TODO : dégager les contraintes ? Il vaut mieux les intégrer à la fonction de manière à éviter les effets de seuil
            this.Creature = creature ??
                throw new ArgumentNullException (nameof (creature));
            this.Glyphs = glyphs ??
                throw new ArgumentNullException (nameof (glyphs));
            this.FunctionToMaximize = functionToMaximize ??
                throw new ArgumentNullException (nameof (functionToMaximize));
            this.Constraints = constraints ??
                throw new ArgumentNullException (nameof (constraints));
        }

        private IEnumerable<string>  GetMaximizingSets()
        {
            List<string> sets = new List<string>();
            GlyphedCreature virtualCreature = new GlyphedCreature(this.Creature, new Glyph[] { new Glyph(), new Glyph(), new Glyph(), new Glyph(), new Glyph(), new Glyph() });

            var baseValue = this.FunctionToMaximize(virtualCreature);
            foreach (var kvp in Set.Bonus)
            {
                virtualCreature.Glyphs[0] = kvp.Value;
                virtualCreature.Recalc();
                if (this.FunctionToMaximize(virtualCreature) > baseValue)
                {
                    sets.Add(kvp.Key);
                }
            }
            return sets;
        }

        public GlyphedCreature Search () {
            var bestSets = this.GetMaximizingSets();
            GlyphedCreature bestResult = null;
            decimal bestValue = -1;

            //IGlyphSelector selector = new ExhaustiveSearchSelector(this.Glyphs);
            IGlyphSelector selector = new MonteCarloSelector(this.Glyphs, bestSets);
            Stopwatch watch = new Stopwatch();
            watch.Start();
            int count = 0;
            foreach (var selection in selector.GetGlyphCombination()) {
                count++;
                var result = new GlyphedCreature (this.Creature, selection.GetGlyphs());
                bool isOk = true;
                foreach (var constraint in this.Constraints)
                    isOk = isOk && constraint (result);
                if (isOk)
                {
                    var value = FunctionToMaximize(result);
                    if (value > bestValue)
                    {
                        bestResult = result;
                        bestValue = value;
                        //Console.WriteLine(bestValue);
                        //foreach (var glyph in selection.GetGlyphs())
                        //{
                        //    Console.WriteLine(glyph);
                        //}
                    }
                    selector.PropagateValue(selection, value);
                }
                else
                {
                    selector.PropagateValue(selection, -1);
                }
                //if (count % 10000 == 0)
                //{
                //    Console.WriteLine("Calculation speed=" + (count * 1000/ watch.ElapsedMilliseconds));
                //}
            }

            return bestResult;
        }

    }
}