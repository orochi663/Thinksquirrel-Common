// Alternate binary serialization
// SerializerCache.cs
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

namespace ThinksquirrelSoftware.Common.Serialization.Binary
{
    /// <summary>
    /// Handles caching of items as they are serialized.
    /// </summary>
    internal class SerializerCache : IDisposable
    {
        #region Nested classes

        /// <summary>
        /// An object/objectID pair.
        /// </summary>
        internal class SubHash
        {
            #region Properties

            private object _storedObject;
            /// <summary>
            /// Object reference to the hashed object.
            /// </summary>
            public object StoredObject
            {
                get { return _storedObject; }
                set { _storedObject = value; }
            }

            private int _objectId;
            /// <summary>
            /// Unique ID of the hashed object.
            /// </summary>
            public int ObjectID
            {
                get { return _objectId; }
                set { _objectId = value; }
            }

            #endregion

            #region Public Methods

            public SubHash()
            {
            }

            public SubHash(object storedObject, int id)
            {
                StoredObject = storedObject;
                ObjectID = id;
            }

            public override int GetHashCode()
            {
                return StoredObject.GetHashCode();
            }

            #endregion
        }

        #endregion

        #region Non-Public Members

        private Dictionary<object, int> _hashByObject = new Dictionary<object, int>();
        private List<object> _objList = new List<object>();

        private int _staticID = 1;
        private int _newUniqueID = 1;

        #endregion

        #region Public Methods

        public SerializerCache()
        {
            _objList.Add(0);
        }

        /// <summary>
        /// Clears the object hash.
        /// </summary>
        public void Clear()
        {
            Clear(false);
        }

        /// <summary>
        /// Clears the object hash.
        /// </summary>
        public void Clear(bool clearPermanant)
        {
            _newUniqueID = 1;
            for (int i = _staticID; i < _objList.Count; i++)
            {
                if (_objList[i] == null) continue;
                _hashByObject.Remove(_objList[i]);
            }

            _objList.RemoveRange(_staticID, (_objList.Count - _staticID));
            _newUniqueID = _staticID ;
        }

        /// <summary>
        /// Gets the ID of an object in the hash.
        /// </summary>
        /// <param name="obj">Hashed object.</param>
        /// <param name="objectType">The object Type</param>
        public int GetObjectCacheID(object obj, Type objectType)
        {
            int objectID;

            if (_hashByObject.TryGetValue(obj, out objectID))
            {
                return objectID;
            }
            return 0;
        }
        
        /// <summary>
        /// Caches an object, and gives it a unique ID.
        /// </summary>
        /// <param name="obj">Object to cache.</param>
        /// <param name="objectType">Object Type.</param>
        /// <param name="permanant">If true, the object is a permanant edition to the cache.</param>
        /// <returns>Returns a unique ID to reference the object.</returns>
        public int CacheObject(object obj, bool permanant)
        {
            if (permanant && _staticID != _newUniqueID)
            {
                throw new Exception("Unable to cache item.");
            }

            int newID = _newUniqueID;
            _newUniqueID++;

            _objList.Insert(newID, obj);
            _hashByObject[obj] = newID;

            if (permanant)
            {
                _staticID++;
            }
            return newID;
        }

        /// <summary>
        /// Gets an object from the hash given the unique id.
        /// </summary>
        /// <param name="uniqueId">Unique ID to retrieve.</param>
        /// <returns>Returns the cached object.</returns>
        public object GetCachedObject(int uniqueId)
        {
            if (uniqueId < _objList.Count)
            {
                return _objList[uniqueId];
            }
            return null;
        }

        /// <summary>
        /// Sets an object ID for a cached object.
        /// </summary>
        /// <param name="obj">Object to store ID for.</param>
        /// <param name="uniqueId">Unique ID of object.</param>
        public void SetCachedObjectId(object obj, int uniqueId)
        {
            while (_objList.Count <= uniqueId)
            {
                _objList.Add(null);
            }
            _objList[uniqueId] = obj;
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Disposes of the SerializerCache.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Disposes of the SerializerCache.
        /// </summary>
        /// <param name="disposeAll">If true, both managed and native resources are
        /// disposed.</param>
        public virtual void Dispose(bool disposeAll)
        {
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
