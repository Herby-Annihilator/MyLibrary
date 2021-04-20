using System;
using System.Collections.Generic;
using System.Text;

namespace MyLibrary.DataStructures.RedBlackTree
{
	[Serializable]
	public class RedBlackTree<T>
	{
		private Node<T> root;

		public RedBlackTree()
		{
			root = null;
		}

		public void AddData(T data, IComparer<T> comparer)
		{
			if (root == null)
			{
				root = new Node<T>(data, null, null, null, Color.Black);
				return;
			}
			Node<T> currentNode = root;
			Node<T> nextNode = root;
			while (nextNode != null)
			{
				currentNode = nextNode;
				if (comparer.Compare(data, nextNode.Data) < 0)
				{
					nextNode = nextNode.LeftChild;
				}
				else
				{
					nextNode = nextNode.RightChild;
				}
			}
			nextNode = new Node<T>(data, currentNode);
			if (comparer.Compare(data, currentNode.Data) < 0)
			{
				currentNode.LeftChild = nextNode;
			}
			else
			{
				currentNode.RightChild = nextNode;
			}
			RestorePropertiesAfterAdding(nextNode);
		}

		private void RestorePropertiesAfterAdding(Node<T> defectiveNode)
		{
			if (defectiveNode == root)
			{
				defectiveNode.Color = Color.Black;
			}
			else
			{
				Node<T> currentNode = defectiveNode;
				Node<T> grandFather;
				Node<T> uncle;
				while (currentNode.Parent != null && currentNode.Parent.Color == Color.Red)
				{
					grandFather = currentNode.Parent.Parent;
					if (currentNode.Parent == grandFather.LeftChild)
					{
						uncle = GetUncle(currentNode);
						if (uncle != null)
						{
							if (uncle.Color == Color.Red)
							{
								uncle.Color = Color.Black;
								currentNode.Parent.Color = Color.Black;
								grandFather.Color = Color.Red;
								currentNode = grandFather;
								continue;
							}
						}
						if (uncle == null || uncle.Color == Color.Black)
						{
							if (currentNode == currentNode.Parent.RightChild)
							{
								currentNode = currentNode.Parent;
								LeftRotation(currentNode);
							}
							currentNode.Parent.Color = Color.Black;
							grandFather = currentNode.Parent.Parent;
							grandFather.Color = Color.Red;
							RightRotation(grandFather);
						}
					}
					else
					{
						uncle = GetUncle(currentNode);
						if (uncle != null)
						{
							if (uncle.Color == Color.Red)
							{
								uncle.Color = Color.Black;
								currentNode.Parent.Color = Color.Black;
								grandFather.Color = Color.Red;
								currentNode = grandFather;
								continue;
							}
						}
						if (uncle == null || uncle.Color == Color.Black)
						{
							if (currentNode == currentNode.Parent.LeftChild)
							{
								currentNode = currentNode.Parent;
								RightRotation(currentNode);
							}
							currentNode.Parent.Color = Color.Black;
							grandFather = currentNode.Parent.Parent;
							grandFather.Color = Color.Red;
							LeftRotation(grandFather);
						}
					}
				}
			}
			root.Color = Color.Black;
		}

		private Node<T> GetUncle(Node<T> nephew)
		{
			Node<T> father;
			Node<T> grandFather;
			Node<T> uncle = null;
			father = nephew.Parent;
			if (father != null)
			{
				grandFather = father.Parent;
				if (grandFather != null)
				{
					if (grandFather.LeftChild == father)
					{
						uncle = grandFather.RightChild;
					}
					else
					{
						uncle = grandFather.LeftChild;
					}
				}
			}
			return uncle;
		}

		private void RightRotation(Node<T> node)
		{
			if (node == null)
			{
				return;
			}
			Node<T> tmp = node.LeftChild;
			Node<T> parent = node.Parent;
			if (parent != null)
			{
				if (node == parent.LeftChild)
				{
					parent.LeftChild = tmp;
				}
				else
				{
					parent.RightChild = tmp;
				}
			}
			else
			{
				root = tmp;
			}
			node.LeftChild = tmp.RightChild;
			tmp.RightChild = node;
			node.Parent = tmp;
			tmp.Parent = parent;
		}

		private void LeftRotation(Node<T> node)
		{
			if (node == null)
			{
				return;
			}
			Node<T> tmp = node.RightChild;
			Node<T> parent = node.Parent;
			if (parent != null)
			{
				if (node == parent.LeftChild)
				{
					parent.LeftChild = tmp;
				}
				else
				{
					parent.RightChild = tmp;
				}
			}
			else
			{
				root = tmp;
			}
			node.RightChild = tmp.LeftChild;
			tmp.LeftChild = node;
			node.Parent = tmp;
			tmp.Parent = parent;
		}

		public void TreeTraversal(Action<Node<T>> action)
		{
			ReverseTraversal(root, action);
		}
		private void ReverseTraversal(Node<T> currentNode, Action<Node<T>> action)
		{
			if (currentNode == null)
			{
				return;
			}
			ReverseTraversal(currentNode.LeftChild, action);			
			ReverseTraversal(currentNode.RightChild, action);
			action.Invoke(currentNode);
		}

		public Node<T> Search(T dataToSearch, IComparer<T> comparer)
		{
			Node<T> currentNode = root;
			while (currentNode != null)
			{
				if (comparer.Compare(dataToSearch, currentNode.Data) > 0)
				{
					currentNode = currentNode.RightChild;
				}
				else if (comparer.Compare(dataToSearch, currentNode.Data) < 0)
				{
					currentNode = currentNode.LeftChild;
				}
				else
				{
					break;
				}
			}
			return currentNode;
		}
		
		public void DeleteData(T data, IComparer<T> comparer)
		{
			throw new NotImplementedException("DeleteData method is not implemented!");
		}

		public void Clear()
		{
			ReverseTraversal(root, (node) => { node.Parent = null;
				node.LeftChild = null;
				node.RightChild = null;
			});
			root = null;
		}

		public bool IsEmpty()
		{
			if (root == null)
				return true;
			else
				return false;
		}

		public bool IsRoot(Node<T> node)
		{
			return node == root;
		}
	}
}
