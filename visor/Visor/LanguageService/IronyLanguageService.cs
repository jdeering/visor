using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Visor.LanguageService.ReservedWords;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Package;
using Irony.Parsing;
using Visor.Extensions;
using TokenType = Microsoft.VisualStudio.Package.TokenType;

namespace Visor.LanguageService
{
    public class IronyLanguageService : Microsoft.VisualStudio.Package.LanguageService
    {
        private readonly Irony.Parsing.Grammar _grammar;
        private readonly Parser _parser;
        private ParsingContext _context;

        public IronyLanguageService()
        {
            _grammar = Configuration.Grammar;
            _parser = new Parser(_grammar);
            _context = new ParsingContext(_parser);
        }

        public IronyLanguageService(Irony.Parsing.Grammar grammar)
        {
            _grammar = grammar;
            _parser = new Parser(_grammar);
            _context = new ParsingContext(_parser);
        }


        #region MPF Accessor and Factory specialisation
        private LanguagePreferences _preferences;
        public override LanguagePreferences GetLanguagePreferences()
        {
            if (_preferences == null)
            {
                _preferences = new LanguagePreferences(Site,
                                                        typeof(IronyLanguageService).GUID,
                                                        Name);
                _preferences.Init();
            }

            return _preferences;
        }

        public override Microsoft.VisualStudio.Package.Source CreateSource(IVsTextLines buffer)
        {
            return new Source(this, buffer, GetColorizer(buffer));
        }

        private IScanner _scanner;
        public override IScanner GetScanner(IVsTextLines buffer)
        {
            return _scanner ?? (_scanner = new LineScanner(_grammar));
        }

        #endregion

        public override void OnIdle(bool periodic)
        {
            // from IronPythonLanguage sample
            // this appears to be necessary to get a parse request with ParseReason = Check?
            var src = GetSource(LastActiveTextView);
            if (src != null && src.LastParseTime >= Int32.MaxValue >> 12)
            {
                src.LastParseTime = 0;
            }
            base.OnIdle(periodic);
        }

        public override Microsoft.VisualStudio.Package.AuthoringScope ParseSource(ParseRequest req)
        {
            Debug.Print("ParseSource at ({0}:{1}), reason {2}", req.Line, req.Col, req.Reason);
            var source = (Source)GetSource(req.FileName);

            var scope = new AuthoringScope(source);

            switch (req.Reason)
            {
                case ParseReason.Check:
                    // This is where you perform your syntax highlighting.
                    // Parse entire source as given in req.Text.
                    // Store results in the AuthoringScope object.
                    var node = _parser.Parse(req.Text, req.FileName).Root;
                    source.ParseResult = node;

                    // Used for brace matching.
                    var braces = _parser.Context.OpenBraces;
                    foreach (var brace in braces)
                    {
                        var openBrace = new TextSpan
                            {
                                iStartLine = brace.Location.Line,
                                iStartIndex = brace.Location.Column,
                                iEndLine = brace.Location.Line,
                                iEndIndex = brace.Location.Column + brace.Length
                            };

                        if (brace.OtherBrace == null) continue;

                        var closeBrace = new TextSpan
                            {
                                iStartLine = brace.OtherBrace.Location.Line,
                                iStartIndex = brace.OtherBrace.Location.Column,
                                iEndLine = brace.OtherBrace.Location.Line,
                                iEndIndex = brace.OtherBrace.Location.Column + brace.OtherBrace.Length
                            };

                        if (source.Braces == null)
                        {
                            source.Braces = new List<TextSpan[]>();
                        }
                        source.Braces.Add(new TextSpan[2] { openBrace, closeBrace });
                    }

                    if (_parser.Context.CurrentParseTree.ParserMessages.Count > 0)
                    {
                        foreach (var error in _parser.Context.CurrentParseTree.ParserMessages)
                        {
                            var span = new TextSpan();
                            span.iStartLine = span.iEndLine = error.Location.Line;
                            span.iStartIndex = error.Location.Column;
                            span.iEndIndex = error.Location.Position;
                            req.Sink.AddError(req.FileName, error.Message, span, Severity.Error);
                        }
                    }
                    break;

                case ParseReason.DisplayMemberList:
                    // Parse the line specified in req.Line for the two
                    // tokens just before req.Col to obtain the identifier
                    // and the member connector symbol.

                    // Examine existing parse tree for members of the identifer
                    // and return a list of members in your version of the
                    // Declarations class as stored in the AuthoringScope
                    // object.
                    break;

                case ParseReason.MethodTip:
                    // Parse the line specified in req.Line for the token
                    // just before req.Col to obtain the name of the method
                    // being entered.

                    // Examine the existing parse tree for all method signatures
                    // with the same name and return a list of those signatures
                    // in your version of the Methods class as stored in the
                    // AuthoringScope object.
                    break;

                case ParseReason.HighlightBraces:
                case ParseReason.MemberSelectAndHighlightBraces:
                    if (source.Braces != null)
                    {
                        foreach (TextSpan[] brace in source.Braces)
                        {
                            if (brace.Length == 2)
                                req.Sink.MatchPair(brace[0], brace[1], 1);
                            else if (brace.Length >= 3)
                                req.Sink.MatchTriple(brace[0], brace[1], brace[2], 1);
                        }
                    }
                    break;
            }

            return scope;
        }

        /// <summary>
        /// Called to determine if the given location can have a breakpoint applied to it. 
        /// </summary>
        /// <param name="buffer">The IVsTextBuffer object containing the source file.</param>
        /// <param name="line">The line number where the breakpoint is to be set.</param>
        /// <param name="col">The offset into the line where the breakpoint is to be set.</param>
        /// <param name="pCodeSpan">
        /// Returns the TextSpan giving the extent of the code affected by the breakpoint if the 
        /// breakpoint can be set.
        /// </param>
        /// <returns>
        /// If successful, returns S_OK; otherwise returns S_FALSE if there is no code at the given 
        /// position or returns an error code (the validation is deferred until the debug engine is loaded). 
        /// </returns>
        /// <remarks>
        /// <para>
        /// CAUTION: Even if you do not intend to support the ValidateBreakpointLocation but your language 
        /// does support breakpoints, you must override the ValidateBreakpointLocation method and return a 
        /// span that contains the specified line and column; otherwise, breakpoints cannot be set anywhere 
        /// except line 1. You can return E_NOTIMPL to indicate that you do not otherwise support this 
        /// method but the span must always be set. The example shows how this can be done.
        /// </para>
        /// <para>
        /// Since the language service parses the code, it generally knows what is considered code and what 
        /// is not. Normally, the debug engine is loaded and the pending breakpoints are bound to the source. It is at this time the breakpoint location is validated. This method is a fast way to determine if a breakpoint can be set at a particular location without loading the debug engine.
        /// </para>
        /// <para>
        /// You can implement this method to call the ParseSource method with the parse reason of CodeSpan. 
        /// The parser examines the specified location and returns a span identifying the code at that 
        /// location. If there is code at the location, the span identifying that code should be passed to 
        /// your implementation of the CodeSpan method in your version of the AuthoringSink class. Then your 
        /// implementation of the ValidateBreakpointLocation method retrieves that span from your version of 
        /// the AuthoringSink class and returns that span in the pCodeSpan argument.
        /// </para>
        /// <para>
        /// The base method returns E_NOTIMPL.
        /// </para>
        /// </remarks>
        public override int ValidateBreakpointLocation(IVsTextBuffer buffer, int line, int col, TextSpan[] pCodeSpan)
        {
            // TODO: Add code to not allow breakpoints to be placed on non-code lines.
            // TODO: Refactor to allow breakpoint locations to span multiple lines.
            if (pCodeSpan != null)
            {
                pCodeSpan[0].iStartLine = line;
                pCodeSpan[0].iStartIndex = col;
                pCodeSpan[0].iEndLine = line;
                pCodeSpan[0].iEndIndex = col;
                if (buffer != null)
                {
                    int length;
                    buffer.GetLengthOfLine(line, out length);
                    pCodeSpan[0].iStartIndex = 0;
                    pCodeSpan[0].iEndIndex = length;
                }
                return VSConstants.S_OK;
            }
            else
            {
                return VSConstants.S_FALSE;
            }
        }

        public override ViewFilter CreateViewFilter(CodeWindowManager mgr, IVsTextView newView)
        {
            return new IronyViewFilter(mgr, newView);
        }

        public override string Name
        {
            get { return Configuration.Name; }
        }

        public override string GetFormatFilterList()
        {
            return Configuration.FormatList;
        }
    }
}