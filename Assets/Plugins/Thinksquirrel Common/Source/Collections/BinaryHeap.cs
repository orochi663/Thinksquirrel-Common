// Binary heap collection class
// BinaryHeap.cs
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
#if !COMPACT
using System;
using System.Collections;
using System.Collections.Generic;

namespace ThinksquirrelSoftware.Common.Collections
{
	/// <summary>
	/// Binary Heap class.
	// </summary>
	public class BinaryHeap<TKey, TValue> : IEnumerable<TValue> where TKey : IComparable<TKey>
	{
		private List<TKey> mKeys;
	    private List<TValue> mItems;
		private HashSet<TValue> mHash;
		private bool mUseHashSet;
		
		public int Capacity
		{
			get
			{
				return mKeys.Capacity;
			}
			set
			{
				if (mKeys.Capacity != value)
				{
					mKeys.Capacity = value;
					mItems.Capacity = value;
				}
			}
		}
		
		public BinaryHeap(int capacity)
		{
			mKeys = new List<TKey>(capacity);
			mItems = new List<TValue>(capacity);
		}
		
		public BinaryHeap(bool useHashSet) : this(1, useHashSet)
		{
		}
		
		public BinaryHeap(IEqualityComparer<TValue> comparer) : this(1, comparer)
		{
		}
		
		public BinaryHeap(int capacity, bool useHashSet) : this(capacity)
		{
			mUseHashSet = useHashSet;
			
			if (mUseHashSet)
			{
				mHash = new HashSet<TValue>();
			}
		}
		
		public BinaryHeap(int capacity, IEqualityComparer<TValue> comparer) : this(capacity)
		{
			mUseHashSet = true;
			mHash = new HashSet<TValue>(comparer);
		}
	
	    /// <summary>
	    /// Get a count of the number of items in the collection.
	    /// </summary>
	    public int Count
	    {
	        get { return mItems.Count; }
	    }
	
	    /// <summary>
	    /// Removes all items from the collection.
	    /// </summary>
	    public void Clear()
	    {
			mKeys.Clear();
	        mItems.Clear();
			if (mUseHashSet) mHash.Clear();
	    }
	
	    /// <summary>
	    /// Sets the capacity to the actual number of elements in the BinaryHeap,
	    /// if that number is less than a threshold value.
	    /// </summary>
	    /// <remarks>
	    /// The current threshold value is 90% (.NET 3.5), but might change in a future release.
	    /// </remarks>
	    public void TrimExcess()
	    {
			mKeys.TrimExcess();
	        mItems.TrimExcess();
			if (mUseHashSet) mHash.TrimExcess();
	    }
	
	    /// <summary>
	    /// Inserts an item onto the heap.
	    /// </summary>
	    /// <param name="newItem">The item to be inserted.</param>
	    public void Insert(TKey key, TValue newItem)
	    {
			if (Count == Capacity)
				throw new InvalidOperationException("BinaryHeap: Capacity is full");
			
	        int i = Count;
			mKeys.Add(key);
	        mItems.Add(newItem);
			if (mUseHashSet) mHash.Add(newItem);
	        while (i > 0 && mKeys[((i - 1) / 2)].CompareTo(key) > 0)
	        {
				mKeys[i] = mKeys[mKeys.Count - 1 - ((i - 1) / 2)];
	            mItems[i] = mItems[mKeys.Count - 1 - ((i - 1) / 2)];
	            i = (i - 1) / 2;
	        }
			mKeys[i] = key;
	        mItems[i] = newItem;
	    }
		
	    /// <summary>
	    /// Return the root item from the collection, without removing it.
	    /// </summary>
	    /// <returns>Returns the item at the root of the heap.</returns>
	    public TValue Peek()
	    {
	        if (mItems.Count == 0)
	        {
	            throw new InvalidOperationException("The heap is empty.");
	        }
	        return mItems[0];
	    }
		
		public TKey MaxPriority()
		{
			if (mKeys.Count == 0)
	        {
	            throw new InvalidOperationException("The heap is empty.");
	        }
	        return mKeys[0];
		}

	    /// <summary>
	    /// Removes and returns the root item from the collection.
	    /// </summary>
	    /// <returns>Returns the item at the root of the heap.</returns>
	    public TValue RemoveRoot()
	    {
	        if (mItems.Count == 0)
	        {
	            throw new InvalidOperationException("The heap is empty.");
	        }
	        // Get the first item
	        TValue rslt = mItems[0];
	        // Get the last item and bubble it down.
			TKey tmpKey = mKeys[mItems.Count - 1];
	        TValue tmp = mItems[mItems.Count - 1];
	
			mKeys.RemoveAt(mItems.Count - 1);
	        mItems.RemoveAt(mItems.Count - 1);
			if (mUseHashSet) mHash.Remove(rslt);	
	        if (mItems.Count > 0)
	        {
	            int i = 0;
	            while (i < mItems.Count / 2)
	            {
	                int j = (2 * i) + 1;
	                if ((j < mItems.Count - 1) && (mKeys[j].CompareTo(mKeys[j + 1]) > 0))
	                {
	                    ++j;
	                }
	                if (mKeys[j].CompareTo(tmpKey) >= 0)
	                {
	                    break;
	                }
					mKeys[i] = mKeys[j];
	                mItems[i] = mItems[j];
	                i = j;
	            }
				mKeys[i] = tmpKey;
	            mItems[i] = tmp;
	        }
	        return rslt;
	    }
		
		public bool Contains(TValue item)
		{
			if (!mUseHashSet)
				throw new InvalidOperationException("Cannot do a Contains() operation without a backing hash set!");
			
			return mHash.Contains(item);
		}
		
	    IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator()
	    {
	        foreach (var i in mItems)
	        {
	            yield return i;
	        }
	    }

	    public IEnumerator GetEnumerator()
	    {
	        return GetEnumerator();
	    }
	}
}
#endif