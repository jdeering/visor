using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Visor.LanguageService.ReservedWords
{
    public static class SymitarDatabase
    {
        private static List<Record> _records;
        private static Dictionary<string, List<Field>> _fields;

        public static List<Record> Records
        {
            get
            {
                if (_records == null)
                    LoadFile();
                return _records;
            }
        }

        public static Dictionary<string, List<Field>> Fields
        {
            get
            {
                if (_fields == null)
                    LoadFile();
                return _fields;
            }
        }

        public static void LoadFile()
        {
            var stream = Resource.GetStream("db.txt");
            if (stream == null) return;

            _records = new List<Record>();
            _fields = new Dictionary<string, List<Field>>();
            using (var reader = new StreamReader(stream))
            {
                while (reader.Peek() >= 0)
                {
                    var line = reader.ReadLine();
                    if (string.IsNullOrEmpty(line)) continue;

                    line = line.Trim();
                    if (line.StartsWith("***")) // record
                    {
                        AddRecord(line);
                    }
                    else // Field - under most recent record
                    {
                        AddField(line);
                    }
                }
            }
        }

        private static void AddRecord(string line)
        {
            var tokens = line.Split('|');
            _records.Add(new Record
            {
                Name = tokens[1].ToUpper(),
                FriendlyName = tokens[2]
            });
        }

        private static void AddField(string line)
        {
            var tokens = line.Split('|');
            var record = _records.Last().Name;
            if (!_fields.ContainsKey(record))
            {
                _fields.Add(record, new List<Field>());
            }

            _fields[record].Add(new Field
            {
                Name = tokens[0].ToUpper(),
                FriendlyName = tokens[1],
                Number = int.Parse(tokens[2]),
                DataType = GetTypeByNumber(int.Parse(tokens[3])),
                MaxValue = tokens[4] == "null" ? 0 : int.Parse(tokens[4])
            });
        }

        private static string GetTypeByNumber(int type)
        {
            switch (type)
            {
                case 0:
                    return "Character";
                case 1:
                    return "Character";
                case 2:
                    return "Rate";
                case 3:
                    return "Date";
                case 4:
                    return "Number";
                case 5:
                    return "Number";
                case 6:
                    return "Unknown";
                case 7:
                    return "Money";
                default:
                    return "Unknown";
            }
        }
    }

    public struct Record
    {
        public string Name;
        public string FriendlyName;
    }

    public struct Field
    {
        public string Name;
        public string FriendlyName;
        public int Number;
        public string DataType;
        public int MaxValue;
    }
}
