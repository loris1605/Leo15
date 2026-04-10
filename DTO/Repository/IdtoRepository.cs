using DTO.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Repository
{
    public interface IdtoRepository<T>  where T : class
    {
        Task<List<T>> Load(int index = 0, CancellationToken ctk = default);
        Task<List<T>> LoadByModel(object model, CancellationToken ctk = default);
        Task<int> Add(T map);
        Task<bool> Del(T map, CancellationToken ctk = default);
    }
}
