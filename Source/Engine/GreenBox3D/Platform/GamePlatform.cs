// GamePlatform.cs
// 
// Copyright (c) 2013 The GreenBox Development LLC, all rights reserved
// 
// This file is a proprietary part of GreenBox3D, disclosing the content
// of this file without the owner consent may lead to legal actions

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using GreenBox3D.Graphics;

namespace GreenBox3D.Platform
{
    public abstract class GamePlatform : IDisposable
    {
        protected GamePlatform(IPlatformController controller)
        {
            Controller = controller;
        }

        protected IPlatformController Controller { get; private set; }

        public abstract bool Running { get; }
        public abstract bool VSync { get; set; }
        public abstract IGameWindow Window { get; }
        public abstract void Dispose();

        public abstract void Run();
        public abstract void InitializeGraphics(PresentationParameters parameters);
        public abstract void InitializeInput();
        public abstract void SkipFrame();
        public abstract void Exit();

        internal static GamePlatform Create(IPlatformController controller)
        {
            // TODO: Implement this
            Assembly assembly = Assembly.LoadFrom("GreenBox3D.Platform.Windows.dll");
            return
                (GamePlatform)
                assembly.CreateInstance("GreenBox3D.Platform.Windows.WindowsGamePlatform", false, BindingFlags.Default,
                                        null, new object[] { controller }, null, null);
        }
    }
}
