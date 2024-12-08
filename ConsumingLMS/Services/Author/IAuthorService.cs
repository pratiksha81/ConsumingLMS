using ConsumingLMS.Models;

namespace ConsumingLMS.Services
{
    public interface IAuthorService
    {
        Task<IEnumerable<Auth>> GetAllAuthorsAsync();
        Task<Auth> GetAuthorByIdAsync(int id);
        Task<bool> AddAuthorAsync(Auth author);
        Task<bool> UpdateAuthorAsync(Auth author);
        Task<bool> DeleteAuthorAsync(int id);
    }
}
