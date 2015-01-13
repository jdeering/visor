using System.Collections.Generic;
using System.IO;

namespace Visor.LanguageService.ReservedWords
{
    public static class SymitarVariables
    {
        private static List<Variable> _list;

        public static List<Variable> List
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
            Stream stream = Resource.GetStream("vars.txt");
            if (stream == null) return;

            _list = new List<Variable>();

            using (var reader = new StreamReader(stream))
            {
                while (reader.Peek() >= 0)
                {
                    string line = reader.ReadLine();
                    if (string.IsNullOrEmpty(line)) continue;

                    string[] tokens = line.Trim().Split('|');

                    _list.Add(new Variable
                        {
                            Name = tokens[0],
                            Description = tokens[1],
                            Type = tokens[3].ToLower()
                        });
                }
            }
        }
    }

    public struct Variable
    {
        public string Description;
        public string Name;
        public string Type;
    }
}