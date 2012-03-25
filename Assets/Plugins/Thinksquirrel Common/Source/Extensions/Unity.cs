// Unity extension methods
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
using ThinksquirrelSoftware.Common.ObjectModel;

namespace ThinksquirrelSoftware.Common.Extensions
{
	/// <summary>
	/// Unity extension methods.
	/// </summary>
	public static class Unity
	{
		/// <summary>
		/// Determines whether a Vector3 is valid (all values are not equal to Infinity or NaN). (Extension Method)
		/// </summary>
		public static bool IsValid(this Vector3 input)
		{
			return (!float.IsNaN(input.x)
					&& !float.IsNaN(input.y)
					&& !float.IsNaN(input.z)
					&& !float.IsInfinity(input.x)
					&& !float.IsInfinity(input.y)
					&& !float.IsInfinity(input.z));
		}
		
		/// <summary>
		/// Determines whether a Vector2 is valid (all values are not equal to Infinity or NaN). (Extension Method)
		/// </summary>
		public static bool IsValid(this Vector2 input)
		{
			return (!float.IsNaN(input.x)
					&& !float.IsNaN(input.y)
					&& !float.IsInfinity(input.x)
					&& !float.IsInfinity(input.y));
		}
		
		/// <summary>
		/// Calculates the torque applied to a rigidbody. (Extension Method)
		/// </summary>
		public static Vector3 CalculateTorque(this Rigidbody rigidbody, Vector3 force, Vector3 position)
		{
			Vector3 r = rigidbody.position - position;
			return Vector3.Cross(force, r);
		}
		
		/// <summary>
		/// Calculates the force and torque applied to a rigidbody. (Extension Method)
		/// </summary>
		public static Vector3 CalculateForceAndTorque(this Rigidbody rigidbody, Vector3 force, Vector3 position)
		{
			return rigidbody.CalculateTorque(force, position) + force;
		}
		
		/// <summary>
		/// Calculates the torque applied to a transform. (Extension Method)
		/// </summary>
		public static Vector3 CalculateTorque(this Transform transform, Vector3 force, Vector3 position)
		{
			Vector3 r = transform.position - position;
			return Vector3.Cross(force, r);
		}
		
		/// <summary>
		/// Calculates the force and torque applied to a transform. (Extension Method)
		/// </summary>
		public static Vector3 CalculateForceAndTorque(this Transform transform, Vector3 force, Vector3 position)
		{
			return transform.CalculateTorque(force, position) + force;
		}
	
		/// <summary>
		/// Recalculates the tangents of a mesh. (Extension Method)
		/// </summary>
		public static void RecalculateTangents(this Mesh mesh)
		{
			int triangleCount = mesh.triangles.Length / 3;
			int vertexCount = mesh.vertices.Length;
	
			Vector3[] tan1 = new Vector3[vertexCount];
			Vector3[] tan2 = new Vector3[vertexCount];
	
			Vector4[] tangents = new Vector4[vertexCount];
	
			for (long a = 0; a < triangleCount; a+=3)
			{
				long i1 = mesh.triangles[a + 0];
				long i2 = mesh.triangles[a + 1];
				long i3 = mesh.triangles[a + 2];
	
				Vector3 v1 = mesh.vertices[i1];
				Vector3 v2 = mesh.vertices[i2];
				Vector3 v3 = mesh.vertices[i3];
	
				Vector2 w1 = mesh.uv[i1];
				Vector2 w2 = mesh.uv[i2];
				Vector2 w3 = mesh.uv[i3];
	
				float x1 = v2.x - v1.x;
				float x2 = v3.x - v1.x;
				float y1 = v2.y - v1.y;
				float y2 = v3.y - v1.y;
				float z1 = v2.z - v1.z;
				float z2 = v3.z - v1.z;
	
				float s1 = w2.x - w1.x;
				float s2 = w3.x - w1.x;
				float t1 = w2.y - w1.y;
				float t2 = w3.y - w1.y;
	
				float r = 1.0f / (s1 * t2 - s2 * t1);
	
				Vector3 sdir = new Vector3((t2 * x1 - t1 * x2) * r, (t2 * y1 - t1 * y2) * r, (t2 * z1 - t1 * z2) * r);
				Vector3 tdir = new Vector3((s1 * x2 - s2 * x1) * r, (s1 * y2 - s2 * y1) * r, (s1 * z2 - s2 * z1) * r);
	
				tan1[i1] += sdir;
				tan1[i2] += sdir;
				tan1[i3] += sdir;
	
				tan2[i1] += tdir;
				tan2[i2] += tdir;
				tan2[i3] += tdir;
			}
	
	
			for (long a = 0; a < vertexCount; ++a)
			{
				Vector3 n = mesh.normals[a];
				Vector3 t = tan1[a];
	
				Vector3 tmp = (t - n * Vector3.Dot(n, t)).normalized;
				tangents[a] = new Vector4(tmp.x, tmp.y, tmp.z);
	
				tangents[a].w = (Vector3.Dot(Vector3.Cross(n, t), tan2[a]) < 0.0f) ? -1.0f : 1.0f;
			}
	
			mesh.tangents = tangents;
		}
		
		/// <summary>
		/// Gets the width of a pixel at a world space position, relative to a camera. (Extension Method)
		/// </summary>
		public static float GetPixelWidth(this Camera camera, Vector3 position)
		{
			//Get the screen coordinate of some point
			var screenPos = camera.WorldToScreenPoint(position);
			var offset = Vector3.zero;

			//Offset by 1 pixel
			if (screenPos.x > 0)
				offset = screenPos - Vector3.right;
			else
				offset = screenPos + Vector3.right;

			if (screenPos.y > 0)
				offset = screenPos - Vector3.up;
			else
				offset = screenPos + Vector3.up;

			//Get the world coordinate once offset.
			offset = camera.ScreenToWorldPoint(offset);

			return (camera.transform.InverseTransformPoint(position) - camera.transform.InverseTransformPoint(offset)).magnitude;	
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
		public static T GetComponentFromInterface<T>(this GameObject gameObject) where T : class, IMonoBehaviour
		{
			var components = gameObject.GetComponents<MonoBehaviour>();
			foreach(var c in components)
			{
				if (typeof(T).IsAssignableFrom(c.GetType()))
				{
					return c as T;
				}
			}
			
			return null;
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
		public static T GetComponentFromInterface<T>(this Component component) where T : class, IMonoBehaviour
		{
			var components = component.GetComponents<MonoBehaviour>();
			foreach(var c in components)
			{
				if (typeof(T).IsAssignableFrom(c.GetType()))
				{
					return c as T;
				}
			}
			
			return null;
		}
	}
}
