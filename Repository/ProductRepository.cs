using ApiEcommerce.Data;
using ApiEcommerce.Models;
using ApiEcommerce.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace ApiEcommerce.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;
        public ProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public bool BuyProduct(int productId, int quantity)
        {
            if (productId <= 0 || quantity <= 0)
            {
                return false;
            }
            // Implementation for buying a product
            var product = _context.Products.FirstOrDefault(p => p.ProductId == productId);
            if (product == null || product.Stock <= 0)
            {
                return false;
            }
            product.Stock -= quantity;
            _context.Products.Update(product);
            return Save();
        }

        public bool CreateProduct(Product product)
        {
            if (product == null)
            {
                return false;
            }
            product.CreationDate = DateTime.Now;
            product.UpdateDate = DateTime.Now;
            _context.Products.Add(product);
            return Save();
        }

        public bool DeleteProduct(Product product)
        {
            if (product == null)
            {
                return false;
            }
            _context.Products.Remove(product);
            return Save();
        }

        public Product? GetProduct(int id)
        {
            if (id <= 0)
            {
                return null;
            }
            var product = _context.Products.Include(p => p.Category).FirstOrDefault(p => p.ProductId == id);
            if (product == null)
            {
                return null;
            }
            return product;
        }

        public ICollection<Product> GetProducts()
        {
            return _context.Products.Include(p => p.Category).OrderBy(p => p.Name).ToList();
        }

        public ICollection<Product> GetProductsForCategory(int categoryId)
        {
            if (categoryId <= 0)
            {
                return new List<Product>();
            }
            return _context.Products.Include(p => p.Category).Where(p => p.CategoryId == categoryId).OrderBy(p => p.CategoryId).ToList();
        }

        public bool ProductExists(int id)
        {
            if (id <= 0)
            {
                return false;
            }
            return _context.Products.Any(p => p.ProductId == id);
        }

        public bool ProductExists(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return false;
            }
            return _context.Products.Any(p => p.Name.ToLower().Trim() == name.ToLower().Trim());
        }

        public bool Save()
        {
            return _context.SaveChanges() >= 0;
        }

        public ICollection<Product> SearchProducts(string searchTerm)
        {
            IQueryable<Product> query = _context.Products;
            searchTerm = searchTerm.ToLower().Trim();
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query
                    .Include(p => p.Category)
                    .Where(
                        p => p.Name.ToLower().Trim().Contains(searchTerm)
                        || p.Description.ToLower().Trim().Contains(searchTerm)
                    );
            }
            return query.OrderBy(p => p.Name).ToList();
        }

        public bool UpdateProduct(Product product)
        {
            if (product == null)
            {
                return false;
            }
            product.UpdateDate = DateTime.Now;
            _context.Products.Update(product);
            return Save();
        }
    }
}
