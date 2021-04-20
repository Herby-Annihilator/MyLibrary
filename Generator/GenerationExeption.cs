using System;
using System.Collections.Generic;
using System.Text;

namespace MyLibrary.Generator
{
	public class GenerationExeption : Exception
	{
		public GenerationExeption() : base("Generation error")
		{

		}

		public GenerationExeption(string message) : base(message)
		{

		}
	}
}
