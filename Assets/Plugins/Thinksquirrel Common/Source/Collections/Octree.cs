// Loose dynamic octree
// Octree.cs
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
#if !COMPACT
using UnityEngine;
using System;
using System.Collections.Generic;

namespace ThinksquirrelSoftware.Common.Collections
{
    public class Octree<T>
    {	
        private float looseness = 0;
        private int depth = 0;
        private Vector3 center = Vector3.zero;
        private float length = 0f;
        private Bounds bounds = default(Bounds);
        private HashSet<T> objects = null;
        private Octree<T>[] children;
		private bool hasChildren = false;
        private float worldSize = 0f;
		
        public Octree(float worldSize, float looseness, int depth)
            : this(worldSize, looseness, depth, 0, Vector3.zero)
        {
        }

        public Octree(float worldSize, float looseness, int depth, Vector3 center)
            : this(worldSize, looseness, depth, 0, center)
        {
        }

        private Octree(float worldSize, float looseness,
            int maxDepth, int depth, Vector3 center)
        {
			children = new Octree<T>[8];
			objects = new HashSet<T>();
            this.worldSize = worldSize;
            this.looseness = looseness;
            this.depth = depth;
            this.center = center;
            this.length = this.looseness * this.worldSize / Mathf.Pow(2, this.depth);
            float radius = this.length / 2f;

            // Create the bounding box.
            Vector3 size = new Vector3(radius, radius, radius);
            this.bounds = new Bounds(center, size);

            // Split the octree if the depth hasn't been reached.
            if (this.depth < maxDepth)
            {
                this.Split(maxDepth);
            }
        }

        public void Remove(T obj)
        {
            objects.Remove(obj);
        }
        
		public bool Contains(Vector3 center, float radius)
        {
            Vector3 size = new Vector3(radius, radius, radius);
            Bounds bounds = new Bounds(center, size);
			
            if (CheckContainment(ref this.bounds, ref bounds))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
		
		public bool Contains(Bounds bounds)
		{
			return CheckContainment(ref this.bounds, ref bounds);
		}
		
		public bool Contains(T obj)
		{
			return objects.Contains(obj);
		}
		
		private bool CheckContainment(ref Bounds b1, ref Bounds b2)
		{
			return b1.Contains(b2.min) && b1.Contains(b2.max);
		}
		
		// 0 = completely disjointed
		// 3 = b2 is fully within b1
		// 12 = b1 is fully within b2
		// 15 = both bounds are equal
		
		// Other results are partial intersections
		
		private int CheckIntersection(ref Bounds b1, ref Bounds b2)
		{	
			int result = 0;
			
			if(b1.Contains(b2.min)) result += 1;
			if(b1.Contains(b2.max)) result += 2;
			if(b2.Contains(b1.min)) result += 4;
			if(b2.Contains(b1.max)) result += 8;
			
			return result;
		}
		
		private bool CheckDisjoint(ref Bounds b1, ref Bounds b2)
		{
			return !b1.Contains(b2.min) && b1.Contains(b2.max);
		}
		
        public Octree<T> Add(T o, Vector3 center, float radius)
        {
            Vector3 size = new Vector3(radius, radius, radius);
            Bounds bounds = new Bounds(center, size);

            if (CheckContainment(ref this.bounds, ref bounds))
            {
                return this.Add(o, bounds, center);
            }
            return null;
        }

        public Octree<T> Add(T o, Bounds bounds)
        {
            if (CheckContainment(ref this.bounds, ref bounds))
            {
				Vector3 center = bounds.center;
                return this.Add(o, bounds, center);
            }
            return null;
        }
        
		private Octree<T> Add(T o, Bounds bounds, Vector3 center)
		{
			if (this.hasChildren)
			{
				// Find which child the object is closest to based on where the
				// object’s center is located in relation to the octree’s center.
				int index = 
					(center.x <= this.center.x ? 0 : 1) + 
					(center.y >= this.center.y ? 0 : 4) +
					(center.z <= this.center.z ? 0 : 2);
				
				// Add object to the child if it fits completely
				if (CheckContainment(ref this.children[index].bounds, ref bounds))
				{
					return this.children[index].Add(o, bounds, center);
				}
				// Add object to this node
				else
				{
					this.objects.Add(o);
					return this;
				}
			}
			this.objects.Add(o);
			return this;
		}
		
		public bool HasChildren()
		{
			return hasChildren;
		}
		
		public void Query(List<T> objects, Vector3 center, float radius)
		{
			Vector3 size = new Vector3(radius * 2f, radius * 2f, radius * 2f);
            Bounds bounds = new Bounds(center, size);

            if (CheckContainment(ref this.bounds, ref bounds))
            {
                this.Query(objects, bounds, center);
            }
		}
		
		public void Query(List<T> objects, Bounds bounds)
		{	
			if (CheckContainment(ref this.bounds, ref bounds))
            {
				Vector3 center = bounds.center;
                this.Query(objects, bounds, center);
            }
		}
		
		private void Query(List<T> objects, Bounds bounds, Vector3 center)
		{
			if (this.hasChildren)
			{
				// Find which child the object is closest to based on where the
				// object’s center is located in relation to the octree’s center.
				int index = 
					(center.x <= this.center.x ? 0 : 1) + 
					(center.y >= this.center.y ? 0 : 4) +
					(center.z <= this.center.z ? 0 : 2);
				
				// Intersection test
				int intersection = CheckIntersection(ref this.children[index].bounds, ref bounds);
				
				// Fully within the nearest octree, set the child as the new root, query and return
				if (intersection == 3)
				{
					this.children[index].Query(objects, bounds, center);
					return;
				}
				
				// Some sort of intersection - check the 8 children
				if (intersection != 0)
				{
					for(int i = 0; i < 8; i++)
					{		
						// Query the child
						this.children[index].Query(objects, bounds, center);
					}
					
					// Our troublesome points that aren't in child objects - add all of the ones we traverse, for narrow-phase checking
					objects.AddRange(this.objects);
				}
			}
			// More troublesome points here.
			objects.AddRange(this.objects);
		}
		
		public int DrawDebug(Plane[] frustrum, Material material, Color color)
		{
			material.SetPass(0);
		    
			GL.Color(color);
			GL.Begin(GL.LINES);
			
			int count = Draw(frustrum);
			
			GL.End();
		
			return count;
		}
        
		private int Draw(Plane[] frustum)
        {
			int count = 0;
			bool contains = GeometryUtility.TestPlanesAABB(frustum, this.bounds);
			FixedArray8<Vector3> boxCorners = new FixedArray8<Vector3>();
			
			boxCorners[0] = this.bounds.max;
			boxCorners[1] = new Vector3(this.bounds.min.x, this.bounds.max.y, this.bounds.max.z);
			boxCorners[2] = new Vector3(this.bounds.min.x, this.bounds.max.y, this.bounds.min.z);
			boxCorners[3] = new Vector3(this.bounds.max.x, this.bounds.max.y, this.bounds.min.z);
			boxCorners[4] = new Vector3(this.bounds.max.x, this.bounds.min.y, this.bounds.max.z);
			boxCorners[5] = new Vector3(this.bounds.min.x, this.bounds.min.y, this.bounds.max.z);
			boxCorners[6] = this.bounds.min;
			boxCorners[7] = new Vector3(this.bounds.max.x, this.bounds.min.y, this.bounds.min.z);
			
			
            // Draw the octree only if it is at least partially in view.
            if (contains)
            {
                // Draw the octree's bounds if there are objects in the octree.
                if (this.objects.Count > 0)
                {
					int j1;
					for (int j = 0; j < 4; ++j)
					{
						GL.Vertex3(boxCorners[j].x,boxCorners[j].y,boxCorners[j].z); //top lines
						j1 = (j+1)%4;
						GL.Vertex3(boxCorners[j1].x,boxCorners[j1].y,boxCorners[j1].z);
						j1 = j + 4;
						GL.Vertex3(boxCorners[j].x,boxCorners[j].y,boxCorners[j].z); //vertical lines
						GL.Vertex3(boxCorners[j1].x,boxCorners[j1].y,boxCorners[j1].z);
						GL.Vertex3(boxCorners[j1].x,boxCorners[j1].y,boxCorners[j1].z); //bottom rectangle
						j1 = 4 + (j+1)%4;
						GL.Vertex3(boxCorners[j1].x,boxCorners[j1].y,boxCorners[j1].z);
					}
					++count;
                }

                // Draw the octree's children, if any
                if (this.hasChildren)
                {
                    foreach (Octree<T> child in this.children)
                    {
                        count += child.Draw(frustum);
                    }
                }
            }
			
			return count;
        }
		
        private void Split(int maxDepth)
        {
            this.hasChildren = true;
            int depth = this.depth + 1;
            float quarter = this.length / this.looseness / 4f;

            this.children[0] = new Octree<T>(this.worldSize, this.looseness,
                maxDepth, depth, this.center + new Vector3(-quarter, quarter, -quarter));
            this.children[1] = new Octree<T>(this.worldSize, this.looseness,
                maxDepth, depth, this.center + new Vector3(quarter, quarter, -quarter));
            this.children[2] = new Octree<T>(this.worldSize, this.looseness,
                maxDepth, depth, this.center + new Vector3(-quarter, quarter, quarter));
            this.children[3] = new Octree<T>(this.worldSize, this.looseness,
                maxDepth, depth, this.center + new Vector3(quarter, quarter, quarter));
            this.children[4] = new Octree<T>(this.worldSize, this.looseness,
                maxDepth, depth, this.center + new Vector3(-quarter, -quarter, -quarter));
            this.children[5] = new Octree<T>(this.worldSize, this.looseness,
                maxDepth, depth, this.center + new Vector3(quarter, -quarter, -quarter));
            this.children[6] = new Octree<T>(this.worldSize, this.looseness,
                maxDepth, depth, this.center + new Vector3(-quarter, -quarter, quarter));
            this.children[7] = new Octree<T>(this.worldSize, this.looseness,
                maxDepth, depth, this.center + new Vector3(quarter, -quarter, quarter));
        }
    }
}
#endif