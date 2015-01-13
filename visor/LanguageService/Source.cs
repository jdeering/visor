/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

using System.Collections.Generic;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Package;
using Microsoft.VisualStudio.TextManager.Interop;

namespace Visor.LanguageService
{
    public class Source : Microsoft.VisualStudio.Package.Source
    {
        public Source(Microsoft.VisualStudio.Package.LanguageService service, IVsTextLines textLines,
                      Colorizer colorizer)
            : base(service, textLines, colorizer)
        {
        }

        public object ParseResult { get; set; }
        public IList<TextSpan[]> Braces { get; set; }

        public override void OnCommand(IVsTextView textView, VSConstants.VSStd2KCmdID command, char ch)
        {
            base.OnCommand(textView, command, ch);

            if (command == VSConstants.VSStd2KCmdID.TYPECHAR && char.IsLetter(ch))
            {
                //TriggerAutoComplete(textView);
            }
        }

        private void TriggerAutoComplete(IVsTextView textView)
        {
            // Don't trigger if already displayed
            if (CompletionSet.IsDisplayed) return;

            int line, idx;
            textView.GetCaretPos(out line, out idx);
            TokenInfo info = GetTokenInfo(line, idx);

            // Do not fire completions when typing a comment, number, or "string"
            if (info.Color != TokenColor.Comment && info.Color != TokenColor.Number && info.Color != TokenColor.String)
            {
                Completion(textView, info, ParseReason.CompleteWord);
            }
        }
    }
}