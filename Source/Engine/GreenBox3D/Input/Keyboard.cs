// Keyboard.cs
// 
// Copyright (c) 2013 The GreenBox Development LLC, all rights reserved
// 
// This file is a proprietary part of GreenBox3D, disclosing the content
// of this file without the owner consent may lead to legal actions

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenBox3D.Input
{
    public static class Keyboard
    {
        private static IKeyboard _keyboard;

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void SetKeyboardImplementation(IKeyboard keyboard)
        {
            _keyboard = keyboard;
        }

        public static KeyboardState GetState()
        {
            if (_keyboard != null)
                return _keyboard.GetState();

            return new KeyboardState();
        }
    }
}
