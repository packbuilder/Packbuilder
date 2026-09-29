using Microsoft.EntityFrameworkCore;
using Packbuilder.Interfaces;
using Packbuilder.Models;
using Packbuilder.Models.enums;

namespace Packbuilder.Services
{
    public class PaginationService() : IPaginationService
    {
        public async Task<PaginatedResponse<T>> GetPaginatedData<T>(IQueryable<T> query, int pageSize, int page)
        {
            if(page < 1 || pageSize < 1)
            {
                throw new Exception("Invalid page/page size given for pagination data.");
            }

            pageSize = Math.Min(pageSize, 50);

            int totalItems = await query.CountAsync();
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            List<T> paginatedData = await query
                    .Skip((page - 1) * pageSize)
                        .Take(pageSize).ToListAsync();

            return new PaginatedResponse<T> { 
                Items = paginatedData,
                Page = page,
                TotalPages = totalPages,
                TotalItems = totalItems,
            };
        }
    }
}