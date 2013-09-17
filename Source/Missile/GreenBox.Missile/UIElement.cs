using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using GreenBox.Missile.Media;
using GreenBox3D;

namespace GreenBox.Missile
{
    public partial class UIElement : Visual
    {
        [Flags]
        internal new enum Flags
        {
            CanAutoMeasure,
            CanAutoArrange,
            MeasureInvalidated = 0x1,
            ArrangeInvalidated = 0x2,
            VisualInvalidated = 0x4,
            IsMeasuring = 0x8,
            IsArranging = 0x10,
            MeasureInArrange = 0x20,
        }

        private Flags _flags;
        private Size _previousAvailableSize;
        private Size _desiredSize;
        private Rect _finalRect;

        internal LayoutEngine.QueueEntry MeasureEntry;
        internal LayoutEngine.QueueEntry ArrangeEntry;
        internal LayoutEngine.QueueEntry RenderEntry;

        internal Size PreviousAvaiableSize
        {
            get { return _previousAvailableSize; }
        }

        internal Rect FinalRect
        {
            get { return _finalRect; }
        }

        public Size DesiredSize
        {
            get { return _desiredSize; }
        }

        public Size RenderSize
        {
            get { return _finalRect.Size; }
        }

        public bool IsMeasureValid
        {
            get { return !HasFlags(Flags.MeasureInvalidated) && HasFlags(Flags.CanAutoMeasure); }
        }

        public bool IsArrangeValid
        {
            get { return !HasFlags(Flags.ArrangeInvalidated) && HasFlags(Flags.CanAutoArrange); }
        }

        public UIElement()
        {
            
        }

        public void InvalidateMeasure()
        {
            SetFlags(Flags.MeasureInvalidated, true);
            LayoutEngine.Current.EnqueueMeasure(this);
        }

        public void InvalidateArrange()
        {
            SetFlags(Flags.ArrangeInvalidated, true);
            LayoutEngine.Current.EnqueueArrange(this);
        }

        public void InvalidateVisual()
        {
            if (HasFlags(Flags.IsArranging))
            {
                InvokeRender();
                return;
            }

            SetFlags(Flags.VisualInvalidated, true);
            LayoutEngine.Current.EnqueueRender(this);
        }

        public void Measure(Size availableSize)
        {
            using (Dispatcher.DisableProcessing())
            {
                bool availableSizeChanged = MathUtils.Compare(availableSize, _previousAvailableSize);

                if (Visibility == Visibility.Collapsed || HasFlags(Visual.Flags.IsDisconnected))
                {
                    LayoutEngine.Current.DequeueMeasure(this);

                    if (availableSizeChanged)
                    {
                        _previousAvailableSize = availableSize;
                        SetFlags(Flags.MeasureInvalidated, true);
                    }

                    return;
                }

                if (!HasFlags(Flags.MeasureInvalidated) && HasFlags(Flags.CanAutoMeasure) && !availableSizeChanged)
                    return;

                SetFlags(Flags.CanAutoMeasure, true);
                SetFlags(Flags.IsMeasuring, true);

                InvalidateArrange();

                Size desiredSize = new Size(0, 0);

                try
                {
                    desiredSize = MeasureCore(availableSize);
                }
                catch
                {
                    // FIXME: Do nothing?
                }

                _desiredSize = desiredSize;
                _previousAvailableSize = availableSize;

                SetFlags(Flags.IsMeasuring, false);
                SetFlags(Flags.MeasureInvalidated, false);

                LayoutEngine.Current.DequeueMeasure(this);

                if (!HasFlags(Flags.MeasureInArrange) && !MathUtils.Compare(_desiredSize, desiredSize))
                {
                    UIElement parent = VisualTreeHelper.GetUIParent(this);

                    if (parent != null && !parent.HasFlags(Flags.IsMeasuring))
                        parent.NotifyChildMeasureChanged(this);
                }
            }
        }

        protected virtual Size MeasureCore(Size availableSize)
        {
            return new Size(0, 0);
        }

        protected virtual void NotifyChildMeasureChanged(UIElement child)
        {
            InvalidateMeasure();
        }

        public void Arrange(Rect finalRect)
        {
            using (Dispatcher.DisableProcessing())
            {
                if (Visibility == Visibility.Collapsed || HasFlags(Visual.Flags.IsDisconnected))
                {
                    LayoutEngine.Current.DequeueArrange(this);

                    _finalRect = finalRect;
                    return;
                }

                if (HasFlags(Flags.MeasureInvalidated) || !HasFlags(Flags.CanAutoMeasure))
                {
                    try
                    {
                        SetFlags(Flags.MeasureInArrange, true);
                        Measure(!HasFlags(Flags.CanAutoMeasure) ? finalRect.Size : _previousAvailableSize);
                    }
                    catch
                    {
                    }

                    SetFlags(Flags.MeasureInArrange, false);
                }

                if (!HasFlags(Flags.MeasureInvalidated) && HasFlags(Flags.CanAutoArrange) &&
                    MathUtils.Compare(finalRect, _finalRect))
                    return;

                SetFlags(Flags.CanAutoArrange, true);
                SetFlags(Flags.IsArranging, true);

                try
                {
                    ArrangeCore(finalRect);
                }
                catch
                {
                    // FIXME: Do nothing?
                }

                if (!MathUtils.Compare(RenderSize, _finalRect.Size))
                    InvokeRenderSizeChanged(finalRect.Size);

                _finalRect = finalRect;

                SetFlags(Flags.IsArranging, false);
                SetFlags(Flags.ArrangeInvalidated, false);

                LayoutEngine.Current.DequeueArrange(this);
            }
        }

        protected virtual void ArrangeCore(Rect finalRect)
        {
            
        }

        private void InvokeRenderSizeChanged(Size newSize)
        {
            if (Visibility == Visibility.Visible)
                InvalidateVisual();

            VisualOffset = new Vector(_finalRect.X, _finalRect.Y);
        }

        internal void InvokeRender()
        {
            
        }

        internal override void PropagateDisconnection(bool disconnected)
        {
            if (HasFlags(Visual.Flags.IsDisconnected) == disconnected)
                return;

            if (!disconnected)
            {
                LayoutEngine.Current.UpdateElement(this);

                if (HasFlags(Flags.MeasureInvalidated) && HasFlags(Flags.CanAutoMeasure))
                    LayoutEngine.Current.EnqueueMeasure(this);

                if (HasFlags(Flags.ArrangeInvalidated) && HasFlags(Flags.CanAutoArrange))
                    LayoutEngine.Current.EnqueueArrange(this);

                if (HasFlags(Flags.VisualInvalidated))
                    LayoutEngine.Current.EnqueueRender(this);
            }
            else
            {
                LayoutEngine.Current.DequeueMeasure(this);
                LayoutEngine.Current.DequeueArrange(this);
            }

            base.PropagateDisconnection(disconnected);
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
    }
}
