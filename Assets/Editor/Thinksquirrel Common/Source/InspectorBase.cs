// Unity inspector base class
// InspectorBase.cs
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
using UnityEditor;
using System.Collections.Generic;

namespace ThinksquirrelSoftware.Common.Editor
{
	public abstract class InspectorBase : UnityEditor.Editor
	{
		protected bool pinState;
	
		protected abstract string inspectorName
		{
			get;
		}
		protected abstract void Initialize();
		
		protected UndoManager undoManager;
		
		protected void OnEnable()
		{
			undoManager = new UndoManager(target, inspectorName, true);
			Initialize();
		}
		
		// Clamp Helper Methods
		protected float ClampPositiveZero(float val)
		{
			return Mathf.Clamp(val, 0, Mathf.Infinity);
		}

		protected float ClampPositive(float val)
		{
			return Mathf.Clamp(val, Mathf.Epsilon, Mathf.Infinity);
		}

		protected int ClampPositiveInt(int val)
		{
			return Mathf.Clamp(val, 1, int.MaxValue);
		}

		protected int ClampPositiveIntZero(int val)
		{
			return Mathf.Clamp(val, 0, int.MaxValue);
		}
		
		// Module Helper Methods
		private Dictionary<string, bool> mModules = new Dictionary<string, bool>();
		private string ModuleLongName(string name)
		{
			return this.GetType().ToString() + name;
		}
		public void RegisterModule(string name, bool defaultState)
		{
			mModules.Add(ModuleLongName(name), EditorPrefs.GetBool(ModuleLongName(name), defaultState));
		}
		public bool GetModuleState(string name)
		{
			if (!mModules.ContainsKey(ModuleLongName(name)))
			{
				RegisterModule(name, true);
				return true;
			}
			return mModules[ModuleLongName(name)];
		}
		private bool SetModuleState(string name, bool state)
		{
			if (mModules[ModuleLongName(name)] != state)
			{
				EditorPrefs.SetBool(ModuleLongName(name), state);
				mModules[ModuleLongName(name)] = state;
				Repaint();
			}
			return mModules[ModuleLongName(name)];
		}
		public void RemoveModule(string name)
		{
			mModules.Remove(ModuleLongName(name));
			EditorPrefs.DeleteKey(ModuleLongName(name));
		}
		public bool DrawModule(string name)
		{
			if (!mModules.ContainsKey(ModuleLongName(name)))
			{
				RegisterModule(name, true);
			}
			return SetModuleState(name,
				GUILayout.Toggle(mModules[ModuleLongName(name)], name, EditorStyles.toolbarButton));
		}
			
	}
}