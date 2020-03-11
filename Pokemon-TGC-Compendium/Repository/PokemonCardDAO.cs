using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Pokemon_TGC_Compendium.Repository
{
    class PokemonCardDAO
    {
        public void createPokemonCardInfoFile(string content, string nameFile)
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
				}
			}
			catch (Exception)
			{

				throw;
			}
		}
    }
}
