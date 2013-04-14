// RuntimeContentLoader.cs
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
using System.Text;
using System.Threading.Tasks;

namespace GreenBox3D.Content
{
    internal class RuntimeContentLoader
    {
        #region Fields

        private static readonly Dictionary<Type, ReaderDescriptor> _readers;

        #endregion

        #region Constructors and Destructors

        static RuntimeContentLoader()
        {
            AssemblyName[] references = Assembly.GetEntryAssembly().GetReferencedAssemblies();

            _readers = new Dictionary<Type, ReaderDescriptor>();

            foreach (ReaderDescriptor descriptor in from assembly in references.Select(Assembly.Load)
                                                    from type in assembly.GetTypes()
                                                    where Attribute.IsDefined(type, typeof(ContentTypeReaderAttribute))
                                                    select new ReaderDescriptor(type))
                _readers.Add(descriptor.Loadee, descriptor);
        }

        #endregion

        #region Public Methods and Operators

        public static T LoadContent<T>(ContentManager manager, string filename) where T : class
        {
            ReaderDescriptor descriptor;

            if (!_readers.TryGetValue(typeof(T), out descriptor))
                return null;

            IContentTypeReader reader = (IContentTypeReader)Activator.CreateInstance(descriptor.Type);
            Stream stream = FileManager.OpenFile(filename + descriptor.Extension);

            if (stream == null)
                return null;

            object result = reader.Load(manager, stream);

            if (result != null)
                manager.CacheObject(filename, result);

            return (T)result;
        }

        #endregion

        private class ReaderDescriptor
        {
            #region Fields

            public readonly string Extension;
            public readonly Type Loadee;
            public readonly Type Type;

            #endregion

            #region Constructors and Destructors

            public ReaderDescriptor(Type type)
            {
                Type = type;
                Loadee = type.BaseType.GenericTypeArguments[0];
                Extension = type.GetCustomAttribute<ContentTypeReaderAttribute>().Extension;
            }

            #endregion
        }
    }
}
