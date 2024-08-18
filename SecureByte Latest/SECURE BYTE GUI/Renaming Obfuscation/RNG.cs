using System;
using System.Collections.Generic;

namespace Protections.Renamer
{
    public enum Schemes
    {
        Safe,      
        Custom
    }
    internal class RNG
    {
        static List<string> UsedNames;
        static Random random = new Random();
        public static string customstr { get; set; }
        public static string Generate(string generated, Schemes schemes)
        {
            UsedNames = new List<string>();
            switch (schemes)
            {
                case Schemes.Safe:
                    if (!UsedNames.Contains(generated))
                    {
                        generated = ICore.Safe.GenerateRandomLetters(random.Next(25, 100));
                        UsedNames.Add(generated);
                    }
                    break;
                case Schemes.Custom:
                    if (!UsedNames.Contains(generated))
                    {
                        generated = string.Concat(customstr, "_", ICore.Safe.GenerateRandomLetters(random.Next(2, 50)));
                        UsedNames.Add(generated);
                    }
                    break;               
            }
            return generated;
        }
    }
}
