using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MiniERPClient.Models
{
    public enum ProductCategory
    {
        Software,
        Hardware,
        Services,
        Consulting,
        Licenses,
        Support,
        Training
    }

    public enum ProductStatus
    {
        Active,
        Discontinued,
        OutOfStock,
        LowStock,
        ComingSoon
    }

    public enum StockAlertLevel
    {
        Normal,
        Low,
        Critical,
        OutOfStock
    }

    public class Product : INotifyPropertyChanged
    {
        private int _id;
        private string _name = string.Empty;
        private string _description = string.Empty;
        private string _sku = string.Empty;
        private ProductCategory _category = ProductCategory.Software;
        private ProductStatus _status = ProductStatus.Active;
        private decimal _price;
        private decimal _costPrice;
        private int _stockQuantity;
        private int _reorderLevel;
        private int _maxStockLevel;
        private string _supplier = string.Empty;
        private string _supplierContactEmail = string.Empty;
        private string _supplierPhone = string.Empty;
        private DateTime _createdDate = DateTime.Now;
        private DateTime? _lastUpdated;
        private DateTime? _lastOrderDate;
        private string _notes = string.Empty;
        private bool _isActive = true;

        public int Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        public string SKU
        {
            get => _sku;
            set
            {
                if (SetProperty(ref _sku, value))
                {
                    OnPropertyChanged(nameof(FormattedSKU));
                }
            }
        }

        public ProductCategory Category
        {
            get => _category;
            set
            {
                if (SetProperty(ref _category, value))
                {
                    OnPropertyChanged(nameof(CategoryBadge));
                }
            }
        }

        public ProductStatus Status
        {
            get => _status;
            set
            {
                if (SetProperty(ref _status, value))
                {
                    OnPropertyChanged(nameof(StatusBadge));
                }
            }
        }

        public decimal Price
        {
            get => _price;
            set
            {
                if (SetProperty(ref _price, value))
                {
                    OnPropertyChanged(nameof(ProfitMargin));
                    OnPropertyChanged(nameof(ProfitAmount));
                }
            }
        }

        public decimal CostPrice
        {
            get => _costPrice;
            set
            {
                if (SetProperty(ref _costPrice, value))
                {
                    OnPropertyChanged(nameof(ProfitMargin));
                    OnPropertyChanged(nameof(ProfitAmount));
                }
            }
        }

        public int StockQuantity
        {
            get => _stockQuantity;
            set
            {
                if (SetProperty(ref _stockQuantity, value))
                {
                    OnPropertyChanged(nameof(StockAlert));
                    OnPropertyChanged(nameof(StockAlertBadge));
                    OnPropertyChanged(nameof(StockStatusDisplay));
                }
            }
        }

        public int ReorderLevel
        {
            get => _reorderLevel;
            set
            {
                if (SetProperty(ref _reorderLevel, value))
                {
                    OnPropertyChanged(nameof(StockAlert));
                    OnPropertyChanged(nameof(StockAlertBadge));
                    OnPropertyChanged(nameof(StockStatusDisplay));
                }
            }
        }

        public int MaxStockLevel
        {
            get => _maxStockLevel;
            set => SetProperty(ref _maxStockLevel, value);
        }

        public string Supplier
        {
            get => _supplier;
            set => SetProperty(ref _supplier, value);
        }

        public string SupplierContactEmail
        {
            get => _supplierContactEmail;
            set => SetProperty(ref _supplierContactEmail, value);
        }

        public string SupplierPhone
        {
            get => _supplierPhone;
            set => SetProperty(ref _supplierPhone, value);
        }

        public DateTime CreatedDate
        {
            get => _createdDate;
            set => SetProperty(ref _createdDate, value);
        }

        public DateTime? LastUpdated
        {
            get => _lastUpdated;
            set => SetProperty(ref _lastUpdated, value);
        }

        public DateTime? LastOrderDate
        {
            get => _lastOrderDate;
            set => SetProperty(ref _lastOrderDate, value);
        }

        public string Notes
        {
            get => _notes;
            set => SetProperty(ref _notes, value);
        }

        public bool IsActive
        {
            get => _isActive;
            set => SetProperty(ref _isActive, value);
        }

        // Computed Properties
        public decimal ProfitMargin => Price > 0 ? ((Price - CostPrice) / Price) * 100 : 0;
        public decimal ProfitAmount => Price - CostPrice;
        public string FormattedSKU => $"PRD-{SKU}";
        public string StatusBadge => Status.ToString();
        public string CategoryBadge => Category.ToString();
        
        public StockAlertLevel StockAlert
        {
            get
            {
                if (StockQuantity <= 0) return StockAlertLevel.OutOfStock;
                if (StockQuantity <= ReorderLevel / 2) return StockAlertLevel.Critical;
                if (StockQuantity <= ReorderLevel) return StockAlertLevel.Low;
                return StockAlertLevel.Normal;
            }
        }

        public string StockAlertBadge => StockAlert.ToString();
        
        public string StockStatusDisplay
        {
            get
            {
                return StockAlert switch
                {
                    StockAlertLevel.OutOfStock => "Out of Stock",
                    StockAlertLevel.Critical => "Critical Stock",
                    StockAlertLevel.Low => "Low Stock",
                    _ => "In Stock"
                };
            }
        }

        public Product Clone()
        {
            return new Product
            {
                Id = this.Id,
                Name = this.Name,
                Description = this.Description,
                SKU = this.SKU,
                Category = this.Category,
                Status = this.Status,
                Price = this.Price,
                CostPrice = this.CostPrice,
                StockQuantity = this.StockQuantity,
                ReorderLevel = this.ReorderLevel,
                MaxStockLevel = this.MaxStockLevel,
                Supplier = this.Supplier,
                SupplierContactEmail = this.SupplierContactEmail,
                SupplierPhone = this.SupplierPhone,
                CreatedDate = this.CreatedDate,
                LastUpdated = this.LastUpdated,
                LastOrderDate = this.LastOrderDate,
                Notes = this.Notes,
                IsActive = this.IsActive
            };
        }

        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;

            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        #endregion
    }

    public class ProductSalesData
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int UnitsSold { get; set; }
        public decimal Revenue { get; set; }
        public DateTime SaleDate { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
    }

    public class InventoryTransaction
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public InventoryTransactionType Type { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string Reference { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public DateTime TransactionDate { get; set; } = DateTime.Now;
        public string CreatedBy { get; set; } = "System";

        // Computed Properties
        public decimal TotalValue => Quantity * UnitPrice;
        public string TypeBadge => Type.ToString();
        public string FormattedReference => $"INV-{Reference}";
    }

    public enum InventoryTransactionType
    {
        Purchase,
        Sale,
        Adjustment,
        Return,
        Transfer,
        Damaged,
        Expired
    }
}