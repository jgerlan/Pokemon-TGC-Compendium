using System;
using System.Collections.Generic;
using System.Text;

namespace Pokemon_TGC_Compendium.Entities
{
    class PokemonCard
    {
        public string numbering { get; set; }
        public string name { get; set; }
        public string expansion { get; set; }
        public string urlImage { get; set; }
        public string codedImage { get; set; }

        public PokemonCard()
        {

        }
        public PokemonCard(string numbering, string name, string expansion, string urlImage, string codedImage)
        {
            this.numbering = numbering;
            this.name = name;
            this.expansion = expansion;
            this.urlImage = urlImage;
            this.codedImage = codedImage;
        }
    }
}
