using System;
using Irony.Parsing;
using Microsoft.VisualStudio.Package;
using TokenColor = Microsoft.VisualStudio.Package.TokenColor;
using TokenTriggers = Microsoft.VisualStudio.Package.TokenTriggers;
using TokenType = Microsoft.VisualStudio.Package.TokenType;

namespace Visor.LanguageService
{
    public class LineScanner : IScanner
    {
        private readonly Parser _parser;

        public LineScanner(Irony.Parsing.Grammar grammar)
        {
            _parser = new Parser(grammar);
            _parser.Context.Mode = ParseMode.VsLineScan;
        }

        public void SetSource(string source, int offset)
        {
            _parser.Scanner.VsSetSource(source, offset);
        }

        public bool ScanTokenAndProvideInfoAboutIt(TokenInfo tokenInfo, ref int state)
        {
            // Reads each token in a source line and performs syntax coloring.  It will continue to
            // be called for the source until false is returned.
            Token token = _parser.Scanner.VsReadToken(ref state);

            if (token != null && token.Category != TokenCategory.Error)
            {
                tokenInfo.StartIndex = token.Location.Position;
                tokenInfo.EndIndex = tokenInfo.StartIndex + token.Length - 1;

                if (token.EditorInfo == null)
                {
                    tokenInfo.Color = TokenColor.Identifier;
                    tokenInfo.Type = TokenType.Identifier;
                }
                else
                {
                    tokenInfo.Color = (TokenColor) token.EditorInfo.Color;
                    tokenInfo.Type = (TokenType) token.EditorInfo.Type;
                    if (token.KeyTerm != null)
                    {
                        tokenInfo.Trigger =
                            (TokenTriggers) token.KeyTerm.EditorInfo.Triggers;
                    }
                    else
                    {
                        tokenInfo.Trigger =
                            (TokenTriggers) token.EditorInfo.Triggers;
                    }
                }

                char c = token.Text[0];
                if (Char.IsPunctuation(c))
                {
                    HandlePunctuation(tokenInfo, token);
                }
                else if (tokenInfo.Type == TokenType.Identifier)
                {
                    tokenInfo.Trigger |= TokenTriggers.MemberSelect;
                }

                return true;
            }

            return false;
        }

        private void HandlePunctuation(TokenInfo tokenInfo, Token token)
        {
            char c = token.ToString()[0];
            switch (c)
            {
                case ':':
                    tokenInfo.Trigger |= TokenTriggers.MemberSelect;
                    break;
                case '(':
                    tokenInfo.Trigger |= TokenTriggers.ParameterStart;
                    break;
                case ')':
                    tokenInfo.Trigger |= TokenTriggers.ParameterEnd;
                    break;
                case ',':
                    tokenInfo.Trigger |= TokenTriggers.ParameterNext;
                    break;
            }

            if ("()".IndexOf(c) != -1)
            {
                tokenInfo.Trigger |= TokenTriggers.MatchBraces;
            }
        }
    }
}