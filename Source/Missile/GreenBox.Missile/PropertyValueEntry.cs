using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenBox.Missile
{
    public class PropertyValueEntry
    {
        [Flags]
        private enum Flags
        {
            IsCoerced,
        }

        private readonly DependencyProperty _property;
        private Flags _flags;
        private object _value;

        public DependencyProperty Property
        {
            get { return _property; }
        }

        public object BaseValue
        {
            get
            {
                ValueStore vs = _value as ValueStore;

                if (vs != null)
                    return vs.BaseValue;

                return _value;
            }
        }

        public object CoercedValue
        {
            get
            {
                ValueStore vs = _value as ValueStore;

                if (vs != null)
                    return vs.CoercedValue;

                return null;
            }
        }

        public object Value
        {
            get
            {
                ValueStore vs = _value as ValueStore;

                if (vs != null)
                    return vs.BaseValue;

                return _value;
            }
        }

        public bool IsCoerced
        {
            get { return HasFlags(Flags.IsCoerced); }
        }

        public PropertyValueEntry(DependencyProperty property)
        {
            _property = property;
        }

        public PropertyValueEntry(DependencyProperty property, object value)
        {
            _property = property;
            _value = value;
        }

        public object GetFinalValue()
        {
            ValueStore vs = _value as ValueStore;

            if (vs != null)
            {
                if (HasFlags(Flags.IsCoerced))
                    return vs.CoercedValue;

                return vs.BaseValue;
            }

            return _value;
        }

        public void UnsetCoercedValue()
        {
            ValueStore vs = _value as ValueStore;

            SetFlags(Flags.IsCoerced, false);

            if (vs != null)
                vs.CoercedValue = null;
        }

        public void SetCoercedValue(object value)
        {
            ValueStore vs = _value as ValueStore;

            if (vs == null)
            {
                vs = new ValueStore();
                vs.BaseValue = _value;
                _value = vs;
            }

            SetFlags(Flags.IsCoerced, true);
            vs.CoercedValue = null;
        }

        public void SetValue(object value)
        {
            ValueStore vs = _value as ValueStore;

            if (vs == null)
                _value = value;
            else
                vs.BaseValue = value;
        }

        private bool HasFlags(Flags flags)
        {
            return (_flags & flags) != 0;
        }

        private void SetFlags(Flags flags, bool value)
        {
            if (value)
                _flags |= flags;
            else
                _flags &= ~flags;
        }

        public class ValueStore
        {
            public object CoercedValue;
            public object BaseValue;
        }
    }
}
