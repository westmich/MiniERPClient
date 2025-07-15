using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using MiniERPClient.Commands;
using MiniERPClient.Models;

namespace MiniERPClient.ViewModels
{
    public class ProductEditDialogViewModel : ViewModelBase
    {
        private Product _product;
        private bool _isNewProduct;
        private bool _isSaving;
        private string _validationSummary = string.Empty;

        public ProductEditDialogViewModel(Product? product = null)
        {
            _isNewProduct = product == null;
            _product = product?.Clone() ?? new Product
            {
                Name = string.Empty,
                Description = string.Empty,
                SKU = string.Empty,
                Category = ProductCategory.Software,
                Status = ProductStatus.Active,
                Price = 0,
                CostPrice = 0,
                StockQuantity = 0,
                ReorderLevel = 5,
                MaxStockLevel = 100,
                Supplier = string.Empty,
                SupplierContactEmail = string.Empty,
                SupplierPhone = string.Empty,
                IsActive = true
            };

            // Initialize available categories and statuses
            AvailableCategories = new ObservableCollection<ProductCategory>
            {
                ProductCategory.Software,
                ProductCategory.Hardware,
                ProductCategory.Services,
                ProductCategory.Consulting,
                ProductCategory.Licenses,
                ProductCategory.Support,
                ProductCategory.Training
            };

            AvailableStatuses = new ObservableCollection<ProductStatus>
            {
                ProductStatus.Active,
                ProductStatus.Discontinued,
                ProductStatus.OutOfStock,
                ProductStatus.LowStock,
                ProductStatus.ComingSoon
            };

            // Initialize commands
            SaveCommand = new RelayCommand(Save, CanSave);
            CancelCommand = new RelayCommand(Cancel);

            // Subscribe to property changes on the product to trigger validation
            _product.PropertyChanged += OnProductPropertyChanged;

            // Validate initially
            ValidateProduct();
        }

        #region Properties

        public Product Product
        {
            get => _product;
            set
            {
                if (_product != null)
                {
                    _product.PropertyChanged -= OnProductPropertyChanged;
                }
                
                if (SetProperty(ref _product, value))
                {
                    if (_product != null)
                    {
                        _product.PropertyChanged += OnProductPropertyChanged;
                    }
                    ValidateProduct();
                }
            }
        }

        public bool IsNewProduct
        {
            get => _isNewProduct;
            set => SetProperty(ref _isNewProduct, value);
        }

        public bool IsSaving
        {
            get => _isSaving;
            set => SetProperty(ref _isSaving, value);
        }

        public string ValidationSummary
        {
            get => _validationSummary;
            set => SetProperty(ref _validationSummary, value);
        }

        public bool HasValidationErrors => !string.IsNullOrEmpty(ValidationSummary);

        public ObservableCollection<ProductCategory> AvailableCategories { get; }
        public ObservableCollection<ProductStatus> AvailableStatuses { get; }

        #endregion

        #region Commands

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        #endregion

        #region Events

        public event EventHandler? SaveRequested;
        public event EventHandler? CancelRequested;

        #endregion

        #region Methods

        private void OnProductPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            ValidateProduct();
            CommandManager.InvalidateRequerySuggested();
        }

        private bool CanSave()
        {
            return !IsSaving && !HasValidationErrors;
        }

        private void Save()
        {
            if (!CanSave()) return;

            IsSaving = true;
            try
            {
                SaveRequested?.Invoke(this, EventArgs.Empty);
            }
            finally
            {
                IsSaving = false;
            }
        }

        private void Cancel()
        {
            CancelRequested?.Invoke(this, EventArgs.Empty);
        }

        private void ValidateProduct()
        {
            var validationErrors = new List<string>();

            // Name validation
            if (string.IsNullOrWhiteSpace(Product.Name))
            {
                validationErrors.Add("Product Name is required.");
            }

            // SKU validation
            if (string.IsNullOrWhiteSpace(Product.SKU))
            {
                validationErrors.Add("SKU is required.");
            }

            // Description validation
            if (string.IsNullOrWhiteSpace(Product.Description))
            {
                validationErrors.Add("Description is required.");
            }

            // Price validation
            if (Product.Price <= 0)
            {
                validationErrors.Add("Price must be greater than 0.");
            }

            // Cost price validation
            if (Product.CostPrice < 0)
            {
                validationErrors.Add("Cost Price cannot be negative.");
            }

            // Stock quantity validation
            if (Product.StockQuantity < 0)
            {
                validationErrors.Add("Stock Quantity cannot be negative.");
            }

            // Reorder level validation
            if (Product.ReorderLevel < 0)
            {
                validationErrors.Add("Reorder Level cannot be negative.");
            }

            // Max stock level validation
            if (Product.MaxStockLevel < Product.ReorderLevel)
            {
                validationErrors.Add("Max Stock Level must be greater than or equal to Reorder Level.");
            }

            // Supplier validation
            if (string.IsNullOrWhiteSpace(Product.Supplier))
            {
                validationErrors.Add("Supplier is required.");
            }

            // Supplier email validation
            if (!string.IsNullOrWhiteSpace(Product.SupplierContactEmail) && !IsValidEmail(Product.SupplierContactEmail))
            {
                validationErrors.Add("Supplier Email format is invalid.");
            }

            ValidationSummary = string.Join(" ", validationErrors);
            OnPropertyChanged(nameof(HasValidationErrors));
        }

        private static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public void Cleanup()
        {
            if (_product != null)
            {
                _product.PropertyChanged -= OnProductPropertyChanged;
            }
        }

        #endregion
    }
}