using Pokemon_TGC_Compendium.Repository;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Pokemon_TGC_Compendium.Business
{
    class PokemonCardBUS
    {
        public PokemonCardDAO pokemonCardDAO { get; set; }

        public PokemonCardBUS()
        {
            pokemonCardDAO = new PokemonCardDAO();
        }

        public void CreatePokemonCardInfoFile(string content, string nameFile)
        {
            pokemonCardDAO.CreatePokemonCardInfoFile(content, nameFile);
        }

        public async Task CreatePokemonCardInfoFileAsync(string content, string nameFile)
        {
            await pokemonCardDAO.CreatePokemonCardInfoFileAsync(content, nameFile);
        }
    }
}
