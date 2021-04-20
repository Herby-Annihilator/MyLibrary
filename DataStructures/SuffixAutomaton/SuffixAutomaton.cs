using System;
using System.Collections.Generic;
using System.Text;

namespace MyLibrary.DataStructures.SuffixAutomaton
{
	public class SuffixAutomaton
	{
		private Condition root;
		private Condition lastCondition;
		private int nodesCount;
		public bool IsBuilted { get; private set; }

		public SuffixAutomaton(string text) : this()
		{
			Build(text);
			IsBuilted = true;
		}
		public SuffixAutomaton()
		{
			IsBuilted = false;
			root = new Condition();
			lastCondition = root;
			nodesCount = 0;
		}

		public void Build(string text)
		{
			if (!IsBuilted)
			{
				if (text == null)
				{
					return;
				}
				IsBuilted = true;
				for (int i = 0; i < text.Length; i++)
				{
					Add(text[i]);
				}
			}
		}

		public void Add(char symbol)
		{
			IsBuilted = true;
			nodesCount += 1;
			Condition newCondition = new Condition(lastCondition.Lenght + 1, nodesCount);
			Condition currentCondition = lastCondition;
			while (currentCondition != null && !currentCondition.Transitions.Exists((trans) => { return trans.Symbol == symbol; }))
			{
				currentCondition.Transitions.Add(new Transition(symbol, newCondition));
				currentCondition = currentCondition.SuffixLink;
			}
			if (currentCondition == null)
			{
				newCondition.SuffixLink = root;
			}
			else
			{
				Condition existingCondition = currentCondition.Transitions.Find(trans => 
				{ return trans.Symbol == symbol; }).NextCondition;
				if (existingCondition.Lenght == currentCondition.Lenght + 1)
				{
					newCondition.SuffixLink = existingCondition;
				}
				else
				{
					nodesCount++;
					Condition clone = existingCondition.Clone(nodesCount);
					clone.Lenght = currentCondition.Lenght + 1;
					existingCondition.SuffixLink = clone;
					newCondition.SuffixLink = clone;
					while (currentCondition != null && currentCondition.Transitions.Exists(trans => { return trans.Symbol == symbol && 
						trans.NextCondition == existingCondition; }))
					{
						currentCondition.Transitions.Find(trans =>
						{
							return trans.Symbol == symbol && trans.NextCondition == existingCondition;
						}).NextCondition = clone;
						currentCondition = currentCondition.SuffixLink;
					}
				}
			}
			lastCondition = newCondition;
		}

		public char[][] GetMatrix()
		{
			char[][] toReturn = InitMatrix('0');
			Condition current = root;
			Queue<Condition> conditions = new Queue<Condition>(root.Transitions.Count);
			for (int i = 0; i < nodesCount + 1; i++)
			{
				for (int j = 0; j < current.Transitions.Count; j++)
				{
					if (!conditions.Contains(current.Transitions[j].NextCondition))
						conditions.Enqueue(current.Transitions[j].NextCondition);
					toReturn[current.ID][current.Transitions[j].NextCondition.ID] = current.Transitions[j].Symbol;
				}
				current = conditions.Dequeue();
			}
			return toReturn;
		}

		private T[][] InitMatrix<T>(T startValue = default(T))
		{
			T[][] toReturn = new T[nodesCount + 1][];   // because root
			for (int i = 0; i < nodesCount + 1; i++)
			{
				toReturn[i] = new T[nodesCount + 1];   // because root
			}
			for (int i = 0; i < nodesCount + 1; i++)
			{
				for (int j = 0; j < nodesCount + 1; j++)
				{
					toReturn[i][j] = startValue;
				}
			}
			return toReturn;
		}

		internal delegate void Act(Condition condition);

		public int[][] GetConditionsOfSuffixLinks()
		{
			int[][] toReturn = new int[nodesCount + 1][];
			for (int i = 0; i < nodesCount + 1; i++)
			{
				toReturn[i] = new int[2];
			}
			Condition current = root;
			Queue<Condition> conditions = new Queue<Condition>(current.Transitions.Count);
			for (int i = 0; i < nodesCount + 1; i++)
			{
				for (int j = 0; j < current.Transitions.Count; j++)
				{
					if (!conditions.Contains(current.Transitions[j].NextCondition))
						conditions.Enqueue(current.Transitions[j].NextCondition);
				}
				toReturn[i][0] = current.ID;
				toReturn[i][1] = current.SuffixLink?.ID ?? -1;
				current = conditions.Dequeue();
			}
			return toReturn;
		}

		internal void Traversal(Act act)
		{
			Condition current = root;
			Queue<Condition> conditions = new Queue<Condition>(current.Transitions.Count);
			for (int i = 0; i < nodesCount + 1; i++)
			{
				for (int j = 0; j < current.Transitions.Count; j++)
				{
					if (!conditions.Contains(current.Transitions[j].NextCondition))
						conditions.Enqueue(current.Transitions[j].NextCondition);
				}
				act(current);
				current = conditions.Dequeue();
			}
		}

		public int CountSubstringOccurences(string substring)
		{
			int conditionNumber = FindConditionForSubstring(substring);
			int occurencesCount;
			if (conditionNumber < 0)
			{
				occurencesCount = 0;
			}
			else
			{
				List<Condition> vertexes = GetVertexes();
				vertexes.Sort((cond1, cond2) =>
				{
					if (cond1.Lenght > cond2.Lenght)
					{
						return 1;
					}
					else if (cond2.Lenght > cond1.Lenght)
					{
						return -1;
					}
					else
					{
						return 0;
					}
				});
				InitVertexesCnt(vertexes);
				for (int i = 0; i < vertexes.Count; i++)
				{
					Condition current = vertexes[i];
					while (current.SuffixLink != null)
					{
						current.SuffixLink.Cnt += current.Cnt;
						current = current.SuffixLink;
					}
				}
				occurencesCount = vertexes.Find(cond => { return cond.ID == conditionNumber; }).Cnt;
				ClearVertexesCnt(vertexes);
			}
			return occurencesCount;
		}

		public int FindConditionForSubstring(string substring)
		{
			bool exist = true;
			Condition current = root;
			int i = 0;
			for ( ; i < substring.Length; i++)
			{
				if (current.Transitions.Exists(trans => { return  trans.Symbol == substring[i]; }))
				{
					current = current.Transitions.Find(trans => { return trans.Symbol == substring[i]; }).NextCondition;
				}
				else
				{
					exist = false;
					break;
				}
			}
			if (exist) return current.ID;
			else return -1;
		}

		private List<Condition> GetVertexes()
		{
			List<Condition> lst = new List<Condition>();
			Traversal(cond => { lst.Add(cond); });
			return lst;
		}

		private void InitVertexesCnt(List<Condition> vertexes)
		{
			for (int i = 0; i < vertexes.Count; i++)
			{
				if (vertexes[i].IsClone)
				{
					vertexes[i].Cnt = 0;
				}
				else
				{
					vertexes[i].Cnt = 1;
				}
			}
		}
		private void ClearVertexesCnt(List<Condition> vertexes)
		{
			for (int i = 0; i < vertexes.Count; i++)
			{
				vertexes[i].Cnt = 0;
			}
		}

		public void Destroy()
		{
			throw new NotImplementedException();
		}
	}
}
