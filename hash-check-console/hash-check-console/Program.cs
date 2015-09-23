using System;
using System.IO;
using System.Security.Cryptography;

namespace hash_check_console
{
	class Program
	{
		private static void CalculateHash(string inputFile, string outputFile, int md5, int sha1, int sha256)
		{
			// Check if the input file exists
			if (File.Exists(inputFile) == false)
				Console.WriteLine("The input file {0} does not exist", inputFile);
			else
			{
				using (var stream = new BufferedStream(File.OpenRead(inputFile), 120000000))
				// Create instances of CryptoProviders to expose the ComputeHash method
				using (var md5CryptoProvider = new MD5CryptoServiceProvider())
				using (var sha1CryptoProvider = new SHA1CryptoServiceProvider())
				using (var sha256CryptoProvider = new SHA256CryptoServiceProvider())
				{
					// Set all the hash strings to null so that later when we write them to file or console, we can know if the user actually requested that hash without looking at the command line arguments
					string md5Hash = null, sha1Hash = null, sha256Hash = null;
					// Calculate only the hashes requested
					if (md5 == 1)
						md5Hash = BitConverter.ToString(md5CryptoProvider.ComputeHash(stream));
					if (sha1 == 1)
						sha1Hash = BitConverter.ToString(sha1CryptoProvider.ComputeHash(stream));
					if (sha256 == 1)
						sha256Hash = BitConverter.ToString(sha256CryptoProvider.ComputeHash(stream));
					// If all three of the arguments were skipped, fall back to default behaviour, ie. generate all the hashes
					if (md5 == 0 && sha1 == 0 && sha256 == 0)
					{
						md5Hash = BitConverter.ToString(md5CryptoProvider.ComputeHash(stream));
						sha1Hash = BitConverter.ToString(sha1CryptoProvider.ComputeHash(stream));
						sha256Hash = BitConverter.ToString(sha256CryptoProvider.ComputeHash(stream));
					}
					// If the outputFile parameter is null, write to the console else write to the file
					if (outputFile == null)
						WriteHashConsole(inputFile, md5Hash, sha1Hash, sha256Hash);
					else
						WriteHashFile(outputFile, inputFile, md5Hash, sha1Hash, sha256Hash);
				}
			}
		}

		static void WriteHashFile(string outputFile, string inputFile, string md5Hash, string sha1Hash, string sha256Hash)
		{
			// Create an instance of a StreamWriter to write the hashes to the file in append mode
			using (var outputFileStream = new StreamWriter(outputFile, true))
			{
				// Check the hash strings for null values. Null means that the user had not requested that hash and hence the hash not been generated.
				if (md5Hash != null)
				{
					outputFileStream.Write("MD5:  ");
					outputFileStream.Write(md5Hash);
					outputFileStream.Write("  ");
					outputFileStream.WriteLine(inputFile);
					outputFileStream.WriteLine();
				}
				if (sha1Hash != null)
				{
					outputFileStream.Write("SHA1:  ");
					outputFileStream.Write(sha1Hash);
					outputFileStream.Write("  ");
					outputFileStream.WriteLine(inputFile);
					outputFileStream.WriteLine();
				}
				if (sha256Hash != null)
				{
					outputFileStream.Write("SHA256:  ");
					outputFileStream.Write(sha256Hash);
					outputFileStream.Write("  ");
					outputFileStream.WriteLine(inputFile);
					outputFileStream.WriteLine();
				}
				// Write a separator to separate hashes of two files in the output file
				outputFileStream.WriteLine("--------------------------------------------------------------------------------");
				outputFileStream.WriteLine();
			}
		}

		static void WriteHashConsole(string inputFile, string md5Hash, string sha1Hash, string sha256Hash)
		{
			// Check the hash strings for null values. Null means that the user had not requested that hash and hence the hash not been generated.
			if (md5Hash != null)
			{
				Console.Write("MD5:  ");
				Console.Write(md5Hash);
				Console.Write("  ");
				Console.WriteLine(inputFile);
				Console.WriteLine();
			}
			if (sha1Hash != null)
			{
				Console.Write("SHA1:  ");
				Console.Write(sha1Hash);
				Console.Write("  ");
				Console.WriteLine(inputFile);
				Console.WriteLine();
			}
			if (sha256Hash != null)
			{
				Console.Write("SHA256:  ");
				Console.Write(sha256Hash);
				Console.Write("  ");
				Console.WriteLine(inputFile);
				Console.WriteLine();
			}
		}

		static void Main(string[] args)
		{
			// Values of these variables set to 1 mean that those hash formats have been requested by the user
			int md5 = 0, sha1 = 0, sha256 = 0;
			if (args.Length == 0)
			{
				ShowUsage();
				return;
			}
			// The output has to be written to the console because /w is missing
			// The additional check for /o at this stage prevents the application from crashing if somebody uses /o without the /w
			// The third clause in the if checks if the user has forgotten to enter output file when using /o
			if (args[0] != "/w" && args[args.Length - 2] != "/o" && args[args.Length-1] != "/o")
			{
				// Keep looping forward until you find an argument without a backslash
				for (int i = 0; args[i].Contains("/") == true; i++)
				{
					switch (args[i])
					{
						case "/m":
							md5 = 1;
							break;
						case "/s":
							sha1 = 1;
							break;
						case "/S":
							sha256 = 1;
							break;
					}
				}
				// By now we have the values of md5, sha1, sha256, crc32 indicating the required hashes
				// Keep looping back the arguments until you find one containing a backslash
				Console.WriteLine();
				for (int j = args.Length - 1; j >= 0 && args[j].Contains("/") == false; j--)
				{
					CalculateHash(args[j], null, md5, sha1, sha256);
					Console.WriteLine("--------------------------------------------------------------------------------");
					Console.WriteLine();
				}
			}
			// The output has to be dumped to a file
			else
			{
				// Check if the outputFile has been specified
				string outputFile;
				if (args[args.Length - 2] == "/o")
					outputFile = args[args.Length - 1];
				else
					// Set the outputFile name as the name of the last input file with the extension .checksum
					outputFile = args[args.Length - 1] + ".checksum";
				// Keep looping forward until you find an argument without a backslash
				for (int i = 1; args[i].Contains("/") == true; i++)
				{
					// Set the values indicating which hashes are requested
					switch (args[i])
					{
						case "/m":
							md5 = 1;
							break;
						case "/s":
							sha1 = 1;
							break;
						case "/S":
							sha256 = 1;
							break;
					}
				}
				// By now we have the values of md5, sha1, sha256, crc32 indicating the required hashes
				// Keep looping back the arguments
				for (int j = args.Length - 1; j >= 0; j--)
				{
					// If the current argument contains a backslash or is the same as the outputFile, skip it
					if (args[j].Contains("/") == true || args[j] == outputFile)
						continue;
					CalculateHash(args[j], outputFile, md5, sha1, sha256);
				}
				Console.WriteLine("The hashes have been written to {0}", outputFile);
			}
			Console.WriteLine("Done! Press any key to terminate.");
			Console.ReadLine();
		}

		private static void ShowUsage()
		{
			//var releaseNotes = File.ReadAllText(@"D:\GitHub\hash-checker\hash-check-console\hash-check-console\ReleaseNotes.md");
			Console.WriteLine("Incorrect usage");
			//Console.Write(releaseNotes);
			Console.WriteLine("hash-checker [/w] [/m] [/s] [/S] file1 [file2] [file3]...[/o] [output file]");
			Console.WriteLine();
			Console.WriteLine("/w - Specifies that the output has to be dumped to a file rather than console");
			Console.WriteLine("/m - Generates the MD5 hash");
			Console.WriteLine("/s - Generates the SHA1 hash");
			Console.WriteLine("/S - Generates the SHA256 hash");
			Console.WriteLine("file - Specifies the input files whose hashes have to be calculated(at least one filename must be specified)");
			Console.WriteLine("/o - Used to set a user defined filename for the output file. Can be used without / w but needs the output file for expected results.");
			Console.WriteLine("output file - Specifies a user defined output file. Should be used only in conjunction with /o for expected results.");
			Console.WriteLine("/w has to be the first argument");
			Console.WriteLine("output file must be specified and must also be the last argument if /o has been used");
			Console.WriteLine("The /m, /s and /S can be used together in any order");
			Console.WriteLine("Unhandled exceptions are thrown when:\noutput file is not specified when the /o argument is passed\n");
			Console.WriteLine("Press any key to terminate...");
			Console.ReadLine();
		}
	}
}

