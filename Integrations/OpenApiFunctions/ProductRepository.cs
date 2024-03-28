using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace OpenApiFunctions
{
    public class ProductRepository
    {
        private readonly CosmosContext _context;

        public ProductRepository(CosmosContext context)
        {
            _context = context;
        }

        public virtual async Task<Product> GetById(string id)
        {
            return await _context.Set<Product>()
                .FirstOrDefaultAsync(entity => entity.Id == id);
        }

        public virtual async Task<IEnumerable<Product>> GetAll()
        {
            return await _context.Set<Product>().ToListAsync();
        }

        public virtual async Task<IEnumerable<Product>> GetByCondition(Expression<Func<Product, bool>> expression)
        {
            return await _context.Set<Product>().Where(expression).ToListAsync();
        }

        public virtual async Task<Product> Create(Product entity)
        {
            if (entity.Id is null)
            {
                entity.Id = Guid.NewGuid().ToString();
            }

            await _context.Set<Product>().AddAsync(entity);
            await _context.SaveChangesAsync();

            return await GetById(entity.Id);
        }

        public virtual async Task<Product> Update(Product entity)
        {
            var entry = _context.Add(entity);
            entry.State = EntityState.Unchanged;

            _context.Set<Product>().Update(entity);
            await _context.SaveChangesAsync();

            return await GetById(entity.Id);
        }

        public virtual async Task Delete(string id)
        {
            var entity = await GetById(id);

            _context.Set<Product>().Remove(entity);
            await _context.SaveChangesAsync();
        }

    }
}
