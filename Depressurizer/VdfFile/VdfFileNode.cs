﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Depressurizer.VdfFile
{
    public enum ValueType
    {
        Array,
        String,
        Int
    }

    public abstract class VdfFileNode
    {
        // Can be an int, string or Dictionary<string,VdfFileNode>
        public Object NodeData;
        public ValueType NodeType;

        /// <summary>
        ///     Creates a new array-type node
        /// </summary>
        protected VdfFileNode()
        {
            NodeType = ValueType.Array;
            NodeData = new Dictionary<string, VdfFileNode>(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        ///     Creates a new string-type node
        /// </summary>
        /// <param name="value">Value of the string</param>
        protected VdfFileNode(string value)
        {
            NodeType = ValueType.String;
            NodeData = value;
        }

        /// <summary>
        ///     Creates a new integer-type node
        /// </summary>
        /// <param name="value">Value of the integer</param>
        protected VdfFileNode(int value)
        {
            NodeType = ValueType.Int;
            NodeData = value;
        }

        #region Utility

        /// <summary>
        ///     Checks whether or not this node has any children
        /// </summary>
        /// <returns>True if an array with no children, false otherwise</returns>
        protected bool IsEmpty()
        {
            if (NodeArray != null)
            {
                return NodeArray.Count == 0;
            }
            return (NodeData as string) == null;
        }

        #endregion

        #region Accessors

        /// <summary>
        ///     Gets the node at the given address. May be used to build structure.
        /// </summary>
        /// <param name="args">An ordered list of keys, like a path</param>
        /// <param name="create">If true, will create any nodes it does not find along the path.</param>
        /// <param name="index">Start index of the arg array</param>
        /// <returns>The FileNode at the given location, or null if the location was not found / created</returns>
        public VdfFileNode GetNodeAt(string[] args, bool create = true, int index = 0)
        {
            if (index >= args.Length)
            {
                return this;
            }
            if (NodeType == ValueType.Array)
            {
                var data = (Dictionary<String, VdfFileNode>) NodeData;
                if (ContainsKey(args[index]))
                {
                    return data[args[index]].GetNodeAt(args, create, index + 1);
                }
                if (create)
                {
                    VdfFileNode newNode = CreateNode();
                    data.Add(args[index], newNode);
                    return newNode.GetNodeAt(args, true, index + 1);
                }
            }
            return null;
        }

        /// <summary>
        ///     Checks whether the given key exists within an array-type node
        /// </summary>
        /// <param name="key">The key to look for</param>
        /// <returns>True if the key was found, false otherwise</returns>
        public bool ContainsKey(string key)
        {
            if (NodeType != ValueType.Array)
            {
                return false;
            }
            return ((Dictionary<string, VdfFileNode>) NodeData).ContainsKey(key);
        }

        #endregion

        #region Modifiers

        /// <summary>
        ///     Removes the subnode with the given key. Can only be called on array nodes.
        /// </summary>
        /// <param name="key">Key of the subnode to remove</param>
        /// <returns>True if node was removed, false if not found</returns>
        public bool RemoveSubnode(string key)
        {
            if (NodeType != ValueType.Array)
            {
                return false;
            }
            return NodeArray.Remove(key);
        }

        /// <summary>
        ///     Removes any array nodes without any value-type children
        /// </summary>
        public void CleanTree()
        {
            Dictionary<string, VdfFileNode> nodes = NodeArray;
            if (nodes != null)
            {
                string[] keys = nodes.Keys.ToArray();
                foreach (string key in keys)
                {
                    nodes[key].CleanTree();
                    if (nodes[key].IsEmpty())
                    {
                        NodeArray.Remove(key);
                    }
                }
            }
        }

        public void MakeArray()
        {
            if (NodeType != ValueType.Array)
            {
                NodeType = ValueType.Array;
                NodeData = new Dictionary<string, VdfFileNode>(StringComparer.OrdinalIgnoreCase);
            }
        }

        #endregion

        /// <summary>
        ///     Gets or sets the subnode with the given key. Can only be used with an array node. If the subnode does not exist,
        ///     creates it as an array type.
        /// </summary>
        /// <param name="key">Key to look for or set</param>
        /// <returns></returns>
        /// <exception cref="ApplicationException">Thrown if used on a value node.</exception>
        public VdfFileNode this[string key]
        {
            get
            {
                if (NodeType != ValueType.Array)
                {
                    throw new ApplicationException(string.Format(GlobalStrings.TextVdfFile_CanNotGetKey, key));
                }
                var arrayData = (Dictionary<string, VdfFileNode>) NodeData;
                if (!arrayData.ContainsKey(key))
                {
                    arrayData.Add(key, CreateNode());
                }
                return arrayData[key];
            }
            set
            {
                if (NodeType == ValueType.String)
                {
                    throw new ApplicationException(string.Format(GlobalStrings.TextVdfFile_CanNotSetKey, key));
                }
                var arrayData = (Dictionary<string, VdfFileNode>) NodeData;
                if (!arrayData.ContainsKey(key))
                {
                    arrayData.Add(key, value);
                }
                else
                {
                    arrayData[key] = value;
                }
            }
        }

        /// <summary>
        ///     Quick shortcut for casting data to a a dictionary
        /// </summary>
        public Dictionary<string, VdfFileNode> NodeArray
        {
            get { return (NodeType == ValueType.Array) ? (NodeData as Dictionary<string, VdfFileNode>) : null; }
        }

        /// <summary>
        ///     Quick shortcut for casting data to string
        /// </summary>
        public string NodeString
        {
            get { return (NodeType == ValueType.String) ? (NodeData as string) : null; }
        }

        /// <summary>
        ///     Quick shortcut for casting data to int
        /// </summary>
        public int NodeInt
        {
            get { return (NodeType == ValueType.Int) ? ((int) NodeData) : 0; }
        }

        /// <summary>
        ///     Create an object of child class. To be implemented by inherited classes.
        /// </summary>
        /// <returns></returns>
        protected abstract VdfFileNode CreateNode();
    }
}