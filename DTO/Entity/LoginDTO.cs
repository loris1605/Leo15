namespace DTO.Entity
{
    public class LoginDTO : BaseDTO, IDTO
    {
        public string NomeOperatore { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        // Collega la proprietà Nome della base a NomeOperatore
        public override string Nome
        {
            get => NomeOperatore;
            set => NomeOperatore = value ?? string.Empty;
        }

        public override string? Titolo => $"Login: {NomeOperatore}";
    }
}
