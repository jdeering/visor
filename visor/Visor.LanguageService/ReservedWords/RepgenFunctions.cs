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
            var stream = Resource.GetStream("functions.txt");
            if (stream == null) return;

            _list = new List<Function>();
            using (var reader = new StreamReader(stream))
            {
                while (reader.Peek() >= 0)
                {
                    var line = reader.ReadLine();
                    if (string.IsNullOrEmpty(line.Trim())) continue;

                    if (line.StartsWith("\t"))
                    {
                        var tokens = line.Trim().Split('|');

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
                            var tokens = line.Trim().Split('|');
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
        public string Name;
        public string Description;
        public string ReturnTypes;
        public List<FunctionParameter> Parameters;
    }

    public struct FunctionParameter
    {
        public string Name;
        public string Description;
    }
}
