using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenBox3D
{
    public delegate void ConsoleFunctionDelegate(string args);

    public class CFunc : IConsoleCommand
    {
        public ConsoleFunctionDelegate _function;

        public CFunc(ConsoleFunctionDelegate function)
        {
            if (function == null)
                throw new ArgumentNullException("function");

            _function = function;
        }

        void IConsoleCommand.Execute(string arguments)
        {
            _function(arguments);
        }
    }

    public class CVarChangedEventArgs<T> : EventArgs
    {
        public CVar<T> Variable { get; private set; }

        public CVarChangedEventArgs(CVar<T> cvar)
        {
            Variable = cvar;
        }
    }

    public class CVar<T> : IConsoleCommand
    {
        private T _value;
        public T Value { get { return _value; } }

        public EventHandler<CVarChangedEventArgs<T>> Changed;

        public CVar(T defaultValue)
        {
            _value = defaultValue;
        }
        
        void IConsoleCommand.Execute(string arguments)
        {
            _value = ConvertFromString(arguments);

            if (Changed != null)
                Changed(this, new CVarChangedEventArgs<T>(this));
        }

        protected virtual T ConvertFromString(string text)
        {
            try
            {
                return (T)Convert.ChangeType(text, typeof(T));
            }
            catch
            {
                return _value;
            }
        }
    }

    public class IntegerCVar : CVar<int>
    {
        private int _minValue, _maxValue;

        public IntegerCVar(int defaultValue)
            : this(defaultValue, int.MinValue, int.MaxValue)
        {
        }

        public IntegerCVar(int defaultValue, int minValue, int maxValue)
            : base(defaultValue)
        {
            _minValue = minValue;
            _maxValue = maxValue;
        }

        protected override int ConvertFromString(string text)
        {
            int value;
            NumberStyles flags = NumberStyles.Integer;

            if (text.StartsWith("0x"))
            {
                flags = NumberStyles.HexNumber;
                text = text.Substring(2);
            }

            if (!int.TryParse(text, flags, CultureInfo.InvariantCulture, out value))
                return Value;

            if (value < _minValue)
                value = _minValue;

            if (value > _maxValue)
                value = _maxValue;

            return value;
        }
    }

    public class SingleCVar : CVar<float>
    {
        private float _minValue, _maxValue;

        public SingleCVar(float defaultValue)
            : this(defaultValue, float.MinValue, float.MaxValue)
        {
        }

        public SingleCVar(float defaultValue, float minValue, float maxValue)
            : base(defaultValue)
        {
            _minValue = minValue;
            _maxValue = maxValue;
        }

        protected override float ConvertFromString(string text)
        {
            float value;

            if (!float.TryParse(text, out value))
                return Value;

            if (value < _minValue)
                value = _minValue;

            if (value > _maxValue)
                value = _maxValue;

            return value;
        }
    }

    public class DoubleCVar : CVar<double>
    {
        private double _minValue, _maxValue;

        public DoubleCVar(double defaultValue)
            : this(defaultValue, double.MinValue, double.MaxValue)
        {
        }

        public DoubleCVar(double defaultValue, double minValue, double maxValue)
            : base(defaultValue)
        {
            _minValue = minValue;
            _maxValue = maxValue;
        }

        protected override double ConvertFromString(string text)
        {
            double value;

            if (!double.TryParse(text, out value))
                return Value;

            if (value < _minValue)
                value = _minValue;

            if (value > _maxValue)
                value = _maxValue;

            return value;
        }
    }

    public class StringCVar : CVar<string>
    {
        public StringCVar(string defaultValue)
            : base(defaultValue)
        {
        }

        protected override string ConvertFromString(string text)
        {
            return text;
        }
    }
}
