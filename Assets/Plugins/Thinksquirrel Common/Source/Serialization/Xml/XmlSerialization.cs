// XML serialization extension methods
// XmlSerialization.cs
// Thinksquirrel Software Common Libraries
//  
// Author:
//       Josh Montoute <josh@thinksquirrel.com>
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
using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Xml serialization classes.
/// </summary>
namespace ThinksquirrelSoftware.Common.Serialization.Xml
{
	/// <summary>
	/// Xml serialization extension methods.
	/// </summary>
	public static class XmlSerialization
	{
		private static XmlSerializer MakeXmlSerializer<T> ()
		{
			return new XmlSerializer (typeof(T));
		}

		public static void CollectionToXml<T> (this ICollection<T> collection, string filePath)
		{
			XmlSerializer serializer = MakeXmlSerializer<ICollection<T>> ();
			TextWriter textWriter = new StreamWriter (filePath);
			serializer.Serialize (textWriter, collection);
			textWriter.Close ();
		}

		private static void DeserializeHelper<T> (ICollection<T> collection, string filePath, bool unityResource)
		{
			XmlSerializer deserializer = MakeXmlSerializer<ICollection<T>> ();
			TextReader textReader = null;
			
			if (unityResource) {
				TextAsset text = Resources.Load (filePath, typeof(TextAsset)) as TextAsset;
				textReader = new StringReader (text.text);
			} else {
				textReader = new StreamReader (filePath);
			}
			
			List<T> newList = (List<T>)deserializer.Deserialize (textReader);
			collection.Clear ();
			
			for (int i = 0; i < newList.Count; i++) {
				collection.Add (newList[i]);
			}
			textReader.Close ();
			
		}

		public static void CollectionFromXml<T> (this ICollection<T> collection, string filePath)
		{
			DeserializeHelper (collection, filePath, false);
		}

		public static void CollectionFromXml<T> (this ICollection<T> collection, string filePath, bool unityResource)
		{
			DeserializeHelper (collection, filePath, unityResource);
		}
		
		public static void ToXml<TKey, TValue> (this IDictionary<TKey, TValue> dictionary, string filePath)
		{
			DictionaryWrapper<TKey, TValue> wrapper = new DictionaryWrapper<TKey, TValue> (dictionary);
			XmlSerializer serializer = MakeXmlSerializer<DictionaryWrapper<TKey, TValue>> ();
			TextWriter textWriter = new StreamWriter (filePath);
			serializer.Serialize (textWriter, wrapper);
			textWriter.Close ();
		}

		private static void DeserializeDictionary<TKey, TValue> (IDictionary<TKey, TValue> dictionary, string filePath, bool unityResource)
		{
			XmlSerializer deserializer = MakeXmlSerializer<DictionaryWrapper<TKey, TValue>> ();
			TextReader textReader = null;
			
			if (unityResource) {
				TextAsset text = Resources.Load (filePath, typeof(TextAsset)) as TextAsset;
				textReader = new StringReader (text.text);
			} else {
				textReader = new StreamReader (filePath);
			}
			
			DictionaryWrapper<TKey, TValue> wrapper = (DictionaryWrapper<TKey, TValue>)deserializer.Deserialize (textReader);
			wrapper.GetMap (ref dictionary);
			textReader.Close ();
		}
		
		public static void FromXml<TKey, TValue> (this IDictionary<TKey, TValue> dictionary, string filePath)
		{
			DeserializeDictionary (dictionary, filePath, false);
		}

		public static void FromXml<TKey, TValue> (this IDictionary<TKey, TValue> dictionary, string filePath, bool unityResource)
		{
			DeserializeDictionary (dictionary, filePath, unityResource);
		}
	
	}
	
	/// <summary>
	/// Dictionary wrapper class for serialization.
	/// </summary>
	[Serializable]
	internal class DictionaryWrapper<TKey, TValue> : ISerializable
	{
		public TKey[] Keys { get; set; }
		public TValue[] Values { get; set; }
		
		public DictionaryWrapper ()
		{
			
		}

		public DictionaryWrapper (IDictionary<TKey, TValue> map)
		{
			List<TKey> keyList = new List<TKey> (map.Keys.Count);
			List<TValue> valueList = new List<TValue> (map.Keys.Count);
			foreach(TKey key in map.Keys)
			{
				keyList.Add(key);
				valueList.Add(map[key]);
			}
			Keys = keyList.ToArray();
			Values = valueList.ToArray();
		}

		public Dictionary<TKey, TValue> GetMap ()
		{
			Dictionary<TKey, TValue> map = new Dictionary<TKey, TValue> ();
			for (int i = 0; i < Keys.Length; i++) {
				map[Keys[i]] = Values[i];
			}
			return map;
		}

		public void GetMap (ref IDictionary<TKey, TValue> dictionary)
		{
			dictionary.Clear ();
			for (int i = 0; i < Keys.Length; i++) {
				dictionary[Keys[i]] = Values[i];
			}
		}
		
		//Deserialization constructor.
		
		public DictionaryWrapper(SerializationInfo info, StreamingContext ctxt)
		{
		    Keys = (TKey[])info.GetValue("Keys", typeof(TKey[]));
			Values = (TValue[])info.GetValue("Values", typeof(TValue[]));
		}
		        
		//Serialization function.
		
		public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
		{
		    info.AddValue("Keys", Keys);
			info.AddValue("Values", Values);
		}
	}
}
#endif