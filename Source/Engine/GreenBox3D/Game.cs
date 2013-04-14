// Game.cs
// 
// Copyright (c) 2013 The GreenBox Development LLC, all rights reserved
// 
// This file is a proprietary part of GreenBox3D, disclosing the content
// of this file without the owner consent may lead to legal actions

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GreenBox3D.Platform;

namespace GreenBox3D
{
    public class Game : IPlatformController, IDisposable
    {
        private static readonly ILogger Log = LogManager.GetLogger(typeof(Game));
        private readonly Dictionary<Type, object> _services;

        public Game()
        {
            _services = new Dictionary<Type, object>();
        }

        public GamePlatform Platform { get; private set; }

        public virtual void Dispose()
        {
            Platform.Dispose();
        }

        void IPlatformController.Initialize()
        {
            Initialize();
        }

        void IPlatformController.Update(GameTime gameTime)
        {
            Update(gameTime);
        }

        void IPlatformController.Render(GameTime gameTime)
        {
            Render(gameTime);
        }

        void IPlatformController.OnResize()
        {
            OnResize();
        }

        void IPlatformController.Shutdown()
        {
            Shutdown();
        }

        public object GetService(Type serviceType)
        {
            object service;
            _services.TryGetValue(serviceType, out service);
            return service;
        }

        public T GetService<T>(Type serviceType)
        {
            return (T)GetService(serviceType);
        }

        public T GetService<T>()
        {
            return GetService<T>(typeof(T));
        }

        public void RegisterService(Type serviceType, object service)
        {
            if (_services.ContainsKey(serviceType))
                throw new InvalidOperationException("A service with this type already is registered.");

            _services.Add(serviceType, service);
        }

        public void RemoveService(Type serviceType)
        {
            _services.Remove(serviceType);
        }

        public void Run()
        {
            Log.Message("GreenBox3D starting");

            Platform = GamePlatform.Create(this);
            Platform.Run();
        }

        public void Exit()
        {
            Platform.Exit();
        }

        protected virtual void Initialize()
        {
        }

        protected virtual void Update(GameTime gameTime)
        {
        }

        protected virtual void Render(GameTime gameTime)
        {
        }

        protected virtual void OnResize()
        {
        }

        protected virtual void Shutdown()
        {
        }
    }
}
