using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace MyLibrary.InputOutput
{
	/// <summary>
	/// You MUST catch the exceptions
	/// </summary>
	public class InputOutput
	{
		public static void SaveToFile(string fileName, object obj)
		{
			FileStream stream = new FileStream(fileName, FileMode.Create);
			BinaryFormatter formatter = new BinaryFormatter();
			formatter.Serialize(stream, obj);
			stream.Close();
		}

		public static object LoadFromFile(string fileName)
		{
			object toReturn;
			FileStream stream = new FileStream(fileName, FileMode.Open);
			BinaryFormatter formatter = new BinaryFormatter();
			toReturn = formatter.Deserialize(stream);
			stream.Close();
			return toReturn;
		}
	}
}
