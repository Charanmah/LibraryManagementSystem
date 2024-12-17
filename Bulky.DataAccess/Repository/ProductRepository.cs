using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Microsoft.EntityFrameworkCore; // Ensure this namespace is included
using System.Linq;

namespace Bulky.DataAccess.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext _db;

        public ProductRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Product obj)
        {
            // Check if the entity is already being tracked
            var existingProduct = _db.Products.Local.FirstOrDefault(u => u.Id == obj.Id);

            if (existingProduct != null)
            {
                // Update only the properties of the tracked entity
                _db.Entry(existingProduct).CurrentValues.SetValues(obj);
            }
            else
            {
                // If not being tracked, attach and update
                _db.Products.Update(obj);
            }
        }
    }
}
