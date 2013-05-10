using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenBox3D.Content
{
    public interface IDisposableContent : IDisposable
    {
        event EventHandler<EventArgs> Disposing;
    }
}
