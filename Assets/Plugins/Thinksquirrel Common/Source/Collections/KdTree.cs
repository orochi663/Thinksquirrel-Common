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
		public KdTree[] lr;
		public Vector3 pivot;
		public int pivotIndex;
		public int axis;
		
		//	Change this value to 2 if you only need two-dimensional X,Y points. The search will
		//	be quicker in two dimensions.
		private int numDims = 3;
		
		public KdTree(int dimensions) {
			lr = new KdTree[2];
			numDims = dimensions;
		}
/*! \endcond */
		
		/// <summary>
		/// Make a new tree from a list of points.
		/// </summary>
		public static KdTree MakeFromPoints(int dimensions, params Vector3[] points) {
			int[] indices = Iota(points.Length);
			return MakeFromPointsInner(0, 0, points.Length - 1, points, indices, dimensions, null);
		}
		
		/// <summary>
		/// Make a new tree from a list of points, reusing any old KDTree objects.
		/// </summary>
		public static KdTree Regenerate(KdTree tree, int dimensions, params Vector3[] points) {
			int[] indices = Iota(points.Length);
			return MakeFromPointsInner(0, 0, points.Length - 1, points, indices, dimensions, tree);
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
			
			root.axis = depth % root.numDims;
			int splitPoint = FindPivotIndex(points, inds, stIndex, enIndex, root.axis);
	
			root.pivotIndex = inds[splitPoint];
			root.pivot = points[root.pivotIndex];
			
			int leftEndIndex = splitPoint - 1;
			
			if (leftEndIndex >= stIndex) {
				root.lr[0] = MakeFromPointsInner(depth + 1, stIndex, leftEndIndex, points, inds, dimensions, root.lr[0]);
			}
			else
			{
				root.lr[0] = null;
			}
			
			int rightStartIndex = splitPoint + 1;
			
			if (rightStartIndex <= enIndex) {
				root.lr[1] = MakeFromPointsInner(depth + 1, rightStartIndex, enIndex, points, inds, dimensions, root.lr[1]);
			}
			else
			{
				root.lr[1] = null;
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
		public static int FindPivotIndex(Vector3[] points, int[] inds, int stIndex, int enIndex, int axis) {
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
		
		
		public static int[] Iota(int num) {
			int[] result = new int[num];
			
			for (int i = 0; i < num; i++) {
				result[i] = i;
			}
			
			return result;
		}
/*! \endcond */		
		
		/// <summary>
		/// Find the nearest point in the set to the supplied point.
		/// </summary>
		public int FindNearest(Vector3 pt) {
			float bestSqDist = 1000000000f;
			int bestIndex = -1;
			
			Search(pt, ref bestSqDist, ref bestIndex);
			
			return bestIndex;
		}
		
		/// <summary>
		/// Find the nearest point in the set to the supplied point, up to the maximum square distance.
		/// </summary>
		public int FindNearest(Vector3 pt, float maxSqDist)
		{
			float bestSqDist = maxSqDist + Mathf.Epsilon;
			int bestIndex = -1;
			
			Search(pt, ref bestSqDist, ref bestIndex);
			
			return bestIndex;
		}
		
		/// <summary>
		/// Find the nearest points in the set up to the maximum amount, within the maximum square distance.
		/// </summary>
		public int[] FindNearest(Vector3 pt, int maxAmount, float maxSqDist)
		{
			float bestSqDist = maxSqDist + Mathf.Epsilon;
			int[] bestIndices = new int[maxAmount];
			for(int i = 0; i < maxAmount; i++)
			{
				bestIndices[i] = -1;
			}
			
			int ptr = 0;
			
			Search(pt, ref bestSqDist, ref bestIndices, ref ptr, maxAmount);
			
			return bestIndices;
		}
		
	
	//	Recursively search the tree.
		void Search(Vector3 pt, ref float bestSqSoFar, ref int bestIndex) {
			float mySqDist = (pivot - pt).sqrMagnitude;
			
			if (mySqDist < bestSqSoFar) {
				bestSqSoFar = mySqDist;
				bestIndex = pivotIndex;
			}
			
			float planeDist = pt[axis] - pivot[axis]; //DistFromSplitPlane(pt, pivot, axis);
			
			int selector = planeDist <= 0 ? 0 : 1;
			
			if (lr[selector] != null) {
				lr[selector].Search(pt, ref bestSqSoFar, ref bestIndex);
			}
			
			selector = (selector + 1) % 2;
			
			float sqPlaneDist = planeDist * planeDist;
	
			if ((lr[selector] != null) && (bestSqSoFar > sqPlaneDist)) {
				lr[selector].Search(pt, ref bestSqSoFar, ref bestIndex);
			}
		}
	
	//	Recursively search the tree for a specified amount of best values.
		void Search(Vector3 pt, ref float bestSqSoFar, ref int[] bestIndices, ref int ptr, int maxAmount) {
			float mySqDist = (pivot - pt).sqrMagnitude;
			
			if (mySqDist < bestSqSoFar) {
				bestSqSoFar = mySqDist;
				bestIndices[ptr] = pivotIndex;
				ptr++;
				if (ptr >= maxAmount)
				{
					ptr = 0;
				}
			}
			
			float planeDist = pt[axis] - pivot[axis]; //DistFromSplitPlane(pt, pivot, axis);
			
			int selector = planeDist <= 0 ? 0 : 1;
			
			if (lr[selector] != null) {
				lr[selector].Search(pt, ref bestSqSoFar, ref bestIndices, ref ptr, maxAmount);
			}
			
			selector = (selector + 1) % 2;
			
			float sqPlaneDist = planeDist * planeDist;
	
			if ((lr[selector] != null) && (bestSqSoFar > sqPlaneDist)) {
				lr[selector].Search(pt, ref bestSqSoFar, ref bestIndices, ref ptr, maxAmount);
			}
		}
		
	
	//	Get a point's distance from an axis-aligned plane.
		float DistFromSplitPlane(Vector3 pt, Vector3 planePt, int axis) {
			return pt[axis] - planePt[axis];
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
}
#endif