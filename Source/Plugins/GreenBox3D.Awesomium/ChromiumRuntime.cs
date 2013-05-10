using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xilium.CefGlue;

namespace GreenBox3D.Awesomium
{
    public static class ChromiumRuntime
    {
        private static GuiApp _app;

        internal static bool SelectedNodeEditable { get; set; }

        public static int Initialize(string[] args)
        {
            CefRuntime.Load();

            var settings = new CefSettings();
            settings.MultiThreadedMessageLoop = CefRuntime.Platform == CefRuntimePlatform.Windows;
            settings.ReleaseDCheckEnabled = true;
            settings.PackLoadingDisabled = true;
            settings.SingleProcess = true;

#if DEBUG
            settings.LogSeverity = CefLogSeverity.Verbose;
            settings.LogFile = "cef.log";
#else
            settings.LogSeverity = CefLogSeverity.Disable;
#endif

            var argv = args;
            if (CefRuntime.Platform != CefRuntimePlatform.Windows)
            {
                argv = new string[args.Length + 1];
                Array.Copy(args, 0, argv, 1, args.Length);
                argv[0] = "-";
            }

            CefMainArgs mainArgs = new CefMainArgs(argv);
            int exitCode = CefRuntime.ExecuteProcess(mainArgs, null);

            if (exitCode != -1)
                return exitCode;

            if (args.Any(arg => arg.StartsWith("--type=")))
                return -2;

            _app = new GuiApp();
            CefRuntime.Initialize(mainArgs, settings, _app);

            return -1;
        }

        public static void Update()
        {
            try
            {
             //   CefRuntime.DoMessageLoopWork();
            }
            catch (AccessViolationException)
            {
            }
        }

        public static void Shutdown()
        {
            CefRuntime.Shutdown();
        }
    }
}
