using Models.Interfaces;

namespace Models.Entity
{
    public class TipoPostazioneMap : BaseMap, IMap
    {
        // Se la tabella TipoPostazione ha solo Id e Nome, basta l'ereditarietà.
        // Ma per sicurezza e chiarezza con i Mapper, definiamo esplicitamente il legame.

        public override string Nome
        {
            get => base.Nome ?? string.Empty;
            set => base.Nome = value ?? string.Empty;
        }

        // Il Titolo per un tipo postazione solitamente coincide con il Nome,
        // ma puoi personalizzarlo (es. "Tipo: Nome")
        public override string? Titolo => Nome;
    }


}
