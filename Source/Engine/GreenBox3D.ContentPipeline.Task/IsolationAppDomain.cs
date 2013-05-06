// IsolationAppDomain.cs
// 
// Copyright (c) 2013 The GreenBox Development LLC, all rights reserved
// 
// This file is a proprietary part of GreenBox3D, disclosing the content
// of this file without the owner consent may lead to legal actions
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

            _domain = AppDomain.CreateDomain(us.GetName().Name, null, new AppDomainSetup
            {
                ApplicationBase = Path.GetDirectoryName(us.Location),
                ShadowCopyFiles = "True"
            });
        }

        public void Dispose()
        {
            if (_domain == null)
                return;

            AppDomain.Unload(_domain);
            _domain = null;
        }

        public T CreateProxy<T>()
        {
            return (T)_domain.CreateInstanceAndUnwrap(typeof(T).Assembly.FullName, typeof(T).FullName);
        }
    }
}
