using System;
using System.Collections.Generic;
using System.Text;

namespace MyLibrary.DataStructures.SuffixAutomaton
{
	internal class Transition
	{
		internal char Symbol { get; set; }
		internal Condition NextCondition { get; set; }

		internal Transition(char symbol, Condition nextCondition)
		{
			Symbol = symbol;
			NextCondition = nextCondition;
		}
	}
}
