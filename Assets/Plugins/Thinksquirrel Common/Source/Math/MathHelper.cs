// Math helper methods
// MathHelper.cs
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
using UnityEngine;

namespace ThinksquirrelSoftware.Common.Math
{
	public static class MathHelper
	{
		public static float MinMax(params float[] values)
		{
			float min = float.MaxValue, max = float.MinValue;
			
			foreach(float f in values)
			{
				min = System.Math.Min(f, min);
				max = System.Math.Max(f, max);
			}
			
			return System.Math.Abs(min) > System.Math.Abs(max) ? min : max;
		}
		
		public static float RoundToIncrement(float value, float increment)
		{
			return Mathf.Round(value / increment) * increment;
		}
		
		public static Vector2 SnapVector(Vector2 vector, Vector2 snapSize)
		{
			return new Vector2(RoundToIncrement(vector.x, snapSize.x), RoundToIncrement(vector.y, snapSize.y));
		}
		
		public static Vector3 SnapVector(Vector3 vector, Vector3 snapSize)
		{
			return new Vector3(RoundToIncrement(vector.x, snapSize.x), RoundToIncrement(vector.y, snapSize.y), RoundToIncrement(vector.z, snapSize.z));
		}
		
		public static Vector3 SnapVector(Vector3 vector, Vector2 snapSize)
		{
			return new Vector3(RoundToIncrement(vector.x, snapSize.x), RoundToIncrement(vector.y, snapSize.y), vector.z);
		}
		
		public static Vector4 SnapVector(Vector4 vector, Vector4 snapSize)
		{
			return new Vector4(RoundToIncrement(vector.x, snapSize.x), RoundToIncrement(vector.y, snapSize.y), RoundToIncrement(vector.z, snapSize.z), RoundToIncrement(vector.w, snapSize.w));
		}
		
		public static Vector4 SnapVector(Vector4 vector, Vector3 snapSize)
		{
			return new Vector4(RoundToIncrement(vector.x, snapSize.x), RoundToIncrement(vector.y, snapSize.y), RoundToIncrement(vector.z, snapSize.z), vector.w);
		}
		
		public static Vector4 SnapVector(Vector4 vector, Vector2 snapSize)
		{
			return new Vector4(RoundToIncrement(vector.x, snapSize.x), RoundToIncrement(vector.y, snapSize.y), vector.z, vector.w);
		}
	
		public static int Wrap(int value, int max)
		{
			return (value + max) % max;
		}
	}
}
