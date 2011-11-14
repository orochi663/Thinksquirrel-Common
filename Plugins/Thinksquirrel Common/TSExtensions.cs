using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ThinksquirrelSoftware.Common
{
	/// <summary>
	/// Various extension methods for .NET and Unity.
	/// </summary>
	public static class TSExtensions
	{
		
		/// <summary>
		/// Break a list of items into chunks of a specific size. (Extension Method)
		/// </summary>
		public static IEnumerable<IEnumerable<T>> Chunk<T>(this IEnumerable<T> source, int chunksize)
		{
			while (source.Any())
			{
				yield return source.Take(chunksize);
				source = source.Skip(chunksize);
			}
		}
			
		/// <summary>
		/// Generates a timestamp. (Extension Method)
		/// </summary>
		public static string GetTimestamp(this System.DateTime value)
		{
			return value.ToString("yyyyMMddHHmmssffff");
		}
			
		/// <summary>
		/// Iterate the specified enumerable and callback. (Extension Method)
		/// </summary>
		/// <param name='enumerable'>
		/// Enumerable.
		/// </param>
		/// <param name='callback'>
		/// Callback.
		/// </param>
		/// <typeparam name='T'>
		/// The 1st type parameter.
		/// </typeparam>
		/// <exception cref='ArgumentNullException'>
		/// Is thrown when an argument passed to a method is invalid because it is <see langword="null" /> .
		/// </exception>
		public static void Iterate<T>(this IEnumerable<T> enumerable, Action<T> callback)
		{
			if (enumerable == null)
			{
				throw new ArgumentNullException("enumerable");
			}
			
			IterateHelper(enumerable, (x, i) => callback(x));
		}
			
		/// <summary>
		/// Iterate the specified enumerable and callback. (Extension Method)
		/// </summary>
		/// <param name='enumerable'>
		/// Enumerable.
		/// </param>
		/// <param name='callback'>
		/// Callback.
		/// </param>
		/// <typeparam name='T'>
		/// The 1st type parameter.
		/// </typeparam>
		/// <exception cref='ArgumentNullException'>
		/// Is thrown when an argument passed to a method is invalid because it is <see langword="null" /> .
		/// </exception>
		public static void Iterate<T>(this IEnumerable<T> enumerable, Action<T,int> callback)
		{
			if (enumerable == null)
			{
				throw new ArgumentNullException("enumerable");
			}
			
			IterateHelper(enumerable, callback);
		}
			
		private static void IterateHelper<T>(this IEnumerable<T> enumerable, Action<T,int> callback)
		{
			int count = 0;
			foreach (var cur in enumerable)
			{
				callback(cur, count);
				count++;
			}
		}
		
		/// <summary>
		/// Calculates the torque applied to a rigidbody.
		/// </summary>
		public static Vector3 CalculateTorque(this Rigidbody rigidbody, Vector3 force, Vector3 position)
		{
			Vector3 r = rigidbody.position - position;
			return Vector3.Cross(force, r);
		}
		
		/// <summary>
		/// Calculates the force and torque applied to a rigidbody.
		/// </summary>
		public static Vector3 CalculateForceAndTorque(this Rigidbody rigidbody, Vector3 force, Vector3 position)
		{
			return rigidbody.CalculateTorque(force, position) + force;
		}
		
		/// <summary>
		/// Calculates the torque applied to a transform.
		/// </summary>
		public static Vector3 CalculateTorque(this Transform transform, Vector3 force, Vector3 position)
		{
			Vector3 r = transform.position - position;
			return Vector3.Cross(force, r);
		}
		
		/// <summary>
		/// Calculates the force and torque applied to a transform.
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
	}
}
