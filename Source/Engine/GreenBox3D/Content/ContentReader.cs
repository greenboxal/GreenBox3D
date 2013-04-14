// ContentReader.cs
// 
// Copyright (c) 2013 The GreenBox Development LLC, all rights reserved
// 
// This file is a proprietary part of GreenBox3D, disclosing the content
// of this file without the owner consent may lead to legal actions

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenBox3D.Content
{
    public class ContentReader : BinaryReader
    {
        #region Constructors and Destructors

        internal ContentReader(Stream input, Encoding encoding)
            : base(input, encoding, true)
        {
        }

        #endregion

        #region Public Methods and Operators

        public Color ReadColor()
        {
            return new Color(ReadUInt32());
        }

        public Matrix4 ReadMatrix4()
        {
            return new Matrix4(ReadSingle(), ReadSingle(), ReadSingle(), ReadSingle(), ReadSingle(), ReadSingle(),
                               ReadSingle(), ReadSingle(), ReadSingle(), ReadSingle(), ReadSingle(), ReadSingle(),
                               ReadSingle(), ReadSingle(), ReadSingle(), ReadSingle());
        }

        public Point ReadPoint()
        {
            return new Point(ReadInt32(), ReadInt32());
        }

        public Rectangle ReadRectangle()
        {
            return new Rectangle(ReadInt32(), ReadInt32(), ReadInt32(), ReadInt32());
        }

        public Vector2 ReadVector2()
        {
            return new Vector2(ReadSingle(), ReadSingle());
        }

        public Vector3 ReadVector3()
        {
            return new Vector3(ReadSingle(), ReadSingle(), ReadSingle());
        }

        public Vector4 ReadVector4()
        {
            return new Vector4(ReadSingle(), ReadSingle(), ReadSingle(), ReadSingle());
        }

        #endregion
    }
}
