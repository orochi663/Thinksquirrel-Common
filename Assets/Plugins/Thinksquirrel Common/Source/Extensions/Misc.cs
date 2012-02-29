// Miscellaneous extension methods
// Misc.cs
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
using System.Text;

namespace ThinksquirrelSoftware.Common.Extensions
{
	/// <summary>
	/// Miscellaneous extension methods.
	/// </summary>
	public static class Misc
	{	
		/// <summary>
		/// Humanize a string (split it by capital letters)
		/// </summary>
		public static string Humanize(this string source)
		{
			StringBuilder sb = new StringBuilder();
			
			char last = char.MinValue;
			foreach (char c in source)
			{
				if (char.IsLower(last) &&
				char.IsUpper(c))
				{
					sb.Append(' ');
				}
				
				sb.Append(c);
				
				last = c;
			}
			return sb.ToString();
		}
		
		/// <summary>
		/// Sanitize a string for output to a file.
		/// </summary>
		public static string Sanitize(this string source)
		{
			if (string.IsNullOrEmpty(source))
				return source;
			
			string invalidChars = System.Text.RegularExpressions.Regex.Escape( new string( System.IO.Path.GetInvalidFileNameChars() ) );
			string invalidReStr = string.Format( @"[{0}]+", invalidChars );
			return System.Text.RegularExpressions.Regex.Replace( source, invalidReStr, "_" );
		}
		
		/// <summary>
		/// Check to see if an Enum has any flags. (Extension Method)
		/// </summary>
		public static bool HasAnyFlag(this Enum value, Enum toTest)
		{
		    var val = ((IConvertible)value).ToUInt64(null);
		    var test = ((IConvertible)toTest).ToUInt64(null);

		    return (val & test) != 0;
		}
		
		/// <summary>
		/// Generates a timestamp. (Extension Method)
		/// </summary>
		public static string GetTimestamp(this System.DateTime value)
		{
			return value.ToString("yyyyMMddHHmmssffff");
		}
	}
}
