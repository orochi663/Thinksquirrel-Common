// Simple StringBuilder replacement
// StringBuilder.cs
// Thinksquirrel Software Common Libraries
//  
// Authors:
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
//
using UnityEngine;
using System.Collections.Generic;

namespace ThinksquirrelSoftware.Common.Text
{
	/// <summary>
	/// Simple string builder implementation. Not as full featured as System.Text.StringBuilder.
	/// </summary>
	public class StringBuilder
	{
		private List<char> charList;
		
		public StringBuilder()
		{
			charList = new List<char>();
		}
		
		public StringBuilder(int capacity)
		{
			charList = new List<char>(capacity);
		}
		
		public StringBuilder(string value)
		{
			charList = new List<char>();
			Append(value);
		}
		
		public StringBuilder(string value, int capacity)
		{
			charList = new List<char>(capacity);
			Append(value);
		}
		
		public StringBuilder(string value, int startIndex, int length, int capacity)
		{
			charList = new List<char>(capacity);
			Append(value.Substring(startIndex, length));
		}
		
		public int Length
		{
			get
			{
				return charList.Count;
			}
		}
		
		public char this[int index]
		{
			get
			{
				return charList[index];
			}
		}
		public StringBuilder Append(string value)
		{
			foreach(char c in value)
			{
				charList.Add(c);
			}
			
			return this;
		}
		
		public StringBuilder Append(char value)
		{
			charList.Add(value);
			
			return this;
		}
		
		public StringBuilder Append(char value, int count)
		{
			for(int i = 0; i < count; i++)
			{
				Append(value);
			}
			
			return this;
		}
			
		public StringBuilder Append(string value, int count)
		{
			for(int i = 0; i < count; i++)
			{
				Append(value);
			}
			
			return this;
		}
			
		public StringBuilder Remove(int index)
		{
			charList.RemoveAt(index);
			
			return this;
		}
		
		public StringBuilder Remove(int index, int count)
		{
			charList.RemoveRange(index, count);
			
			return this;
		}
		
		public void Clear()
		{
			charList.Clear();
		}
			
		public override string ToString()
		{
			return new string(charList.ToArray());
		}
	}
}