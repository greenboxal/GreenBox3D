using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Platform;

namespace GreenBox3D.Platform
{
    public interface IInternalGameWindow : IGameWindow
    {
        IWindowInfo WindowInfo { get; }
    }
}
