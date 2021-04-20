using System;
using System.Collections.Generic;
using System.Text;

namespace MyLibrary.DataStructures.RedBlackTree
{
	[Serializable]
	public class Node<T>
	{
		public Node<T> LeftChild { get; internal set; }
		public Node<T> RightChild { get; internal set; }
		public Node<T> Parent { get; internal set; }
		public T Data { get; internal set; }
		public Color Color { get; internal set; }

		public Node(T data, Node<T> parent, Node<T> leftChild = null, Node<T> rightChild = null, Color color = Color.Red)
		{
			Data = data;
			LeftChild = leftChild;
			RightChild = rightChild;
			Parent = parent;
			Color = color;
		}
	}

	[Serializable]
	public enum Color
	{
		Red,
		Black,
	}
}
