using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenBox.Missile
{
    public sealed class DependencyPropertyKey
    {
        private readonly DependencyProperty _dp;

        public DependencyProperty DependencyProperty
        {
            get { return _dp; }
        }

        internal DependencyPropertyKey(DependencyProperty dependencyProperty)
        {
            _dp = dependencyProperty;
        }

        public void OverrideMetadata(Type forType, PropertyMetadata typeMetadata)
        {
            _dp.OverrideMetadata(forType, typeMetadata, this);
        }
    }
}
