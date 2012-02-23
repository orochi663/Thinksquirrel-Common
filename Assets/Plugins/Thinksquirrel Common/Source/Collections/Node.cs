// Graph node collection classes
// Node.cs
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
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace ThinksquirrelSoftware.Common.Collections
{
	public class Node<T>
	{
		private bool mEnabled = true;
		private NodeList<T> mNeighbors;
		private List<float> mCosts;
		private T mValue;
		
		public bool Enabled
		{
			get
			{
				return mEnabled;
			}
		}
		public NodeList<T> Neighbors
		{
			get
			{
				if (mNeighbors == null)
					mNeighbors = new NodeList<T>();
					
				return mNeighbors;
			}
		}
		
		public List<float> Costs
		{
			get
			{
				if (mCosts == null)
					mCosts = new List<float>();
					
				return mCosts;
			}
		}
		public T Value
		{
			get
			{
				return mValue;
			}
			set
			{
				mValue = value;
			}
		}
		
		public Node() {}
		public Node(T value) : this(value, null, true) {}
		public Node(T value, bool enabled) : this(value, null, enabled) {}
		public Node(T value, NodeList<T> neighbors) : this(value, neighbors, true) {}
		public Node(T value, NodeList<T> neighbors, bool enabled)
		{
			this.mValue = value;
			this.mNeighbors = neighbors;
			this.mEnabled = enabled;
		}
		
		public void Toggle(bool toggle)
		{
			mEnabled = toggle;
		}
		
	}
	
	public class NodeList<T> : List<Node<T>>
	{
	    public NodeList() : base() { }

	    public NodeList(int initialSize)
	    {
	        for (int i = 0; i < initialSize; i++)
	            base.Add(default(Node<T>));
	    }

	    public Node<T> FindByValue(T value)
	    {
	        foreach (Node<T> node in this)
	            if (node.Value.Equals(value))
	                return node;

	        return null;
	    }
	}
	
	public class Graph<T> : IEnumerable<T>
	{
		private bool mChanged;
		
		public bool Changed
		{
			get
			{
				return mChanged;
			}
		}
		public delegate void GraphChangeHandler();
		public event GraphChangeHandler GraphChange;
		
		public void PushGraphChange()
		{
			if (mChanged)
			{
				if (GraphChange != null)
				{
					GraphChange();
				}
				mChanged = false;
			}
		}
		
	    private NodeList<T> nodeSet;

	    public Graph() : this(null) {}
	    public Graph(NodeList<T> nodeSet)
	    {
	        if (nodeSet == null)
	            this.nodeSet = new NodeList<T>();
	        else
	            this.nodeSet = nodeSet;
	    }

	    public Node<T> AddNode(Node<T> node)
	    {
	        nodeSet.Add(node);
			mChanged = true;
			return node;
	    }

	    public Node<T> AddNode(T value)
	    {
			Node<T> node = new Node<T>(value);
	        nodeSet.Add(node);
			mChanged = true;
			return node;
	    }
		
		
		public void ToggleNode(Node<T> node)
		{
			node.Toggle(!node.Enabled);
			mChanged = true;
		}
		
		public void ToggleNode(Node<T> node, bool enabled)
		{
			if (node.Enabled != enabled)
			{
				node.Toggle(enabled);
				mChanged = true;
			}
		}

	    public bool AddDirectedEdge(Node<T> from, Node<T> to, float cost)
	    {
			if (!from.Neighbors.Contains(to))
			{
	        	from.Neighbors.Add(to);
				from.Costs.Add(cost);
				mChanged = true;
				return true;
			}
			
			return false;
	    }

	    public bool AddUndirectedEdge(Node<T> from, Node<T> to, float cost)
	    {
			if (from == null || to == null)
				return false;
				
			if (!from.Neighbors.Contains(to) && !to.Neighbors.Contains(from))
			{
	        	from.Neighbors.Add(to);
	        	from.Costs.Add(cost);
	
	        	to.Neighbors.Add(from);
	        	to.Costs.Add(cost);
				mChanged = true;
				return true;
			}
			
			return false;
	    }

	    public bool Contains(T value)
	    {
	        return nodeSet.FindByValue(value) != null;
	    }
	
		public Node<T> Find(T value)
		{
			return nodeSet.FindByValue(value);
		}

	    public bool Remove(T value)
	    {
	        Node<T> nodeToRemove = (Node<T>) nodeSet.FindByValue(value);
	        
			if (nodeToRemove == null)
	            return false;
			
	        nodeSet.Remove(nodeToRemove);

	        foreach (Node<T> node in nodeSet)
	        {
	            int index = node.Neighbors.IndexOf(nodeToRemove);
	
	            if (index != -1)
	            {
	                node.Neighbors.RemoveAt(index);
	                node.Costs.RemoveAt(index);
	            }
	        }
			
			mChanged = true;
	        return true;
	    }

	    public NodeList<T> Nodes
	    {
	        get
	        {
	            return nodeSet;
	        }
	    }
	
	    public Node<T> this[int i]
	    {
	        get
	        {
	            return nodeSet[i];
	        }
	        set
	        {
	            nodeSet[i] = value;
	        }
	    }
	
	    public int Count
	    {
	        get { return nodeSet.Count; }
	    }
	
		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
		    return (IEnumerator<T>)nodeSet;			  
		}
		
		IEnumerator IEnumerable.GetEnumerator()
		{
		    return nodeSet.GetEnumerator();			  
		}
	}
}