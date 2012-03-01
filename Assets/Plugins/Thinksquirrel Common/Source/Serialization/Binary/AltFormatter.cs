// Alternate binary serialization
// AltFormatter.cs
// Thinksquirrel Software Common Libraries
//  
// Authors:
//       Josh Montoute <josh@thinksquirrel.com>
//		 neocognitron <http://www.codeproject.com/Articles/15375/AltSerializer-An-Alternate-Binary-Serializer>
// 
// Copyright (c) 2011-2012, Thinksquirrel Software, LLC
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
// Redistributions of source code must retain the above copyright notice,
// this list of conditions and the following disclaimer.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND 
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES 
// OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT 
// SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, 
// SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT 
// OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, 
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, 
// EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//
// This file is available at https://github.com/Thinksquirrel-Software/Thinksquirrel-Common
//
using System;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// Binary serialization classes.
/// </summary>
/// <remarks>
/// Ported from the AltSerializer project - <http://www.codeproject.com/Articles/15375/AltSerializer-An-Alternate-Binary-Serializer>.
/// </remarks>
namespace ThinksquirrelSoftware.Common.Serialization.Binary
{
    public class AltFormatter : System.Runtime.Serialization.IFormatterConverter
    {
        #region IFormatterConverter Members

        public object Convert(object value, TypeCode typeCode)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public object Convert(object value, Type type)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool ToBoolean(object value)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public byte ToByte(object value)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public char ToChar(object value)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public DateTime ToDateTime(object value)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public decimal ToDecimal(object value)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public double ToDouble(object value)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public short ToInt16(object value)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int ToInt32(object value)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public long ToInt64(object value)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public sbyte ToSByte(object value)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public float ToSingle(object value)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public string ToString(object value)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public ushort ToUInt16(object value)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public uint ToUInt32(object value)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public ulong ToUInt64(object value)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
}
