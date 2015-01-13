using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Visor.LanguageService.ReservedWords
{
    public static class RepgenFunctions
    {
        private static List<Function> _list;

        public static List<Function> List
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
            Stream stream = Resource.GetStream("functions.txt");
            if (stream == null) return;

            _list = new List<Function>();
            using (var reader = new StreamReader(stream))
            {
                while (reader.Peek() >= 0)
                {
                    string line = reader.ReadLine();
                    if (string.IsNullOrEmpty(line.Trim())) continue;

                    if (line.StartsWith("\t"))
                    {
                        string[] tokens = line.Trim().Split('|');

                        _list.Last().Parameters.Add(new FunctionParameter
                            {
                                Name = tokens[0],
                                Description = tokens[1]
                            });
                    }
                    else
                    {
                        if (line.Contains("|"))
                        {
                            string[] tokens = line.Trim().Split('|');
                            _list.Add(new Function
                                {
                                    Name = tokens[0],
                                    Description = tokens[1],
                                    ReturnTypes = tokens[2],
                                    Parameters = new List<FunctionParameter>()
                                });
                        }
                        else
                        {
                            _list.Add(new Function
                                {
                                    Name = line.Trim(),
                                    Parameters = new List<FunctionParameter>()
                                });
                        }
                    }
                }
            }
        }
    }

    public struct Function
    {
        public string Description;
        public string Name;
        public List<FunctionParameter> Parameters;
        public string ReturnTypes;
    }

    public struct FunctionParameter
    {
        public string Description;
        public string Name;
    }
}