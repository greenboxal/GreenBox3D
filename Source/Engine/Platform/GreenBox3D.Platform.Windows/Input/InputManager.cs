using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GreenBox3D.Input;

namespace GreenBox3D.Platform.Windows.Input
{
    public class InputManager : IInputManager
    {
        private readonly WindowsGamePlatform _platform;
        private readonly WindowsGameWindow _window;

        internal readonly List<IMouseFilter> _mouseFilters;
        internal readonly List<IKeyboardFilter> _keyboardFilters;

        public InputManager(WindowsGamePlatform platform, WindowsGameWindow window)
        {
            _platform = platform;
            _window = window;

            _mouseFilters = new List<IMouseFilter>();
            _keyboardFilters = new List<IKeyboardFilter>();

            _window.SetInputManager(this);
        }

        public void RegisterMouseFilter(IMouseFilter filter)
        {
            lock (_mouseFilters)
                _mouseFilters.Add(filter);
        }

        public void UnregisterMouseFilter(IMouseFilter filter)
        {
            lock (_mouseFilters)
                _mouseFilters.Remove(filter);
        }

        public void RegisterKeyboardFilter(IKeyboardFilter filter)
        {
            lock (_keyboardFilters)
                _keyboardFilters.Add(filter);
        }

        public void UnregisterKeyboardFilter(IKeyboardFilter filter)
        {
            lock (_keyboardFilters)
                _keyboardFilters.Remove(filter);
        }
    }
}
