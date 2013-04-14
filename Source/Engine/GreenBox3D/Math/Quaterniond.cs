// Quaterniond.cs
// 
// Copyright (c) 2013 The GreenBox Development LLC, all rights reserved
// 
// This file is a proprietary part of GreenBox3D, disclosing the content
// of this file without the owner consent may lead to legal actions

using System;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Xml.Serialization;

namespace GreenBox3D
{
    /// <summary>
    ///     Represents a double-precision Quaternion.
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Quaterniond : IEquatable<Quaterniond>
    {
        #region Fields

        private Vector3d xyz;
        private double w;

        #endregion

        #region Constructors

        /// <summary>
        ///     Construct a new Quaterniond from vector and w components
        /// </summary>
        /// <param name="v">The vector part</param>
        /// <param name="w">The w part</param>
        public Quaterniond(Vector3d v, double w)
        {
            xyz = v;
            this.w = w;
        }

        /// <summary>
        ///     Construct a new Quaterniond
        /// </summary>
        /// <param name="x">The x component</param>
        /// <param name="y">The y component</param>
        /// <param name="z">The z component</param>
        /// <param name="w">The w component</param>
        public Quaterniond(double x, double y, double z, double w)
            : this(new Vector3d(x, y, z), w)
        {
        }

        #endregion

        #region Public Members

        #region Properties

        /// <summary>
        ///     Gets or sets an OpenTK.Vector3d with the X, Y and Z components of this instance.
        /// </summary>
        public Vector3d Xyz
        {
            get { return xyz; }
            set { xyz = value; }
        }

        /// <summary>
        ///     Gets or sets the X component of this instance.
        /// </summary>
        [XmlIgnore]
        public double X
        {
            get { return xyz.X; }
            set { xyz.X = value; }
        }

        /// <summary>
        ///     Gets or sets the Y component of this instance.
        /// </summary>
        [XmlIgnore]
        public double Y
        {
            get { return xyz.Y; }
            set { xyz.Y = value; }
        }

        /// <summary>
        ///     Gets or sets the Z component of this instance.
        /// </summary>
        [XmlIgnore]
        public double Z
        {
            get { return xyz.Z; }
            set { xyz.Z = value; }
        }

        /// <summary>
        ///     Gets or sets the W component of this instance.
        /// </summary>
        public double W
        {
            get { return w; }
            set { w = value; }
        }

        #endregion

        #region Instance

        #region ToAxisAngle

        /// <summary>
        ///     Convert the current quaternion to axis angle representation
        /// </summary>
        /// <param name="axis">The resultant axis</param>
        /// <param name="angle">The resultant angle</param>
        public void ToAxisAngle(out Vector3d axis, out double angle)
        {
            Vector4d result = ToAxisAngle();
            axis = result.Xyz;
            angle = result.W;
        }

        /// <summary>
        ///     Convert this instance to an axis-angle representation.
        /// </summary>
        /// <returns>A Vector4 that is the axis-angle representation of this quaternion.</returns>
        public Vector4d ToAxisAngle()
        {
            Quaterniond q = this;
            if (Math.Abs(q.W) > 1.0f)
                q.Normalize();

            Vector4d result = new Vector4d();

            result.W = 2.0f * (float)Math.Acos(q.W); // angle
            float den = (float)Math.Sqrt(1.0 - q.W * q.W);
            if (den > 0.0001f)
            {
                result.Xyz = q.Xyz / den;
            }
            else
            {
                // This occurs when the angle is zero. 
                // Not a problem: just set an arbitrary normalized axis.
                result.Xyz = Vector3d.UnitX;
            }

            return result;
        }

        #endregion

        #region public double Length

        /// <summary>
        ///     Gets the length (magnitude) of the Quaterniond.
        /// </summary>
        /// <seealso cref="LengthSquared" />
        public double Length
        {
            get { return Math.Sqrt(W * W + Xyz.LengthSquared); }
        }

        #endregion

        #region public double LengthSquared

        /// <summary>
        ///     Gets the square of the Quaterniond length (magnitude).
        /// </summary>
        public double LengthSquared
        {
            get { return W * W + Xyz.LengthSquared; }
        }

        #endregion

        #region public void Normalize()

        /// <summary>
        ///     Scales the Quaterniond to unit length.
        /// </summary>
        public void Normalize()
        {
            double scale = 1.0f / Length;
            Xyz *= scale;
            W *= scale;
        }

        #endregion

        #region public void Conjugate()

        /// <summary>
        ///     Convert this Quaterniond to its conjugate
        /// </summary>
        public void Conjugate()
        {
            Xyz = -Xyz;
        }

        #endregion

        #endregion

        #region Static

        #region Fields

        /// <summary>
        ///     Defines the identity quaternion.
        /// </summary>
        public static readonly Quaterniond Identity = new Quaterniond(0, 0, 0, 1);

        #endregion

        #region Add

        /// <summary>
        ///     Add two quaternions
        /// </summary>
        /// <param name="left">The first operand</param>
        /// <param name="right">The second operand</param>
        /// <returns>The result of the addition</returns>
        public static Quaterniond Add(Quaterniond left, Quaterniond right)
        {
            return new Quaterniond(
                left.Xyz + right.Xyz,
                left.W + right.W);
        }

        /// <summary>
        ///     Add two quaternions
        /// </summary>
        /// <param name="left">The first operand</param>
        /// <param name="right">The second operand</param>
        /// <param name="result">The result of the addition</param>
        public static void Add(ref Quaterniond left, ref Quaterniond right, out Quaterniond result)
        {
            result = new Quaterniond(
                left.Xyz + right.Xyz,
                left.W + right.W);
        }

        #endregion

        #region Sub

        /// <summary>
        ///     Subtracts two instances.
        /// </summary>
        /// <param name="left">The left instance.</param>
        /// <param name="right">The right instance.</param>
        /// <returns>The result of the operation.</returns>
        public static Quaterniond Sub(Quaterniond left, Quaterniond right)
        {
            return new Quaterniond(
                left.Xyz - right.Xyz,
                left.W - right.W);
        }

        /// <summary>
        ///     Subtracts two instances.
        /// </summary>
        /// <param name="left">The left instance.</param>
        /// <param name="right">The right instance.</param>
        /// <param name="result">The result of the operation.</param>
        public static void Sub(ref Quaterniond left, ref Quaterniond right, out Quaterniond result)
        {
            result = new Quaterniond(
                left.Xyz - right.Xyz,
                left.W - right.W);
        }

        #endregion

        #region Mult

        /// <summary>
        ///     Multiplies two instances.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>A new instance containing the result of the calculation.</returns>
        [Obsolete("Use Multiply instead.")]
        public static Quaterniond Mult(Quaterniond left, Quaterniond right)
        {
            return new Quaterniond(
                right.W * left.Xyz + left.W * right.Xyz + Vector3d.Cross(left.Xyz, right.Xyz),
                left.W * right.W - Vector3d.Dot(left.Xyz, right.Xyz));
        }

        /// <summary>
        ///     Multiplies two instances.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <param name="result">A new instance containing the result of the calculation.</param>
        [Obsolete("Use Multiply instead.")]
        public static void Mult(ref Quaterniond left, ref Quaterniond right, out Quaterniond result)
        {
            result = new Quaterniond(
                right.W * left.Xyz + left.W * right.Xyz + Vector3d.Cross(left.Xyz, right.Xyz),
                left.W * right.W - Vector3d.Dot(left.Xyz, right.Xyz));
        }

        /// <summary>
        ///     Multiplies two instances.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>A new instance containing the result of the calculation.</returns>
        public static Quaterniond Multiply(Quaterniond left, Quaterniond right)
        {
            Quaterniond result;
            Multiply(ref left, ref right, out result);
            return result;
        }

        /// <summary>
        ///     Multiplies two instances.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <param name="result">A new instance containing the result of the calculation.</param>
        public static void Multiply(ref Quaterniond left, ref Quaterniond right, out Quaterniond result)
        {
            result = new Quaterniond(
                right.W * left.Xyz + left.W * right.Xyz + Vector3d.Cross(left.Xyz, right.Xyz),
                left.W * right.W - Vector3d.Dot(left.Xyz, right.Xyz));
        }

        /// <summary>
        ///     Multiplies an instance by a scalar.
        /// </summary>
        /// <param name="quaternion">The instance.</param>
        /// <param name="scale">The scalar.</param>
        /// <param name="result">A new instance containing the result of the calculation.</param>
        public static void Multiply(ref Quaterniond quaternion, double scale, out Quaterniond result)
        {
            result = new Quaterniond(quaternion.X * scale, quaternion.Y * scale, quaternion.Z * scale,
                                     quaternion.W * scale);
        }

        /// <summary>
        ///     Multiplies an instance by a scalar.
        /// </summary>
        /// <param name="quaternion">The instance.</param>
        /// <param name="scale">The scalar.</param>
        /// <returns>A new instance containing the result of the calculation.</returns>
        public static Quaterniond Multiply(Quaterniond quaternion, double scale)
        {
            return new Quaterniond(quaternion.X * scale, quaternion.Y * scale, quaternion.Z * scale,
                                   quaternion.W * scale);
        }

        #endregion

        #region Conjugate

        /// <summary>
        ///     Get the conjugate of the given Quaterniond
        /// </summary>
        /// <param name="q">The Quaterniond</param>
        /// <returns>The conjugate of the given Quaterniond</returns>
        public static Quaterniond Conjugate(Quaterniond q)
        {
            return new Quaterniond(-q.Xyz, q.W);
        }

        /// <summary>
        ///     Get the conjugate of the given Quaterniond
        /// </summary>
        /// <param name="q">The Quaterniond</param>
        /// <param name="result">The conjugate of the given Quaterniond</param>
        public static void Conjugate(ref Quaterniond q, out Quaterniond result)
        {
            result = new Quaterniond(-q.Xyz, q.W);
        }

        #endregion

        #region Invert

        /// <summary>
        ///     Get the inverse of the given Quaterniond
        /// </summary>
        /// <param name="q">The Quaterniond to invert</param>
        /// <returns>The inverse of the given Quaterniond</returns>
        public static Quaterniond Invert(Quaterniond q)
        {
            Quaterniond result;
            Invert(ref q, out result);
            return result;
        }

        /// <summary>
        ///     Get the inverse of the given Quaterniond
        /// </summary>
        /// <param name="q">The Quaterniond to invert</param>
        /// <param name="result">The inverse of the given Quaterniond</param>
        public static void Invert(ref Quaterniond q, out Quaterniond result)
        {
            double lengthSq = q.LengthSquared;
            if (lengthSq != 0.0)
            {
                double i = 1.0f / lengthSq;
                result = new Quaterniond(q.Xyz * -i, q.W * i);
            }
            else
            {
                result = q;
            }
        }

        #endregion

        #region Normalize

        /// <summary>
        ///     Scale the given Quaterniond to unit length
        /// </summary>
        /// <param name="q">The Quaterniond to normalize</param>
        /// <returns>The normalized Quaterniond</returns>
        public static Quaterniond Normalize(Quaterniond q)
        {
            Quaterniond result;
            Normalize(ref q, out result);
            return result;
        }

        /// <summary>
        ///     Scale the given Quaterniond to unit length
        /// </summary>
        /// <param name="q">The Quaterniond to normalize</param>
        /// <param name="result">The normalized Quaterniond</param>
        public static void Normalize(ref Quaterniond q, out Quaterniond result)
        {
            double scale = 1.0f / q.Length;
            result = new Quaterniond(q.Xyz * scale, q.W * scale);
        }

        #endregion

        #region FromAxisAngle

        /// <summary>
        ///     Build a Quaterniond from the given axis and angle
        /// </summary>
        /// <param name="axis">The axis to rotate about</param>
        /// <param name="angle">The rotation angle in radians</param>
        /// <returns></returns>
        public static Quaterniond FromAxisAngle(Vector3d axis, double angle)
        {
            if (axis.LengthSquared == 0.0f)
                return Identity;

            Quaterniond result = Identity;

            angle *= 0.5f;
            axis.Normalize();
            result.Xyz = axis * Math.Sin(angle);
            result.W = Math.Cos(angle);

            return Normalize(result);
        }

        #endregion

        #region Slerp

        /// <summary>
        ///     Do Spherical linear interpolation between two quaternions
        /// </summary>
        /// <param name="q1">The first Quaterniond</param>
        /// <param name="q2">The second Quaterniond</param>
        /// <param name="blend">The blend factor</param>
        /// <returns>A smooth blend between the given quaternions</returns>
        public static Quaterniond Slerp(Quaterniond q1, Quaterniond q2, double blend)
        {
            // if either input is zero, return the other.
            if (q1.LengthSquared == 0.0f)
            {
                if (q2.LengthSquared == 0.0f)
                {
                    return Identity;
                }
                return q2;
            }
            else if (q2.LengthSquared == 0.0f)
            {
                return q1;
            }

            double cosHalfAngle = q1.W * q2.W + Vector3d.Dot(q1.Xyz, q2.Xyz);

            if (cosHalfAngle >= 1.0f || cosHalfAngle <= -1.0f)
            {
                // angle = 0.0f, so just return one input.
                return q1;
            }
            else if (cosHalfAngle < 0.0f)
            {
                q2.Xyz = -q2.Xyz;
                q2.W = -q2.W;
                cosHalfAngle = -cosHalfAngle;
            }

            double blendA;
            double blendB;
            if (cosHalfAngle < 0.99f)
            {
                // do proper slerp for big angles
                double halfAngle = Math.Acos(cosHalfAngle);
                double sinHalfAngle = Math.Sin(halfAngle);
                double oneOverSinHalfAngle = 1.0f / sinHalfAngle;
                blendA = Math.Sin(halfAngle * (1.0f - blend)) * oneOverSinHalfAngle;
                blendB = Math.Sin(halfAngle * blend) * oneOverSinHalfAngle;
            }
            else
            {
                // do lerp if angle is really small.
                blendA = 1.0f - blend;
                blendB = blend;
            }

            Quaterniond result = new Quaterniond(blendA * q1.Xyz + blendB * q2.Xyz, blendA * q1.W + blendB * q2.W);
            if (result.LengthSquared > 0.0f)
                return Normalize(result);
            else
                return Identity;
        }

        #endregion

        #endregion

        #region Operators

        /// <summary>
        ///     Adds two instances.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>The result of the calculation.</returns>
        public static Quaterniond operator +(Quaterniond left, Quaterniond right)
        {
            left.Xyz += right.Xyz;
            left.W += right.W;
            return left;
        }

        /// <summary>
        ///     Subtracts two instances.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>The result of the calculation.</returns>
        public static Quaterniond operator -(Quaterniond left, Quaterniond right)
        {
            left.Xyz -= right.Xyz;
            left.W -= right.W;
            return left;
        }

        /// <summary>
        ///     Multiplies two instances.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>The result of the calculation.</returns>
        public static Quaterniond operator *(Quaterniond left, Quaterniond right)
        {
            Multiply(ref left, ref right, out left);
            return left;
        }

        /// <summary>
        ///     Multiplies an instance by a scalar.
        /// </summary>
        /// <param name="quaternion">The instance.</param>
        /// <param name="scale">The scalar.</param>
        /// <returns>A new instance containing the result of the calculation.</returns>
        public static Quaterniond operator *(Quaterniond quaternion, double scale)
        {
            Multiply(ref quaternion, scale, out quaternion);
            return quaternion;
        }

        /// <summary>
        ///     Multiplies an instance by a scalar.
        /// </summary>
        /// <param name="quaternion">The instance.</param>
        /// <param name="scale">The scalar.</param>
        /// <returns>A new instance containing the result of the calculation.</returns>
        public static Quaterniond operator *(double scale, Quaterniond quaternion)
        {
            return new Quaterniond(quaternion.X * scale, quaternion.Y * scale, quaternion.Z * scale,
                                   quaternion.W * scale);
        }

        /// <summary>
        ///     Compares two instances for equality.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>True, if left equals right; false otherwise.</returns>
        public static bool operator ==(Quaterniond left, Quaterniond right)
        {
            return left.Equals(right);
        }

        /// <summary>
        ///     Compares two instances for inequality.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>True, if left does not equal right; false otherwise.</returns>
        public static bool operator !=(Quaterniond left, Quaterniond right)
        {
            return !left.Equals(right);
        }

        #endregion

        #region Overrides

        #region public override string ToString()

        /// <summary>
        ///     Returns a System.String that represents the current Quaterniond.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("V: {0}, W: {1}", Xyz, W);
        }

        #endregion

        #region public override bool Equals (object o)

        /// <summary>
        ///     Compares this object instance to another object for equality.
        /// </summary>
        /// <param name="other">The other object to be used in the comparison.</param>
        /// <returns>True if both objects are Quaternions of equal value. Otherwise it returns false.</returns>
        public override bool Equals(object other)
        {
            if (other is Quaterniond == false)
                return false;
            return this == (Quaterniond)other;
        }

        #endregion

        #region public override int GetHashCode ()

        /// <summary>
        ///     Provides the hash code for this object.
        /// </summary>
        /// <returns>A hash code formed from the bitwise XOR of this objects members.</returns>
        public override int GetHashCode()
        {
            return Xyz.GetHashCode() ^ W.GetHashCode();
        }

        #endregion

        #endregion

        #endregion

        #region IEquatable<Quaterniond> Members

        /// <summary>
        ///     Compares this Quaterniond instance to another Quaterniond for equality.
        /// </summary>
        /// <param name="other">The other Quaterniond to be used in the comparison.</param>
        /// <returns>True if both instances are equal; false otherwise.</returns>
        public bool Equals(Quaterniond other)
        {
            return Xyz == other.Xyz && W == other.W;
        }

        #endregion
    }
}
