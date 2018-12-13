using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using Mmeg.MmegExtract;
using Mmeg.Model;
using Mmeg.Optimization;

namespace Mmeg
{
    class Program
    {
        static void Main(string[] args)
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("en-us");
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("en-us");

            var file = File.ReadAllText (@"/home/cozar/Documents/VS Code/mmeg/Glyphes/nico.json");
            

            var creatures = CreaturesHelper.LoadAllCreatures();
            var glyphs = GlyphsHelper.GetGlyphs(file, false).Where(g => g.Rarity == "legendary" || g.Rarity == "dark")
                ;


            //Croisé vent => le plus rapide possible
            var paladin = creatures.Where(c => c.Name == "Exalted Paladin").First();
            var maxSpeedglyphs = SpeedMaximizer.Optimize(glyphs);
            GlyphedCreature glyphedPaladin = new GlyphedCreature(paladin, maxSpeedglyphs);
            Console.WriteLine("Croisé / Speed =" + glyphedPaladin.Speed);
            foreach (var glyph in maxSpeedglyphs)
            {
                Console.WriteLine(glyph);
            }
            glyphs = glyphs.Except(maxSpeedglyphs);

            //dragon eau => le plus rapide possible avec au moins 100 de précision
            var dragon = creatures.Where(c => c.Name == "Sapphire Dragon").First();
            var dragonOptimizer = new FunctionOptimizer(
                dragon,
                glyphs,
                c => Math.Min(c.Accuracy, 100) * 100 + c.Speed
            );
            var glyphedDragon = dragonOptimizer.Search();
            Console.WriteLine($"Dragon / Speed ={glyphedDragon.Speed}, précision = {glyphedDragon.Accuracy}");
            foreach (var glyph in glyphedDragon.Glyphs) {
                Console.WriteLine(glyph);
            }
            glyphs = glyphs.Except(glyphedDragon.Glyphs);


            //Chaman terre => le plus rapide possible
            var shaman = creatures.Where(c => c.Name == "Forest Hexxer").First();
            maxSpeedglyphs = SpeedMaximizer.Optimize(glyphs);
            GlyphedCreature glyphedShaman = new GlyphedCreature(shaman, maxSpeedglyphs);
            Console.WriteLine($"Chaman / Speed ={glyphedShaman.Speed}, précision = {glyphedShaman.Accuracy}");
            foreach (var glyph in maxSpeedglyphs)
            {
                Console.WriteLine(glyph);
            }
            glyphs = glyphs.Except(maxSpeedglyphs);


            //rak vent => le plus de dégâts possibles
            var rak = creatures.Where(c => c.Name == "Storm Rakshasa Raja").First();
            //Le fichier xml ne contient pas les valeurs au niveau 35, il faut donc les adapter en attendant de trouver la formule de calcul
            rak.Attack = 1621;
            var rakOptimizer = new FunctionOptimizer(
                rak,
                glyphs,
                c => (1.2m * c.Attack + 10 * c.Speed) * (1 + Math.Min(c.CriticalChance,100) / 100 * c.CriticalDamage)
            );
            var glyphedRak = rakOptimizer.Search();
            Console.WriteLine($"Rak / Speed ={glyphedShaman.Speed}, Atk = {glyphedRak.Accuracy}, Tcc = {glyphedRak.CriticalChance}, Dcc = {glyphedRak.CriticalDamage}");
            foreach (var glyph in glyphedRak.Glyphs)
            {
                Console.WriteLine(glyph);
            }

        }
    }
}