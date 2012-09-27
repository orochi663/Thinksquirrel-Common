// Fixed stack-allocated arrays
// FixedArray.cs
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
// This file is available at https://github.com/Thinksquirrel-Software/Thinksquirrel-Common
//
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ThinksquirrelSoftware.Common.Collections
{
	public struct FixedArray2<T> : IEnumerable<T>
	{
		T m_Item0, m_Item1;
		
		public T this[int index]
		{
			get
			{
				switch(index)
				{
				case 0:
					return m_Item0;
				case 1:
					return m_Item1;
				default:
					throw new System.ArgumentOutOfRangeException();
				}
			}
			set
			{
				switch(index)
				{
				case 0:
					m_Item0 = value;
					return;
				case 1:
					m_Item1 = value;
					return;
				default:
					throw new System.ArgumentOutOfRangeException();
				}
			}
		}

		#region IEnumerable[T] implementation
		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			return Enumerate().GetEnumerator();
		}
		#endregion

		#region IEnumerable implementation
		IEnumerator IEnumerable.GetEnumerator()
		{
			return Enumerate().GetEnumerator();
		}
		#endregion
		
		IEnumerable<T> Enumerate()
		{
			for(int i = 0; i < 2; ++i)
				yield return this[i];
		}
	}
	
	public struct FixedArray8<T> : IEnumerable<T>
	{
		T m_Item0, m_Item1, m_Item2, m_Item3, m_Item4, m_Item5, m_Item6, m_Item7;
		
		public T this[int index]
		{
			get
			{
				switch(index)
				{
				case 0:
					return m_Item0;
				case 1:
					return m_Item1;
				case 2:
					return m_Item2;
				case 3:
					return m_Item3;
				case 4:
					return m_Item4;
				case 5:
					return m_Item5;
				case 6:
					return m_Item6;
				case 7:
					return m_Item7;
				default:
					throw new System.ArgumentOutOfRangeException();
				}
			}
			set
			{
				switch(index)
				{
				case 0:
					m_Item0 = value;
					return;
				case 1:
					m_Item1 = value;
					return;
				case 2:
					m_Item2 = value;
					return;
				case 3:
					m_Item3 = value;
					return;
				case 4:
					m_Item4 = value;
					return;
				case 5:
					m_Item5 = value;
					return;
				case 6:
					m_Item6 = value;
					return;
				case 7:
					m_Item7 = value;
					return;
				default:
					throw new System.ArgumentOutOfRangeException();
				}
			}
		}

		#region IEnumerable[T] implementation
		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			return Enumerate().GetEnumerator();
		}
		#endregion

		#region IEnumerable implementation
		IEnumerator IEnumerable.GetEnumerator()
		{
			return Enumerate().GetEnumerator();
		}
		#endregion
		
		IEnumerable<T> Enumerate()
		{
			for(int i = 0; i < 8; ++i)
				yield return this[i];
		}
	}
}