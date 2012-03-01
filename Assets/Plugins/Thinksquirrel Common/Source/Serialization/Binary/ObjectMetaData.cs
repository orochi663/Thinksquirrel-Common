// Alternate binary serialization
// ObjectMetaData.cs
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
using System.Reflection;

namespace ThinksquirrelSoftware.Common.Serialization.Binary
{
    /// <summary>
    /// Data class container holding information about
    /// how to serialize an object.
    /// </summary>
    internal class ObjectMetaData
    {
        #region Properties

        private AltSerializer _owner;
        /// <summary>
        /// Owner of the meta-data.
        /// </summary>
        public AltSerializer Owner
        {
            get { return _owner; }
            set { _owner = value; }
        }

        private Type _type;
        /// <summary>
        /// Gets or sets the object type.
        /// </summary>
        public Type ObjectType
        {
            get { return _type; }
            set { _type = value; }
        }

        private bool _implementsIList;
        /// <summary>
        /// Indicates if the type implements System.Collections.IList
        /// </summary>
        public bool ImplementsIList
        {
            get { return _implementsIList; }
            set { _implementsIList = value; }
        }

        private bool _implementsIDictionary;
        /// <summary>
        /// Indicates if the type implements System.Collections.IDictionary
        /// </summary>
        public bool ImplementsIDictionary
        {
            get { return _implementsIDictionary; }
            set { _implementsIDictionary = value; }
        }

        private bool _isGenericList;
        /// <summary>
        /// Indicates if the type is a generic List.
        /// </summary>
        public bool IsGenericList
        {
            get { return _isGenericList; }
            set { _isGenericList = value; }
        }

        private bool _isISerializable;
        /// <summary>
        /// Indicates if the type implements the ISerializable interface.
        /// </summary>
        public bool IsISerializable
        {
            get { return _isISerializable; }
            set { _isISerializable = value; }
        }

        private bool _useInterface;
        /// <summary>
        /// True of the object implements the IAltSerializable interface.
        /// </summary>
        public bool IsIAltSerializable
        {
            get { return _useInterface; }
            set { _useInterface = value; }
        }

        private ReflectedMemberInfo [] _fields;
        /// <summary>
        /// The fields that are serializable.
        /// </summary>
        public ReflectedMemberInfo [] Fields
        {
            get { return _fields; }
            set { _fields = value; }
        }

        private ReflectedMemberInfo [] _properties;
        /// <summary>
        /// The properties that are serializable.
        /// </summary>
        public ReflectedMemberInfo [] Properties
        {
            get { return _properties; }
            set { _properties = value; }
        }

        /// <summary>
        /// Gets or sets the array of fields serialized
        /// by this object.
        /// </summary>
        public ReflectedMemberInfo[] Values
        {
            get
            {
                // Return the array based on the setting of the owner.
                if (Owner.SerializeProperties)
                {
                    return Properties;
                }
                return Fields;
            }
        }

        private DynamicSerializer _dynamicSerializer;
        /// <summary>
        /// The dynamic serializer used for this object.
        /// </summary>
        public DynamicSerializer DynamicSerializer
        {
            get { return _dynamicSerializer; }
            set { _dynamicSerializer = value; }
        }

        private Type _genericTypeDefinition;
        /// <summary>
        /// The generic type definition, if any, of the type.
        /// </summary>
        public Type GenericTypeDefinition
        {
            get { return _genericTypeDefinition; }
            set { _genericTypeDefinition = value; }
        }

        private Type [] _genericParameters;
        /// <summary>
        /// The generic parameters of the type.
        /// </summary>
        public Type [] GenericParameters
        {
            get { return _genericParameters; }
            set { _genericParameters = value; }
        }

        private MethodInfo _deserializeMethod;
        public MethodInfo DeserializeMethod
        {
            get { return _deserializeMethod; }
            set { _deserializeMethod = value; }
        }

        private MethodInfo _serializeMethod;
        public MethodInfo SerializeMethod
        {
            get { return _serializeMethod; }
            set { _serializeMethod = value; }
        }

        private object _extra;
        /// <summary>
        /// Gets or sets any extra information about the class.
        /// </summary>
        public object Extra
        {
            get { return _extra; }
            set { _extra = value; }
        }

        private FieldInfo _size;
        /// <summary>
        /// FieldInfo to get the size.
        /// </summary>
        public FieldInfo SizeField
        {
            get { return _size; }
            set { _size = value; }
        }

        #endregion

        #region Public Methods

        public ObjectMetaData(AltSerializer owner)
        {
            Owner = owner;
        }

        /// <summary>
        /// Searches the member information for a property/field name.
        /// </summary>
        public ReflectedMemberInfo FindMemberInfoByName(string propertyName)
        {
            for (int i = 0; i < Values.Length; i++)
            {
                if (Values[i].Name == propertyName)
                {
                    return Values[i];
                }
            }
            return null;
        }

        #endregion
    }
}
#endif