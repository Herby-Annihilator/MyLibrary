using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using MyLibrary.Generator;
using MyLibrary.Extensions.StringExtensions;
using MyLibrary.InputOutput;

namespace MyLibrary.UserTypes
{
	public class Node : IGeneratable, IWriter, IReader<Node>
	{
		public string FirstField { get; set; }
		public string SecondField { get; set; }

		public int ThirdField { get; set; }
		public int FourthField { get; set; }
		public int FifthField { get; set; }

		public void Randomize()
		{
			Random random = new Random();
			ThirdField = random.Next(-50, 100);
			FourthField = random.Next(-50, 100);
			FifthField = random.Next(-50, 100);
			FirstField = StringExtension.CreateRandomString(FirstField, random.Next(1, GlobalConstants.STRING_MAX_LENGHT));
			SecondField = StringExtension.CreateRandomString(FirstField, random.Next(1, GlobalConstants.STRING_MAX_LENGHT));
		}

		public void WriteToFile(string fileName)
		{
			StreamWriter writer = new StreamWriter(fileName, true);
			writer.WriteLine("$***************************************\r\n\r\n");
			writer.WriteLine("FirstField (string): " + FirstField);
			writer.WriteLine("SecondField (string): " + SecondField);
			writer.WriteLine("ThirdField (number): " + ThirdField);
			writer.WriteLine("FourthField (number): " + FourthField);
			writer.WriteLine("FifthField (number): " + FifthField);
			writer.WriteLine("\r\n\r\n\r\n***************************************");
			writer.Close();
		}

		public void WriteToFile(TextWriter writer)
		{
			writer.WriteLine();
			writer.WriteLine();
			writer.WriteLine("$***************************************");
			writer.WriteLine();
			writer.WriteLine("FirstField (string): " + FirstField);
			writer.WriteLine("SecondField (string): " + SecondField);
			writer.WriteLine("ThirdField (number): " + ThirdField);
			writer.WriteLine("FourthField (number): " + FourthField);
			writer.WriteLine("FifthField (number): " + FifthField);
			writer.WriteLine();
			writer.WriteLine();
			writer.WriteLine("***************************************");
		}

		public static Node[] ReadFromFile(string fileName)
		{
			List<Node> nodes = new List<Node>();
			StreamReader reader = new StreamReader(fileName);
			nodes.Add(ReadNodeFromFile(reader));
			reader.Close();
			return nodes.ToArray();
		}

		public static Node ReadNodeFromFile(StreamReader reader)
		{
			Node toReturn = null;
			string buffer;
			while ((buffer = reader.ReadLine()) != null)
			{
				if (buffer != "" && buffer[0] == '$')
				{
					toReturn = new Node();
					string[] words;
					for (int i = 0; i < 5; i++)
					{
						buffer = reader.ReadLine();
						if (buffer == "")
						{
							i--;
							continue;
						}
						words = buffer.Split(new char[] { ' ' });
						string willBeWritten = "";
						for (int j = 2; j < words.Length; j++)
						{
							string str = words[j];
							if (str == "")
							{
								str = " ";
							}
							willBeWritten += str;
						}
						switch (i)
						{
							case 0:
								toReturn.FirstField = willBeWritten;
								break;
							case 1:
								toReturn.SecondField = willBeWritten;
								break;
							case 2:
								toReturn.ThirdField = Convert.ToInt32(willBeWritten);
								break;
							case 3:
								toReturn.FourthField = Convert.ToInt32(willBeWritten);
								break;
							case 4:
								toReturn.FifthField = Convert.ToInt32(willBeWritten);
								break;
						}
					}
					break;
				}
			}
			return toReturn;
		}

		public Node ReadFromFile(StreamReader reader)
		{
			return ReadNodeFromFile(reader);
		}

		public override string ToString()
		{
			string toReturn = "\r\nFirst field (string): " + FirstField + "\r\n" +
				"SecondField (string): " + SecondField + "\r\n" +
				"ThirdField (number): " + ThirdField + "\r\n" +
				"FourthField (number): " + FourthField + "\r\n" +
				"FifthField (number): " + FifthField + "\r\n";
			return toReturn;
		}
	}
}
