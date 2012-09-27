// MonoBehaviour Interface Implementation
// IMonoBehaviour.cs
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

namespace ThinksquirrelSoftware.Common.ObjectModel
{
	public interface IMonoBehaviour
	{	
		#if !COMPACT
		/// MonoBehaviour
		bool useGUILayout
		{
			get;
			set;
		}
		
		void Invoke(string methodName, float time);
		void InvokeRepeating(string methodName, float time, float repeatRate);
		void CancelInvoke();
		void CancelInvoke(string methodName);
		bool IsInvoking();
		bool IsInvoking(string methodName);
		UnityEngine.Coroutine StartCoroutine(IEnumerator routine);
		UnityEngine.Coroutine StartCoroutine(string methodName);
		UnityEngine.Coroutine StartCoroutine(string methodName, object value);
		void StopCoroutine(string methodName);
		void StopAllCoroutines();
		
		// Behaviour
		bool enabled
		{
			get;
			set;
		}
		
		// Component
		GameObject gameObject
		{
			get;
		}
		
		Transform transform
		{
			get;
		}
		
		Rigidbody rigidbody
		{
			get;
		}
		
		Camera camera
		{
			get;
		}
		
		Light light
		{
			get;
		}
		
		Animation animation
		{
			get;
		}
		
		ConstantForce constantForce
		{
			get;
		}
		
		Renderer renderer
		{
			get;
		}
		
		AudioSource audio
		{
			get;
		}
		
		GUIText guiText
		{
			get;
		}
		
		NetworkView networkView
		{
			get;
		}
		
		GUITexture guiTexture
		{
			get;
		}
		
		Collider collider
		{
			get;
		}
		
		HingeJoint hingeJoint
		{
			get;
		}
		
		ParticleEmitter particleEmitter
		{
			get;
		}
		
		ParticleSystem particleSystem
		{
			get;
		}
		
		string tag
		{
			get;
			set;
		}
		
		// Object
		string name
		{
			get;
			set;
		}
		
		HideFlags hideFlags
		{
			get;
			set;
		}
		
		int GetInstanceID();
		string ToString();
		
		#endif
	}
}