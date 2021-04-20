using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace MyLibrary.InputOutput
{
	public interface IReader<T>
	{
		T ReadFromFile(StreamReader reader);
	}
}
