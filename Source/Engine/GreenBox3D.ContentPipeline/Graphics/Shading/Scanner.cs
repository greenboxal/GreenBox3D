using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenBox3D.ContentPipeline.Graphics.Shading
{
    public class Scanner
    {
        private TextReader _reader;
        private int _line, _column;
        private string _filename;
        private char _ch;
        private bool _isEof;

        public Scanner(string filename, TextReader reader)
        {
            _filename = filename;
            _reader = reader;
            _line = 1;
            _column = 0;
            _isEof = false;

            Next();
        }

        public Token Scan()
        {
            Token token = new Token();

            SkipWhitespace();

            token.File = _filename;
            token.Line = _line;
            token.Column = _column;

            if (_isEof)
            {
                token.Type = TokenType.Eof;
            }
            else if (_ch == '"')
            {
                MatchString(token);
            }
            else if (char.IsLetter(_ch) || _ch == '_')
            {
                MatchIdentifier(token);
            }
            else if (char.IsNumber(_ch))
            {
                MatchNumber(token);
            }
            else
            {
                switch (_ch)
                {
                    case '{':
                        token.Type = TokenType.LeftBrackets;
                        break;
                    case '}':
                        token.Type = TokenType.RightBrackets;
                        break;
                    case '[':
                        token.Type = TokenType.LeftSquare;
                        break;
                    case ']':
                        token.Type = TokenType.RightSquare;
                        break;
                    case ':':
                        token.Type = TokenType.Colon;
                        break;
                    case ';':
                        token.Type = TokenType.Semicolon;
                        break;
                    case '/':
                        Next();

                        if (_ch == '/')
                        {
                            int line = _line;
                            while (_line == line)
                                Next();

                            return Scan();
                        }
                        else
                        {
                            token.Type = TokenType.Error;
                            token.Text = "/";
                            return token;
                        }
                    default:
                        token.Type = TokenType.Error;
                        token.Text = _ch.ToString();
                        break;
                }

                Next();
            }

            token.EndLine = _line;
            token.EndColumn = _column;

            return token;
        }

        private void MatchString(Token token)
        {
            string text = "";

            while (true)
            {
                Next();

                if (_ch == '"')
                    break;

                text += _ch;
            }

            Next();

            token.Type = TokenType.String;
            token.Text = text;
        }

        private void MatchIdentifier(Token token)
        {
            string text = "";

            do
            {
                text += _ch;
                Next();
            }
            while (char.IsLetterOrDigit(_ch) || _ch == '_');

            token.Text = text;

            switch (text.ToLower())
            {
                case "shader":
                    token.Type = TokenType.Shader;
                    break;
                case "version":
                    token.Type = TokenType.Version;
                    break;
                case "input":
                    token.Type = TokenType.Input;
                    break;
                case "parameters":
                    token.Type = TokenType.Parameters;
                    break;
                case "globals":
                    token.Type = TokenType.Globals;
                    break;
                case "passes":
                    token.Type = TokenType.Passes;
                    break;
                case "pass":
                    token.Type = TokenType.Pass;
                    break;
                case "vertexglsl":
                    token.Type = TokenType.VertexGlsl;
                    break;
                case "pixelglsl":
                    token.Type = TokenType.PixelGlsl;
                    break;
                case "vertexhlsl":
                    token.Type = TokenType.VertexHlsl;
                    break;
                case "pixelhlsl":
                    token.Type = TokenType.PixelHlsl;
                    break;
                case "fallback":
                    token.Type = TokenType.Fallback;
                    break;
                default:
                    token.Type = TokenType.Identifier;
                    break;
            }
        }

        private void MatchNumber(Token token)
        {
            string text = "";

            do
            {
                text += _ch;
                Next();
            } 
            while (char.IsNumber(_ch));

            token.Type = TokenType.Number;
            token.Text = text;
        }

        private void SkipWhitespace()
        {
            while (char.IsWhiteSpace(_ch))
                Next();
        }

        private void Next()
        {
            int ch = _reader.Read();

            if (ch == -1)
            {
                _isEof = true;
            }
            else if (ch == '\n')
            {
                _line++;
                _column = 0;
                Next();
            }
            else if (ch == '\r')
            {
                // Ignore
                Next();
            }
            else
            {
                _column++;
                _ch = (char)ch;
            }
        }
    }
}
