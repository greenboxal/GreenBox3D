// VectorUtils.cs
// 
// Copyright (c) 2013 The GreenBox Development LLC, all rights reserved
// 
// This file is a proprietary part of GreenBox3D, disclosing the content
// of this file without the owner consent may lead to legal actions
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using GreenBox3D.Graphics;

namespace GreenBox3D
{
    public static class VectorUtils
    {
        private static readonly Dictionary<Type, PixelDataType> _type2surfaceFormat;

        static VectorUtils()
        {
            _type2surfaceFormat = new Dictionary<Type, PixelDataType>();

            _type2surfaceFormat[typeof(float)] = new PixelDataType(PixelFormat.R, PixelType.Float);
            _type2surfaceFormat[typeof(Half)] = new PixelDataType(PixelFormat.R, PixelType.HalfFloat);
            _type2surfaceFormat[typeof(Vector2)] = new PixelDataType(PixelFormat.Rg, PixelType.Float);
            _type2surfaceFormat[typeof(Vector4)] = new PixelDataType(PixelFormat.Rgba, PixelType.Float);
            _type2surfaceFormat[typeof(Vector2h)] = new PixelDataType(PixelFormat.Rg, PixelType.HalfFloat);
            _type2surfaceFormat[typeof(Vector4h)] = new PixelDataType(PixelFormat.Rgba, PixelType.HalfFloat);
            _type2surfaceFormat[typeof(Color)] = new PixelDataType(PixelFormat.Rgba, PixelType.UnsignedByte);
            _type2surfaceFormat[typeof(byte)] = new PixelDataType(PixelFormat.R, PixelType.UnsignedByte);
            _type2surfaceFormat[typeof(sbyte)] = new PixelDataType(PixelFormat.R, PixelType.Byte);
        }

        public static Converter<TSource, TDest> GetConverter<TSource, TDest>()
        {
            Converter<TSource, Vector4> unpack = (Converter<TSource, Vector4>)GetUnpacker(typeof(TSource));
            Converter<Vector4, TDest> pack = (Converter<Vector4, TDest>)GetPacker(typeof(TDest));

            return value => pack(unpack(value));
        }

        public static object GetUnpacker(Type type)
        {
            if (type == typeof(float))
                return new Converter<float, Vector4>(value => new Vector4(value, 0.0f, 0.0f, 1.0f));

            if (type == typeof(Vector2))
                return new Converter<Vector2, Vector4>(value => new Vector4(value.X, value.Y, 0.0f, 1.0f));

            if (type == typeof(Vector3))
                return new Converter<Vector3, Vector4>(value => new Vector4(value.X, value.Y, value.Z, 1.0f));

            if (type == typeof(Vector4))
                return new Converter<Vector4, Vector4>(value => value);

            if (IsPackedVector(type))
            {
                return
                    typeof(VectorUtils).GetMethod("MakeUnpacker", BindingFlags.Static | BindingFlags.NonPublic)
                                       .MakeGenericMethod(type)
                                       .Invoke(null, null);
            }

            throw new NotSupportedException("This type can't be unpacked");
        }

        public static object GetPacker(Type type)
        {
            if (type == typeof(float))
                return new Converter<Vector4, float>(value => value.X);

            if (type == typeof(Vector2))
                return new Converter<Vector4, Vector2>(value => new Vector2(value.X, value.Y));

            if (type == typeof(Vector3))
                return new Converter<Vector4, Vector3>(value => new Vector3(value.X, value.Y, value.Z));

            if (type == typeof(Vector4))
                return new Converter<Vector4, Vector4>(value => value);

            if (IsPackedVector(type))
            {
                return
                    typeof(VectorUtils).GetMethod("MakePacker", BindingFlags.Static | BindingFlags.NonPublic)
                                       .MakeGenericMethod(type)
                                       .Invoke(null, null);
            }

            throw new NotSupportedException("This type can't be packed");
        }

// ReSharper disable UnusedMember.Local
        private static Converter<T, Vector4> MakeUnpacker<T>() where T : IPackedVector
        {
            return value => value.ToVector4();
        }

        private static Converter<Vector4, T> MakePacker<T>() where T : IPackedVector
        {
            return value =>
            {
                T packed = default(T);
                packed.LoadFromVector4(value);
                return packed;
            };
        }

// ReSharper restore UnusedMember.Local

        private static bool IsPackedVector(Type type)
        {
            return type.IsValueType && typeof(IPackedVector).IsAssignableFrom(type);
        }

        public static bool TryGetSurfaceFormat(Type type, out PixelDataType format)
        {
            return _type2surfaceFormat.TryGetValue(type, out format);
        }
    }
}
