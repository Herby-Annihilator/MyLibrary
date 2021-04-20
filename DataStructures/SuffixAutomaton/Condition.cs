using System;
using System.Collections.Generic;
using System.Text;

namespace MyLibrary.DataStructures.SuffixAutomaton
{
	internal class Condition
	{
		internal int ID;
		internal int Lenght { get; set; }
		internal Condition SuffixLink { get; set; }
		internal List<Transition> Transitions { get; set; }
		internal bool IsClone { get; private set; }
		internal int Cnt { get; set; }

		internal Condition(int lenght = default(int), int id = default(int), Condition suffixLink = null)
		{
			ID = id;
			Transitions = new List<Transition>();
			Lenght = lenght;
			SuffixLink = suffixLink;
			IsClone = false;
			Cnt = 0;
		}

		internal Condition Clone(int newID)
		{
			Condition toReturn = new Condition(Lenght, newID, SuffixLink)
			{
				Transitions = new List<Transition>(Transitions), 
				IsClone = true
			};
			return toReturn;
		}
	}
}
