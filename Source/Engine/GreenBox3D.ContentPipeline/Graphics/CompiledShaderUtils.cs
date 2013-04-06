using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GreenBox3D.Graphics;
using GreenBox3D.Graphics.Detail;
using GreenBox3D.ContentPipeline.Graphics.Shading.Ast;

namespace GreenBox3D.ContentPipeline.Graphics
{
    public static class CompiledShaderUtils
    {
        private static Dictionary<string, VertexElementUsage> _string2vertexElementUsage;

        static CompiledShaderUtils()
        {
            _string2vertexElementUsage = new Dictionary<string, VertexElementUsage>();
            _string2vertexElementUsage["POSITION"] = VertexElementUsage.Position;
            _string2vertexElementUsage["COLOR"] = VertexElementUsage.Color;
            _string2vertexElementUsage["TEXCOORD"] = VertexElementUsage.TextureCoordinate;
            _string2vertexElementUsage["NORMAL"] = VertexElementUsage.Normal;
            _string2vertexElementUsage["BINORMAL"] = VertexElementUsage.Binormal;
            _string2vertexElementUsage["TANGENT"] = VertexElementUsage.Tangent;
            _string2vertexElementUsage["BLENDINDEX"] = VertexElementUsage.BlendIndices;
            _string2vertexElementUsage["BLENDWEIGHT"] = VertexElementUsage.BlendWeight;
            _string2vertexElementUsage["DEPTH"] = VertexElementUsage.Depth;
            _string2vertexElementUsage["FOG"] = VertexElementUsage.Fog;
            _string2vertexElementUsage["POINTSIZE"] = VertexElementUsage.PointSize;
            _string2vertexElementUsage["SAMPLE"] = VertexElementUsage.Sample;
            _string2vertexElementUsage["TESSELATEFACTOR"] = VertexElementUsage.TessellateFactor;
        }

        public static CompiledVariable VariableFromAst(Variable var)
        {
            EffectParameterClass parameterClass;
            EffectParameterType parameterType;
            int rowCount, columnCount;

            ExtractDefinitionsFromGlsl(var.Type, out parameterClass, out parameterType, out rowCount, out columnCount);

            return new CompiledVariable(var.Name, parameterClass, parameterType, var.Count, rowCount, columnCount);
        }

        public static CompiledInputVariable InputVariableFromAst(InputVariable var)
        {
            EffectParameterClass parameterClass;
            EffectParameterType parameterType;
            int rowCount, columnCount;
            VertexElementUsage usage;

            ExtractDefinitionsFromGlsl(var.Type, out parameterClass, out parameterType, out rowCount, out columnCount);

            if (!_string2vertexElementUsage.TryGetValue(var.Usage, out usage))
                throw new NotSupportedException();

            return new CompiledInputVariable(var.Name, parameterClass, parameterType, var.Count, rowCount, columnCount, usage, var.UsageIndex);
        }

        public static void ExtractDefinitionsFromGlsl(string type, out EffectParameterClass parameterClass, out EffectParameterType parameterType, out int rowCount, out int columnCount)
        {
            if (type.StartsWith("mat") || type.Substring(1, 3) == "mat")
                parameterClass = EffectParameterClass.Matrix;
            else if (type.StartsWith("vec") || type.Substring(1, 3) == "vec")
                parameterClass = EffectParameterClass.Vector;
            else if (type.StartsWith("sampler"))
                parameterClass = EffectParameterClass.Sampler;
            else
                parameterClass = EffectParameterClass.Scalar;

            switch (parameterClass)
            {
                case EffectParameterClass.Matrix:
                    {
                        char m = type[type.Length - 1];
                        char n = m;

                        if (type[type.Length - 2] == 'x')
                            n = type[type.Length - 3];

                        switch (type[0])
                        {
                            case 'd':
                                parameterType = EffectParameterType.Double;
                                break;
                            default:
                                parameterType = EffectParameterType.Single;
                                break;
                        }

                        rowCount = (n - '0');
                        columnCount = (m - '0');
                    }
                    break;
                case EffectParameterClass.Vector:
                    switch (type[0])
                    {
                        case 'd':
                            parameterType = EffectParameterType.Double;
                            break;
                        case 'i':
                            parameterType = EffectParameterType.Int32;
                            break;
                        case 'b':
                            parameterType = EffectParameterType.Bool;
                            break;
                        default:
                            parameterType = EffectParameterType.Single;
                            break;
                    }

                    rowCount = 1;
                    columnCount = type[type.Length - 1] - '0';
                    break;
                case EffectParameterClass.Sampler:
                    switch (type[type.Length - 2])
                    {
                        case '1':
                            parameterType = EffectParameterType.Texture1D;
                            break;
                        case '2':
                            parameterType = EffectParameterType.Texture2D;
                            break;
                        case '3':
                            parameterType = EffectParameterType.Texture3D;
                            break;
                        default:
                            if (type == "sampleCube")
                                parameterType = EffectParameterType.TextureCube;
                            else
                                throw new NotSupportedException();
                            break;
                    }
                    rowCount = 1;
                    columnCount = 1;
                    break;
                default:
                    switch (type)
                    {
                        case "int":
                        case "uint":
                            parameterType = EffectParameterType.Int32;
                            break;
                        case "bool":
                            parameterType = EffectParameterType.Bool;
                            break;
                        case "float":
                            parameterType = EffectParameterType.Single;
                            break;
                        case "double":
                            parameterType = EffectParameterType.Double;
                            break;
                        default:
                            throw new NotSupportedException();
                    }
                    rowCount = 1;
                    columnCount = 1;
                    break;
            }
        }
    }
}
