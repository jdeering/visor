using System;
using System.Collections.Generic;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Package;
using Microsoft.VisualStudio.TextManager.Interop;

namespace Visor.LanguageService
{
    public class AuthoringScope : Microsoft.VisualStudio.Package.AuthoringScope
    {
        private readonly IAstResolver _resolver;
        private readonly Source _source;

        public AuthoringScope(Source source)
        {
            _source = source;
            _resolver = new Resolver();
        }

        // ParseReason.QuickInfo
        public override string GetDataTipText(int line, int col, out TextSpan span)
        {
            span = new TextSpan();
            return null;
        }

        // ParseReason.CompleteWord
        // ParseReason.DisplayMemberList
        // ParseReason.MemberSelect
        // ParseReason.MemberSelectAndHilightBraces
        public override Microsoft.VisualStudio.Package.Declarations GetDeclarations(IVsTextView view, int line, int col,
                                                                                    TokenInfo info, ParseReason reason)
        {
            IList<Declaration> declarations;
            switch (reason)
            {
                case ParseReason.CompleteWord:
                    declarations = _resolver.FindCompletions(_source, line, col);
                    break;
                case ParseReason.DisplayMemberList:
                case ParseReason.MemberSelect:
                case ParseReason.MemberSelectAndHighlightBraces:
                    declarations = _resolver.FindMembers(_source, line, col);
                    break;
                default:
                    throw new ArgumentException("reason");
            }

            return new Declarations(declarations);
        }

        // ParseReason.GetMethods
        public override Microsoft.VisualStudio.Package.Methods GetMethods(int line, int col, string name)
        {
            return new Methods(_resolver.FindMethods(_source, line, col, name));
        }

        // ParseReason.Goto
        public override string Goto(VSConstants.VSStd97CmdID cmd, IVsTextView textView, int line, int col,
                                    out TextSpan span)
        {
            span = new TextSpan();
            return null;
        }
    }
}