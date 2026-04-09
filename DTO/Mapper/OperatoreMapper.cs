using DTO.Entity;
using Models.Entity;
using Models.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Mapper
{
    public static class OperatoreMapper
    {
        
        public static Expression<Func<Operatore, LoginDTO>> ToLoginDto => entity => new LoginDTO
        {
            Id = entity.Id,
            NomeOperatore = entity.Nome,
            Password = entity.Password
        };

        public static Expression<Func<Operatore, OperatoreDTO>> ToOperatoreDto => entity => new OperatoreDTO
        {
            Id = entity.Id,
            NomeOperatore = entity.Nome,
            Password = entity.Password,
            Abilitato = entity.Abilitato,
            Badge = entity.Pass
        };
    }
}
