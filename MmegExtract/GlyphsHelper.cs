using System;
using System.Collections.Generic;
using System.IO;
using Mmeg.Model;
using Newtonsoft.Json.Linq;

namespace Mmeg.MmegExtract {
    public static class GlyphsHelper {

        private delegate void GlyphMainStatInitializer(Glyph glyph);

        private static Dictionary<string, Dictionary<string, GlyphMainStatInitializer>> MainStats = new Dictionary<string, Dictionary<string, GlyphMainStatInitializer>>() {
            {
            "defense",
            new Dictionary<string, GlyphMainStatInitializer> () { { "dark", (g) => throw new NotImplementedException () }, { "legendary", (g) => g.DefenseFlat = 265 }, { "epic", (g) => g.DefenseFlat = 169 }, { "rare", (g) => g.DefenseFlat = 0 }, { "common", (g) => g.DefenseFlat = 0 }, { "uncommon", (g) => g.DefenseFlat = 0 }

            }
            }, {
            "defense%",
            new Dictionary<string, GlyphMainStatInitializer> () { { "dark", (g) => g.DefenseMult = 48.75m }, { "legendary", (g) => g.DefenseMult = 40.75m }, { "epic", (g) => g.DefenseMult = 32.75m }, { "rare", (g) => g.DefenseMult = 26.25m }, { "common", (g) => g.DefenseMult = 21.25m }, { "uncommon", (g) => g.DefenseMult = 16.3m }
            }
            }, {
            "hp",
            new Dictionary<string, GlyphMainStatInitializer> () { { "dark", (g) => g.HPFlat = 4003 }, { "legendary", (g) => g.HPFlat = 3577 }, { "epic", (g) => g.HPFlat = 0 }, { "rare", (g) => g.HPFlat = 0 }, { "common", (g) => g.HPFlat = 0 }, { "uncommon", (g) => g.HPFlat = 0 }
            }
            }, {
            "hp%",
            new Dictionary<string, GlyphMainStatInitializer> () { { "dark", (g) => throw new NotImplementedException () }, { "legendary", (g) => g.HPMult = 59 }, { "epic", (g) => g.HPMult = 0 }, { "rare", (g) => g.HPMult = 0 }, { "common", (g) => g.HPMult = 0 }, { "uncommon", (g) => g.HPMult = 0 }
            }
            }, {
            "attack",
            new Dictionary<string, GlyphMainStatInitializer> () { { "dark", (g) => throw new NotImplementedException () }, { "legendary", (g) => g.AttackFlat = 630 }, { "epic", (g) => g.AttackFlat = 0 }, { "rare", (g) => g.AttackFlat = 0 }, { "common", (g) => g.AttackFlat = 0 }, { "uncommon", (g) => g.AttackFlat = 0 }
            }
            }, {
            "attack%",
            new Dictionary<string, GlyphMainStatInitializer> () { { "dark", (g) => g.AttackMult = 48.75m }, { "legendary", (g) => g.AttackMult = 40.75m }, { "epic", (g) => g.AttackMult = 32.75m }, { "rare", (g) => g.AttackMult = 26.25m }, { "common", (g) => g.AttackMult = 21.25m }, { "uncommon", (g) => g.AttackMult = 16.3m }
            }
            },

            {
            "speed",
            new Dictionary<string, GlyphMainStatInitializer> () { { "dark", (g) => g.Speed = 30 }, { "legendary", (g) => g.Speed = 27 }, { "epic", (g) => g.Speed = 23 }, { "rare", (g) => g.Speed = 21 }, { "common", (g) => g.Speed = 20 }, { "uncommon", (g) => g.Speed = 19 }
            }
            }, {
            "accuracy",
            new Dictionary<string, GlyphMainStatInitializer> () { { "dark", (g) => g.Accuracy = 25 }, { "legendary", (g) => g.Accuracy = 23 }, { "epic", (g) => g.Accuracy = 20 }, { "rare", (g) => g.Accuracy = 18 }, { "common", (g) => g.Accuracy = 17 }, { "uncommon", (g) => g.Accuracy = 16 }
            }
            }, {
            "resistance",
            new Dictionary<string, GlyphMainStatInitializer> () { { "dark", (g) => g.Resistance = 25 }, { "legendary", (g) => g.Resistance = 23 }, { "epic", (g) => g.Resistance = 20 }, { "rare", (g) => g.Resistance = 18 }, { "common", (g) => g.Resistance = 17 }, { "uncommon", (g) => g.Resistance = 16 }
            }
            }, {
            "criticalDamage%",
            new Dictionary<string, GlyphMainStatInitializer> () { { "dark", (g) => g.CriticalDamage = 60.5m }, { "legendary", (g) => g.CriticalDamage = 51 }, { "epic", (g) => g.CriticalDamage = 41.5m }, { "rare", (g) => g.CriticalDamage = 32m }, { "common", (g) => g.CriticalDamage = 23.5m }, { "uncommon", (g) => g.CriticalDamage = 16m }
            }
            }, {
            "criticalChance%",
            new Dictionary<string, GlyphMainStatInitializer> () { { "dark", (g) => g.CriticalChance = 25 }, { "legendary", (g) => g.CriticalChance = 23 }, { "epic", (g) => g.CriticalChance = 20 }, { "rare", (g) => g.CriticalChance = 18 }, { "common", (g) => g.CriticalChance = 17 }, { "uncommon", (g) => g.CriticalChance = 16 }
            }
            }
        };

        private static Dictionary<int, string> GetCreatures(JObject jFile) {
            var creatures = new Dictionary<int, string>();
            var creaturesTokens = jFile.SelectToken("creatures");
            foreach (var token in creaturesTokens) {
                creatures[(int)token["id"]] = (string)token["name"];
            }
            return creatures;
        }

        public static IEnumerable<Glyph> GetGlyphs(string path, bool simulateMaxSpeed) {


            JObject jFile = JObject.Parse(path);
            var creatures = GetCreatures(jFile);

            var glyphTokens = jFile.SelectToken("runes");

            List<Glyph> glyphs = new List<Glyph>();
            foreach (var token in glyphTokens) {
                Glyph glyph = new Glyph();
                glyph.Id = (string)token["id"];
                string shape = (string)token["shape"];
                switch (shape) {
                    case "leaf":
                        glyph.Shape = Shape.Leaf;
                        break;
                    case "hex":
                        glyph.Shape = Shape.Hex;
                        break;
                    case "square":
                        glyph.Shape = Shape.Square;
                        break;
                    default:
                        throw new ApplicationException($"Unknown shape: {shape}");
                }

                string mainStat = (string)token["main"];
                string rarity = (string)token["rarity"];
                if (!MainStats.TryGetValue(mainStat, out var statValues)) {
                    throw new ApplicationException($"Main stats are not defined for {mainStat}");
                }

                if (!statValues.TryGetValue(rarity, out var valueInitializer)) {
                    throw new ApplicationException($"Main stat is not defined for {mainStat}/{rarity}");
                }

                valueInitializer(glyph);
                glyph.Rarity = rarity;
                glyph.SetName = (string)token["type"];
                glyphs.Add(glyph);
                var creature = token["creature"];
                if (creature != null) {
                    glyph.Creature = creatures[(int) creature];
                }

                int nbSub = 0;
                var tokenStats = token["stats"];
                var tokenDefense = tokenStats["defense"];
                if (tokenDefense != null) {
                    nbSub++;
                    foreach (var value in (JArray)tokenDefense) {
                        glyph.DefenseFlat += value.Value<int>();
                    }
                }
                var tokenDefensePer = tokenStats["defense%"];
                if (tokenDefensePer != null) {
                    nbSub++;
                    foreach (var value in (JArray)tokenDefensePer) {
                        glyph.DefenseMult += value.Value<decimal>();
                    }
                }
                var tokenHp = tokenStats["hp"];
                if (tokenHp != null) {
                    nbSub++;
                    foreach (var value in (JArray)tokenHp) {
                        glyph.HPFlat += value.Value<int>();
                    }
                }
                var tokenHpPer = tokenStats["hp%"];
                if (tokenHpPer != null) {
                    nbSub++;
                    foreach (var value in (JArray)tokenHpPer) {
                        glyph.HPMult += value.Value<decimal>();
                    }
                }
                var tokenAttack = tokenStats["attack"];
                if (tokenAttack != null) {
                    nbSub++;
                    foreach (var value in (JArray)tokenAttack) {
                        glyph.AttackFlat += value.Value<int>();
                    }
                }
                var tokenAttackPer = tokenStats["attack%"];
                if (tokenAttackPer != null) {
                    nbSub++;
                    foreach (var value in (JArray)tokenAttackPer) {
                        glyph.AttackMult += value.Value<decimal>();
                    }
                }
                var tokenAccuracy = tokenStats["accuracy"];
                if (tokenAccuracy != null) {
                    nbSub++;
                    foreach (var value in (JArray)tokenAccuracy) {
                        glyph.Accuracy += value.Value<int>();
                    }
                }
                var tokenResistance = tokenStats["resistance"];
                if (tokenResistance != null) {
                    nbSub++;
                    foreach (var value in (JArray)tokenResistance) {
                        glyph.Resistance += (int)(value.Value<decimal>() * 100);
                    }
                }
                var tokenCritDam = tokenStats["criticalDamage%"];
                if (tokenCritDam != null) {
                    nbSub++;
                    foreach (var value in (JArray)tokenCritDam) {
                        glyph.CriticalDamage += value.Value<int>();
                    }
                }
                var tokenCritChance = tokenStats["criticalChance%"];
                if (tokenCritChance != null) {
                    nbSub++;
                    foreach (var value in (JArray)tokenCritChance) {
                        glyph.CriticalChance += value.Value<int>();
                    }
                }
                var tokenSpeed = tokenStats["speed"];
                if (tokenSpeed != null) {
                    nbSub = 0;
                    foreach (var value in (JArray)tokenSpeed) {
                        glyph.Speed += (int)(value.Value<decimal>() * 1000);
                    }
                }

                if (simulateMaxSpeed && nbSub < 5) {
                    glyph.RealSpeed = glyph.Speed;
                    int level = (int)token["level"];
                    if (level < 16) {
                        glyph.Speed += 4;
                    }

                    if (level < 13) {
                        glyph.Speed += 4;
                    }

                    if (level < 10) {
                        glyph.Speed += 4;
                    }

                    if (level < 7) {
                        glyph.Speed += 4;
                    }

                    if (level < 4) {
                        glyph.Speed += 4;
                    }
                }
            }

            return glyphs;
        }
    }
}