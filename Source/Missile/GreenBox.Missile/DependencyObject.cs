using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GreenBox.Missile.Threading;

namespace GreenBox.Missile
{
    public class DependencyObject : DispatcherObject
    {
        private readonly Dictionary<int, PropertyValueEntry> _valueStore;
        private DependencyObject _parent;

        public DependencyObjectType DependencyObjectType { get; private set; }

        public DependencyObject LogicalParent
        {
            get { return _parent; }
        }

        public DependencyObject()
        {
            _valueStore = new Dictionary<int, PropertyValueEntry>();

            DependencyObjectType = DependencyObjectType.FromSystemType(GetType());
        }

        public object GetValue(DependencyPropertyKey dp)
        {
            VerifyAccess();

            if (dp == null)
                throw new ArgumentNullException("dp");

            return GetValue(dp.DependencyProperty);
        }

        public object GetValue(DependencyProperty dp)
        {
            VerifyAccess();

            if (dp == null)
                throw new ArgumentNullException("dp");

            return GetValueEntry(dp).GetFinalValue();
        }

        public void SetValue(DependencyProperty dp, object value)
        {
            VerifyAccess();

            if (dp == null)
                throw new ArgumentNullException("dp");

            if (dp.ReadOnly)
                throw new ArgumentException("Property is read-only", "dp");

            SetValueCore(dp, value);
        }

        public void SetValue(DependencyPropertyKey dp, object value)
        {
            VerifyAccess();

            if (dp == null)
                throw new ArgumentNullException("dp");

            SetValueCore(dp.DependencyProperty, value);
        }

        public void ClearValue(DependencyProperty dp)
        {
            VerifyAccess();

            if (dp == null)
                throw new ArgumentNullException("dp");

            if (dp.ReadOnly)
                throw new ArgumentException("Property is read-only", "dp");

            ClearValueCore(dp);
        }

        public void ClearValue(DependencyPropertyKey dp)
        {
            VerifyAccess();

            if (dp == null)
                throw new ArgumentNullException("dp");

            ClearValueCore(dp.DependencyProperty);
        }

        public void CoerceValue(DependencyProperty dp)
        {
            VerifyAccess();

            if (dp == null)
                throw new ArgumentNullException("dp");

            if (dp.ReadOnly)
                throw new ArgumentException("Property is read-only", "dp");

            SetValueCore(dp, GetValueEntry(dp).BaseValue);
        }

        public void CoerceValue(DependencyPropertyKey dp)
        {
            VerifyAccess();

            if (dp == null)
                throw new ArgumentNullException("dp");

            SetValueCore(dp.DependencyProperty, GetValueEntry(dp.DependencyProperty).BaseValue);
        }

        protected void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {

        }

        private void InvokePropertyChanged(DependencyProperty dp, PropertyMetadata metadata, object oldValue, object newValue)
        {
            DependencyPropertyChangedEventArgs e = new DependencyPropertyChangedEventArgs(dp, oldValue, newValue);

            OnPropertyChanged(e);

            if (metadata.PropertyChangedCallback != null)
                metadata.PropertyChangedCallback(this, e);
        }

        private PropertyValueEntry GetValueEntry(DependencyProperty dp)
        {
            PropertyValueEntry valueEntry = null;
            PropertyMetadata metadata;

            if (_valueStore.TryGetValue(dp.GlobalIndex, out valueEntry))
                return valueEntry;
            
            metadata = dp.GetMetadata(DependencyObjectType);

            if (metadata.Inheritable && _parent != null)
                return _parent.GetValueEntry(dp);

            if (metadata.DefaultValueSet)
                return new PropertyValueEntry(dp, metadata.DefaultValue);

            return new PropertyValueEntry(dp, dp.DefaultMetadata.DefaultValue);
        }

        private void SetValueCore(DependencyProperty dp, object value)
        {
            object coercedValue = value, oldValue = null;
            PropertyValueEntry valueEntry = null, oldEntry = null;
            PropertyMetadata metadata;

            if (!dp.IsValidValue(value))
                throw new InvalidOperationException("Invalid value for property");

            if (!_valueStore.TryGetValue(dp.GlobalIndex, out oldEntry))
            {
                valueEntry = new PropertyValueEntry(dp);
                _valueStore[dp.GlobalIndex] = valueEntry;
            }
            else
            {
                oldValue = oldEntry.BaseValue;
            }

            metadata = dp.GetMetadata(DependencyObjectType);

            if (valueEntry == null)
                valueEntry = oldEntry;

            if (metadata.CoerceValueCallback != null)
                coercedValue = metadata.CoerceValueCallback(this, coercedValue);

            if (coercedValue == DependencyProperty.UnsetValue)
                return;

            valueEntry.SetValue(value);

            if (metadata.CoerceValueCallback != null)
                valueEntry.SetCoercedValue(coercedValue);

            if (oldEntry == null || !value.Equals(oldValue))
                InvokePropertyChanged(dp, metadata, oldValue, coercedValue);
        }

        private void ClearValueCore(DependencyProperty dp)
        {
            _valueStore.Remove(dp.GlobalIndex);
        }

        internal void SetLogicalParent(DependencyObject d)
        {
            _parent = d;
        }
    }
}
