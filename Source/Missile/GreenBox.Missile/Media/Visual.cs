using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GreenBox.Missile.Media.Renderer;
using GreenBox3D;

namespace GreenBox.Missile.Media
{
    public class Visual : DependencyObject
    {
        [Flags]
        internal enum Flags
        {
            IsScreenRoot = 0x1,
            IsDisconnected = 0x2,
        }

        internal int TreeLevel;

        private readonly IVisualHandle _visualHandle;
        private Flags _flags;
        private Visual _visualParent;

        protected Visual VisualParent
        {
            get { return _visualParent; }
        }

        protected Vector VisualOffset
        {
            get { return _visualHandle.Offset; }
            set { _visualHandle.Offset = value; }
        }

        protected float VisualOpacity
        {
            get { return _visualHandle.Opacity; }
            set { _visualHandle.Opacity = value; }
        }

        protected int VisualChildrenCount
        {
            get { return _visualHandle.ChildrenCount; }
        }

        public Visual()
        {
            _visualHandle = PresentationManager.Renderer.CreateVisualHandle(this);
        }

        ~Visual()
        {
            _visualHandle.Dispose();
        }

        protected void AddVisualChild(Visual visual)
        {
            if (visual == null)
                return;

            if (visual.HasFlags(Flags.IsScreenRoot))
                return;

            if (visual._visualParent != null)
                throw new InvalidOperationException("Visual already have a parent");

            _visualHandle.AddChild(visual._visualHandle);

            visual.SetVisualParent(visual);
            OnVisualChildrenChanged(visual, null);
        }

        protected void RemoveVisualChild(Visual visual)
        {
            if (visual == null)
                return;

            if (visual.HasFlags(Flags.IsScreenRoot))
                return;

            if (visual._visualParent != this)
                throw new InvalidOperationException("Visual isn't a child of this instance");

            _visualHandle.RemoveChild(visual._visualHandle);

            visual.SetVisualParent(null);
            OnVisualChildrenChanged(null, visual);
        }

        protected virtual Visual GetVisualChild(int index)
        {
            return _visualHandle.GetChild(index).Visual;
        }

        protected virtual void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
        {
            
        }

        protected virtual void OnVisualParentChanged(DependencyObject oldParent)
        {
            
        }

        private void SetVisualParent(Visual visual)
        {
            Visual oldParent = _visualParent;

            _visualParent = visual;

            PropagateDisconnection(_visualParent == null);
            OnVisualParentChanged(oldParent);
        }

        internal virtual void PropagateDisconnection(bool disconnected)
        {
            if (HasFlags(Flags.IsDisconnected) == disconnected)
                return;

            SetFlags(Flags.IsDisconnected, disconnected);
            TreeLevel = disconnected ? 0 : _visualParent.TreeLevel + 1;

            for (int i = 0; i < VisualChildrenCount; i++)
                GetVisualChild(i).PropagateDisconnection(disconnected);
        }

        internal bool HasFlags(Flags flags)
        {
            return (_flags & flags) != 0;
        }

        internal void SetFlags(Flags flags, bool value)
        {
            if (value)
                _flags |= flags;
            else
                _flags &= ~flags;
        }

        internal Visual GetParent()
        {
            return _visualParent;
        }
    }
}
