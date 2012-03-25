// Unity MonoBehaviour base
// MonoBase.cs
// Thinksquirrel Software Common Libraries
//  
// Authors:
//       Josh Montoute <josh@thinksquirrel.com>
//		 Neil Carter <http://www.nether.org.uk>
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
using ThinksquirrelSoftware.Common.ObjectModel;

/// <summary>
/// Thinksquirrel Software common Unity libraries.
/// </summary>
namespace ThinksquirrelSoftware.Common
{
	/// <summary>
	/// Monobehavior base class for all Thinksquirrel Software components. 
	/// </summary>
	public abstract class MonoBase : MonoBehaviour
	{
		/// <summary>
		/// Generic FindObjectOfType() implementation.
		/// </summary>
	    public static T FindObjectOfType<T>() where T : Object
	        {return (T)Object.FindObjectOfType(typeof(T));}
		
		/// <summary>
		/// Generic FindObjectsOfType() implementation.
		/// </summary>
	    public static T[] FindObjectsOfType<T>() where T : Object
	        {return ConvertObjectArray<T>(Object.FindObjectsOfType(typeof(T)));}
		
		/// <summary>
		/// Object array conversion. 
		/// </summary>
	    public static T[] ConvertObjectArray<T>(Object[] objects) where T : Object
	        {return System.Array.ConvertAll<Object, T>(objects, delegate(Object o) {return (T)o;});}
	
		/// <summary>
		/// Searches for the component on this GameObject and every parent until it hits the root.
		/// </summary>
	    public T GetComponentUpwards<T>() where T : Component
	        {return GetComponentUpwardsFrom<T>(gameObject);}
		
		/// <summary>
		/// Searches for the component on this GameObject and every parent until it hits the root.
		/// </summary>
	    public static T GetComponentUpwardsFrom<T>(GameObject g) where T : Component
	    {
	        T component;
	
	        Transform t = g.transform;
	        do
	        {
	            component = t.GetComponent<T>();
	
	            if(component != null)
	                break;
	
	            t = t.parent;
	        }
	        while(t != null);
	
	        return component;
	    }
	
	    /// <summary>
		/// Searches for the component on this GameObject and every parent until it hits the root.
		/// </summary>
		public static T GetComponentUpwardsFrom<T>(Component c) where T : Component
	        {return GetComponentUpwardsFrom<T>(c.gameObject);}
	
		/// <summary>
		/// Generic Instantiate() implementation.
		/// </summary>
	    public static T Instantiate<T>(T original, Vector3 position, Quaternion rotation) where T : Object
	    {
	        return (T)MonoBehaviour.Instantiate(original, position, rotation);
	    }
		
		/// <summary>
		/// Generic Instantiate() implementation.
		/// </summary>
	    public static T Instantiate<T>(T original) where T : Object
	    {
	        return (T)MonoBehaviour.Instantiate(original);
	    }
		
		/// <summary>
		/// Gets the component from its interface type.
		/// </summary>
		/// <returns>
		/// The component (as its interface type).
		/// </returns>
		/// <typeparam name='T'>
		/// The interface type of the component. Must be an IMonoBehaviour.
		/// </typeparam>
		public T GetComponentFromInterface<T>() where T : class, IMonoBehaviour
		{
			return Extensions.Unity.GetComponentFromInterface<T>(this);
		}
	}
}
