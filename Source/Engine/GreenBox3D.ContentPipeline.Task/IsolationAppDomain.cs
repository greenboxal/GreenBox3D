using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace GreenBox3D.ContentPipeline.Task
{
    public class IsolationAppDomain : IDisposable
    {
        private AppDomain _domain;

        public IsolationAppDomain()
        {
            Assembly us = typeof(IsolationAppDomain).Assembly;

            _domain = AppDomain.CreateDomain(us.GetName().Name, (Evidence)null, new AppDomainSetup()
            {
                ApplicationBase = Path.GetDirectoryName(us.Location),
                ShadowCopyFiles = "True"
            });
        }

        public T CreateProxy<T>()
        {
            return (T)_domain.CreateInstanceAndUnwrap(typeof(T).Assembly.FullName, typeof(T).FullName);
        }

        public void Dispose()
        {
            if (_domain == null)
                return;

            AppDomain.Unload(_domain);
            _domain = null;
        }
    }
}
