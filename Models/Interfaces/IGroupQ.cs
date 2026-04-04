using Models.Entity;

namespace Models.Interfaces
{
    public interface IGroupQ<T> : IDisposable where T : class
    {
        Task<List<T>> Load(int index = 0);
        Task<List<T>> LoadByModel(object model);
        

    }
}
