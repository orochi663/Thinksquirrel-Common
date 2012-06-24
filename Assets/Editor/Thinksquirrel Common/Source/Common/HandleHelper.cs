// Handle helper methods
// HandleHelper.cs
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
	public static class HandleHelper
	{
		public static Vector2 WorldToScreenPoint(Vector3 worldPoint)
		{
			return HandleUtility.WorldToGUIPoint(worldPoint);
		}
		
		public static Vector3 ScreenToWorldPoint(Vector3 screenPoint)
		{
			Ray ray = HandleUtility.GUIPointToWorldRay(screenPoint);
			Plane plane = new Plane(-Camera.current.transform.forward, Camera.current.transform.forward * screenPoint.z);
			float enter;
			plane.Raycast(ray, out enter);

			return ray.origin + ray.direction * enter;
		}
		
		public static Vector3 ScreenToLocalPoint(Vector2 screenPoint, Transform transform)
		{
			return transform.InverseTransformPoint(ScreenToWorldPoint(screenPoint));
		}
		
		public static Vector2 LocalToScreenPoint(Vector3 localPoint, Transform transform)
		{
			return WorldToScreenPoint(transform.TransformPoint(localPoint));
		}
		
		public static void DrawTexture(Texture2D texture, Vector2 position, HandleAlignment alignment)
		{
			Vector2 offset = position;
			
			switch(alignment)
			{
			case HandleAlignment.TopCenter:
				offset -= new Vector2(texture.width / 2, 0);
				break;
			case HandleAlignment.TopRight:
				offset -= new Vector2(texture.width, 0);
				break;
			case HandleAlignment.Left:
				offset -= new Vector2(0, texture.height / 2);
				break;
			case HandleAlignment.Center:
				offset -= new Vector2(texture.width / 2, texture.height / 2);
				break;
			case HandleAlignment.Right:
				offset -= new Vector2(texture.width, texture.height / 2);
				break;
			case HandleAlignment.BottomLeft:
				offset -= new Vector2(0, texture.height);
				break;
			case HandleAlignment.BottomCenter:
				offset -= new Vector2(texture.width / 2, texture.height);
				break;
			case HandleAlignment.BottomRight:
				offset -= new Vector2(texture.width, texture.height);
				break;
			}
			
			GUI.DrawTexture(new Rect(offset.x, offset.y, texture.width, texture.height), texture);
		}
				
		public enum HandleAlignment
		{
			TopLeft,
			TopCenter,
			TopRight,
			Left,
			Center,
			Right,
			BottomLeft,
			BottomCenter,
			BottomRight
		}
	}
}