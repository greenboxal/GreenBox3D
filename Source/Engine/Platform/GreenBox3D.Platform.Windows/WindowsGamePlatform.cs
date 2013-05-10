// WindowsGamePlatform.cs
// 
// Copyright (c) 2013 The GreenBox Development LLC, all rights reserved
// 
// This file is a proprietary part of GreenBox3D, disclosing the content
// of this file without the owner consent may lead to legal actions
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GreenBox3D.Graphics;
using GreenBox3D.Input;
using GreenBox3D.Platform.Windows.Input;

namespace GreenBox3D.Platform.Windows
{
    public class WindowsGamePlatform : GamePlatform
    {
        private static readonly ILogger Log = LogManager.GetLogger(typeof(WindowsGamePlatform));
        private readonly Stopwatch _renderTimer;
        private readonly Stopwatch _updateTimer;

        private WindowsGameWindow _gameWindow;
        private IInternalGraphicsDeviceManager _graphicsDeviceManager;
        private InputManager _inputManager;
        private double _nextRender;
        private double _nextUpdate;
        private double _renderPeriod;
        private double _renderTime;
        private bool _running;
        private bool _skipFrame;

        private DateTime _startTime;
        private double _targetRenderPeriod;
        private double _targetUpdatePeriod;
        private double _updatePeriod;
        private double _updateTime;

        public WindowsGamePlatform(IPlatformController controller)
            : base(controller)
        {
            _updateTimer = new Stopwatch();
            _renderTimer = new Stopwatch();
        }

        public override bool Running
        {
            get { return _running; }
        }

        public override IGameWindow Window
        {
            get { return _gameWindow; }
        }

        public override double RenderFrequency
        {
            get
            {
                if (_renderPeriod == 0.0)
                    return 1.0;

                return 1.0 / _renderPeriod;
            }
        }

        public override double RenderPeriod
        {
            get { return _renderPeriod; }
        }

        public override double RenderTime
        {
            get { return _renderTime; }
        }

        public override double TargetRenderFrequency
        {
            get
            {
                if (TargetRenderPeriod == 0.0)
                    return 0.0;

                return 1.0 / TargetRenderPeriod;
            }
            set
            {
                if (value < 1.0)
                {
                    TargetRenderPeriod = 0.0;
                }
                else if (value <= 200.0)
                {
                    TargetRenderPeriod = 1.0 / value;
                }
            }
        }

        public override double TargetRenderPeriod
        {
            get { return _targetRenderPeriod; }
            set
            {
                if (value <= 0.005)
                {
                    _targetRenderPeriod = 0.0;
                }
                else if (value <= 1.0)
                {
                    _targetRenderPeriod = value;
                }
            }
        }

        public override double TargetUpdateFrequency
        {
            get
            {
                if (TargetUpdatePeriod == 0.0)
                    return 0.0;

                return 1.0 / TargetUpdatePeriod;
            }
            set
            {
                if (value < 1.0)
                {
                    TargetUpdatePeriod = 0.0;
                }
                else if (value <= 200.0)
                {
                    TargetUpdatePeriod = 1.0 / value;
                }
            }
        }

        public override double TargetUpdatePeriod
        {
            get { return _targetUpdatePeriod; }
            set
            {
                if (value <= 0.005)
                {
                    _targetUpdatePeriod = 0.0;
                }
                else if (value <= 1.0)
                {
                    _targetUpdatePeriod = value;
                }
            }
        }

        public override double UpdateFrequency
        {
            get
            {
                if (_updatePeriod == 0.0)
                    return 1.0;

                return 1.0 / _updatePeriod;
            }
        }

        public override double UpdatePeriod
        {
            get { return _updatePeriod; }
        }

        public override double UpdateTime
        {
            get { return _updateTime; }
        }

        public override void Run()
        {
            Controller.Initialize();
            WindowResized();

            _startTime = DateTime.Now;
            _updateTimer.Start();
            _renderTimer.Start();
            _running = true;

            while (_running)
            {
                DispatchUpdate();

                if (!_running)
                    return;

                DispatchRender();
            }

            _updateTimer.Stop();
            _renderTimer.Stop();

            Controller.Shutdown();

            Exit();
        }
        
        public override void InitializeGraphics(PresentationParameters parameters)
        {
            if (_gameWindow != null && _graphicsDeviceManager != null)
                throw new InvalidOperationException("The graphic subsystem is already setup");
            
            Log.Message("Initializing Graphic subsystem");

            _gameWindow = new WindowsGameWindow(this, parameters);
            _graphicsDeviceManager = new GraphicsDeviceManager(Controller as Game, parameters, _gameWindow);

            if (_inputManager != null)
                _inputManager.Initialize(_gameWindow);
            
            Controller.RegisterService(typeof(IGraphicsDeviceManager), _graphicsDeviceManager);
        }

        public override void InitializeInput()
        {
            if (_inputManager != null)
                throw new InvalidOperationException("The input subsystem is already setup");

            Log.Message("Initializing Input subsystem");

            _inputManager = new InputManager();

            if (_gameWindow != null)
                _inputManager.Initialize(_gameWindow);

            Mouse.SetMouseImplementation(_inputManager);
            Keyboard.SetKeyboardImplementation(_inputManager);

            Controller.RegisterService(typeof(IInputManager), _inputManager);
        }

        public override void SkipFrame()
        {
            _skipFrame = true;
        }

        public override void Exit()
        {
            _running = false;

            _graphicsDeviceManager.Dispose();
            _gameWindow.Dispose();
        }

        public override void Dispose()
        {
            if (_running)
                Exit();
        }

        public void WindowResized()
        {
            if (_graphicsDeviceManager != null && _graphicsDeviceManager.GraphicsDevice != null)
                _graphicsDeviceManager.Update();

            Controller.OnResize();
        }

        public void SetActive(bool active)
        {
            Controller.SetActive(active);
        }

        private void DispatchUpdate()
        {
            int numUpdates = 0;
            double totalUpdateTime = 0;
            double time = _updateTimer.Elapsed.TotalSeconds;

            if (time <= 0)
            {
                _updateTimer.Reset();
                _updateTimer.Start();
                return;
            }

            if (time > 1.0)
                time = 1.0;

            while (_nextUpdate - time <= 0 && time > 0)
            {
                _nextUpdate -= time;

                _gameWindow.HandleEvents();

                if (!_running)
                    return;

                Controller.Update(new GameTime(DateTime.Now - _startTime, TimeSpan.FromSeconds(time)));

                if (!_running)
                    return;

                time = _updateTime = Math.Max(_updateTimer.Elapsed.TotalSeconds, 0) - time;

                _updateTimer.Reset();
                _updateTimer.Start();

                // Don't schedule a new update more than 1 second in the future.
                // Sometimes the hardware cannot keep up with updates
                // (e.g. when the update rate is too high, or the UpdateFrame processing
                // is too costly). This cap ensures  we can catch up in a reasonable time
                // once the load becomes lighter.
                _nextUpdate += TargetUpdatePeriod;
                _nextUpdate = Math.Max(_nextUpdate, -1.0);

                totalUpdateTime += _updateTime;

                // Allow up to 10 consecutive UpdateFrame events to prevent the
                // application from "hanging" when the hardware cannot keep up
                // with the requested update rate.
                if (++numUpdates >= 10 || TargetUpdateFrequency == 0.0)
                    break;
            }

            if (numUpdates > 0)
                _updatePeriod = totalUpdateTime / numUpdates;
        }

        private void DispatchRender()
        {
            double time = _renderTimer.Elapsed.TotalSeconds;

            if (time <= 0)
            {
                _renderTimer.Reset();
                _renderTimer.Start();
                return;
            }

            if (time > 1.0)
                time = 1.0;

            double timeLeft = _nextRender - time;

            if (timeLeft <= 0.0 && time > 0)
            {
                _nextRender = timeLeft + TargetRenderPeriod;

                if (_nextRender < -1.0)
                    _nextRender = -1.0;

                _renderTimer.Reset();
                _renderTimer.Start();

                if (time > 0 && !_skipFrame)
                {
                    Controller.Render(new GameTime(DateTime.Now - _startTime, TimeSpan.FromSeconds(time),
                                                   RenderTime > 2.0 * TargetRenderPeriod));
                    _renderTime = _renderTimer.Elapsed.TotalSeconds;
                }
                else if (_skipFrame)
                {
                    _skipFrame = false;
                }
            }
        }

    }
}
