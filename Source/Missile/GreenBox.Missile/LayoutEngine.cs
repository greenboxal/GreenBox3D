using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GreenBox.Missile.Media;
using GreenBox.Missile.Threading;

namespace GreenBox.Missile
{
    internal class LayoutEngine : DispatcherObject
    {
        [ThreadStatic]
        private static LayoutEngine _currentEngine;

        public static LayoutEngine Current
        {
            get { return _currentEngine ?? (_currentEngine = new LayoutEngine()); }
        }

        private readonly PriorityQueue _measureQueue;
        private readonly PriorityQueue _arrangeQueue;
        private readonly PriorityQueue _renderQueue;
        private bool _inDispatcher;

        public LayoutEngine()
        {
            _measureQueue = new PriorityQueue();
            _arrangeQueue = new PriorityQueue();
            _renderQueue = new PriorityQueue();
        }

        public void EnqueueMeasure(UIElement element)
        {
            if (element.MeasureEntry != null)
                return;

            if (element.HasFlags(Visual.Flags.IsScreenRoot))
                return;

            if (!element.HasFlags(UIElement.Flags.CanAutoMeasure) || element.HasFlags(Visual.Flags.IsDisconnected))
                return;

            EnsureDispatching();

            element.MeasureEntry = _measureQueue.Enqueue(element);
        }

        public void EnqueueArrange(UIElement element)
        {
            if (element.ArrangeEntry != null)
                return;

            if (element.HasFlags(Visual.Flags.IsScreenRoot))
                return;

            if (!element.HasFlags(UIElement.Flags.CanAutoArrange) || element.HasFlags(Visual.Flags.IsDisconnected))
                return;

            EnsureDispatching();

            element.ArrangeEntry = _arrangeQueue.Enqueue(element);
        }

        public void EnqueueRender(UIElement element)
        {
            if (element.RenderEntry != null)
                return;

            if (element.HasFlags(Visual.Flags.IsScreenRoot))
                return;

            EnsureDispatching();

            element.RenderEntry = _renderQueue.Enqueue(element);
        }

        public void DequeueMeasure(UIElement element)
        {
            if (element.MeasureEntry == null)
                return;

            _measureQueue.Dequeue(element.MeasureEntry);
            element.MeasureEntry = null;
        }

        public void DequeueArrange(UIElement element)
        {
            if (element.ArrangeEntry == null)
                return;

            _arrangeQueue.Dequeue(element.ArrangeEntry);
            element.ArrangeEntry = null;
        }

        public void DequeueRender(UIElement element)
        {
            if (element.RenderEntry == null)
                return;

            _renderQueue.Dequeue(element.RenderEntry);
            element.RenderEntry = null;
        }

        public void UpdateElement(UIElement element)
        {
            if (element.MeasureEntry != null)
            {
                DequeueMeasure(element);

                if (element.HasFlags(UIElement.Flags.CanAutoMeasure) && !element.HasFlags(Visual.Flags.IsDisconnected))
                    EnqueueMeasure(element);
            }

            if (element.ArrangeEntry != null)
            {
                DequeueArrange(element);

                if (element.HasFlags(UIElement.Flags.CanAutoArrange) && !element.HasFlags(Visual.Flags.IsDisconnected))
                    EnqueueArrange(element);
            }
        }

        private void EnsureDispatching()
        {
            if (!_inDispatcher)
            {
                Dispatcher.BeginInvoke(new Action(Process), DispatcherPriority.Render);
                _inDispatcher = true;
            }
        }

        public void Process()
        {
        processMeasure:
            while (_measureQueue.Head != null)
                _measureQueue.Head.Element.Measure(_measureQueue.Head.Element.PreviousAvaiableSize);

            while (_arrangeQueue.Head != null)
            {
                _arrangeQueue.Head.Element.Arrange(_arrangeQueue.Head.Element.FinalRect);

                if (_measureQueue.Head != null)
                    goto processMeasure;
            }

            while (_renderQueue.Head != null)
                _renderQueue.Head.Element.InvokeRender();

            _inDispatcher = false;
        }

        public class QueueEntry
        {
            public QueueEntry Next;
            public QueueEntry Prev;
            public UIElement Element;
        }

        private class PriorityQueue
        {
            // TODO: Find a good value for here
            private const int QueueHeapSize = 25;

            private int _heapSize;
            private QueueEntry _heap;
            private QueueEntry _head;

            public QueueEntry Head
            {
                get { return _head; }
            }

            public PriorityQueue()
            {
                for (int i = 0; i < QueueHeapSize; i++)
                    _heap = new QueueEntry { Next = _heap };

                _heapSize = QueueHeapSize;
                _head = null;
            }

            public QueueEntry Enqueue(UIElement element)
            {
                QueueEntry entry;

                if (_heap != null)
                {
                    entry = _heap;
                    _heap = _heap.Next;
                    _heapSize--;
                }
                else
                {
                    entry = new QueueEntry();
                }

                entry.Element = element;
                entry.Next = null;
                entry.Prev = null;

                if (_head == null)
                {
                    _head = entry;
                }
                else
                {
                    QueueEntry ptr = _head;

                    while (ptr.Next != null && ptr.Element.TreeLevel > element.TreeLevel)
                        ptr = ptr.Next;

                    if (ptr.Prev == null)
                    {
                        entry.Next = _head;
                        _head = entry;
                    }
                    else
                    {
                        ptr.Prev.Next = entry;

                        entry.Next = ptr;
                        entry.Prev = ptr.Prev;

                        ptr.Prev = entry;
                    }
                }

                return entry;
            }

            public void Dequeue(QueueEntry entry)
            {
                if (entry.Prev == null)
                {
                    _head = entry.Next;
                }
                else
                {
                    entry.Prev.Next = entry.Next;

                    if (entry.Next != null)
                        entry.Next.Prev = entry.Prev;
                }

                entry.Next = null;
                entry.Prev = null;

                if (_heapSize < QueueHeapSize)
                {
                    entry.Next = _heap;
                    _heap = entry;
                    _heapSize++;
                }
            }
        }
    }
}
