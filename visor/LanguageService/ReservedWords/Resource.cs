using System.IO;
using System.Linq;
using System.Reflection;

namespace Visor.LanguageService.ReservedWords
{
    public static class Resource
    {
        public static Stream GetStream(string filename)
        {
            string[] names = Assembly.GetExecutingAssembly().GetManifestResourceNames();

            return
                (from name in names
                 where name.Contains(filename)
                 select Assembly.GetExecutingAssembly().GetManifestResourceStream(name)).FirstOrDefault();
        }
    }
}