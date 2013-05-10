using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GreenBox3D.Graphics;

namespace GreenBox3D
{
    public sealed class TaskDispatcher
    {
        private const int GraphicWorkerThreadCount = 1;

        private readonly Queue<Action> _mainQueue, _graphicQueue;
        private readonly List<Thread> _graphicThreads;
        private readonly AutoResetEvent _graphicQueueSync;
        private bool _stop;

        public TaskDispatcher()
        {
            _mainQueue = new Queue<Action>();
            _graphicQueue = new Queue<Action>();
            _graphicThreads = new List<Thread>();
            _graphicQueueSync = new AutoResetEvent(false);
        }

        public void SpawnDefaultThreads()
        {
            if (GraphicsDevice.ActiveDevice != null)
                SpawnGraphicThread(GraphicWorkerThreadCount);
        }

        public void SpawnGraphicThread(int count = 1)
        {
            GraphicsDevice graphicsDevice = GraphicsDevice.ActiveDevice;

            if (graphicsDevice == null)
                throw new InvalidOperationException("A GraphicDevice must be bound prior to this call");

            for (int i = 0; i < count; i++)
            {
                Thread thread = new Thread(() =>
                {
                    graphicsDevice.MakeCurrent();

                    while (!_stop)
                    {
                        UpdateGraphicThread(-1);
                        _graphicQueueSync.WaitOne();
                    }
                });

                thread.Start();
                _graphicThreads.Add(thread);
            }
        }

        public void RunOnGraphicThread(GraphicsDevice graphicsDevice, Action action)
        {
            if (GraphicsDevice.ActiveDevice == graphicsDevice)
            {
                action();
            }
            else
            {
                Enqueue(_graphicQueue, action);
                _graphicQueueSync.Set();
            }
        }

        public void RunOnMainThread(Action action)
        {
            Enqueue(_mainQueue, action);
        }

        internal void UpdateMainThread(int max)
        {
            UpdateQueue(_mainQueue, max);
        }

        internal void UpdateGraphicThread(int max)
        {
            UpdateQueue(_graphicQueue, max);
        }

        internal void Shutdown()
        {
            _stop = true;
        }

        private void Enqueue(Queue<Action> queue, Action action)
        {
            lock (queue)
                queue.Enqueue(action);
        }

        private void UpdateQueue(Queue<Action> queue, int max)
        {
            int i = 0;

            while (true)
            {
                Action action;

                if (i >= max)
                    break;

                lock (queue)
                {
                    if (queue.Count == 0)
                        break;

                    action = queue.Dequeue();
                }

                action();
                i++;
            }
        }
    }
}
