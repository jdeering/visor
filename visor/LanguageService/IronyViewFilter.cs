using System;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Package;
using Microsoft.VisualStudio.TextManager.Interop;

namespace Visor.LanguageService
{
    public class IronyViewFilter : ViewFilter
    {
        public IronyViewFilter(CodeWindowManager mgr, IVsTextView view)
            : base(mgr, view)
        {

        }

        public override void HandlePostExec(ref Guid guidCmdGroup, uint nCmdId, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut, bool bufferWasChanged)
        {
            if (guidCmdGroup == typeof(VSConstants.VSStd2KCmdID).GUID)
            {
                VSConstants.VSStd2KCmdID cmd = (VSConstants.VSStd2KCmdID)nCmdId;
                switch (cmd)
                {
                    case VSConstants.VSStd2KCmdID.UP:
                    case VSConstants.VSStd2KCmdID.UP_EXT:
                    case VSConstants.VSStd2KCmdID.UP_EXT_COL:
                    case VSConstants.VSStd2KCmdID.DOWN:
                    case VSConstants.VSStd2KCmdID.DOWN_EXT:
                    case VSConstants.VSStd2KCmdID.DOWN_EXT_COL:
                        Source.OnCommand(TextView, cmd, '\0');
                        return;
                }
            }


            base.HandlePostExec(ref guidCmdGroup, nCmdId, nCmdexecopt, pvaIn, pvaOut, bufferWasChanged);
        }
    }
}
