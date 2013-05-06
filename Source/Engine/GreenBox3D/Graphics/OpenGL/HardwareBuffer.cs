// HardwareBuffer.cs
// 
// Copyright (c) 2013 The GreenBox Development LLC, all rights reserved
// 
// This file is a proprietary part of GreenBox3D, disclosing the content
// of this file without the owner consent may lead to legal actions

#if DESKTOP

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;

namespace GreenBox3D.Graphics
{
    public abstract class HardwareBuffer : GraphicsResource
    {
        #region Fields

        internal int BufferID;
        internal BufferTarget BufferTarget;
        internal BufferUsageHint BufferUsageHint;
        internal int ElementSize;

        #endregion

        #region Constructors and Destructors

        internal protected HardwareBuffer(BufferTarget bufferTarget, int elementSize,
                                int elementCount, BufferUsage usage)
        {
            BufferID = -1;
            BufferUsage = usage;
            BufferTarget = bufferTarget;
            ElementSize = elementSize;
            ElementCount = elementCount;

            switch (BufferUsage)
            {
                case BufferUsage.DynamicCopy:
                    BufferUsageHint = BufferUsageHint.DynamicCopy;
                    break;
                case BufferUsage.DynamicDraw:
                    BufferUsageHint = BufferUsageHint.DynamicDraw;
                    break;
                case BufferUsage.DynamicRead:
                    BufferUsageHint = BufferUsageHint.DynamicRead;
                    break;
                case BufferUsage.StaticCopy:
                    BufferUsageHint = BufferUsageHint.StaticCopy;
                    break;
                case BufferUsage.StaticDraw:
                    BufferUsageHint = BufferUsageHint.StaticDraw;
                    break;
                case BufferUsage.StaticRead:
                    BufferUsageHint = BufferUsageHint.StaticRead;
                    break;
                case BufferUsage.StreamCopy:
                    BufferUsageHint = BufferUsageHint.StreamCopy;
                    break;
                case BufferUsage.StreamDraw:
                    BufferUsageHint = BufferUsageHint.StreamDraw;
                    break;
                case BufferUsage.StreamRead:
                    BufferUsageHint = BufferUsageHint.StreamRead;
                    break;
            }
        }

        #endregion

        #region Public Properties

        public BufferUsage BufferUsage { get; private set; }
        public int ElementCount { get; private set; }

        #endregion

        #region Public Methods and Operators

        public void SetData<T>(T[] data) where T : struct
        {
            SetData(data, 0, data.Length);
        }

        public void SetData<T>(T[] data, int offset, int count) where T : struct
        {
            Create();

            GCHandle handle = GCHandle.Alloc(data, GCHandleType.Pinned);
            IntPtr address = Marshal.UnsafeAddrOfPinnedArrayElement(data, offset);

            Bind();
            GL.BufferData(BufferTarget, (IntPtr)(count * Marshal.SizeOf(typeof(T))), address, BufferUsageHint);

            handle.Free();
        }

        public void SetData<T>(int offsetInBytes, T[] data, int offset, int count) where T : struct
        {
            Create();

            GCHandle handle = GCHandle.Alloc(data);
            IntPtr address = Marshal.UnsafeAddrOfPinnedArrayElement(data, offset);

            Bind();
            GL.BufferSubData(BufferTarget, (IntPtr)offsetInBytes, (IntPtr)(count * Marshal.SizeOf(typeof(T))), address);

            handle.Free();
        }

        #endregion

        #region Methods

        internal void Bind()
        {
            GL.BindBuffer(BufferTarget, BufferID);
        }

        internal void Create()
        {
            if (BufferID == -1)
            {
                GL.GenBuffers(1, out BufferID);

                if (BufferID == -1)
                    throw new OpenGLException();

                // Bind();
                // GL.BufferData(BufferTarget, (IntPtr)(ElementSize * ElementCount), IntPtr.Zero, BufferUsageHint);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (BufferID != -1)
            {
                GL.DeleteBuffers(1, ref BufferID);
                BufferID = -1;
            }

            base.Dispose(disposing);
        }

        #endregion
    }
}

#endif
