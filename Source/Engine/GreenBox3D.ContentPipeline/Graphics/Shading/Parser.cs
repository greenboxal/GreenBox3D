// Parser.cs
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
using GreenBox3D.ContentPipeline.Graphics.Shading.Ast;
using GreenBox3D.ContentPipeline.CompilerServices;
using System.IO;

namespace GreenBox3D.ContentPipeline.Graphics.Shading
{
    public class Parser
    {
        private readonly ILoggerHelper _reporter;
        private readonly Scanner _scanner;
        private Token _tk;

        public Parser(Scanner scanner, ILoggerHelper reporter)
        {
            _scanner = scanner;
            _reporter = reporter;

            Next();
        }

        public CompilationUnit Parse()
        {
            CompilationUnit unit = new CompilationUnit();

            while (_tk.Type != TokenType.Eof)
            {
                Shader shader = null;

                if (_tk.Type == TokenType.Shader)
                {
                    if (!MatchShader(out shader))
                        return null;

                    unit.Add(shader);
                }
                else
                {
                    Report(MessageLevel.Error, _tk, _tk, "SH0001", "Unexpected token '{0}'", _tk.Type);
                    return null;
                }
            }

            return unit;
        }

        private bool MatchShader(out Shader shader)
        {
            string text;
            shader = new Shader();

            if (!Match(TokenType.Shader, true))
                return false;

            if (!Match(TokenType.String, out text, true))
                return false;

            shader.Name = text;

            if (!Match(TokenType.LeftBrackets))
                return false;

            while (_tk.Type != TokenType.RightBrackets)
            {
                switch (_tk.Type)
                {
                    case TokenType.Version:
                        {
                            Next();

                            if (!Match(TokenType.Number, out text))
                                return false;

                            shader.Version = int.Parse(text);
                        }
                        break;
                    case TokenType.Input:
                        {
                            Next();

                            if (!Match(TokenType.LeftBrackets))
                                return false;

                            while (_tk.Type != TokenType.RightBrackets)
                            {
                                InputVariable variable;

                                if (!MatchInputVariable(out variable))
                                    return false;

                                if (!Match(TokenType.Semicolon))
                                    return false;

                                shader.Input.Add(variable);
                            }

                            Next();
                        }
                        break;
                    case TokenType.Fallback:
                        {
                            Next();

                            if (!Match(TokenType.String, out text))
                                return false;

                            shader.Fallback = text;
                        }
                        break;
                    case TokenType.VertexGlsl:
                        {
                            Next();

                            if (_tk.Type != TokenType.String)
                            {
                                Report(MessageLevel.Error, _tk, _tk, "SH0003", "Expecting file path'");
                                return false;
                            }

                            shader.GlslVertexCode = MakePath(_tk);
                            Next();
                        }
                        break;
                    case TokenType.VertexHlsl:
                        {
                            Next();

                            if (_tk.Type != TokenType.String)
                            {
                                Report(MessageLevel.Error, _tk, _tk, "SH0003", "Expecting file path'");
                                return false;
                            }

                            shader.HlslVertexCode = MakePath(_tk);
                            Next();
                        }
                        break;
                    case TokenType.PixelGlsl:
                        {
                            Next();

                            if (_tk.Type != TokenType.String)
                            {
                                Report(MessageLevel.Error, _tk, _tk, "SH0003", "Expecting file path'");
                                return false;
                            }

                            shader.GlslPixelCode = MakePath(_tk);
                            Next();
                        }
                        break;
                    case TokenType.PixelHlsl:
                        {
                            Next();

                            if (_tk.Type != TokenType.String)
                            {
                                Report(MessageLevel.Error, _tk, _tk, "SH0003", "Expecting file path'");
                                return false;
                            }

                            shader.HlslPixelCode = MakePath(_tk);
                            Next();
                        }
                        break;
                    default:
                        Report(MessageLevel.Error, _tk, _tk, "SH0001", "Unexpected token '{0}'", _tk.Type);
                        break;
                }
            }

            Next();

            return true;
        }

        private bool MatchInputVariable(out InputVariable variable)
        {
            string text;
            variable = new InputVariable();

            if (!Match(TokenType.Identifier, out text))
                return false;

            variable.Name = text;

            if (!Match(TokenType.Colon))
                return false;

            if (!Match(TokenType.Identifier, out text))
                return false;

            variable.Usage = text;

            if (_tk.Type == TokenType.LeftSquare)
            {
                Next();

                if (!Match(TokenType.Number, out text))
                    return false;

                variable.UsageIndex = int.Parse(text);

                if (!Match(TokenType.RightSquare))
                    return false;
            }

            return true;
        }

        private bool Match(TokenType type, bool error = true)
        {
            if (_tk.Type == type)
            {
                Next();
                return true;
            }

            if (error)
                Report(MessageLevel.Error, _tk, _tk, "SH0001", "Unexpected token '{0}'", _tk.Type);

            return false;
        }

        private bool Match(TokenType type, out string text, bool error = true)
        {
            if (_tk.Type == type)
            {
                text = _tk.Text;
                Next();
                return true;
            }

            if (error)
                Report(MessageLevel.Error, _tk, _tk, "SH0002", "Unexpected token '{0}', expecting '{1}'", _tk.Type, type);

            text = null;
            return false;
        }

        private void Report(MessageLevel errorLevel, Token tk1, Token tk2, string errorCode, string message,
                            params object[] args)
        {
            _reporter.Log(errorLevel, errorCode, tk1.File, tk1.Line, tk1.Column, tk2.EndLine, tk2.EndColumn, message,
                          args);
        }

        private void Next()
        {
            _tk = _scanner.Scan();
        }

        private string MakePath(Token tk)
        {
            return Path.Combine(Path.GetDirectoryName(tk.File), tk.Text);
        }
    }
}
