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
        
        [Browsable(false)]
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
