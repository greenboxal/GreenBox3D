using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using GreenBox3D.Graphics;

namespace GreenBox3D.Platform
{
    public abstract class GamePlatform : IDisposable
    {
        protected IPlatformController Controller { get; private set; }

        public abstract bool Running { get; }
        public abstract bool VSync { get; set; }
        public abstract IGameWindow Window { get; }

        protected GamePlatform(IPlatformController controller)
        {
            Controller = controller;
        }

        public abstract void Run();
        public abstract void InitializeGraphics(PresentationParameters parameters);
        public abstract void InitializeInput();
        public abstract void SkipFrame();
        public abstract void Exit();
        public abstract void Dispose();

        internal static GamePlatform Create(IPlatformController controller)
        {
            // TODO: Implement this
            Assembly assembly = Assembly.LoadFrom("../Build/GreenBox3D.Platform.Windows.dll");
            return (GamePlatform)assembly.CreateInstance("GreenBox3D.Platform.Windows.WindowsGamePlatform", false, BindingFlags.Default, null, new object[] { controller }, null, null);
        }
    }
}
