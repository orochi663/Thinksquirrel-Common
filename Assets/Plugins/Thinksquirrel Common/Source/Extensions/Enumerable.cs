// Enumerable extension methods
// Enumerable.cs
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
using System;
using System.Collections.Generic;
using System.Linq;

namespace ThinksquirrelSoftware.Common.Extensions
{
	/// <summary>
	/// Enumerable extension methods.
	/// </summary>
	public static class Enumerable
	{
		private static Random rand = new Random();
		
		/// <summary>
		/// Break an enumerable into chunks of a specific size. (Extension Method)
		/// </summary>
		public static IEnumerable<IEnumerable<T>> Chunk<T>(this IEnumerable<T> source, int chunksize)
		{
			while (source.Any())
			{
				yield return source.Take(chunksize);
				source = source.Skip(chunksize);
			}
		}

		/// <summary>
		/// Gets random keys from a dictionary. (Extension Method)
		/// </summary>
		public static IEnumerable<TKey> RandomKeys<TKey, TValue>(this IDictionary<TKey, TValue> dict)
		{
		    List<TKey> keys = System.Linq.Enumerable.ToList(dict.Keys);
		    int size = dict.Count;
		    while(true)
		    {
		        yield return keys[rand.Next(size)];
		    }
		}
				
		/// <summary>
		/// Gets random values from a dictionary. (Extension Method)
		/// </summary>
		public static IEnumerable<TValue> RandomValues<TKey, TValue>(this IDictionary<TKey, TValue> dict)
		{
		    List<TValue> values = System.Linq.Enumerable.ToList(dict.Values);
		    int size = dict.Count;
		    while(true)
		    {
		        yield return values[rand.Next(size)];
		    }
		}
				
		/// <summary>
		/// Iterate through specified enumerable and invoke an action. (Extension Method)
		/// </summary>
		public static void Iterate<T>(this IEnumerable<T> enumerable, Action<T> callback)
		{
			if (enumerable == null)
			{
				throw new ArgumentNullException("enumerable");
			}
			
			IterateHelper(enumerable, (x, i) => callback(x));
		}
			
		/// <summary>
		/// Iterate through specified enumerable and invoke an action. (Extension Method)
		/// </summary>
		public static void Iterate<T>(this IEnumerable<T> enumerable, Action<T,int> callback)
		{
			if (enumerable == null)
			{
				throw new ArgumentNullException("enumerable");
			}
			
			IterateHelper(enumerable, callback);
		}
			
		private static void IterateHelper<T>(this IEnumerable<T> enumerable, Action<T,int> callback)
		{
			int count = 0;
			foreach (var cur in enumerable)
			{
				callback(cur, count);
				count++;
			}
		}
		
		/// <summary>
		/// Gets random keys from a dictionary. (Extension Method)
		/// </summary>
		public static IEnumerable<TKey> RandomKeys<TKey, TValue>(this IDictionary<TKey, TValue> dict)
		{
		    List<TKey> keys = System.Linq.Enumerable.ToList(dict.Keys);
		    int size = dict.Count;
		    while(true)
		    {
		        yield return keys[rand.Next(size)];
		    }
		}
		
		/// <summary>
		/// Gets random values from a dictionary. (Extension Method)
		/// </summary>
		public static IEnumerable<TValue> RandomValues<TKey, TValue>(this IDictionary<TKey, TValue> dict)
		{
		    List<TValue> values = System.Linq.Enumerable.ToList(dict.Values);
		    int size = dict.Count;
		    while(true)
		    {
		        yield return values[rand.Next(size)];
		    }
		}
		
		/// <summary>
		/// Shuffles an enumerable. (Extension Method)
		/// </summary>		
		public static void Shuffle<T>(this IEnumerable<T> enumerable)  
		{  
			IList<T> list = enumerable.ToList();
			list.Shuffle();
			
			int i = 0;
			
			enumerable.Select(x => list[i++]);
		}
		
		/// <summary>
		/// Shuffles a list. (Extension Method)
		/// </summary>		
		public static void Shuffle<T>(this IList<T> list)  
		{  
		    int n = list.Count;  
		    while (n > 1) {  
		        n--;  
		        int k = rand.Next(n + 1);  
		        T value = list[k];  
		        list[k] = list[n];  
		        list[n] = value;  
		    }  
		}
	}
}