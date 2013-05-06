using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenBox3D.Graphics
{
    public abstract class SamplerStateCollection : IReadOnlyCollection<SamplerState>
    {
        #region Public Properties

        public abstract int Count { get; }

        #endregion

        #region Public Indexers

        public abstract SamplerState this[int index] { get; set; }

        #endregion

        #region Public Methods and Operators

        public abstract IEnumerator<SamplerState> GetEnumerator();

        #endregion

        #region Explicit Interface Methods

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}
