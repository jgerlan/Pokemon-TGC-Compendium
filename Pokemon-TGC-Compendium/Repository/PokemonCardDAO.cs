using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pokemon_TGC_Compendium.Repository
{
    class PokemonCardDAO
    {
		private Object locker = new Object();
		public void CreatePokemonCardInfoFile(string content, string nameFile)
        {
			string path = @"C:\PokemonCardCompendium";
			try
			{
				if (!Directory.Exists(path))
				{
					DirectoryInfo dir = Directory.CreateDirectory(path);
				}

				using (StreamWriter writer = new StreamWriter(path + "\\"+nameFile+ ".json", true))
				{
					writer.WriteLine(content);
					writer.Close();
				}
			}
			catch (Exception)
			{

				throw;
			}
		}

		public async Task CreatePokemonCardInfoFileAsync(string content, string nameFile)
		{
			string path = @"C:\PokemonCardCompendium";
			int timeOut = 100;
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();

			while (true)
			{
				try
				{
					if (!Directory.Exists(path))
					{
						DirectoryInfo dir = Directory.CreateDirectory(path);
					}

					lock (locker)
					{
						File.AppendAllText(path + "\\" + nameFile + ".json", content);
					}
					break;

				}
				catch (Exception)
				{
					//throw;
				}
				if (stopwatch.ElapsedMilliseconds > timeOut)
				{
					break;
				}
				await Task.Delay(5);
			}
			stopwatch.Stop();
		}
    }
}
