using Models.Interfaces;
using Models.Tables;

namespace Models.Entity
{
    public class TariffaMap : BaseMap, IMap, IMappable<Tariffa>
    {
        public string NomeTariffa { get; set; } = string.Empty;
        public string EtichettaTariffa { get; set; } = string.Empty;
        public decimal PrezzoTariffa { get; set; } = decimal.Zero;
        public bool IsFreeDrink { get; set; }
        public bool HasListino { get; set; }

        // Sincronizzazione Getter/Setter per l'interfaccia IMap
        public override string Nome
        {
            get => NomeTariffa;
            set => NomeTariffa = value ?? string.Empty;
        }

        // Titolo descrittivo per ComboBox o liste: Nome + Prezzo
        public override string? Titolo => $"{NomeTariffa} - " +
            $"{PrezzoTariffa:C2} {(IsFreeDrink ? "(Free Drink)" : "")}";

        public Tariffa ToTable() => Mappers.TariffaMapper.ToTable(this);

        public void UpdateTable(Tariffa existing)
        {
            // Aggiorniamo solo i campi che possono cambiare
            existing.Nome = this.NomeTariffa;
            existing.Label = this.EtichettaTariffa;
            existing.Prezzo = this.PrezzoTariffa;

            // Non tocchiamo l'ID!
        }
    }

}
