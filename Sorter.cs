using System;
using System.Collections.Generic;
using MyLibrary.DataStructures;
using System.Text;
using MyLibrary.InputOutput;
using System.IO;

namespace MyLibrary
{
	public delegate void WriteTypeToFile<T>(StreamWriter writer, T objectToWrite);
	public delegate T ReadTypeFromFile<T>(StreamReader reader);

	public class Sorter<T>
	{
		public T[] SortByPyramid(T[] sortingArray, IComparer<T> comparer)
		{
			BinaryHeap<T> heap = new BinaryHeap<T>(comparer);
			heap.Built(sortingArray);
			heap.Sort();
			return sortingArray;
		}

		public int ExternalNaturalMergeSort(string externalFile, int maxSeriesLenght, IComparer<T> comparer,
			WriteTypeToFile<T> write, ReadTypeFromFile<T> read)
		{
			Random random = new Random();
			bool isSorted = false;
			int iterations = 0;
			int currentSeriesMaxLenght;
			while (!isSorted)
			{
				currentSeriesMaxLenght = random.Next(1, maxSeriesLenght);
				SplitFile(externalFile, currentSeriesMaxLenght, comparer, write, read);
				if (IsEmpty("f1.txt", read) || IsEmpty("f2.txt", read))
				{
					isSorted = true;
				}
				MergeAuxiliaryFiles(externalFile, "f1.txt", "f2.txt", comparer, write, read);
				iterations++;
			}
			return iterations;
		}

		private bool IsEmpty(string fileName, ReadTypeFromFile<T> read)
		{
			bool isEmpty = false;
			StreamReader reader = new StreamReader(fileName);
			if (read(reader) == null)
			{
				isEmpty = true;
			}
			reader.Close();
			return isEmpty;
		}

		/// <summary>
		/// Сливает вспомогательные файлы в один, используя слияние
		/// </summary>
		/// <param name="firstFileName"></param>
		/// <param name="secondFileName"></param>
		/// <param name="comparer"></param>
		private void MergeAuxiliaryFiles(string fileWhereToMerge, string firstFileName, string secondFileName, IComparer<T> comparer,
			WriteTypeToFile<T> write, ReadTypeFromFile<T> read)
		{
			T firstNode;
			T secondNode;
			StreamWriter writer = new StreamWriter(fileWhereToMerge);
			StreamReader firstReader = new StreamReader(firstFileName);
			StreamReader secondReader = new StreamReader(secondFileName);
			firstNode = read(firstReader);
			secondNode = read(secondReader);
			while (firstNode != null && secondNode != null)
			{
				if (comparer.Compare(firstNode, secondNode) < 0)
				{
					write(writer, firstNode);
					firstNode = read(firstReader);
				}
				else
				{
					write(writer, secondNode);
					secondNode = read(secondReader);
				}	
			}
			while (firstNode != null)
			{
				write(writer, firstNode);
				firstNode = read(firstReader);
			}
			while (secondNode != null)
			{
				write(writer, secondNode);
				secondNode = read(secondReader);
			}
			secondReader.Close();
			firstReader.Close();
			writer.Close();
		}

		/// <summary>
		/// Разбивает указанный файл на два вспомогательных
		/// </summary>
		/// <param name="fileName"></param>
		/// <param name="maxSeriesLenght"></param>
		/// <param name="comparer"></param>
		private void SplitFile(string fileName, int maxSeriesLenght, IComparer<T> comparer,
			WriteTypeToFile<T> write, ReadTypeFromFile<T> read)
		{
			T currentNode;
			StreamReader reader = new StreamReader(fileName);
			StreamWriter firstWriter = new StreamWriter("f1.txt");
			StreamWriter secondWriter = new StreamWriter("f2.txt");
			StreamWriter currentWriter = firstWriter;
			currentNode = read(reader);
			T tmpNode;
			int currentSeriesLenght = 0;
			while (currentNode != null)
			{
				currentSeriesLenght++;
				write(currentWriter, currentNode);
				tmpNode = read(reader);
				if (tmpNode != null)
				{
					if (comparer.Compare(tmpNode, currentNode) < 0 /*|| currentSeriesLenght >= maxSeriesLenght*/)  // tmpNode < currentNode
					{
						currentWriter.WriteLine("`");
						currentSeriesLenght = 0;
						currentWriter = SetOppositeWriter(firstWriter, secondWriter, currentWriter);
					}
				}
				currentNode = tmpNode;
			}
			secondWriter.Close();
			firstWriter.Close();
			reader.Close();
		}

		private StreamWriter SetOppositeWriter(StreamWriter firstWriter, StreamWriter secondWriter, StreamWriter currentWriter)
		{
			if (currentWriter == firstWriter)
			{
				return secondWriter;
			}
			else
			{
				return firstWriter;
			}
		}
	}
}
