// RectF.cs
// 
// Copyright (c) 2013 The GreenBox Development LLC, all rights reserved
// 
// This file is a proprietary part of GreenBox3D, disclosing the content
// of this file without the owner consent may lead to legal actions
using System;
using System.Collections.Generic;
using System.Text;

namespace GreenBox3D
{
    /// <summary>
    ///     Represents a rectangular region on a two-dimensional plane.
    /// </summary>
    public struct RectF : IEquatable<RectF>
    {
        #region Fields

        private Vector2 location;
        private SizeF size;

        #endregion

        #region Constructors

        /// <summary>
        ///     Constructs a new RectF instance.
        /// </summary>
        /// <param name="location">The top-left corner of the RectF.</param>
        /// <param name="size">The width and height of the RectF.</param>
        public RectF(Vector2 location, SizeF size)
            : this()
        {
            Location = location;
            Size = size;
        }

        /// <summary>
        ///     Constructs a new RectF instance.
        /// </summary>
        /// <param name="x">The x coordinate of the RectF.</param>
        /// <param name="y">The y coordinate of the RectF.</param>
        /// <param name="width">The width coordinate of the RectF.</param>
        /// <param name="height">The height coordinate of the RectF.</param>
        public RectF(int x, int y, int width, int height)
            : this(new Vector2(x, y), new SizeF(width, height))
        {
        }

        #endregion

        #region Public Members

        /// <summary>
        ///     Defines the empty RectF.
        /// </summary>
        public static readonly RectF Zero = new RectF();

        /// <summary>
        ///     Defines the empty RectF.
        /// </summary>
        public static readonly RectF Empty = new RectF();

        /// <summary>
        ///     Gets or sets the x coordinate of the RectF.
        /// </summary>
        public float X
        {
            get { return Location.X; }
            set { Location = new Vector2(value, Y); }
        }

        /// <summary>
        ///     Gets or sets the y coordinate of the RectF.
        /// </summary>
        public float Y
        {
            get { return Location.Y; }
            set { Location = new Vector2(X, value); }
        }

        /// <summary>
        ///     Gets or sets the width of the RectF.
        /// </summary>
        public float Width
        {
            get { return Size.Width; }
            set { Size = new SizeF(value, Height); }
        }

        /// <summary>
        ///     Gets or sets the height of the RectF.
        /// </summary>
        public float Height
        {
            get { return Size.Height; }
            set { Size = new SizeF(Width, value); }
        }

        /// <summary>
        ///     Gets or sets a <see cref="Vector2" /> representing the x and y coordinates
        ///     of the RectF.
        /// </summary>
        public Vector2 Location
        {
            get { return location; }
            set { location = value; }
        }

        /// <summary>
        ///     Gets or sets a <see cref="SizeF" /> representing the width and height
        ///     of the RectF.
        /// </summary>
        public SizeF Size
        {
            get { return size; }
            set { size = value; }
        }

        /// <summary>
        ///     Gets the y coordinate of the top edge of this RectF.
        /// </summary>
        public float Top
        {
            get { return Y; }
        }

        /// <summary>
        ///     Gets the x coordinate of the right edge of this RectF.
        /// </summary>
        public float Right
        {
            get { return X + Width; }
        }

        /// <summary>
        ///     Gets the y coordinate of the bottom edge of this RectF.
        /// </summary>
        public float Bottom
        {
            get { return Y + Height; }
        }

        /// <summary>
        ///     Gets the x coordinate of the left edge of this RectF.
        /// </summary>
        public float Left
        {
            get { return X; }
        }

        /// <summary>
        ///     Gets a <see cref="System.Boolean" /> that indicates whether this
        ///     RectF is equal to the empty RectF.
        /// </summary>
        public bool IsEmpty
        {
            get { return Location == Vector2.Zero && Size == SizeF.Zero; }
        }

        /// <summary>
        ///     Constructs a new instance with the specified edges.
        /// </summary>
        /// <param name="left">The left edge of the RectF.</param>
        /// <param name="top">The top edge of the RectF.</param>
        /// <param name="right">The right edge of the RectF.</param>
        /// <param name="bottom">The bottom edge of the RectF.</param>
        /// <returns>A new RectF instance with the specified edges.</returns>
        public static RectF FromLTRB(float left, float top, float right, float bottom)
        {
            return new RectF(new Vector2(left, top), new SizeF(right - left, bottom - top));
        }

        /// <summary>
        ///     Tests whether this instance contains the specified Vector2.
        /// </summary>
        /// <param name="point">
        ///     The <see cref="Vector2" /> to test.
        /// </param>
        /// <returns>True if this instance contains point; false otherwise.</returns>
        /// <remarks>
        ///     The left and top edges are inclusive. The right and bottom edges
        ///     are exclusive.
        /// </remarks>
        public bool Contains(Vector2 point)
        {
            return point.X >= Left && point.X < Right &&
                   point.Y >= Top && point.Y < Bottom;
        }

        /// <summary>
        ///     Tests whether this instance contains the specified RectF.
        /// </summary>
        /// <param name="rect">
        ///     The <see cref="RectF" /> to test.
        /// </param>
        /// <returns>True if this instance contains rect; false otherwise.</returns>
        /// <remarks>
        ///     The left and top edges are inclusive. The right and bottom edges
        ///     are exclusive.
        /// </remarks>
        public bool Contains(RectF rect)
        {
            return Contains(rect.Location) && Contains(rect.Location + rect.Size.ToVector2());
        }

        /// <summary>
        ///     Compares two instances for equality.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>True, if left is equal to right; false otherwise.</returns>
        public static bool operator ==(RectF left, RectF right)
        {
            return left.Equals(right);
        }

        /// <summary>
        ///     Compares two instances for inequality.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>True, if left is not equal to right; false otherwise.</returns>
        public static bool operator !=(RectF left, RectF right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        ///     Indicates whether this instance is equal to the specified object.
        /// </summary>
        /// <param name="obj">The object instance to compare to.</param>
        /// <returns>True, if both instances are equal; false otherwise.</returns>
        public override bool Equals(object obj)
        {
            if (obj is RectF)
                return Equals((RectF)obj);

            return false;
        }

        /// <summary>
        ///     Returns the hash code for this instance.
        /// </summary>
        /// <returns>
        ///     A <see cref="System.Int32" /> that represents the hash code for this instance./>
        /// </returns>
        public override int GetHashCode()
        {
            return Location.GetHashCode() & Size.GetHashCode();
        }

        /// <summary>
        ///     Returns a <see cref="System.String" /> that describes this instance.
        /// </summary>
        /// <returns>
        ///     A <see cref="System.String" /> that describes this instance.
        /// </returns>
        public override string ToString()
        {
            return String.Format("{{{0}-{1}}}", Location, Location + Size.ToVector2());
        }

        #endregion

        #region IEquatable<RectF> Members

        /// <summary>
        ///     Indicates whether this instance is equal to the specified RectF.
        /// </summary>
        /// <param name="other">The instance to compare to.</param>
        /// <returns>True, if both instances are equal; false otherwise.</returns>
        public bool Equals(RectF other)
        {
            return Location.Equals(other.Location) &&
                   Size.Equals(other.Size);
        }

        #endregion
    }
}
