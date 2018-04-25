﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Localization.Extensions
{
    internal static class DictionaryExtensions
    {
        internal static bool AreEquals(this Dictionary<string, string> d1, Dictionary<string, string> d2)
        {
            // compare num items
            if (d1.Keys.Count != d2.Keys.Count)
            {
                return false;
            }

            // compare keys names
            if (d1.Keys.Any(k => !d2.Keys.Contains(k)))
            {
                return false;
            }

            // compare values
            if (d1.Any(i => i.Value != d2[i.Key]))
            {
                return false;
            }

            return true;
        }
    }
}