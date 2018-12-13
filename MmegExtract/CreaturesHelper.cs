using Mmeg.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace Mmeg.MmegExtract
{
    public static class CreaturesHelper
    {
        public static List<Creature> LoadAllCreatures()
        {
            List<Creature> creatures = new List<Creature>();
            foreach (var file in Directory.GetFiles("Data/creatures", "*.xml"))
            {
                LoadFile(file, creatures);
            }
            return creatures;
        }

        private static void LoadFile(string file, List<Creature> creatures)
        {
            XmlDocument xmlFile = new XmlDocument();
            xmlFile.LoadXml(File.ReadAllText(file));
            foreach (XmlNode xmlCreature in xmlFile.SelectNodes("//Definition"))
            {
                var playable = xmlCreature.Attributes["playable"]?.Value;
                if (playable != "true")
                    continue;
                Creature creature = new Creature();
                creature.Id = xmlCreature.Attributes["sku"]?.Value;
                creature.Name = Translations.GetText(xmlCreature.Attributes["name"]?.Value);
                creature.EvolvesTo = xmlCreature.Attributes["evolvesTo"]?.Value;
                creature.Accuracy = decimal.Parse(xmlCreature.Attributes["accuracy"]?.Value ?? "0") * 100;
                creature.Attack = int.Parse(xmlCreature.Attributes["attack"]?.Value ?? "0");
                creature.CriticalChance = int.Parse(xmlCreature.Attributes["criticalChance"]?.Value ?? "0");
                creature.CriticalDamage = int.Parse(xmlCreature.Attributes["criticalDamage"]?.Value ?? "0");
                creature.Defense = int.Parse(xmlCreature.Attributes["defense"]?.Value ?? "0");
                creature.HP = int.Parse(xmlCreature.Attributes["hp"]?.Value ?? "0");
                creature.Resistance = (int) (100 * decimal.Parse(xmlCreature.Attributes["resistance"]?.Value ?? "0"));
                creature.Speed = (int)(1000 * decimal.Parse(xmlCreature.Attributes["speed"]?.Value ?? "0"));
                creatures.Add(creature);
            }
        }
    }
}
