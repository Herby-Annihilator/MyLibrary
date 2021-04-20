using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace MyLibrary.InputOutput
{
	public interface IWriter
	{
		void WriteToFile(string fileName);

		void WriteToFile(TextWriter writer);
	}
}
