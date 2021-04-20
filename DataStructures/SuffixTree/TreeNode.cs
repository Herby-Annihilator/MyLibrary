using System;
using System.Collections.Generic;
using System.Text;

namespace MyLibrary.DataStructures.SuffixTree
{
	internal class TreeNode
	{
		internal string Text { get; set; }
		internal List<TreeNode> Children { get; set; }
		internal int Position { get; set; }

		internal TreeNode(string text, int position)
		{
			Text = text;
			Position = position;
			Children = new List<TreeNode>();
		}
	}
}
