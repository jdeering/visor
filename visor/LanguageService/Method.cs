using System.Collections.Generic;

namespace Visor.LanguageService
{
    public struct Method
    {
        public string Description;
        public string Name;
        public IList<Parameter> Parameters;
        public string Type;
    }

    public struct Parameter
    {
        public string Description;
        public string Display;
        public string Name;
    }
}