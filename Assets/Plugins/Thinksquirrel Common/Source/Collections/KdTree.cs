// k-d tree collection class
// KdTree.cs
// Thinksquirrel Software Common Libraries
//  
// Authors:
//       Josh Montoute <josh@thinksquirrel.com>
//		 A Stark <http://forum.unity3d.com/members/81-andeeee>
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

namespace ThinksquirrelSoftware.Common.Collections
{
	/// <summary>
	/// Organizes points into a kd-tree structure.
	/// </summary>
	/// <remarks>
	/// This class implements a data structure that stores a list of points in space.
	/// A common task in game programming is to take a supplied point and discover which
	/// of a stored set of points is nearest to it. The kd-tree allows this "nearest neighbour" 
	/// search to be carried out quickly, or at least much more quickly than a simple linear search 
	/// through the list.
	/// The nearest-neighbour search returns an integer index - it is assumed that the original
	/// array of points is available for the lifetime of the tree, and the index refers to that
	/// array.
	/// </remarks>
	public class KdTree {

/*! \cond PRIVATE */
		public FixedArray2<KdTree> lr;
		public Vector3 pivot;
		public int pivotIndex;
		public int axis;
		
		private Vector3[] rootPoints;
		private int[] rootIndices;
		private NeighborList rootNeighborList;
		private bool enabled = true;
		
		//	Change this value to 2 if you only need two-dimensional X,Y points. The search will
		//	be quicker in two dimensions.
		private int numDims = 3;
		
		public KdTree(int dimensions) {
			numDims = dimensions;
		}
/*! \endcond */
		
		/// <summary>
		/// Make a new tree from a list of points.
		/// </summary>
		public static KdTree MakeFromPoints(int dimensions, Vector3[] points) {
			int[] indices = Iota(points.Length);
			return MakeFromPointsInner(0, 0, points.Length - 1, points, indices, dimensions, null);
		}
		
		/// <summary>
		/// Regenerate a tree from a list of points, reusing any old objects from the root.
		/// </summary>
		public static KdTree Regenerate(KdTree root, int dimensions, Vector3[] points) {
			if (root.rootIndices.Length != points.Length)
			{
				root.rootIndices = Iota(points.Length);
			}
			else
			{
				Iota(root.rootIndices);
			}
			return MakeFromPointsInner(0, 0, points.Length - 1, points, root.rootIndices, dimensions, root);
		}
		
		/// <summary>
		/// Rebalance the tree.
		/// </summary>
		public static void Rebalance(KdTree root)
		{
			Regenerate(root, root.numDims, root.rootPoints);
		}
	
		//	Recursively build a tree by separating points at plane boundaries.
		static KdTree MakeFromPointsInner(
						int depth,
						int stIndex, int enIndex,
						Vector3[] points,
						int[] inds,
						int dimensions,
						KdTree tree) {
			
			
			KdTree root = tree != null ? tree : new KdTree(dimensions);
			root.enabled = true;
			
			if (depth == 0 && root.rootPoints != points)
			{
				root.rootPoints = points;
				root.rootIndices = inds;
			}
			
			root.axis = depth % root.numDims;
			int splitPoint = FindPivotIndex(points, inds, stIndex, enIndex, root.axis);
	
			root.pivotIndex = inds[splitPoint];
			root.pivot = points[root.pivotIndex];
			
			int leftEndIndex = splitPoint - 1;
			
			if (leftEndIndex >= stIndex) {
				root.lr[0] = MakeFromPointsInner(depth + 1, stIndex, leftEndIndex, points, inds, dimensions, root.lr[0]);
			}
			else if (root.lr[0] != null)
			{
				root.lr[0].enabled = false;
			}
			
			int rightStartIndex = splitPoint + 1;
			
			if (rightStartIndex <= enIndex) {
				root.lr[1] = MakeFromPointsInner(depth + 1, rightStartIndex, enIndex, points, inds, dimensions, root.lr[1]);
			}
			else if (root.lr[1] != null)
			{
				root.lr[1].enabled = false;
			}
			
			return root;
		}
		
		
		static void SwapElements(int[] arr, int a, int b) {
			int temp = arr[a];
			arr[a] = arr[b];
			arr[b] = temp;
		}
		
	
		//	Simple "median of three" heuristic to find a reasonable splitting plane.
		static int FindSplitPoint(Vector3[] points, int[] inds, int stIndex, int enIndex, int axis) {
			float a = points[inds[stIndex]][axis];
			float b = points[inds[enIndex]][axis];
			int midIndex = (stIndex + enIndex) / 2;
			float m = points[inds[midIndex]][axis];
			
			if (a > b) {
				if (m > a) {
					return stIndex;
				}
				
				if (b > m) {
					return enIndex;
				}
				
				return midIndex;
			} else {
				if (a > m) {
					return stIndex;
				}
				
				if (m > b) {
					return enIndex;
				}
				
				return midIndex;
			}
		}

/*! \cond PRIVATE */
		
		// Find a new pivot index from the range by splitting the points that fall either side of its plane.
		static int FindPivotIndex(Vector3[] points, int[] inds, int stIndex, int enIndex, int axis) {
			int splitPoint = FindSplitPoint(points, inds, stIndex, enIndex, axis);
			// int splitPoint = Random.Range(stIndex, enIndex);
	
			Vector3 pivot = points[inds[splitPoint]];
			SwapElements(inds, stIndex, splitPoint);
	
			int currPt = stIndex + 1;
			int endPt = enIndex;
			
			while (currPt <= endPt) {
				Vector3 curr = points[inds[currPt]];
				
				if ((curr[axis] > pivot[axis])) {
					SwapElements(inds, currPt, endPt);
					endPt--;
				} else {
					SwapElements(inds, currPt - 1, currPt);
					currPt++;
				}
			}
			
			return currPt - 1;
		}
		
		
		static void Iota(int[] arr)
		{
			for (int i = 0; i < arr.Length; i++)
			{
				arr[i] = i;
			}
		}
		
		static int[] Iota(int num) {
			int[] result = new int[num];
			
			Iota(result);
			
			return result;
		}
/*! \endcond */		
		
		/// <summary>
		/// Find the nearest point in the set to the supplied point.
		/// </summary>
		public int FindNearest(Vector3 position)
		{	
			return FindNearest(position, float.MaxValue);	
		}
		
		/// <summary>
		/// Find the nearest point in the set to the supplied point, up to the maximum square distance.
		/// </summary>
		public int FindNearest(Vector3 position, float maxSqrDistance)
		{	
			if (rootNeighborList == null)
				rootNeighborList = new NeighborList(1);
			else
			{
				rootNeighborList.Clear();
				rootNeighborList.Capacity = 1;
			}
			
			K_NN(position, HyperRect.Infinite, maxSqrDistance, 0, rootNeighborList);
			
			return rootNeighborList.RemoveRoot().pivotIndex;
		}
		
		
		/// <summary>
		/// Find the nearest points in the set up to the maximum amount.
		/// </summary>
		public int[] FindNearest(Vector3 position, int maxAmount)
		{
			return FindNearest(position, maxAmount, float.MaxValue);
		}
		
		/// <summary>
		/// Find the nearest points in the set up to the maximum amount, up to the maximum square distance.
		/// </summary>
		public int[] FindNearest(Vector3 position, int maxAmount, float maxSqrDistance)
		{
			int[] result = new int[maxAmount];
			
			FindNearest(position, result, maxAmount, maxSqrDistance);
			
			return result;
		}
		
		/// <summary>
		/// Find the nearest points in the set up to the maximum amount, up to the maximum square distance.
		/// </summary>
		public void FindNearest(Vector3 position, int[] result, int len, float maxSqrDistance)
		{
			if (rootNeighborList == null)
				rootNeighborList = new NeighborList(len);
			else
			{
				rootNeighborList.Clear();
				rootNeighborList.Capacity = len;
			}
			
			K_NN(position, HyperRect.Infinite, maxSqrDistance, 0, rootNeighborList);
			
			for(int i = 0; i < len; i++)
			{
				if (rootNeighborList.Count > 0)
					result[i++] = rootNeighborList.RemoveRoot().pivotIndex;
				else
					result[i++] = -1;
			}
		}
		
		/// <summary>
		/// K-Nearest neighbor algorithm
		/// </summary>
		public void K_NN(Vector3 position, HyperRect hr, float maxSqrDist, int level, NeighborList neighbors)
		{	
			// Split field of kd
			int s = level % numDims;
			
			// Pivot to target
			float mySqrDist = (pivot - position).sqrMagnitude;
			
			// Cut rectangle into sub-hyperrectangles
			HyperRect left_hr = hr;
			HyperRect right_hr = hr;
			left_hr.max[s] = pivot[s];
			right_hr.min[s] = pivot[s];
			
			// Check to see if target is in the left/right side and set closer/further
			bool inLeft = position[s] < pivot[s];
			
			KdTree closer, further;
			HyperRect closerRect, furtherRect;
			
			if (inLeft)
			{
				closer = lr[0];
				closerRect = left_hr;
				further = lr[1];
				furtherRect = right_hr;
			}
			else
			{
				closer = lr[1];
				closerRect = right_hr;
				further = lr[0];
				furtherRect = left_hr;
			}
			
			// Recursively call nearest neighbor
			if (closer != null && closer.enabled)
				closer.K_NN(position, closerRect, maxSqrDist, level + 1, neighbors);
			
			//KdTree nearest = neighbors.Peek();
			float distSqr;
			
			if (!neighbors.MaxCapacity)
			{
				distSqr = float.MaxValue;
			}
			else
			{
				distSqr = neighbors.MaxPriority();
			}
			
			// Set maximum distance squared
			maxSqrDist = Mathf.Min(maxSqrDist, distSqr);
			
			// Check to see if a point in the further is within maxSqrDist
			Vector3 closest = furtherRect.Closest(position);
			
			if (Vector3.Distance(closest, position) < Mathf.Sqrt(maxSqrDist))
			{
				if (mySqrDist < distSqr)
				{
					//nearest = this;
					
					distSqr = mySqrDist;
					
					if (neighbors.MaxCapacity)
					{
						maxSqrDist = neighbors.MaxPriority();
					}
					else
					{
						neighbors.Insert(distSqr, this);
						maxSqrDist = float.MaxValue;
					}
				}
				
				// Recursively call NN
				if (further != null && further.enabled)
					further.K_NN(position, furtherRect, maxSqrDist, level + 1, neighbors);
				
				//KdTree tempNearest = neighbors.Peek();
				float tempDistSqr = neighbors.MaxPriority();
				
				if (tempDistSqr < distSqr)
				{
					//nearest = tempNearest;
					distSqr = tempDistSqr;
				}
			}
			else if (mySqrDist < maxSqrDist)
			{
				//nearest = this;
				distSqr = mySqrDist;
			}
		}

		
		public int DrawDebug(Plane[] frustrum, Material material, Color color, Vector3 center, float distance)
		{
			material.SetPass(0);
		    
			GL.Color(color);
			GL.Begin(GL.LINES);
			
			int count = Draw(frustrum, center, distance);
			
			GL.End();
		
			return count;
		}
        
		private int Draw(Plane[] frustum, Vector3 center, float distance)
        {
			int count = 0;
			Vector3 c = center;
			Vector3 extents = Vector3.zero;
			switch(this.axis)
			{
			case 0:
				c.x = pivot.x;
				extents.y = distance;
				extents.z = distance;
				break;
			case 1:
				c.y = pivot.y;
				extents.x = distance;
				extents.z = distance;
				break;
			case 2:
				c.z = pivot.z;
				extents.x = distance;
				extents.y = distance;
				break;
			}
			
			Bounds bounds = new Bounds(c, extents); 
				
			bool contains = GeometryUtility.TestPlanesAABB(frustum, bounds);
			FixedArray8<Vector3> boxCorners = new FixedArray8<Vector3>();
			
			boxCorners[0] = bounds.max;
			boxCorners[1] = new Vector3(bounds.min.x, bounds.max.y, bounds.max.z);
			boxCorners[2] = new Vector3(bounds.min.x, bounds.max.y, bounds.min.z);
			boxCorners[3] = new Vector3(bounds.max.x, bounds.max.y, bounds.min.z);
			boxCorners[4] = new Vector3(bounds.max.x, bounds.min.y, bounds.max.z);
			boxCorners[5] = new Vector3(bounds.min.x, bounds.min.y, bounds.max.z);
			boxCorners[6] = bounds.min;
			boxCorners[7] = new Vector3(bounds.max.x, bounds.min.y, bounds.min.z);
			
			
            // Draw the plane only if it is at least partially in view.
            if (contains)
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

                // Draw the tree's children, if any
                foreach (KdTree child in this.lr)
                {
					if (child != null && child.enabled)
                    	count += child.Draw(frustum, center, distance);
                }
            }
			
			return count;
        }

		
		/// <summary>
		/// Simple output of tree structure - mainly useful for getting a rough idea of how deep the tree is (and therefore how well the splitting heuristic is performing).
		/// </summary>
		public string Dump(int level) {
			string result = pivotIndex.ToString().PadLeft(level) + "\n";
			
			if (lr[0] != null) {
				result += lr[0].Dump(level + 2);
			}
			
			if (lr[1] != null) {
				result += lr[1].Dump(level + 2);
			}
			
			return result;
		}
	}

	public struct HyperRect
	{
		public Vector3 min, max;
		static HyperRect infinite = new HyperRect(
					new Vector3(Mathf.NegativeInfinity, Mathf.NegativeInfinity, Mathf.NegativeInfinity),
					new Vector3(Mathf.Infinity, Mathf.Infinity, Mathf.Infinity));
				
		public HyperRect(Vector3 min, Vector3 max)
		{
			this.min = min;
			this.max = max;
		}
				
		public static HyperRect Infinite { get { return infinite; } }
		
		public Vector3 Closest(Vector3 point)
		{
			Vector3 p = Vector3.zero;

            for (int i = 0; i < 3; ++i)
            {
                if (point[i] <= min[i])
                {
                    p[i] = min[i];
                }
                else if (point[i] >= max[i])
                {
                    p[i] = max[i];
                }
                else
                {
                    p[i] = point[i];
                }
            }

            return p;
		}
	}
	
	public class NeighborList : BinaryHeap<float, KdTree>
	{		
		public NeighborList(int capacity) : base(capacity) { }
		public bool MaxCapacity { get { return this.Count == Capacity; } }
		
		new public float MaxPriority()
		{
			return Count == 0 ? Mathf.Infinity : (float)base.MaxPriority();
		}
	}
}
#endif