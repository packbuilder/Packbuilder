using Packbuilder.Models;
using Packbuilder.Models.enums;

namespace Packbuilder.Interfaces
{
    public interface IPaginationService
    {
        public Task<PaginatedResponse<T>> GetPaginatedData<T>(IQueryable<T> query, int page, int pageSize);
    }
}