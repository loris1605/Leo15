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
        //public static OperatoreDTO ToMap(this Operatore entity)
        //{
        //    if (entity == null) return new OperatoreMap();

        //    return new OperatoreMap
        //    {
        //        Id = entity.Id,
        //        NomeOperatore = entity.Nome ?? string.Empty,
        //        Password = entity.Password ?? string.Empty,
        //        Abilitato = entity.Abilitato,
        //        Badge = entity.Pass,
        //        CodicePerson = entity.PersonId
        //    };
        //}

        public static Expression<Func<Operatore, LoginDTO>> ToLoginDto => entity => new LoginDTO
        {
            Id = entity.Id,
            NomeOperatore = entity.Nome,
            Password = entity.Password
        };
    }
}
