using System;
using System.IO;
using System.Security.Cryptography;

namespace hash_check_console
{
	class Program
	{
		static void Main(string[] args)
		{
			using (var stream = new BufferedStream(File.OpenRead(@"D:\Software\OS\Windows10Pro_x64.iso"), 1200000))
			using (var cryptoProvider = new SHA1CryptoServiceProvider())
			{
				var hash = BitConverter.ToString(cryptoProvider.ComputeHash(stream));
				using (var file = new StreamWriter(@"D:\output.txt"))
					file.WriteLine(hash);
			}
		}
	}
}

