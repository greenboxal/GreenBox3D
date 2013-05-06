// ShaderParameterCollection.cs
// 
// Copyright (c) 2013 The GreenBox Development LLC, all rights reserved
// 
// This file is a proprietary part of GreenBox3D, disclosing the content
// of this file without the owner consent may lead to legal actions
#if DESKTOP

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenBox3D.Graphics
{
    public class ShaderParameterCollection : IReadOnlyCollection<ShaderParameter>
    {
        private readonly Dictionary<string, ShaderParameter> _internal;

        public ShaderParameterCollection(IEnumerable<ShaderParameter> parameters)
        {
            _internal = new Dictionary<string, ShaderParameter>(StringComparer.InvariantCultureIgnoreCase);

            foreach (ShaderParameter parameter in parameters)
                _internal[parameter.Name] = parameter;
        }

        public ShaderParameter this[int index]
        {
            get { return _internal.Values.ElementAt(index); }
        }

        public ShaderParameter this[string name]
        {
            get { return _internal[name]; }
        }

        public int Count
        {
            get { return _internal.Count; }
        }

        public IEnumerator<ShaderParameter> GetEnumerator()
        {
            return _internal.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}

#endif
