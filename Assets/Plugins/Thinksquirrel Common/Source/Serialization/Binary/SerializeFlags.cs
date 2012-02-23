// Alternate binary serialization
// SerializeFlags.cs
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
    [Flags]
    public enum SerializeFlags
    {
        /// <summary>
        /// Do not include any extra information for serialization.
        /// </summary>
        None = 0,
        /// <summary>
        /// Serialize the property names of each property.
        /// Use this flag whenever the properties of an object may change,
        /// or if for any reason the order of the reflected properties would
        /// be different.
        /// </summary>
        SerializePropertyNames = 0x1,
        /// <summary>
        /// If this flag is set, then the serializer attempts to cache objects it's
        /// seen before.  This allows the serializer to serialize objects that have
        /// circular references, and can reduce the amount of space the serialized
        /// object consumes.
        /// </summary>
        SerializationCache = 0x2,
        /// <summary>
        /// Serializes the properties of an object.  If this flag is not set,
        /// then the fields are serialized.
        /// </summary>
        SerializeProperties = 0x10,
        /// <summary>
        /// Enables all serialization flags.
        /// </summary>
        All = SerializePropertyNames + SerializationCache,
    }
}
