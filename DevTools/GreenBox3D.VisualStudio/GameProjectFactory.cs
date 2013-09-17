using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Shell.Flavor;

namespace GreenBox3D.VisualStudio
{
    [Guid(GuidList.GameProjectFactory)]
    public class GameProjectFactory : FlavoredProjectFactoryBase
    {
        private readonly GreenBox3DPackage _package;

        public GameProjectFactory(GreenBox3DPackage package)
        {
            _package = package;
        }

        protected override object PreCreateForOuter(IntPtr outerProjectIUnknown)
        {
            return new GameFlavoredProject(_package);
        }
    }
}
