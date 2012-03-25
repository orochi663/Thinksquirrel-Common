// Unity editor binding - provides dynamic invokation of editor methods from runtime assemblies.
// Unity.cs
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
using System.Collections;
using System.Reflection;

namespace ThinksquirrelSoftware.Common.Editor
{
	public static class UnityEditorBinding
	{
		private static Assembly editorAssembly;
		
		private static void LoadAssembly()
		{
			// Get the Unity Editor assembly
			editorAssembly = Assembly.Load("UnityEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null");	
		}
		
		/// <summary>
		/// Run the specified static method with the specified arguments.
		/// </summary>
		/// <param name='method'>
		/// The name of the static method in the UnityEditor namespace to run (ex: EditorUtility.DisplayProgressBar)
		/// </param>
		/// <param name='arguments'>
		/// The arguments to pass to the method. (optional)
		/// </param>
		public static object RunStatic(string method, params object[] arguments)
		{
			// Return if not in the editor
			if (!Application.isEditor)
				return null;
				
			if (editorAssembly == null)
				LoadAssembly();
				
			if (editorAssembly == null)
				return null;
			
			// Get the type
			var typeString = method.Substring(0, method.LastIndexOf('.'));
			var type = editorAssembly.GetType("UnityEditor." + typeString);
			
			if (type == null)
				return null;
			
			// Get the Method
			var methodString = method.Substring(method.LastIndexOf('.') + 1);
			var methd = type.GetMethod(methodString, BindingFlags.Static | BindingFlags.Public);
			
			if (methd == null)
				return null;
			
			return methd.Invoke(null, arguments);
		}
		
		/// <summary>
		/// Run the specified instance method with the specified arguments.
		/// </summary>
		/// <param name='instance'>
		/// The object to run the method on. (ex: an EditorWindow instance)
		/// </param>
		/// <param name='method'>
		/// The name of the instance method in the UnityEditor namespace to run (ex: Repaint)
		/// </param>
		/// <param name='arguments'>
		/// The arguments to pass to the method. (optional)
		/// </param>
		public static object RunInstance(object instance, string method, params object[] arguments)
		{
			// Return if not in the editor
			if (!Application.isEditor)
				return null;
			
			if (editorAssembly == null)
				LoadAssembly();
				
			if (editorAssembly == null)
				return null;
			
			// Get the type
			var type = instance.GetType();
			
			if (type == null)
				return null;
			
			// Get the Method
			var methodString = method.Substring(method.LastIndexOf('.') + 1);
			var methd = type.GetMethod(methodString, BindingFlags.Instance | BindingFlags.Public);
			
			if (methd == null)
				return null;
			
			return methd.Invoke(instance, arguments);
		}
	}
}
