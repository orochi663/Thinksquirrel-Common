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
using System.Collections.Generic;

namespace ThinksquirrelSoftware.Common.Editor
{
	public static class EventHelper
	{
		private static HashSet<int> trackedMouseButtons = new HashSet<int>();
		private static HashSet<KeyCode> trackedKeys = new HashSet<KeyCode>();
		
		public static bool LeftMouseDown
		{
			get { return IsMouseDown(0); }
		}
		
		public static bool LeftMouseUp
		{
			get { return IsMouseUp(0); }	
		}
		
		public static bool LeftMousePressed
		{
			get { return IsMousePressed(0); }	
		}
		
		public static bool RightMouseDown
		{
			get { return IsMouseDown(1); }
		}
		
		public static bool RightMouseUp
		{
			get { return IsMouseUp(1); }	
		}
		
		public static bool RightMousePressed
		{
			get { return IsMousePressed(1); }	
		}
		
		public static bool IsMouseDown(int button)
		{
			bool down = Event.current.type == EventType.MouseDown && Event.current.button == button;
			
			if (down && !trackedMouseButtons.Contains(button))
				trackedMouseButtons.Add(button);
				
			return down;
		}
		
		public static bool IsMouseUp(int button)
		{
			bool up = Event.current.type == EventType.MouseUp && Event.current.button == button;
			
			if (up && trackedMouseButtons.Contains(button))
				trackedMouseButtons.Remove(button);
				
			return up;
		}
		
		public static bool IsMousePressed(int button)
		{
			return trackedMouseButtons.Contains(button);
		}
		
		public static bool DeleteKeyDown
		{
			get { return IsKeyDown(KeyCode.Delete) || IsKeyDown(KeyCode.Backspace); }
		}
		
		public static bool EnterKeyDown
		{
			get { return IsKeyDown(KeyCode.Return) || IsKeyDown(KeyCode.KeypadEnter); }
		}
		
		public static bool DeleteKeyUp
		{
			get { return IsKeyUp(KeyCode.Delete) || IsKeyUp(KeyCode.Backspace); }
		}

		public static bool EnterKeyUp
		{
			get { return IsKeyUp(KeyCode.Return) || IsKeyUp(KeyCode.KeypadEnter); }
		}
		
		public static bool DeleteKeyPressed
		{
			get { return IsKeyPressed(KeyCode.Delete) || IsKeyPressed(KeyCode.Backspace); }
		}

		public static bool EnterKeyPressed
		{
			get { return IsKeyPressed(KeyCode.Return) || IsKeyPressed(KeyCode.KeypadEnter); }
		}

		public static bool IsKeyDown(KeyCode keyCode)
		{
			bool down = Event.current.type == EventType.KeyDown && Event.current.keyCode == keyCode;
			
			if (down && !trackedKeys.Contains(keyCode))
				trackedKeys.Add(keyCode);
			
			return down;
		}
		
		public static bool IsKeyUp(KeyCode keyCode)
		{
			bool up = Event.current.type == EventType.keyUp && Event.current.keyCode == keyCode;
			
			if (up && trackedKeys.Contains(keyCode))
				trackedKeys.Remove(keyCode);
			
			return up;
		}
		
		public static bool IsKeyPressed(KeyCode keyCode)
		{
			return trackedKeys.Contains(keyCode);
		}
		
		public static bool ShiftPressed
		{
			get { return IsModifierPressed(EventModifiers.Shift); }
		}
		
		public static bool AltPressed
		{
			get { return IsModifierPressed(EventModifiers.Alt); }
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
