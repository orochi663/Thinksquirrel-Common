// Alternate binary serialization
// ByteSerializer.cs
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
using System.IO;

namespace ThinksquirrelSoftware.Common.Serialization.Binary
{
    /// <summary>
    /// Wraps the AltSerializer, implementing a simple class for serializing objects
    /// into byte arrays and deserializing byte arrays into objects.
    /// </summary>
    public class ByteSerializer : IDisposable
    {
        #region Properties

        private AltSerializer _AltSerializer;
        private MemoryStream _MemStream = null;

        private SerializeFlags _defaultSerializeFlags = SerializeFlags.SerializationCache | SerializeFlags.SerializeProperties;
        /// <summary>
        /// Gets or sets the default serialization flags.
        /// This method defaults to None.
        /// </summary>
        public SerializeFlags DefaultSerializeFlags
        {
            get { return _defaultSerializeFlags; }
            set { _defaultSerializeFlags = value; }
        }

        /// <summary>
        /// Gets or sets the default encoding for strings.
        /// </summary>
        public Encoding DefaultEncoding
        {
            get { return _AltSerializer.Encoding; }
            set { _AltSerializer.Encoding = value; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new instance of a ByteSerializer.
        /// </summary>
        public ByteSerializer()
        {
            _AltSerializer = new AltSerializer();
            _MemStream = new MemoryStream();
        }

        #endregion

        #region Non-public Methods

        // Creates the serializer memory stream if it hasn't been already,
        // and initializes the serialization flags
        private void InitSerializer(SerializeFlags flags)
        {
            _AltSerializer.Stream = _MemStream;
            _AltSerializer.SerializePropertyNames = ((flags & SerializeFlags.SerializePropertyNames) != 0);
            _AltSerializer.CacheEnabled = ((flags & SerializeFlags.SerializationCache) != 0);
            _AltSerializer.SerializeProperties = ((flags & SerializeFlags.SerializeProperties) != 0);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Serializes an object using the default serialization flags
        /// and returns a byte array of the result.
        /// </summary>
        /// <param name="anObject">The object to serialize.</param>
        /// <returns>Returns a byte array of the serialized object.</returns>
        public byte[] Serialize(object anObject)
        {
            byte[] result;

            lock (_AltSerializer)
            {
                InitSerializer(DefaultSerializeFlags);
                _AltSerializer.Reset();
                _AltSerializer.Serialize(anObject);
                result = new byte[_MemStream.Position];
                _MemStream.Position = 0;
                _MemStream.Read(result, 0, result.Length);
                _MemStream.SetLength(0);
                _AltSerializer.Reset();
            }
            return result;
        }

        /// <summary>
        /// Serializes an object and returns a byte array of the result.
        /// </summary>
        /// <param name="anObject">The object to serialize.</param>
        /// <param name="flags">Flags to control the serialization.</param>
        /// <returns>Returns a byte array of the serialized object.</returns>
        public byte[] Serialize(object anObject, Type objectType)
        {
            byte[] result;

            lock (_AltSerializer)
            {
                InitSerializer(DefaultSerializeFlags);
                _AltSerializer.Reset();
                _AltSerializer.Serialize(anObject, objectType);
                result = new byte[_MemStream.Position];
                _MemStream.Position = 0;
                _MemStream.Read(result, 0, result.Length);
                _MemStream.SetLength(0);
                _AltSerializer.Reset();
            }
            return result;
        }

        /// <summary>
        /// Deserializes an object using the default serialization flags
        /// and returns the result.
        /// </summary>
        /// <param name="bytes">Array of bytes containing the serialized object.</param>
        /// <param name="objectType">The object type contained in the serialized byte array.</param>
        /// /// <param name="flags">Flags to control the deserialization.</param>
        /// <returns>Returns the deserialized object.</returns>
        public object Deserialize(byte[] bytes, Type objectType)
        {
            object returnValue;

            lock (_AltSerializer)
            {

                InitSerializer(DefaultSerializeFlags);
                MemoryStream objectStream = new MemoryStream(bytes);
                _AltSerializer.Reset();
                _AltSerializer.Stream = objectStream;
                returnValue = _AltSerializer.Deserialize(objectType);
                _AltSerializer.Stream = _MemStream;
                objectStream.Dispose();
            }
            return returnValue;
        }

        /// <summary>
        /// Deserializes an object and returns the result.
        /// </summary>
        /// <param name="bytes">Array of bytes containing the serialized object.</param>
        /// <param name="objectType">The object type contained in the serialized byte array.</param>
        /// /// <param name="flags">Flags to control the deserialization.</param>
        /// <returns>Returns the deserialized object.</returns>
        public object Deserialize(byte[] bytes, Type objectType, SerializeFlags flags)
        {
            object returnValue;

            lock (_AltSerializer)
            {
                InitSerializer(flags);
                MemoryStream objectStream = new MemoryStream(bytes);
                _AltSerializer.Reset();
                _AltSerializer.Stream = objectStream;
                returnValue = _AltSerializer.Deserialize(objectType);
                _AltSerializer.Stream = _MemStream;
                objectStream.Dispose();
            }
            return returnValue;
        }

        /// <summary>
        /// Deserializes an object and returns the result.
        /// </summary>
        public object Deserialize(byte[] bytes)
        {
            object returnValue;

            lock (_AltSerializer)
            {

                InitSerializer(DefaultSerializeFlags);
                MemoryStream objectStream = new MemoryStream(bytes);
                _AltSerializer.Reset();
                _AltSerializer.Stream = objectStream;
                returnValue = _AltSerializer.Deserialize();
                _AltSerializer.Stream = _MemStream;
                objectStream.Dispose();
            }
            return returnValue;
        }

        /// <summary>
        /// Adds an object to the serialization cache.
        /// </summary>
        /// <remarks>This method makes a permanant addition to the serialization class.
        /// Any time the serializer encounters the object, it will use the cached reference
        /// instead of serializing the entire object.</remarks>
        /// <param name="cachedObject">Object to cache.</param>
        public void CacheObject(object cachedObject)
        {
            _AltSerializer.CacheObject(cachedObject);
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Disposes of the ByteSerializer.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Disposes of the ByteSerializer.
        /// </summary>
        /// <param name="disposeAll">If true, both managed and native resources are
        /// disposed.</param>
        protected virtual void Dispose(bool disposeAll)
        {
            if (_MemStream != null)
            {
                _MemStream.Dispose();
            }
            if (_AltSerializer != null)
            {
                _AltSerializer.Dispose();
            }

            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
