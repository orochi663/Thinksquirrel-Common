// Alternate binary serialization
// Serializer.cs
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
#if !COMPACT
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;

namespace ThinksquirrelSoftware.Common.Serialization.Binary
{    
    /// <summary>
    /// Static methods for easy, thread-safe serialization.
    /// </summary>
    public static class Serializer
    {        
        #region Properties

        private static ByteSerializer _serializer = new ByteSerializer();

        /// <summary>
        /// Gets or sets the default serialization flags.
        /// This method defaults to None.
        /// </summary>
        public static SerializeFlags DefaultSerializeFlags
        {
            get { return _serializer.DefaultSerializeFlags; }
            set { _serializer.DefaultSerializeFlags = value; }
        }

        /// <summary>
        /// Gets the default string encoding to use for serialization.
        /// </summary>
        public static Encoding DefaultEncoding
        {
            get { return _serializer.DefaultEncoding; }
            set { _serializer.DefaultEncoding = value; }
        }        
        
        #endregion

        #region Public Methods

        /// <summary>
        /// Serializes an object using the default serialization flags
        /// and returns a byte array of the result.
        /// </summary>
        /// <param name="anObject">The object to serialize.</param>
        /// <returns>Returns a byte array of the serialized object.</returns>
        public static byte[] Serialize(object anObject)
        {
            return _serializer.Serialize(anObject);
        }

        /// <summary>
        /// Serializes an object and returns a byte array of the result.
        /// </summary>
        /// <param name="anObject">The object to serialize.</param>
        /// <param name="flags">Flags to control the serialization.</param>
        /// <returns>Returns a byte array of the serialized object.</returns>
        public static byte[] Serialize(object anObject, Type objectType)
        {
            return _serializer.Serialize(anObject, objectType);
        }

        /// <summary>
        /// Deserializes an object using the default serialization flags
        /// and returns the result.
        /// </summary>
        /// <param name="bytes">Array of bytes containing the serialized object.</param>
        /// <param name="objectType">The object type contained in the serialized byte array.</param>
        /// /// <param name="flags">Flags to control the deserialization.</param>
        /// <returns>Returns the deserialized object.</returns>
        public static object Deserialize(byte[] bytes, Type objectType)
        {
            return _serializer.Deserialize(bytes, objectType);
        }

        /// <summary>
        /// Deserializes an object.
        /// </summary>
        /// <param name="bytes">Array of bytes containing the serialized object.</param>
        /// <returns>Returns the deserialized object.</returns>
        public static object Deserialize(byte[] bytes)
        {
            return _serializer.Deserialize(bytes);
        }

        /// <summary>
        /// Adds an object to the serialization cache.
        /// </summary>
        /// <remarks>This method makes a permanant addition to the serialization class.
        /// Any time the serializer encounters the object, it will use the cached reference
        /// instead of serializing the entire object.</remarks>
        /// <param name="cachedObject">Object to cache.</param>
        public static void CacheObject(object cachedObject)
        {
            _serializer.CacheObject(cachedObject);
        }

        #endregion
    }
}
#endif