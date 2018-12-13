using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Mmeg.MmegExtract
{
    public static class Translations
    {
        private static Dictionary<string, string> Texts;
        public static string GetText(string key)
        {
            if (Texts == null)
                LoadTexts();
            return Texts[key];
        }

        private static void LoadTexts()
        {
            Texts = new Dictionary<string, string>();
            foreach (var line in File.ReadAllLines("Data/creatures/names.txt"))
            {
                int index = line.IndexOf("=");
                if (index == -1 || index == line.Length - 1)
                {
                    Texts[line] = "";
                }
                else
                {
                    Texts[line.Substring(0, index)] = line.Substring(index + 1);
                }
            }
        }
    }
}
