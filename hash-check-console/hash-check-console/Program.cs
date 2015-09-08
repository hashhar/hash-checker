using System;
using System.IO;
using System.Security.Cryptography;

namespace hash_check_console
{
	class Program
	{
		static void Main(string[] args)
		{
			// Read the path of the input file
			Console.WriteLine("Enter the full path of the file whose SHA1 hash you want to calculate: ");
			var path = Console.ReadLine();
		choice:
			Console.WriteLine("1. Only show the SHA1 hash\n2. Store the SHA1 hash in a file and do not show it\n3. Both store the SHA1 hash in a file and output it to the console\n");
			var choice = Console.ReadLine();
			// Create a Stream object to pass to ComputeHash method with a buffer size of 120MB
			using (var stream = new BufferedStream(File.OpenRead(path), 120000000))
			// Create a new instance of the SHA1 Class that exposes the ComputeHash method to compute the SHA1 hash of an input stream
			using (var cryptoProvider = new SHA1CryptoServiceProvider())
			{
				// Convert the byte array output by the ComputeHash method to a string
				var hash = BitConverter.ToString(cryptoProvider.ComputeHash(stream));
				// Do as demanded by the user
				switch (choice)
				{
					// Write the hash to the console
					case "1":
						Console.WriteLine(hash);
						break;
					// Write the hash to a file
					case "2":
						// Name the output file containing the SHA1 hash as file.sha1
						var hashFile = path + ".sha1";
						// Write the computed hash to the file
						using (var file = new StreamWriter(hashFile))
							file.WriteLine(hash);
						break;
					// Write the hash to the file and console
					case "3":
						Console.WriteLine(hash);
						goto case "2";
					default:
						goto choice;
				}
			}
			Console.ReadLine();
		}
	}
}

