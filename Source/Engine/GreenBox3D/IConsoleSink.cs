// IConsoleSink.cs
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
    public interface IConsoleSink
    {
        void DevMessage(string message);
        void ErrorMessage(string message);
        void WarningMessage(string message);
        void Message(string message);
    }

    internal class SystemConsoleSink : IConsoleSink
    {
        public void DevMessage(string message)
        {
            Console.WriteLine("{0}", message);
        }

        public void ErrorMessage(string message)
        {
            Console.WriteLine("Error: {0}", message);
        }

        public void WarningMessage(string message)
        {
            Console.WriteLine("Warning: {0}", message);
        }

        public void Message(string message)
        {
            Console.WriteLine(message);
        }
    }
}
