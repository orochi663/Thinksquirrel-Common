// Event helper methods
// EventHelper.cs
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

namespace ThinksquirrelSoftware.Common.Editor
{
	public static class EventHelper
	{
		public static bool LeftMouseDown
		{
			get { return IsMouseDown(0); }
		}
		
		public static bool LeftMouseUp
		{
			get { return IsMouseUp(0); }	
		}
		
		public static bool RightMouseDown
		{
			get { return IsMouseDown(1); }
		}
		
		public static bool RightMouseUp
		{
			get { return IsMouseUp(1); }	
		}
		
		public static bool IsMouseDown(int button)
		{
			return Event.current.type == EventType.MouseDown && Event.current.button == button;
		}
		
		public static bool IsMouseUp(int button)
		{
			return Event.current.type == EventType.MouseUp && Event.current.button == button;
		}
		
		public static bool DeleteKeyPressed
		{
			get { return IsKeyPressed(KeyCode.Delete) || IsKeyPressed(KeyCode.Backspace); }
		}
		
		public static bool EnterKeyPressed
		{
			get { return IsKeyPressed(KeyCode.Return) || IsKeyPressed(KeyCode.KeypadEnter); }
		}
		
		public static bool IsKeyPressed(KeyCode keyCode)
		{
			return Event.current.type == EventType.keyDown && Event.current.keyCode == keyCode;
		}
		
		public static bool ShiftPressed
		{
			get { return IsModifierPressed(EventModifiers.Shift); }
		}
		
		public static bool AltPressed
		{
			get { return IsModifierPressed(EventModifiers.Command); }
		}
		
		public static bool CtrlPressed
		{
			get { return IsModifierPressed(EventModifiers.Control); }
		}
		
		public static bool CmdPressed
		{
			get { return IsModifierPressed(EventModifiers.Command); }
		}
		
		public static bool CtrlOrCommandPressed
		{
			get { return EditorGUI.actionKey; }
		}
		
		public static bool IsModifierPressed(EventModifiers modifiers)
		{
			return (Event.current.modifiers & modifiers) != 0;
		}
	}
}
