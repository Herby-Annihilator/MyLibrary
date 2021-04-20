using System;
using System.Collections.Generic;
using System.Text;


// Spizgeno from https://github.com/eugenp/tutorials/blob/e9366673f786304af9674bfcac80ee91ba0886c2/algorithms-searching/src/main/java/com/baeldung/algorithms/suffixtree/SuffixTree.java#L124


namespace MyLibrary.DataStructures.SuffixTree
{
	public class SuffixTree
	{
		private const string WORD_TERMINATION = "$";
		private const int POISITION_UNDEFINED = -1;
		private TreeNode root;
		private string fullText;

		public SuffixTree(string sourceText)
		{
			fullText = sourceText;
			root = new TreeNode("", POISITION_UNDEFINED);
			for (int i = 0; i < fullText.Length; i++)
			{
				AddSuffix(fullText.Substring(i) + WORD_TERMINATION, i);
			}
		}

		private void AddSuffix(string suffix, int position)
		{
            List<TreeNode> nodes = GetAllNodesInTraversePath(suffix, root, true);
            if (nodes.Count == 0)
            {
                AddChild(root, suffix, position);
            }
            else
            {
                TreeNode lastNode = nodes[nodes.Count - 1];
                nodes.RemoveAt(nodes.Count - 1);
                string newText = suffix;
                if (nodes.Count > 0)
                {
                    string existingSuffixUptoLastNode = "";
                    foreach(var node in nodes)
					{
                        existingSuffixUptoLastNode += node.Text;
					}
                    newText = newText.Substring(existingSuffixUptoLastNode.Length);
                }
                ExtendNode(lastNode, newText, position);
            }
        }

        private List<TreeNode> GetAllNodesInTraversePath(string pattern, TreeNode startNode, bool isAllowPartialMatch)
        {
            List<TreeNode> nodes = new List<TreeNode>();
            for (int i = 0; i < startNode.Children.Count; i++)
            {
                TreeNode currentNode = startNode.Children[i];
                string nodeText = currentNode.Text;
                if (pattern[0] == nodeText[0])
                {
                    if (isAllowPartialMatch && pattern.Length <= nodeText.Length)
                    {
                        nodes.Add(currentNode);
                        return nodes;
                    }

                    int compareLength = Math.Min(nodeText.Length, pattern.Length);
                    for (int j = 1; j < compareLength; j++)
                    {
                        if (pattern[j] != nodeText[j])
                        {
                            if (isAllowPartialMatch)
                            {
                                nodes.Add(currentNode);
                            }
                            return nodes;
                        }
                    }

                    nodes.Add(currentNode);
                    if (pattern.Length > compareLength)
                    {
                        List<TreeNode> nodes2 = GetAllNodesInTraversePath(pattern.Substring(compareLength), currentNode, isAllowPartialMatch);
                        if (nodes2.Count > 0)
                        {
                            nodes.AddRange(nodes2);
                        }
                        else if (!isAllowPartialMatch)
                        {
                            nodes.Add(null);   // ?????
                        }
                    }
                    return nodes;
                }
            }
            return nodes;
        }
        
        private void AddChild(TreeNode parent, string text, int position)
		{
            parent.Children.Add(new TreeNode(text, position));
		}
        private void ExtendNode(TreeNode node, string text, int position)
		{
            string currentText = node.Text;
            string commonPrefix = GetLongestCommonPrefix(currentText, text);
            if (commonPrefix != currentText)
			{
                string parentText = currentText.Substring(0, commonPrefix.Length);
                string childText = currentText.Substring(commonPrefix.Length);
                SplitNodeToParentAndChild(node, parentText, childText);
			}
            string remainingText = text.Substring(commonPrefix.Length);
            AddChild(node, remainingText, position);
		}
        private string GetLongestCommonPrefix(string str1, string str2)
		{
            int minLength = Math.Min(str1.Length, str2.Length);
            for (int i = 0; i < minLength; i++)
			{
                if (str1[i] != str2[i])
				{
                    return str1.Substring(0, i);
				}
			}
            return str1.Substring(0, minLength);
		}
        private void SplitNodeToParentAndChild(TreeNode parent, string parentNewText, string childNewText)
		{
            TreeNode child = new TreeNode(childNewText, parent.Position);
            while(parent.Children.Count > 0)
			{
                child.Children.Add(parent.Children[0]);  // косяк может быть
                parent.Children.RemoveAt(0);
			}
            parent.Children.Add(child);
            parent.Text = parentNewText;
            parent.Position = POISITION_UNDEFINED;
		}
        private List<int> GetPositions(TreeNode node)
        {
            List<int> positions = new List<int>();
            if (node.Text.EndsWith(WORD_TERMINATION))
            {
                positions.Add(node.Position);
            }
            for (int i = 0; i < node.Children.Count; i++)
            {
                positions.AddRange(GetPositions(node.Children[i]));
            }
            return positions;
        }
        public List<int> SearchTextIntersections(string text)
		{
            List<int> intersections = new List<int>();
            List<TreeNode> nodes = GetAllNodesInTraversePath(text, root, false);
            if (nodes.Count > 0)
			{
                TreeNode lastNode = nodes[nodes.Count - 1];
                if (lastNode != null)
				{
                    intersections = GetPositions(lastNode);
				}
			}
            return intersections;
		}
    }
}
