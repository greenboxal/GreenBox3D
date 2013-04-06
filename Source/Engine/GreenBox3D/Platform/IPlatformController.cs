using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenBox3D.Platform
{
    public interface IPlatformController : IServiceManager
    {
        void Initialize();
        void OnResize();
        void Update(GameTime gameTime);
        void Render(GameTime gameTime);
        void Shutdown();
    }
}
