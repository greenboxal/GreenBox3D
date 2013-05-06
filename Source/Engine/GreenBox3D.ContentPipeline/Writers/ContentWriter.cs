// ContentWriter.cs
// 
// Copyright (c) 2013 The GreenBox Development LLC, all rights reserved
// 
// This file is a proprietary part of GreenBox3D, disclosing the content
// of this file without the owner consent may lead to legal actions
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GreenBox3D.ContentPipeline.CompilerServices;

namespace GreenBox3D.ContentPipeline.Writers
{
    public class ContentWriter : BinaryWriter
    {
        private readonly BuildCoordinator _coordinator;

        #region Constructors and Destructors

        internal ContentWriter(BuildCoordinator coordinator, Stream stream, Encoding encoding)
            : base(stream, encoding, true)
        {
            _coordinator = coordinator;
        }

        #endregion

        #region Public Methods and Operators

        public void Write(Vector2 value)
        {
            Write(value.X);
            Write(value.Y);
        }

        public void Write(Vector3 value)
        {
            Write(value.X);
            Write(value.Y);
            Write(value.Z);
        }

        public void Write(Vector4 value)
        {
            Write(value.X);
            Write(value.Y);
            Write(value.Z);
            Write(value.W);
        }

        public void Write(Color value)
        {
            Write(value.PackedValue);
        }

        public void Write(Quaternion value)
        {
            Write(value.X);
            Write(value.Y);
            Write(value.Z);
            Write(value.W);
        }

        public void Write(Matrix4 value)
        {
            Write(value.M11);
            Write(value.M12);
            Write(value.M13);
            Write(value.M14);
            Write(value.M21);
            Write(value.M22);
            Write(value.M23);
            Write(value.M24);
            Write(value.M31);
            Write(value.M32);
            Write(value.M33);
            Write(value.M34);
            Write(value.M41);
            Write(value.M42);
            Write(value.M43);
            Write(value.M44);
        }

        public void Write(Point value)
        {
            Write(value.X);
            Write(value.Y);
        }

        public void Write(Rectangle value)
        {
            Write(value.X);
            Write(value.Y);
            Write(value.Width);
            Write(value.Height);
        }

        #endregion

        public void WriteRawObject<T>(T value)
        {
            Flush();
            _coordinator.StartWriteRawObject(typeof(T), BaseStream, value);
        }
    }
}
