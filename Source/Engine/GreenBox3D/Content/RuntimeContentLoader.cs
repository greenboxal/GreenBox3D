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
using GreenBox3D.Graphics;

namespace GreenBox3D.Content
{
    internal class RuntimeContentLoader
    {
        private static readonly Dictionary<Type, ReaderDescriptor> Readers;

        static RuntimeContentLoader()
        {
            AssemblyName[] references = Assembly.GetEntryAssembly().GetReferencedAssemblies();

            Readers = new Dictionary<Type, ReaderDescriptor>();

            foreach (ReaderDescriptor descriptor in from assembly in references.Select(Assembly.Load)
                                                    from type in assembly.GetTypes()
                                                    where (Attribute.IsDefined(type, typeof(ContentTypeReaderAttribute)) && type.BaseType != null)
                                                    select new ReaderDescriptor(type))
                Readers.Add(descriptor.Loadee, descriptor);
        }
        
        public static T LoadContent<T>(ContentManager manager, string filename) where T : class
        {
            ReaderDescriptor descriptor;

            if (!Readers.TryGetValue(typeof(T), out descriptor))
                return null;

            IContentTypeReader reader = (IContentTypeReader)Activator.CreateInstance(descriptor.Type);
            Stream stream = FileManager.OpenFile(filename + descriptor.Extension);

            if (stream == null)
                return null;

            object result = reader.Load(manager, stream);

            stream.Close();

            if (result != null)
            {
                manager.CacheObject(filename, result);

                if (result is GraphicsResource)
                    (result as GraphicsResource).Disposing += (sender, e) => manager.RemoveFromCache(filename);
            }

            return (T)result;
        }

        internal static object StartLoadRawObject(ContentManager manager, Type type, Stream stream)
        {
            ReaderDescriptor descriptor;

            if (!Readers.TryGetValue(type, out descriptor))
                return null;

            return ((IContentTypeReader)Activator.CreateInstance(descriptor.Type)).Load(manager, stream);
        }

        private class ReaderDescriptor
        {
            public readonly string Extension;
            public readonly Type Loadee;
            public readonly Type Type;

            public ReaderDescriptor(Type type)
            {
                Type = type;
                Loadee = type.BaseType.GenericTypeArguments[0];
                Extension = type.GetCustomAttribute<ContentTypeReaderAttribute>().Extension;
            }
        }
    }
}
