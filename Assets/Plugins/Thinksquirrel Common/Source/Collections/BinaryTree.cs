// Binary tree collection class
// BinaryTree.cs
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
	/// Binary Tree class.
	// </summary>
	public class BinaryTree<TKey, TValue> where TKey: IComparable<TKey>
	{
	    private class Node
        {
            // node internal data
            internal int level;
            internal Node left;
            internal Node right;

            // user data
            internal TKey key;
            internal TValue value;

            // constuctor for the sentinel node
            internal Node()
            {
                this.level = 0;
                this.left = this;
                this.right = this;
            }

            // constuctor for regular nodes (that all start life as leaf nodes)
            internal Node(TKey key, TValue value, Node sentinel)
            {
                    this.level = 1;
                    this.left = sentinel;
                    this.right = sentinel;
                    this.key = key;
                    this.value = value;
            }
        }

        Node root;
        Node sentinel;
        Node deleted;

        public BinaryTree()
        {
            root = sentinel = new Node();
            deleted = null;
        }

        private void Skew(ref Node node)
        {
            if (node.level == node.left.level)
            {
                // rotate right
                Node left = node.left;
                node.left = left.right;
                left.right = node;
                node = left;
            }
        }

        private void Split(ref Node node)
        {
            if (node.right.right.level == node.level)
            {
                // rotate left
                Node right = node.right;
                node.right = right.left;
                right.left = node;
                node = right;
                node.level++;
            }
        }

        private bool Insert(ref Node node, TKey key, TValue value)
        {
            if (node == sentinel)
            {
                node = new Node(key, value, sentinel);
                return true;
            }

            int compare = key.CompareTo(node.key);
            if (compare < 0)
            {
                if (!Insert(ref node.left, key, value))
                {
                        return false;
                }
            }
            else if (compare > 0)
            {
                if (!Insert(ref node.right, key, value))
                {
                        return false;
                }
            }
            else
            {
                return false;
            }

            Skew(ref node);
            Split(ref node);

            return true;
        }

        private bool Delete(ref Node node, TKey key)
        {
            if (node == sentinel)
            {
                return (deleted != null);
            }

            int compare = key.CompareTo(node.key);
            if (compare < 0)
            {
                if (!Delete(ref node.left, key))
                {
                        return false;
                }
            }
            else
            {
                if (compare == 0)
                {
                        deleted = node;
                }
                if (!Delete(ref node.right, key))
                {
                        return false;
                }
            }

            if (deleted != null)
            {
                deleted.key = node.key;
                deleted.value = node.value;
                deleted = null;
                node = node.right;
            }
            else if (node.left.level < node.level - 1
                    || node.right.level < node.level - 1)
            {
                --node.level;
                if (node.right.level > node.level)
                {
                        node.right.level = node.level;
                }
                Skew(ref node);
                Skew(ref node.right);
                Skew(ref node.right.right);
                Split(ref node);
                Split(ref node.right);
            }

            return true;
        }

        private Node Search(Node node, TKey key)
        {
            if (node == sentinel)
            {
                return null;
            }

            int compare = key.CompareTo(node.key);
            if (compare < 0)
            {
                return Search(node.left, key);
            }
            else if (compare > 0)
            {
                return Search(node.right, key);
            }
            else
            {
                return node;
            }
        }

        public bool Add(TKey key, TValue value)
        {
            return Insert(ref root, key, value);
        }

        public bool Remove(TKey key)
        {
            return Delete(ref root, key);
        }

		public bool ContainsKey(TKey key)
		{
			return Search(root, key) != null;
		}
		
        public TValue this[TKey key]
        {
            get
            {
                Node node = Search(root, key);
                return node == null ? default(TValue) : node.value;
      		}
            set
            {
                Node node = Search(root, key);
                if (node == null)
                {
                    Add(key, value);
                }
                else
                {
                    node.value = value;
                }
            }
        }
	}
}
#endif