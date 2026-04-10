using Models.Interfaces;
using Models.Tables;
using System.Linq.Expressions;

namespace DTO.Entity
{
    public class LoginDTO : BaseDTO, IMap
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

        public LoginDTO() { }

        public LoginDTO(Operatore table)
        {
            this.Id = table.Id;
            this.NomeOperatore = table.Nome;
            this.Password = table.Password;
        }

        public static Expression<Func<Operatore, LoginDTO>> ToLoginDto => entity => new LoginDTO
        {
            Id = entity.Id,
            NomeOperatore = entity.Nome,
            Password = entity.Password,
            
        };

        
    }
}
