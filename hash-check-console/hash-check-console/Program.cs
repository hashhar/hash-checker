using System;
using System.IO;
using System.Security.Cryptography;

/* To-do:
- Add multi-letter argument support (Done)
- Add support for using either of '/' or '-' as the argument delimiter (Done)
- Add support for verifying hashes by specifying an input file containing hash and file pairs
- Decide between using I/O redirection or continuing the native option to dump the output to a file (I/O redirection decided)
*/

/* Expected usages:
- hash-check
	Show the proper usage
- hash-check (No input file specified)
	Message telling an input file has to be specified
- hash-check (Incorrect arguments)
	- Any argument not defined in the documentation (multi-letter arguments are supported so don't use argument length as a criteria)
*/

namespace hash_check_console
{
	class Program
	{
		private static void CalculateHash(string inputFile, string outFile, bool writeToFile, bool md5, bool sha1, bool sha256)
		{
			// Proceed only if the input file exists
			if (File.Exists(inputFile))
			{
				// Create an instance of a BufferredStream with a stream size of 12MB to read the input file
				var stream = new BufferedStream(File.OpenRead(inputFile), 120000000);
				// Create instances of CryptoProviders to expose the ComputeHash method
				var md5CryptoProvider = new MD5CryptoServiceProvider();
				var sha1CryptoProvider = new SHA1CryptoServiceProvider();
				var sha256CryptoProvider = new SHA256CryptoServiceProvider();
				// Set all the hash strings to null so that later when we write them we can tell if the user requested that hash without looking at the command line arguments
				string md5Hash = null, sha1Hash = null, sha256Hash = null;
				// Calculate only the requested hashes
				if (md5)
					md5Hash = BitConverter.ToString(md5CryptoProvider.ComputeHash(stream));
				if (sha1)
					sha1Hash = BitConverter.ToString(sha1CryptoProvider.ComputeHash(stream));
				if (sha256)
					sha256Hash = BitConverter.ToString(sha256CryptoProvider.ComputeHash(stream));
				// If all three of the arguments were skipped generate all the hashes
				if (md5 == false && sha1 == false && sha256 == false)
				{
					md5Hash = BitConverter.ToString(md5CryptoProvider.ComputeHash(stream));
					sha1Hash = BitConverter.ToString(sha1CryptoProvider.ComputeHash(stream));
					sha256Hash = BitConverter.ToString(sha256CryptoProvider.ComputeHash(stream));
				}
				// If the output has to be dumped to a default dump-file, do so. Else write the output to the console.
				if (writeToFile)
					WriteHashFile(inputFile, outFile, md5Hash, sha1Hash, sha256Hash);
				else
					WriteHashConsole(inputFile, md5Hash, sha1Hash, sha256Hash);
			}
			// If we reached here that means the input file doesn't exist
			else
				Console.WriteLine("The input file {0} does not exist", inputFile);
		}

		private static void WriteHashFile(string inputFile, string outFile, string md5Hash, string sha1Hash, string sha256Hash)
		{
			// ?????Is this if-else redundant?????
			if (outFile == null)
				return;
			// Create an instance of a StreamWriter to write the hashes to the file in append mode
			using (StreamWriter outFileStream = new StreamWriter(outFile, true))
			{
				// Check the hash strings for null values. Null means that the user had not requested that hash and hence the hash not been generated.
				if (md5Hash != null)
				{
					outFileStream.Write("MD5:  ");
					outFileStream.Write(md5Hash);
					outFileStream.Write("  ");
					outFileStream.WriteLine(inputFile);
					outFileStream.WriteLine();
				}
				if (sha1Hash != null)
				{
					outFileStream.Write("SHA1:  ");
					outFileStream.Write(sha1Hash);
					outFileStream.Write("  ");
					outFileStream.WriteLine(inputFile);
					outFileStream.WriteLine();
				}
				if (sha256Hash != null)
				{
					outFileStream.Write("SHA256:  ");
					outFileStream.Write(sha256Hash);
					outFileStream.Write("  ");
					outFileStream.WriteLine(inputFile);
					outFileStream.WriteLine();
				}
				// Write a separator to separate hashes of two files in the output file
				outFileStream.WriteLine("--------------------------------------------------------------------------------");
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
			// Write a separator to separate hashes of two files
			Console.WriteLine("--------------------------------------------------------------------------------");
		}

		static void Main(string[] args)
		{
			// Tracks the hashes requested by the user (md5, sha1, sha256) and if the output has to be dumped to a default dump-file (write)
			bool md5 = false, sha1 = false, sha256 = false, write = false;

		/* ----Error Handling---- */
			// Tracks if an input file has been specified
			bool inputFileSpecified = true;
			// No arguments supplied, show the expected usage and terminate
			if (args.Length == 0)
			{
				ShowCorrectUsage();
				return;
			}
			// Confirm if an input file is specified by looking for at least one argument without /, - or --
			foreach (string t in args)
				if (!t.StartsWith("/") && !t.StartsWith("-") && !t.StartsWith("--"))
				{
					inputFileSpecified = false;
					break;
				}
			// No input file was specified, show the expected usage and terminate
			if (inputFileSpecified)
			{
				ShowCorrectUsage();
				return;
			}
		/* ----Error Handling Ends---- */

			// Set the default dump-file's path and name as the last input file. The control should not reach here unless an input file has been specified (to prevent OutOfRange exception)
			string outFile = args[args.Length - 1].Remove(args[args.Length - 1].Length - 4);
			outFile = outFile + ".checksum";
			// Check for all the arguments and set the values of the options as required
			foreach (string t in args)
			{
				if (t == "/m" || t == "/md5" || t == "-m" || t == "--md5")
					md5 = true;
				else if (t == "/s" || t == "/sha1" || t == "-s" || t == "--sha1")
					sha1 = true;
				else if (t == "/S" || t == "/sha256" || t == "-S" || t == "--sha256")
					sha256 = true;
				else if (t == "/w" || t == "/write-to-file" || t == "-w" || t == "--write-to-file")
					write = true;
				else if (!t.StartsWith("/") && !t.StartsWith("-") && !t.StartsWith("--"))
					CalculateHash(t, outFile, write, md5, sha1, sha256);
				// Unexpected argument encountered, show the expected usage and terminate
				else
				{
					Console.WriteLine("Unexpected argument {0} encountered... Aborting!\n\n", t);
					ShowCorrectUsage();
					return;
				}
			}
			// If we have reached here, it means everything has happened successfully
			Console.WriteLine("Done! Press any key to terminate.");
			Console.ReadLine();
		}

		private static void ShowCorrectUsage()
		{
			Console.WriteLine("Expected usage:");
			Console.WriteLine("--------------------------------------------------------------------------------");
			Console.WriteLine("hash-checker [/write-to-file] [/md5] [/sha1] [/sha256] file1 [file2] [file3]...");
			Console.WriteLine("hash-checker [/w] [/m] [/s] [/S] file1 [file2] [file3]...");
			Console.WriteLine("hash-checker [-w] [-m] [-s] [-S] file1 [file2] [file3]...");
			Console.WriteLine("hash-checker [--write-to-file] [--md5] [--sha1] [--sha256] file1 [file2] [file3]...");
			Console.WriteLine();
			Console.WriteLine("w or write-to-file - Specifies that the output has to be dumped to a file rather than console");
			Console.WriteLine("m or md5 - Generates the MD5 hash");
			Console.WriteLine("s or sha1 - Generates the SHA1 hash");
			Console.WriteLine("S or sha256 - Generates the SHA256 hash");
			Console.WriteLine("file - Specifies the input files whose hashes have to be calculated(at least one filename must be specified)");
			Console.WriteLine();
			Console.WriteLine("The arguments can be used in any order and in any combination");
			Console.WriteLine("Press any key to terminate the program and try again...");
			Console.ReadLine();
		}
	}
}

