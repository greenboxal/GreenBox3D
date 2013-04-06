using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenBox3D
{
    public interface IServiceManager : IServiceProvider
    {
        T GetService<T>(Type serviceType);
        T GetService<T>();
        void RegisterService(Type serviceType, object service);
        void RemoveService(Type serviceType);
    }
}
