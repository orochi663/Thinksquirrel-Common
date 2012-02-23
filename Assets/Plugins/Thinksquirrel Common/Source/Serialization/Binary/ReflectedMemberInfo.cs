// Alternate binary serialization
// ReflectedMemberInfo.cs
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
using System.Reflection;

namespace ThinksquirrelSoftware.Common.Serialization.Binary
{
    /// <summary>
    /// Stores information about a reflected property or field.
    /// </summary>
    internal class ReflectedMemberInfo
    {
        #region Properties

        private FieldInfo _fieldInfo;
        private PropertyInfo _propertyInfo;
        private bool _isFieldInfo;

        public string Name
        {
            get
            {
                if (_isFieldInfo) return _fieldInfo.Name;
                return _propertyInfo.Name;
            }
        }

        /// <summary>
        /// Gets the Type of the property/field
        /// </summary>
        public Type FieldType
        {
            get
            {
                return GetFieldOrPropertyType();
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new ReflectedMemberInfo instance with information about a field.
        /// </summary>
        public ReflectedMemberInfo(FieldInfo field)
        {
            if (field == null) throw new AltSerializeException("Could not create meta data information for serialization.");
            if (field.FieldType == null)
            {
                throw new AltSerializeException("The field '" + field.Name + "' has no Type information.");
            }
            _isFieldInfo = true;
            _fieldInfo = field;
        }

        /// <summary>
        /// Creates a new ReflectedMemberInfo instance with information about a property.
        /// </summary>
        public ReflectedMemberInfo(PropertyInfo propinfo)
        {
            if (propinfo == null) throw new AltSerializeException("Could not create meta data information for serialization.");
            if (propinfo.PropertyType == null)
            {
                throw new AltSerializeException("The property '" + propinfo.Name + "' has no Type information.");
            }

            _propertyInfo = propinfo;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the value of the property/field.
        /// </summary>
        /// <param name="obj">Object to get the property/field of.</param>
        public object GetValue(object obj)
        {
            if (_isFieldInfo)
            {
                return _fieldInfo.GetValue(obj);
            }
            return _propertyInfo.GetValue(obj, null);
        }

        /// <summary>
        /// Sets the value of the property/field.
        /// </summary>
        /// <param name="obj">Object to set the property/field on.</param>
        /// <param name="newValue">New value of the property/field.</param>
        public void SetValue(object obj, object newValue)
        {
            if (_isFieldInfo)
            {
                _fieldInfo.SetValue(obj, newValue);
            }
            else
            {
                _propertyInfo.SetValue(obj, newValue, null);
            }
        }

        /// <summary>
        /// Gets the field or property Type.
        /// </summary>
        public Type GetFieldOrPropertyType()
        {
            try
            {
                if (_isFieldInfo) return _fieldInfo.FieldType;
                return _propertyInfo.PropertyType;
            }
            catch (Exception err)
            {
                if (_isFieldInfo)
                {
                    if (_fieldInfo == null) throw new AltSerializeException("The field information for a reflected member was null.");
                    throw new AltSerializeException("Failed to retrieve the field type for the field named '" + _fieldInfo.Name + "': " + err.Message);
                }
                else
                {
                    if (_propertyInfo == null) throw new AltSerializeException("The property information for a reflected member was null!");
                    throw new AltSerializeException("Failed to retrieve the property type for the property named '" + _propertyInfo.Name + "': " + err.Message);
                }
            }
        }

        #endregion
    }
}
