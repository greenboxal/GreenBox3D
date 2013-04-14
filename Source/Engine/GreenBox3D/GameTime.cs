// GameTime.cs
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

namespace GreenBox3D
{
    public class GameTime
    {
        #region Constructors and Destructors

        public GameTime()
        {
            TotalTime = TimeSpan.Zero;
            ElapsedTime = TimeSpan.Zero;
            IsRunningSlowly = false;
        }

        public GameTime(TimeSpan totalGameTime, TimeSpan elapsedGameTime)
        {
            TotalTime = totalGameTime;
            ElapsedTime = elapsedGameTime;
            IsRunningSlowly = false;
        }

        public GameTime(TimeSpan totalRealTime, TimeSpan elapsedRealTime, bool isRunningSlowly)
        {
            TotalTime = totalRealTime;
            ElapsedTime = elapsedRealTime;
            IsRunningSlowly = isRunningSlowly;
        }

        #endregion

        #region Public Properties

        public TimeSpan ElapsedTime { get; set; }
        public bool IsRunningSlowly { get; set; }
        public TimeSpan TotalTime { get; set; }

        #endregion
    }
}
