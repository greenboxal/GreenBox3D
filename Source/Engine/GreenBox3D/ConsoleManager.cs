// ConsoleManager.cs
// 
// Copyright (c) 2013 The GreenBox Development LLC, all rights reserved
// 
// This file is a proprietary part of GreenBox3D, disclosing the content
// of this file without the owner consent may lead to legal actions
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenBox3D
{
    public static class ConsoleManager
    {
        private static readonly Dictionary<string, IConsoleCommand> _commands;
        private static readonly List<IConsoleSink> _sinks;

        static ConsoleManager()
        {
            _commands = new Dictionary<string, IConsoleCommand>();
            _sinks = new List<IConsoleSink>();

#if DEBUG
            _sinks.Add(new SystemConsoleSink());
#endif
        }

        public static IReadOnlyDictionary<string, IConsoleCommand> Commands
        {
            get { return _commands; }
        }

        public static void RegisterConsoleSink(IConsoleSink sink)
        {
            _sinks.Add(sink);
        }

        public static void UnregisterConsoleSink(IConsoleSink sink)
        {
            _sinks.Remove(sink);
        }

        public static void RegisterCommand(string name, IConsoleCommand command)
        {
            _commands.Add(name, command);
        }

        public static void UnregisterCommand(string name)
        {
            _commands.Remove(name);
        }

        public static IConsoleCommand Find(string name)
        {
            IConsoleCommand command;
            _commands.TryGetValue(name, out command);
            return command;
        }

        public static void Exec(string command)
        {
            string cmd = "", args = "";
            int i;

            for (i = 0; i < command.Length; i++)
            {
                char c = command[i];

                if (c == ' ')
                    break;

                cmd += c;
            }

            if (i + 1 < command.Length)
                args = command.Substring(i + 1);

            IConsoleCommand ccmd = Find(cmd);

            if (ccmd == null)
            {
                // TODO: Report?
                return;
            }

            // FIXME: We should put a try/catch here to ensure stability, or not
            ccmd.Execute(args);
        }

        [Conditional("DEBUG")]
        public static void DevMessage(string message, params object[] args)
        {
            string text = string.Format(message, args);

            foreach (IConsoleSink sink in _sinks)
                sink.DevMessage(text);
        }

        public static void ErrorMessage(string message, params object[] args)
        {
            string text = string.Format(message, args);

            foreach (IConsoleSink sink in _sinks)
                sink.ErrorMessage(text);
        }

        public static void WarningMessage(string message, params object[] args)
        {
            string text = string.Format(message, args);

            foreach (IConsoleSink sink in _sinks)
                sink.WarningMessage(text);
        }

        public static void Message(string message, params object[] args)
        {
            string text = string.Format(message, args);

            foreach (IConsoleSink sink in _sinks)
                sink.Message(text);
        }

        public static string[] ParseArguments(string arguments)
        {
            List<string> args = new List<string>();
            string current = "";
            bool inString = false, addLast = false;

            for (int i = 0; i < arguments.Length; i++)
            {
                char c = arguments[i];

                if (inString)
                {
                    if (c == '"')
                    {
                        if (i + 1 < arguments.Length && arguments[i + 1] == '"')
                        {
                            current += '"';
                            i++;
                        }
                        else
                        {
                            inString = false;
                        }
                    }
                    else
                    {
                        current += c;
                    }
                }
                else
                {
                    if (c == ' ')
                    {
                        while (i < arguments.Length && arguments[i] == ' ')
                            i++;

                        args.Add(arguments);
                        addLast = false;
                        arguments = "";
                    }
                    else if (c == '"')
                    {
                        addLast = true;
                        inString = true;
                    }
                    else
                    {
                        current += c;
                    }
                }
            }

            if (addLast)
                args.Add(current);

            return args.ToArray();
        }
    }
}
