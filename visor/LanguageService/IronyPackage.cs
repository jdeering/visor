using System;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Project;
using Visor.LanguageService.ReservedWords;

namespace Visor.LanguageService
{
    public class IronyPackage : ProjectPackage, IOleComponent
    {
        private uint _componentId;

        #region ProjectPackage components

        public override string ProductUserContext
        {
            get { return null; }
        }

        #endregion

        protected override void Initialize()
        {
            base.Initialize();
            var container = this as IServiceContainer;
            container.AddService(typeof (IronyLanguageService), ProfferLanguageService, true);

            SymitarDatabase.LoadFile();
            RepgenKeywords.LoadFile();
            RepgenFunctions.LoadFile();
            SymitarVariables.LoadFile();
        }

        private IronyLanguageService ProfferLanguageService(IServiceContainer container, Type serviceType)
        {
            IronyLanguageService languageService = null;
            if (typeof (IronyLanguageService) == serviceType)
            {
                languageService = new IronyLanguageService();
                languageService.SetSite(this);
                RegisterIdleCallback();
            }
            return languageService;
        }

        private void RegisterIdleCallback()
        {
            // register for idle time callbacks
            var mgr = GetService(typeof (SOleComponentManager)) as IOleComponentManager;
            if (_componentId == 0 && mgr != null)
            {
                var crinfo = new OLECRINFO[1];
                crinfo[0].cbSize = (uint) Marshal.SizeOf(typeof (OLECRINFO));
                crinfo[0].grfcrf = (uint) _OLECRF.olecrfNeedIdleTime |
                                   (uint) _OLECRF.olecrfNeedPeriodicIdleTime;
                crinfo[0].grfcadvf = (uint) _OLECADVF.olecadvfModal |
                                     (uint) _OLECADVF.olecadvfRedrawOff |
                                     (uint) _OLECADVF.olecadvfWarningsOff;
                crinfo[0].uIdleTimeInterval = 1000;
                mgr.FRegisterComponent(this, crinfo, out _componentId);
            }
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (_componentId != 0)
                {
                    var mgr = GetService(typeof (SOleComponentManager)) as IOleComponentManager;
                    if (mgr != null)
                    {
                        mgr.FRevokeComponent(_componentId);
                    }
                    _componentId = 0;
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        #region IOleComponent Members

        public int FContinueMessageLoop(uint uReason, IntPtr pvLoopData, MSG[] pMsgPeeked)
        {
            return 1;
        }

        public int FDoIdle(uint grfidlef)
        {
            var ls = GetService(typeof (IronyLanguageService)) as IronyLanguageService;

            if (ls != null)
            {
                ls.OnIdle((grfidlef & (uint) _OLEIDLEF.oleidlefPeriodic) != 0);
            }

            return 0;
        }

        public int FPreTranslateMessage(MSG[] pMsg)
        {
            return 0;
        }

        public int FQueryTerminate(int fPromptUser)
        {
            return 1;
        }

        public int FReserved1(uint dwReserved, uint message, IntPtr wParam, IntPtr lParam)
        {
            return 1;
        }

        public IntPtr HwndGetWindow(uint dwWhich, uint dwReserved)
        {
            return IntPtr.Zero;
        }

        public void OnActivationChange(IOleComponent pic, int fSameComponent, OLECRINFO[] pcrinfo, int fHostIsActivating,
                                       OLECHOSTINFO[] pchostinfo, uint dwReserved)
        {
        }

        public void OnAppActivate(int fActive, uint dwOtherThreadId)
        {
        }

        public void OnEnterState(uint uStateId, int fEnter)
        {
        }

        public void OnLoseActivation()
        {
        }

        public void Terminate()
        {
        }

        #endregion
    }
}