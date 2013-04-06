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

namespace GreenBox3D.Input
{
    public struct KeyboardState
    {
        // Used for the common situation where GetPressedKeys will return an empty array

        #region Static Fields

        private static readonly Keys[] _Empty = new Keys[0];

        #endregion

        // Array of 256 bits:

        #region Fields

        private uint _Keys0, _Keys1, _Keys2, _Keys3, _Keys4, _Keys5, _Keys6, _Keys7;

        #endregion

        #region Constructors and Destructors

        public KeyboardState(params Keys[] keys)
        {
            _Keys0 = 0;
            _Keys1 = 0;
            _Keys2 = 0;
            _Keys3 = 0;
            _Keys4 = 0;
            _Keys5 = 0;
            _Keys6 = 0;
            _Keys7 = 0;

            if (keys != null)
            {
                foreach (Keys k in keys)
                    InternalSetKey(k);
            }
        }

        internal KeyboardState(IEnumerable<Keys> keys)
        {
            _Keys0 = 0;
            _Keys1 = 0;
            _Keys2 = 0;
            _Keys3 = 0;
            _Keys4 = 0;
            _Keys5 = 0;
            _Keys6 = 0;
            _Keys7 = 0;

            if (keys != null)
            {
                foreach (Keys k in keys)
                    InternalSetKey(k);
            }
        }

        #endregion

        #region Public Indexers

        public KeyState this[Keys key] { get { return InternalGetKey(key) ? KeyState.Down : KeyState.Up; } }

        #endregion

        #region Public Methods and Operators

        public static bool operator ==(KeyboardState a, KeyboardState b)
        {
            return a._Keys0 == b._Keys0 && a._Keys1 == b._Keys1 && a._Keys2 == b._Keys2 && a._Keys3 == b._Keys3 && a._Keys4 == b._Keys4 && a._Keys5 == b._Keys5 && a._Keys6 == b._Keys6 && a._Keys7 == b._Keys7;
        }

        public static bool operator !=(KeyboardState a, KeyboardState b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            return obj is KeyboardState && this == (KeyboardState)obj;
        }

        public override int GetHashCode()
        {
            return (int)(_Keys0 ^ _Keys1 ^ _Keys2 ^ _Keys3 ^ _Keys4 ^ _Keys5 ^ _Keys6 ^ _Keys7);
        }

        public Keys[] GetPressedKeys()
        {
            uint count = CountBits(_Keys0) + CountBits(_Keys1) + CountBits(_Keys2) + CountBits(_Keys3) + CountBits(_Keys4) + CountBits(_Keys5) + CountBits(_Keys6) + CountBits(_Keys7);

            if (count == 0)
                return _Empty;

            Keys[] keys = new Keys[count];

            int index = 0;

            if (_Keys0 != 0)
                index = AddKeysToArray(_Keys0, 0 * 32, keys, index);

            if (_Keys1 != 0)
                index = AddKeysToArray(_Keys1, 1 * 32, keys, index);

            if (_Keys2 != 0)
                index = AddKeysToArray(_Keys2, 2 * 32, keys, index);

            if (_Keys3 != 0)
                index = AddKeysToArray(_Keys3, 3 * 32, keys, index);

            if (_Keys4 != 0)
                index = AddKeysToArray(_Keys4, 4 * 32, keys, index);

            if (_Keys5 != 0)
                index = AddKeysToArray(_Keys5, 5 * 32, keys, index);

            if (_Keys6 != 0)
                index = AddKeysToArray(_Keys6, 6 * 32, keys, index);

            if (_Keys7 != 0)
                index = AddKeysToArray(_Keys7, 7 * 32, keys, index);

            return keys;
        }

        public bool IsKeyDown(Keys key)
        {
            return InternalGetKey(key);
        }

        public bool IsKeyUp(Keys key)
        {
            return !InternalGetKey(key);
        }

        #endregion

        #region Methods

        private static int AddKeysToArray(uint keys, int offset, Keys[] pressedKeys, int index)
        {
            for (int i = 0; i < 32; i++)
            {
                if ((keys & (1 << i)) != 0)
                    pressedKeys[index++] = (Keys)(offset + i);
            }
            return index;
        }

        private static uint CountBits(uint v)
        {
            // http://graphics.stanford.edu/~seander/bithacks.html#CountBitsSetParallel
            v = v - ((v >> 1) & 0x55555555); // reuse input as temporary
            v = (v & 0x33333333) + ((v >> 2) & 0x33333333); // temp
            return ((v + (v >> 4) & 0xF0F0F0F) * 0x1010101) >> 24; // count
        }

        private void InternalClearAllKeys()
        {
            _Keys0 = 0;
            _Keys1 = 0;
            _Keys2 = 0;
            _Keys3 = 0;
            _Keys4 = 0;
            _Keys5 = 0;
            _Keys6 = 0;
            _Keys7 = 0;
        }

        private void InternalClearKey(Keys key)
        {
            uint mask = (uint)1 << (((int)key) & 0x1f);
            switch (((int)key) >> 5)
            {
                case 0:
                    _Keys0 &= ~mask;
                    break;
                case 1:
                    _Keys1 &= ~mask;
                    break;
                case 2:
                    _Keys2 &= ~mask;
                    break;
                case 3:
                    _Keys3 &= ~mask;
                    break;
                case 4:
                    _Keys4 &= ~mask;
                    break;
                case 5:
                    _Keys5 &= ~mask;
                    break;
                case 6:
                    _Keys6 &= ~mask;
                    break;
                case 7:
                    _Keys7 &= ~mask;
                    break;
            }
        }

        private bool InternalGetKey(Keys key)
        {
            uint mask = (uint)1 << (((int)key) & 0x1f);

            uint element;
            switch (((int)key) >> 5)
            {
                case 0:
                    element = _Keys0;
                    break;
                case 1:
                    element = _Keys1;
                    break;
                case 2:
                    element = _Keys2;
                    break;
                case 3:
                    element = _Keys3;
                    break;
                case 4:
                    element = _Keys4;
                    break;
                case 5:
                    element = _Keys5;
                    break;
                case 6:
                    element = _Keys6;
                    break;
                case 7:
                    element = _Keys7;
                    break;
                default:
                    element = 0;
                    break;
            }

            return (element & mask) != 0;
        }

        private void InternalSetKey(Keys key)
        {
            uint mask = (uint)1 << (((int)key) & 0x1f);
            switch (((int)key) >> 5)
            {
                case 0:
                    _Keys0 |= mask;
                    break;
                case 1:
                    _Keys1 |= mask;
                    break;
                case 2:
                    _Keys2 |= mask;
                    break;
                case 3:
                    _Keys3 |= mask;
                    break;
                case 4:
                    _Keys4 |= mask;
                    break;
                case 5:
                    _Keys5 |= mask;
                    break;
                case 6:
                    _Keys6 |= mask;
                    break;
                case 7:
                    _Keys7 |= mask;
                    break;
            }
        }

        #endregion
    }
}