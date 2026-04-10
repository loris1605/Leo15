using Models.Interfaces;
using Models.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Entity
{
    public class OperatoreDTO : BaseDTO, IMap, IMappable<Operatore>
    {
        public OperatoreDTO() { }  

        public OperatoreDTO(Operatore table)
        {
            this.Id = table.Id;
            this.NomeOperatore = table.Nome;
            this.Password = table.Password;
            this.Abilitato = table.Abilitato;
        }

        public Operatore ToTable()
        {
            return new Operatore
            {
                Id = this.Id,
                Nome = this.NomeOperatore,
                Password = this.Password,
                Abilitato = this.Abilitato,
                Pass = this.Badge,
                PersonId = this.CodicePerson
            };
        }

        public static Expression<Func<Operatore, OperatoreDTO>> ToOperatoreDto => entity => new OperatoreDTO
        {
            Id = entity.Id,
            NomeOperatore = entity.Nome,
            Password = entity.Password,
            Abilitato = entity.Abilitato,
            Badge = entity.Pass
        };

        public static Expression<Func<Operatore, Permesso?, OperatoreDTO>> ToOperatoriDtoRelationed =>
        (o, p) => new OperatoreDTO
        {
            Id = o.Id,
            NomeOperatore = o.Nome,
            Password = o.Password,
            Abilitato = o.Abilitato,
            Badge = o.Pass,
            CodicePerson = o.PersonId,
            CodicePermesso = p != null ? p.Id : 0,
            NomePostazione = p != null && p.Postazione != null ? p.Postazione.Nome : "Nessuna",
            TipoPostazione = p != null && p.Postazione != null && p.Postazione.TipoPostazione != null
                             ? p.Postazione.TipoPostazione.Nome
                             : "N/A"
        };

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
