using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenBox3D.Graphics.Detail
{
    public class CompiledInputVariable : CompiledVariable
    {
        public VertexElementUsage Usage { get; private set; }
        public int UsageIndex { get; private set; }

        public CompiledInputVariable(string name, EffectParameterClass parameterClass, EffectParameterType parameterType, int count, int rowCount, int columnCount, VertexElementUsage usage, int usageIndex)
            : base(name, parameterClass, parameterType, count, rowCount, columnCount)
        {
            Usage = usage;
            UsageIndex = usageIndex;
        }
    }
}
