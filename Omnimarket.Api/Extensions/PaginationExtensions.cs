using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Omnimarket.Api.Models.Entidades;

namespace Omnimarket.Api.Extensions
{
    public static class PaginationExtensions
    {
        public static async Task<PageResult<T>> ToPagedResultAsync<T>(
            this IQueryable<T> query,
            int page,
            int pageSize)
        {
            page = NormalizarPage(page);
            pageSize = NormalizarPageSize(pageSize);

            var pagedQuery = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize);

            if (query.Provider is IAsyncQueryProvider)
            {
                var total = await query.CountAsync();
                var items = await pagedQuery.ToListAsync();

                return new PageResult<T>
                {
                    Items = items,
                    Total = total,
                    Page = page,
                    PageSize = pageSize
                };
            }

            return new PageResult<T>
            {
                Items = pagedQuery.ToList(),
                Total = query.Count(),
                Page = page,
                PageSize = pageSize
            };
        }

        private static int NormalizarPage(int page)
            => page < 1 ? 1 : page;

        private static int NormalizarPageSize(int pageSize)
        {
            if (pageSize < 1)
                return 20;

            return pageSize > 100 ? 100 : pageSize;
        }
    }
}
