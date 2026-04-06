using SysNet.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Entity
{
    public class PersonDTO : BaseDTO, IDTO
    {
        public string Cognome { get; set; } = string.Empty;
        public int Natoil { get; set; }
        
        //public DateTime? NatoilDate { get; set; }
        public int CodiceSocio { get; set; }
        public string NumeroSocio { get; set; } = string.Empty;
        public int CodiceTessera { get; set; }
        public string NumeroTessera { get; set; } = string.Empty;
        public int Scadenza { get; set; }
        //public DateTime? ScadenzaDate { get; set; } = DateTime.Now;

        public string CodiceUnivoco { get; set; } = string.Empty;

        // 2. Aggiungi un controllo di sicurezza sulle date (se l'int è 0, ToShortDateString crasha)
        public override string? Titolo => $"{Nome} {Cognome} ({NatoilDate.ToShortDateString()})";

        public DateTime NatoilDate => Natoil.DateIntToDate();
        public DateTime ScadenzaDate => Scadenza.DateIntToDate();

        public bool IsMaggiorenne => Natoil.IsLegalAge();

        public override string Nome { get; set; } = string.Empty;
    }
}
