using System.Collections.Generic;

namespace Visor.LanguageService
{
    public class Methods : Microsoft.VisualStudio.Package.Methods
    {
        private readonly IList<Method> _methods;

        public Methods(IList<Method> methods)
        {
            _methods = methods;
        }

        public Method this[int index]
        {
            get { return _methods[index]; }
        }

        public override int GetCount()
        {
            return _methods.Count;
        }

        public override string GetName(int index)
        {
            return _methods[index].Name;
        }

        public override string GetDescription(int index)
        {
            return _methods[index].Description;
        }

        public override string GetType(int index)
        {
            return _methods[index].Type;
        }

        public override int GetParameterCount(int index)
        {
            return (_methods[index].Parameters == null) ? 0 : _methods[index].Parameters.Count;
        }

        public override void GetParameterInfo(int index, int paramIndex, out string name, out string display,
                                              out string description)
        {
            Parameter parameter = _methods[index].Parameters[paramIndex];
            name = parameter.Name;
            display = parameter.Display;
            description = parameter.Description;
        }
    }
}