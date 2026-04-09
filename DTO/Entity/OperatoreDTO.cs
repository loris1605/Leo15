using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Entity
{
    public class OperatoreDTO : BaseDTO, IDTO
    {
        public string NomeOperatore { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool Abilitato { get; set; }
        public int Badge { get; set; }
        public int CodicePermesso { get; set; }
        public string NomePostazione { get; set; } = string.Empty;
        public string TipoPostazione { get; set; } = string.Empty;
        public int CodicePerson { get; set; }

        public override string? Titolo => $"{NomeOperatore} - {(Abilitato ? "Abilitato" : "Non abilitato")}";

        
    }
}
