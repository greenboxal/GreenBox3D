// GreenBox3D
// 
// Copyright (c) 2013 The GreenBox Development Inc.
// Copyright (c) 2013 Mono.Xna Team and Contributors
// 
// Licensed under MIT license terms.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenBox3D.Graphics
{
    [Serializable]
    public struct Viewport
    {
        #region Fields

        private int _height;
        private float _maxDepth;
        private float _minDepth;
        private int _width;
        private int _x;
        private int _y;

        #endregion

        #region Constructors and Destructors

        public Viewport(int x, int y, int width, int height)
        {
            _x = x;
            _y = y;
            _width = width;
            _height = height;
            _minDepth = 0.0f;
            _maxDepth = 1.0f;
        }

        public Viewport(Rectangle bounds)
            : this(bounds.X, bounds.Y, bounds.Width, bounds.Height)
        {
        }

        #endregion

        #region Public Properties

        public float AspectRatio
        {
            get
            {
                if ((_height != 0) && (_width != 0))
                    return ((_width) / ((float)_height));
                return 0f;
            }
        }

        public Rectangle Bounds
        {
            get
            {
                Rectangle rectangle = new Rectangle();

                rectangle.X = _x;
                rectangle.Y = _y;
                rectangle.Width = _width;
                rectangle.Height = _height;
                return rectangle;
            }
            set
            {
                _x = value.X;
                _y = value.Y;
                _width = value.Width;
                _height = value.Height;
            }
        }

        public int Height { get { return _height; } set { _height = value; } }
        public float MaxDepth { get { return _maxDepth; } set { _maxDepth = value; } }
        public float MinDepth { get { return _minDepth; } set { _minDepth = value; } }
        public Rectangle TitleSafeArea { get { return new Rectangle(_x, _y, _width, _height); } }
        public int Width { get { return _width; } set { _width = value; } }
        public int X { get { return _x; } set { _x = value; } }
        public int Y { get { return _y; } set { _y = value; } }

        #endregion

        #region Public Methods and Operators

        public Vector3 Project(Vector3 source, Matrix4 projection, Matrix4 view, Matrix4 world)
        {
            Matrix4 matrix = Matrix4.Mult(Matrix4.Mult(world, view), projection);
            Vector3 vector = Vector3.Transform(source, matrix);
            float a = (((source.X * matrix.M14) + (source.Y * matrix.M24)) + (source.Z * matrix.M34)) + matrix.M44;

            if (!WithinEpsilon(a, 1f))
                vector = (vector / a);

            vector.X = (((vector.X + 1f) * 0.5f) * Width) + X;
            vector.Y = (((-vector.Y + 1f) * 0.5f) * Height) + Y;
            vector.Z = (vector.Z * (MaxDepth - MinDepth)) + MinDepth;

            return vector;
        }

        public override string ToString()
        {
            return string.Format("[Viewport: X={0} Y={1} Width={2} Height={3}]", X, Y, Width, Height);
        }

        public Vector3 Unproject(Vector3 source, Matrix4 projection, Matrix4 view, Matrix4 world)
        {
            Matrix4 matrix = Matrix4.Invert(Matrix4.Mult(Matrix4.Mult(world, view), projection));

            source.X = (((source.X - X) / (Width)) * 2f) - 1f;
            source.Y = -((((source.Y - Y) / (Height)) * 2f) - 1f);
            source.Z = (source.Z - MinDepth) / (MaxDepth - MinDepth);

            Vector3 vector = Vector3.Transform(source, matrix);
            float a = (((source.X * matrix.M14) + (source.Y * matrix.M24)) + (source.Z * matrix.M34)) + matrix.M44;

            if (!WithinEpsilon(a, 1f))
                vector = (vector / a);

            return vector;
        }

        #endregion

        #region Methods

        private static bool WithinEpsilon(float a, float b)
        {
            float num = a - b;
            return ((-1.401298E-45f <= num) && (num <= float.Epsilon));
        }

        #endregion
    }
}