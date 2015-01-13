using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Package;
using Visor.Extensions;
using Visor.LanguageService.ReservedWords;

namespace Visor.LanguageService
{
    public class Resolver : IAstResolver
    {
        #region IASTResolver Members

        public IList<Declaration> FindCompletions(Source source, int line, int col)
        {
            // Used for intellisense.
            var declarations = new List<Declaration>();

            // Add keywords defined by grammar
            foreach (string keyword in RepgenKeywords.List)
            {
                declarations.Add(new Declaration("", keyword.Titleize(), 206, keyword.Titleize()));
            }
            foreach (Record record in SymitarDatabase.Records)
            {
                declarations.Add(new Declaration(record.FriendlyName, record.Name.Titleize(), 206,
                                                 record.Name.Titleize()));
            }

            declarations.Sort();
            return declarations;
        }

        public IList<Declaration> FindMembers(Source source, int line, int col)
        {
            // Find the token trigger
            string record = GetTriggerToken(source, line, col).ToUpper();

            if (!SymitarDatabase.Fields.ContainsKey(record)) return new List<Declaration>();

            var members = new List<Declaration>();
            foreach (Field field in SymitarDatabase.Fields[record])
            {
                members.Add(new Declaration(field.FriendlyName, field.Name, 206, field.Name));
            }

            members.Sort();
            return members;
        }

        public string FindQuickInfo(Source source, int line, int col)
        {
            return "unknown";
        }

        public IList<Method> FindMethods(Source source, int line, int col, string name)
        {
            var methods = new List<Method>();

            foreach (Function function in RepgenFunctions.List.Where(x => x.Name.ToLower() == name.ToLower()))
            {
                var method = new Method
                    {
                        Name = function.Name.Titleize(),
                        Description = function.Description,
                        Type = function.ReturnTypes,
                        Parameters = new List<Parameter>()
                    };

                foreach (FunctionParameter param in function.Parameters)
                {
                    method.Parameters.Add(
                        new Parameter
                            {
                                Name = param.Name,
                                Display = param.Name,
                                Description = param.Description
                            }
                        );
                }

                methods.Add(method);
            }


            return methods;
        }

        private string GetTriggerToken(Source source, int line, int col)
        {
            string sourceLine = source.GetLine(line).Substring(0, col - 1);
            var scanner = new LineScanner(Configuration.Grammar);
            scanner.SetSource(sourceLine, 0);

            var info = new TokenInfo();
            int state = 0;

            // Get last token before trigger point
            string result = "";
            while (scanner.ScanTokenAndProvideInfoAboutIt(info, ref state))
            {
                result = sourceLine.Substring(info.StartIndex, info.EndIndex - info.StartIndex + 1);
            }

            return result;
        }

        #endregion
    }
}