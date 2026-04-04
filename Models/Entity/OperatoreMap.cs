using Models.Interfaces;
using Models.Tables;

namespace Models.Entity
{
    public class OperatoreMap : BaseMap, IMap, IMappable<Operatore>
    {
        public string NomeOperatore { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool Abilitato { get; set; }
        public int Badge { get; set; }
        public int CodicePermesso { get; set; }
        public string NomePostazione {  get; set; } = string.Empty;
        public string TipoPostazione {  get; set; } = string.Empty;
        public int CodicePerson { get; set; }

        public override string? Titolo => $"{NomeOperatore} - {(Abilitato ? "Abilitato" : "Non abilitato")}";

        public override string Nome
        {
            get => NomeOperatore;
            set => NomeOperatore = value ?? string.Empty;
        }

        public Operatore ToTable()
        {
            return Mappers.OperatoreMapper.ToTable(this);
        }

        public void UpdateTable(Operatore existing)
        {
            if (existing == null) return;
            // Aggiorniamo solo i campi che possono cambiare
            existing.Nome = this.NomeOperatore;
            existing.Password = this.Password;
            existing.Abilitato = this.Abilitato;
            existing.Pass = this.Badge;
            // Non tocchiamo l'ID!
        }
    }
}
