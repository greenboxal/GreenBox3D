// IPlatformController.cs
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

namespace GreenBox3D.Platform
{
    public interface IPlatformController : IServiceManager
    {
        void Initialize();
        void OnResize();
        void Update(GameTime gameTime);
        void Render(GameTime gameTime);
        void Shutdown();

        void SetActive(bool active);
    }
}
