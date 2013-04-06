using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenBox3D.Graphics.Detail
{
    public class CompiledVariable
    {
        public string Name { get; private set; }
        public EffectParameterClass EffectParameterClass { get; private set; }
        public EffectParameterType EffectParameterType { get; private set; }
        public int Count { get; private set; }
        public int RowCount { get; private set; }
        public int ColumnCount { get; private set; }

        public CompiledVariable(string name, EffectParameterClass parameterClass, EffectParameterType parameterType, int count, int rowCount, int columnCount)
        {
            Name = name;
            EffectParameterClass = parameterClass;
            EffectParameterType = parameterType;
            Count = count;
            RowCount = rowCount;
            ColumnCount = columnCount;
        }
    }
}
