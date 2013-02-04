﻿using System.Collections.Generic;
using System.IO;

namespace Visor.LanguageService.ReservedWords
{
    public static class RepgenKeywords
    {
        private static List<string> _list;
        public static List<string> List
        {
            get
            {
                if (_list == null)
                    LoadFile();
                return _list;
            }
        }

        public static void LoadFile()
        {
            var stream = Resource.GetStream("keywords.txt");
            if (stream == null) return;

            _list = new List<string>();
            using (var reader = new StreamReader(stream))
            {
                while (reader.Peek() >= 0)
                {
                    var line = reader.ReadLine();
                    if (string.IsNullOrEmpty(line)) continue;

                    var tokens = line.Trim().Split('|');

                    _list.Add(tokens[0].ToLower());
                }
            }
        }
    }
}
