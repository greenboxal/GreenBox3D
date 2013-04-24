using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenBox3D.Graphics
{
    public class ShaderParameterCollection : IReadOnlyCollection<IShaderParameter>
    {
        private readonly Dictionary<string, IShaderParameter> _internal;

        public ShaderParameterCollection(IEnumerable<IShaderParameter> parameters)
        {
            _internal = new Dictionary<string, IShaderParameter>(StringComparer.InvariantCultureIgnoreCase);

            foreach (IShaderParameter parameter in parameters)
                _internal[parameter.Name] = parameter;
        }

        public IShaderParameter this[int index]
        {
            get { return _internal.Values.ElementAt(index); }
        }

        public IShaderParameter this[string name]
        {
            get { return _internal[name]; }
        }

        public int Count
        {
            get { return _internal.Count; }
        }

        public IEnumerator<IShaderParameter> GetEnumerator()
        {
            return _internal.Values.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
