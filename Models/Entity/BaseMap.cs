namespace Models.Entity
{
    public class BaseMap
    {
        public int Id { get; set; }
        public virtual string? Titolo { get; set; } = string.Empty;
        public virtual string Nome { get; set; } = String.Empty;
        public override string ToString() => Nome ?? string.Empty;
    }

    public interface IMap
    {
        int Id { get; set; }
        string Nome { get; set; }
    }
}
