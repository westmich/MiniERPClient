using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using MiniERPClient.Commands;
using MiniERPClient.Models;
using MiniERPClient.Services;
using MiniERPClient.Views;

namespace MiniERPClient.ViewModels
{
    public class ProductViewModel : ViewModelBase
    {
        private readonly IProductService _productService;
        private bool _isLoading;
        private string _searchText = string.Empty;
        private Product? _selectedProduct;
        private string _statusMessage = "Ready";
        private ProductCategory? _selectedCategoryFilter;
        private ProductStatus? _selectedStatusFilter;
        private StockAlertLevel? _selectedStockFilter;

        public ProductViewModel() : this(new ProductService())
        {
        }

        public ProductViewModel(IProductService productService)
        {
            _productService = productService;
            
            // Initialize collections
            Products = new ObservableCollection<Product>();
            FilteredProducts = new ObservableCollection<Product>();
            ProductSales = new ObservableCollection<ProductSalesData>();
            InventoryTransactions = new ObservableCollection<InventoryTransaction>();
            LowStockProducts = new ObservableCollection<Product>();
            
            // Initialize filter collections
            ProductCategories = new ObservableCollection<ProductCategory?>
            {
                null, // All
                ProductCategory.Software,
                ProductCategory.Hardware,
                ProductCategory.Services,
                ProductCategory.Consulting,
                ProductCategory.Licenses,
                ProductCategory.Support,
                ProductCategory.Training
            };
            
            ProductStatuses = new ObservableCollection<ProductStatus?>
            {
                null, // All
                ProductStatus.Active,
                ProductStatus.Discontinued,
                ProductStatus.OutOfStock,
                ProductStatus.LowStock,
                ProductStatus.ComingSoon
            };

            StockAlertLevels = new ObservableCollection<StockAlertLevel?>
            {
                null, // All
                StockAlertLevel.Normal,
                StockAlertLevel.Low,
                StockAlertLevel.Critical,
                StockAlertLevel.OutOfStock
            };
            
            // Initialize commands
            LoadProductsCommand = new RelayCommand(async () => await LoadProductsAsync(), () => !IsLoading);
            AddProductCommand = new RelayCommand(AddProduct, () => !IsLoading);
            EditProductCommand = new RelayCommand(EditProduct, () => SelectedProduct != null && !IsLoading);
            DeleteProductCommand = new RelayCommand(async () => await DeleteProductAsync(), () => SelectedProduct != null && !IsLoading);
            ViewSalesDataCommand = new RelayCommand(async () => await LoadSalesDataAsync(), () => SelectedProduct != null && !IsLoading);
            ViewInventoryCommand = new RelayCommand(async () => await LoadInventoryTransactionsAsync(), () => SelectedProduct != null && !IsLoading);
            UpdateStockCommand = new RelayCommand(async () => await UpdateStockAsync(), () => SelectedProduct != null && !IsLoading);
            LoadLowStockCommand = new RelayCommand(async () => await LoadLowStockProductsAsync(), () => !IsLoading);
            SearchCommand = new RelayCommand(PerformSearch);
            ClearSearchCommand = new RelayCommand(ClearSearch, () => !string.IsNullOrEmpty(SearchText));
            ApplyFiltersCommand = new RelayCommand(PerformSearch);
            
            // Load initial data
            _ = LoadProductsAsync();
            _ = LoadLowStockProductsAsync();
        }

        #region Properties

        public ObservableCollection<Product> Products { get; }
        public ObservableCollection<Product> FilteredProducts { get; }
        public ObservableCollection<ProductSalesData> ProductSales { get; }
        public ObservableCollection<InventoryTransaction> InventoryTransactions { get; }
        public ObservableCollection<Product> LowStockProducts { get; }
        public ObservableCollection<ProductCategory?> ProductCategories { get; }
        public ObservableCollection<ProductStatus?> ProductStatuses { get; }
        public ObservableCollection<StockAlertLevel?> StockAlertLevels { get; }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                {
                    PerformSearch();
                }
            }
        }

        public Product? SelectedProduct
        {
            get => _selectedProduct;
            set
            {
                if (SetProperty(ref _selectedProduct, value))
                {
                    _ = LoadSalesDataAsync();
                    _ = LoadInventoryTransactionsAsync();
                }
            }
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        public ProductCategory? SelectedCategoryFilter
        {
            get => _selectedCategoryFilter;
            set
            {
                if (SetProperty(ref _selectedCategoryFilter, value))
                {
                    PerformSearch();
                }
            }
        }

        public ProductStatus? SelectedStatusFilter
        {
            get => _selectedStatusFilter;
            set
            {
                if (SetProperty(ref _selectedStatusFilter, value))
                {
                    PerformSearch();
                }
            }
        }

        public StockAlertLevel? SelectedStockFilter
        {
            get => _selectedStockFilter;
            set
            {
                if (SetProperty(ref _selectedStockFilter, value))
                {
                    PerformSearch();
                }
            }
        }

        // Dashboard Properties
        public int TotalProducts => Products.Count;
        public int ActiveProducts => Products.Count(p => p.Status == ProductStatus.Active);
        public int LowStockCount => Products.Count(p => p.StockAlert == StockAlertLevel.Low || p.StockAlert == StockAlertLevel.Critical);
        public int OutOfStockCount => Products.Count(p => p.StockAlert == StockAlertLevel.OutOfStock);
        public decimal TotalInventoryValue => Products.Sum(p => p.StockQuantity * p.CostPrice);
        public decimal AverageMargin => Products.Any() ? Products.Average(p => p.ProfitMargin) : 0;

        #endregion

        #region Commands

        public ICommand LoadProductsCommand { get; }
        public ICommand AddProductCommand { get; }
        public ICommand EditProductCommand { get; }
        public ICommand DeleteProductCommand { get; }
        public ICommand ViewSalesDataCommand { get; }
        public ICommand ViewInventoryCommand { get; }
        public ICommand UpdateStockCommand { get; }
        public ICommand LoadLowStockCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand ClearSearchCommand { get; }
        public ICommand ApplyFiltersCommand { get; }

        #endregion

        #region Methods

        private async Task LoadProductsAsync()
        {
            try
            {
                IsLoading = true;
                StatusMessage = "Loading products...";

                await Application.Current.Dispatcher.InvokeAsync(async () =>
                {
                    var products = await _productService.GetProductsAsync();
                    
                    Products.Clear();
                    foreach (var product in products)
                    {
                        Products.Add(product);
                    }
                    
                    PerformSearch();
                    StatusMessage = $"Loaded {products.Count} products";
                    
                    // Notify dashboard properties changed
                    OnPropertyChanged(nameof(TotalProducts));
                    OnPropertyChanged(nameof(ActiveProducts));
                    OnPropertyChanged(nameof(LowStockCount));
                    OnPropertyChanged(nameof(OutOfStockCount));
                    OnPropertyChanged(nameof(TotalInventoryValue));
                    OnPropertyChanged(nameof(AverageMargin));
                });
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error loading products: {ex.Message}";
                MessageBox.Show($"Error loading products: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void AddProduct()
        {
            var dialog = new ProductEditWindow();
            dialog.Owner = Application.Current.MainWindow;
            
            if (dialog.ShowDialog() == true && dialog.Result != null)
            {
                _ = SaveProductAsync(dialog.Result, isNew: true);
            }
        }

        private void EditProduct()
        {
            if (SelectedProduct == null) return;

            var dialog = new ProductEditWindow(SelectedProduct);
            dialog.Owner = Application.Current.MainWindow;
            
            if (dialog.ShowDialog() == true && dialog.Result != null)
            {
                _ = SaveProductAsync(dialog.Result, isNew: false);
            }
        }

        private async Task SaveProductAsync(Product product, bool isNew)
        {
            try
            {
                IsLoading = true;
                StatusMessage = isNew ? "Adding product..." : "Updating product...";

                Product savedProduct;
                if (isNew)
                {
                    savedProduct = await _productService.AddProductAsync(product);
                    await Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        Products.Add(savedProduct);
                    });
                }
                else
                {
                    savedProduct = await _productService.UpdateProductAsync(product);
                    await Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        var existingProduct = Products.FirstOrDefault(p => p.Id == savedProduct.Id);
                        if (existingProduct != null)
                        {
                            var index = Products.IndexOf(existingProduct);
                            Products[index] = savedProduct;
                        }
                    });
                }

                PerformSearch();
                StatusMessage = isNew ? "Product added successfully" : "Product updated successfully";
                
                // Update dashboard properties
                OnPropertyChanged(nameof(TotalProducts));
                OnPropertyChanged(nameof(ActiveProducts));
                OnPropertyChanged(nameof(LowStockCount));
                OnPropertyChanged(nameof(OutOfStockCount));
                OnPropertyChanged(nameof(TotalInventoryValue));
                OnPropertyChanged(nameof(AverageMargin));
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error saving product: {ex.Message}";
                MessageBox.Show($"Error saving product: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task DeleteProductAsync()
        {
            if (SelectedProduct == null) return;

            var result = MessageBox.Show(
                $"Are you sure you want to delete {SelectedProduct.Name}?",
                "Confirm Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes) return;

            try
            {
                IsLoading = true;
                StatusMessage = "Deleting product...";

                var success = await _productService.DeleteProductAsync(SelectedProduct.Id);
                if (success)
                {
                    await Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        Products.Remove(SelectedProduct);
                        SelectedProduct = null;
                    });
                    
                    PerformSearch();
                    StatusMessage = "Product deleted successfully";
                    
                    // Update dashboard properties
                    OnPropertyChanged(nameof(TotalProducts));
                    OnPropertyChanged(nameof(ActiveProducts));
                    OnPropertyChanged(nameof(LowStockCount));
                    OnPropertyChanged(nameof(OutOfStockCount));
                    OnPropertyChanged(nameof(TotalInventoryValue));
                    OnPropertyChanged(nameof(AverageMargin));
                }
                else
                {
                    StatusMessage = "Failed to delete product";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error deleting product: {ex.Message}";
                MessageBox.Show($"Error deleting product: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task LoadSalesDataAsync()
        {
            if (SelectedProduct == null) return;

            try
            {
                StatusMessage = "Loading sales data...";
                
                var salesData = await _productService.GetProductSalesDataAsync(SelectedProduct.Id);
                
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    ProductSales.Clear();
                    foreach (var sale in salesData)
                    {
                        ProductSales.Add(sale);
                    }
                });
                
                StatusMessage = $"Loaded {salesData.Count} sales records for {SelectedProduct.Name}";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error loading sales data: {ex.Message}";
            }
        }

        private async Task LoadInventoryTransactionsAsync()
        {
            if (SelectedProduct == null) return;

            try
            {
                StatusMessage = "Loading inventory transactions...";
                
                var transactions = await _productService.GetInventoryTransactionsAsync(SelectedProduct.Id);
                
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    InventoryTransactions.Clear();
                    foreach (var transaction in transactions)
                    {
                        InventoryTransactions.Add(transaction);
                    }
                });
                
                StatusMessage = $"Loaded {transactions.Count} inventory transactions for {SelectedProduct.Name}";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error loading inventory transactions: {ex.Message}";
            }
        }

        private async Task LoadLowStockProductsAsync()
        {
            try
            {
                var lowStockProducts = await _productService.GetLowStockProductsAsync();
                
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    LowStockProducts.Clear();
                    foreach (var product in lowStockProducts)
                    {
                        LowStockProducts.Add(product);
                    }
                });
                
                StatusMessage = $"Found {lowStockProducts.Count} products requiring attention";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error loading low stock products: {ex.Message}";
            }
        }

        private async Task UpdateStockAsync()
        {
            if (SelectedProduct == null) return;

            // For demo purposes, let's add 10 units to stock
            var newQuantity = SelectedProduct.StockQuantity + 10;
            var reason = $"Stock replenishment - added 10 units";

            try
            {
                IsLoading = true;
                StatusMessage = "Updating stock...";

                var success = await _productService.UpdateStockAsync(SelectedProduct.Id, newQuantity, reason);
                if (success)
                {
                    await LoadProductsAsync(); // Reload to get updated data
                    await LoadInventoryTransactionsAsync(); // Reload transactions
                    StatusMessage = "Stock updated successfully";
                }
                else
                {
                    StatusMessage = "Failed to update stock";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error updating stock: {ex.Message}";
                MessageBox.Show($"Error updating stock: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void PerformSearch()
        {
            Application.Current.Dispatcher.InvokeAsync(() =>
            {
                FilteredProducts.Clear();

                var searchResults = Products.AsEnumerable();

                // Apply text search
                if (!string.IsNullOrWhiteSpace(SearchText))
                {
                    searchResults = searchResults.Where(p =>
                        p.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                        p.SKU.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                        p.Description.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                        p.Supplier.Contains(SearchText, StringComparison.OrdinalIgnoreCase));
                }

                // Apply category filter
                if (SelectedCategoryFilter.HasValue)
                {
                    searchResults = searchResults.Where(p => p.Category == SelectedCategoryFilter.Value);
                }

                // Apply status filter
                if (SelectedStatusFilter.HasValue)
                {
                    searchResults = searchResults.Where(p => p.Status == SelectedStatusFilter.Value);
                }

                // Apply stock alert filter
                if (SelectedStockFilter.HasValue)
                {
                    searchResults = searchResults.Where(p => p.StockAlert == SelectedStockFilter.Value);
                }

                foreach (var product in searchResults.OrderBy(p => p.Name))
                {
                    FilteredProducts.Add(product);
                }
            });
        }

        private void ClearSearch()
        {
            SearchText = string.Empty;
            SelectedCategoryFilter = null;
            SelectedStatusFilter = null;
            SelectedStockFilter = null;
        }

        #endregion
    }
}