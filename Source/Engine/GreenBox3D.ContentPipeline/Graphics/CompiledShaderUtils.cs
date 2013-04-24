// CompiledShaderUtils.cs
// 
// Copyright (c) 2013 The GreenBox Development LLC, all rights reserved
// 
// This file is a proprietary part of GreenBox3D, disclosing the content
// of this file without the owner consent may lead to legal actions

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
        private static readonly Dictionary<string, VertexElementUsage> _string2vertexElementUsage;

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

        public static CompiledInputVariable InputVariableFromAst(InputVariable var)
        {
            VertexElementUsage usage;

            if (!_string2vertexElementUsage.TryGetValue(var.Usage, out usage))
                throw new NotSupportedException();

            return new CompiledInputVariable(var.Name, usage, var.UsageIndex);
        }

        public static void ExtractDefinitionsFromGlsl(string type, out ShaderParameterClass parameterClass,
                                                      out ShaderParameterType parameterType, out int rowCount,
                                                      out int columnCount)
        {
            if (type.StartsWith("mat") || type.Substring(1, 3) == "mat")
                parameterClass = ShaderParameterClass.Matrix;
            else if (type.StartsWith("vec") || type.Substring(1, 3) == "vec")
                parameterClass = ShaderParameterClass.Vector;
            else if (type.StartsWith("sampler"))
                parameterClass = ShaderParameterClass.Sampler;
            else
                parameterClass = ShaderParameterClass.Scalar;

            switch (parameterClass)
            {
                case ShaderParameterClass.Matrix:
                    {
                        char m = type[type.Length - 1];
                        char n = m;

                        if (type[type.Length - 2] == 'x')
                            n = type[type.Length - 3];

                        switch (type[0])
                        {
                            case 'd':
                                parameterType = ShaderParameterType.Double;
                                break;
                            default:
                                parameterType = ShaderParameterType.Single;
                                break;
                        }

                        rowCount = (n - '0');
                        columnCount = (m - '0');
                    }
                    break;
                case ShaderParameterClass.Vector:
                    switch (type[0])
                    {
                        case 'd':
                            parameterType = ShaderParameterType.Double;
                            break;
                        case 'i':
                            parameterType = ShaderParameterType.Int32;
                            break;
                        case 'b':
                            parameterType = ShaderParameterType.Bool;
                            break;
                        default:
                            parameterType = ShaderParameterType.Single;
                            break;
                    }

                    rowCount = 1;
                    columnCount = type[type.Length - 1] - '0';
                    break;
                case ShaderParameterClass.Sampler:
                    switch (type[type.Length - 2])
                    {
                        case '1':
                            parameterType = ShaderParameterType.Texture1D;
                            break;
                        case '2':
                            parameterType = ShaderParameterType.Texture2D;
                            break;
                        case '3':
                            parameterType = ShaderParameterType.Texture3D;
                            break;
                        default:
                            if (type == "sampleCube")
                                parameterType = ShaderParameterType.TextureCube;
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
                            parameterType = ShaderParameterType.Int32;
                            break;
                        case "bool":
                            parameterType = ShaderParameterType.Bool;
                            break;
                        case "float":
                            parameterType = ShaderParameterType.Single;
                            break;
                        case "double":
                            parameterType = ShaderParameterType.Double;
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
