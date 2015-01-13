using System.Collections.Generic;

namespace Visor.LanguageService
{
    internal interface IAstResolver
    {
        IList<Declaration> FindCompletions(Source source, int line, int col);
        IList<Declaration> FindMembers(Source source, int line, int col);
        string FindQuickInfo(Source source, int line, int col);
        IList<Method> FindMethods(Source source, int line, int col, string name);
    }
}