using MiniERPClient.Models;

namespace MiniERPClient.Services
{
    public interface IProductService
    {
        Task<List<Product>> GetProductsAsync();
        Task<Product?> GetProductByIdAsync(int id);
        Task<Product> AddProductAsync(Product product);
        Task<Product> UpdateProductAsync(Product product);
        Task<bool> DeleteProductAsync(int id);
        Task<List<Product>> GetProductsByCategoryAsync(ProductCategory category);
        Task<List<Product>> GetLowStockProductsAsync();
        Task<List<ProductSalesData>> GetProductSalesDataAsync(int productId);
        Task<List<InventoryTransaction>> GetInventoryTransactionsAsync(int? productId = null);
        Task<InventoryTransaction> AddInventoryTransactionAsync(InventoryTransaction transaction);
        Task<bool> UpdateStockAsync(int productId, int newQuantity, string reason);
    }

    public class ProductService : IProductService
    {
        private readonly List<Product> _products;
        private readonly List<ProductSalesData> _salesData;
        private readonly List<InventoryTransaction> _transactions;
        private int _nextId = 1;
        private int _nextTransactionId = 1;

        public ProductService()
        {
            // Sample product data
            _products = new List<Product>
            {
                new Product
                {
                    Id = _nextId++,
                    Name = "Enterprise ERP Suite",
                    Description = "Comprehensive enterprise resource planning software solution",
                    SKU = "ERP001",
                    Category = ProductCategory.Software,
                    Status = ProductStatus.Active,
                    Price = 15000,
                    CostPrice = 8000,
                    StockQuantity = 50,
                    ReorderLevel = 10,
                    MaxStockLevel = 100,
                    Supplier = "TechSoft Solutions",
                    SupplierContactEmail = "sales@techsoft.com",
                    SupplierPhone = "(555) 100-2000",
                    CreatedDate = DateTime.Now.AddMonths(-18),
                    LastUpdated = DateTime.Now.AddDays(-5),
                    LastOrderDate = DateTime.Now.AddDays(-30),
                    Notes = "Best-selling enterprise solution with high profit margin"
                },
                new Product
                {
                    Id = _nextId++,
                    Name = "Professional Services Package",
                    Description = "Implementation and customization services for ERP systems",
                    SKU = "SRV001",
                    Category = ProductCategory.Services,
                    Status = ProductStatus.Active,
                    Price = 5000,
                    CostPrice = 2500,
                    StockQuantity = 25,
                    ReorderLevel = 5,
                    MaxStockLevel = 50,
                    Supplier = "Internal",
                    SupplierContactEmail = "services@company.com",
                    SupplierPhone = "(555) 100-3000",
                    CreatedDate = DateTime.Now.AddMonths(-12),
                    LastUpdated = DateTime.Now.AddDays(-2),
                    LastOrderDate = DateTime.Now.AddDays(-15),
                    Notes = "High-margin service offering with strong demand"
                },
                new Product
                {
                    Id = _nextId++,
                    Name = "Database Server Hardware",
                    Description = "High-performance server hardware for database operations",
                    SKU = "HW001",
                    Category = ProductCategory.Hardware,
                    Status = ProductStatus.Active,
                    Price = 8000,
                    CostPrice = 6000,
                    StockQuantity = 3,
                    ReorderLevel = 5,
                    MaxStockLevel = 20,
                    Supplier = "ServerTech Inc",
                    SupplierContactEmail = "orders@servertech.com",
                    SupplierPhone = "(555) 200-4000",
                    CreatedDate = DateTime.Now.AddMonths(-6),
                    LastUpdated = DateTime.Now.AddDays(-1),
                    LastOrderDate = DateTime.Now.AddDays(-10),
                    Notes = "Critical low stock - reorder urgently needed"
                },
                new Product
                {
                    Id = _nextId++,
                    Name = "Software Training Program",
                    Description = "Comprehensive training program for ERP software users",
                    SKU = "TRN001",
                    Category = ProductCategory.Training,
                    Status = ProductStatus.Active,
                    Price = 2500,
                    CostPrice = 1000,
                    StockQuantity = 100,
                    ReorderLevel = 20,
                    MaxStockLevel = 200,
                    Supplier = "Training Solutions LLC",
                    SupplierContactEmail = "info@trainingsolutions.com",
                    SupplierPhone = "(555) 300-5000",
                    CreatedDate = DateTime.Now.AddMonths(-9),
                    LastUpdated = DateTime.Now.AddDays(-7),
                    LastOrderDate = DateTime.Now.AddDays(-20),
                    Notes = "Popular training package with excellent feedback"
                },
                new Product
                {
                    Id = _nextId++,
                    Name = "Legacy System License",
                    Description = "Software license for legacy system integration",
                    SKU = "LIC001",
                    Category = ProductCategory.Licenses,
                    Status = ProductStatus.Discontinued,
                    Price = 3000,
                    CostPrice = 1500,
                    StockQuantity = 0,
                    ReorderLevel = 0,
                    MaxStockLevel = 0,
                    Supplier = "LegacySoft Corp",
                    SupplierContactEmail = "legacy@legacysoft.com",
                    SupplierPhone = "(555) 400-6000",
                    CreatedDate = DateTime.Now.AddMonths(-24),
                    LastUpdated = DateTime.Now.AddMonths(-6),
                    LastOrderDate = DateTime.Now.AddMonths(-12),
                    Notes = "Discontinued product - no longer supported"
                },
                new Product
                {
                    Id = _nextId++,
                    Name = "Premium Support Package",
                    Description = "24/7 premium support with guaranteed response times",
                    SKU = "SUP001",
                    Category = ProductCategory.Support,
                    Status = ProductStatus.Active,
                    Price = 4000,
                    CostPrice = 1800,
                    StockQuantity = 15,
                    ReorderLevel = 10,
                    MaxStockLevel = 30,
                    Supplier = "Internal",
                    SupplierContactEmail = "support@company.com",
                    SupplierPhone = "(555) 100-7000",
                    CreatedDate = DateTime.Now.AddMonths(-15),
                    LastUpdated = DateTime.Now.AddDays(-3),
                    LastOrderDate = DateTime.Now.AddDays(-8),
                    Notes = "High-value recurring revenue stream"
                }
            };

            // Sample sales data
            _salesData = new List<ProductSalesData>
            {
                new ProductSalesData { ProductId = 1, ProductName = "Enterprise ERP Suite", UnitsSold = 5, Revenue = 75000, SaleDate = DateTime.Now.AddDays(-10), CustomerId = 1, CustomerName = "Acme Corporation" },
                new ProductSalesData { ProductId = 2, ProductName = "Professional Services Package", UnitsSold = 3, Revenue = 15000, SaleDate = DateTime.Now.AddDays(-8), CustomerId = 1, CustomerName = "Acme Corporation" },
                new ProductSalesData { ProductId = 1, ProductName = "Enterprise ERP Suite", UnitsSold = 2, Revenue = 30000, SaleDate = DateTime.Now.AddDays(-15), CustomerId = 2, CustomerName = "TechStart Inc" },
                new ProductSalesData { ProductId = 4, ProductName = "Software Training Program", UnitsSold = 8, Revenue = 20000, SaleDate = DateTime.Now.AddDays(-5), CustomerId = 4, CustomerName = "Global Enterprises Ltd" },
                new ProductSalesData { ProductId = 6, ProductName = "Premium Support Package", UnitsSold = 2, Revenue = 8000, SaleDate = DateTime.Now.AddDays(-12), CustomerId = 1, CustomerName = "Acme Corporation" }
            };

            // Sample inventory transactions
            _transactions = new List<InventoryTransaction>
            {
                new InventoryTransaction { Id = _nextTransactionId++, ProductId = 1, ProductName = "Enterprise ERP Suite", Type = InventoryTransactionType.Purchase, Quantity = 20, UnitPrice = 8000, Reference = "PO-2024-001", Notes = "Initial stock purchase", TransactionDate = DateTime.Now.AddMonths(-1) },
                new InventoryTransaction { Id = _nextTransactionId++, ProductId = 1, ProductName = "Enterprise ERP Suite", Type = InventoryTransactionType.Sale, Quantity = -5, UnitPrice = 15000, Reference = "SO-2024-001", Notes = "Sale to Acme Corporation", TransactionDate = DateTime.Now.AddDays(-10) },
                new InventoryTransaction { Id = _nextTransactionId++, ProductId = 3, ProductName = "Database Server Hardware", Type = InventoryTransactionType.Purchase, Quantity = 10, UnitPrice = 6000, Reference = "PO-2024-002", Notes = "Hardware procurement", TransactionDate = DateTime.Now.AddMonths(-1) },
                new InventoryTransaction { Id = _nextTransactionId++, ProductId = 3, ProductName = "Database Server Hardware", Type = InventoryTransactionType.Sale, Quantity = -7, UnitPrice = 8000, Reference = "SO-2024-002", Notes = "Hardware sales", TransactionDate = DateTime.Now.AddDays(-15) },
                new InventoryTransaction { Id = _nextTransactionId++, ProductId = 4, ProductName = "Software Training Program", Type = InventoryTransactionType.Adjustment, Quantity = 10, UnitPrice = 1000, Reference = "ADJ-2024-001", Notes = "Stock adjustment - damaged items removed", TransactionDate = DateTime.Now.AddDays(-5) }
            };
        }

        public async Task<List<Product>> GetProductsAsync()
        {
            await Task.Delay(120);
            return new List<Product>(_products);
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            await Task.Delay(50);
            return _products.FirstOrDefault(p => p.Id == id);
        }

        public async Task<Product> AddProductAsync(Product product)
        {
            await Task.Delay(180);
            product.Id = _nextId++;
            product.CreatedDate = DateTime.Now;
            product.LastUpdated = DateTime.Now;
            _products.Add(product);
            return product;
        }

        public async Task<Product> UpdateProductAsync(Product product)
        {
            await Task.Delay(150);
            var existingProduct = _products.FirstOrDefault(p => p.Id == product.Id);
            if (existingProduct != null)
            {
                product.LastUpdated = DateTime.Now;
                var index = _products.IndexOf(existingProduct);
                _products[index] = product;
            }
            return product;
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            await Task.Delay(100);
            var product = _products.FirstOrDefault(p => p.Id == id);
            if (product != null)
            {
                _products.Remove(product);
                return true;
            }
            return false;
        }

        public async Task<List<Product>> GetProductsByCategoryAsync(ProductCategory category)
        {
            await Task.Delay(80);
            return _products.Where(p => p.Category == category).ToList();
        }

        public async Task<List<Product>> GetLowStockProductsAsync()
        {
            await Task.Delay(60);
            return _products.Where(p => p.StockAlert != StockAlertLevel.Normal).ToList();
        }

        public async Task<List<ProductSalesData>> GetProductSalesDataAsync(int productId)
        {
            await Task.Delay(90);
            return _salesData.Where(s => s.ProductId == productId).OrderByDescending(s => s.SaleDate).ToList();
        }

        public async Task<List<InventoryTransaction>> GetInventoryTransactionsAsync(int? productId = null)
        {
            await Task.Delay(100);
            var transactions = productId.HasValue 
                ? _transactions.Where(t => t.ProductId == productId.Value)
                : _transactions;
            return transactions.OrderByDescending(t => t.TransactionDate).ToList();
        }

        public async Task<InventoryTransaction> AddInventoryTransactionAsync(InventoryTransaction transaction)
        {
            await Task.Delay(120);
            transaction.Id = _nextTransactionId++;
            transaction.TransactionDate = DateTime.Now;
            _transactions.Add(transaction);
            
            // Update product stock
            var product = _products.FirstOrDefault(p => p.Id == transaction.ProductId);
            if (product != null)
            {
                product.StockQuantity += transaction.Quantity;
                product.LastUpdated = DateTime.Now;
            }
            
            return transaction;
        }

        public async Task<bool> UpdateStockAsync(int productId, int newQuantity, string reason)
        {
            await Task.Delay(80);
            var product = _products.FirstOrDefault(p => p.Id == productId);
            if (product != null)
            {
                var oldQuantity = product.StockQuantity;
                var adjustment = newQuantity - oldQuantity;
                
                product.StockQuantity = newQuantity;
                product.LastUpdated = DateTime.Now;
                
                // Create adjustment transaction
                var transaction = new InventoryTransaction
                {
                    Id = _nextTransactionId++,
                    ProductId = productId,
                    ProductName = product.Name,
                    Type = InventoryTransactionType.Adjustment,
                    Quantity = adjustment,
                    UnitPrice = product.CostPrice,
                    Reference = $"ADJ-{DateTime.Now:yyyyMMdd}-{_nextTransactionId}",
                    Notes = reason,
                    TransactionDate = DateTime.Now,
                    CreatedBy = "User"
                };
                
                _transactions.Add(transaction);
                return true;
            }
            return false;
        }
    }
}