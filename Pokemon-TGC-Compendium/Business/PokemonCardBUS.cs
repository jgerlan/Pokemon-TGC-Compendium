using Pokemon_TGC_Compendium.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pokemon_TGC_Compendium.Business
{
    class PokemonCardBUS
    {
        public PokemonCardDAO pokemonCardDAO { get; set; }

        public PokemonCardBUS()
        {
            pokemonCardDAO = new PokemonCardDAO();
        }

        public void createcreatePokemonCardInfoFile(string content, string nameFile)
        {
            pokemonCardDAO.createPokemonCardInfoFile(content, nameFile);
        }
    }
}
