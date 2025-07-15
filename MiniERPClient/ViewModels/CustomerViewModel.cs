using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using MiniERPClient.Commands;
using MiniERPClient.Models;
using MiniERPClient.Services;
using MiniERPClient.Views;

namespace MiniERPClient.ViewModels
{
    public class CustomerViewModel : ViewModelBase
    {
        private readonly ICustomerService _customerService;
        private bool _isLoading;
        private string _searchText = string.Empty;
        private Customer? _selectedCustomer;
        private string _statusMessage = "Ready";
        private CustomerStatus? _selectedStatusFilter;
        private CustomerType? _selectedTypeFilter;

        public CustomerViewModel() : this(new CustomerService())
        {
        }

        public CustomerViewModel(ICustomerService customerService)
        {
            _customerService = customerService;
            
            // Initialize collections
            Customers = new ObservableCollection<Customer>();
            FilteredCustomers = new ObservableCollection<Customer>();
            CustomerOpportunities = new ObservableCollection<SalesOpportunity>();
            
            // Initialize filter collections
            CustomerStatuses = new ObservableCollection<CustomerStatus?>
            {
                null, // All
                CustomerStatus.Prospect,
                CustomerStatus.Active,
                CustomerStatus.Inactive,
                CustomerStatus.Churned
            };
            
            CustomerTypes = new ObservableCollection<CustomerType?>
            {
                null, // All
                CustomerType.Individual,
                CustomerType.Business,
                CustomerType.Enterprise
            };
            
            // Initialize commands
            LoadCustomersCommand = new RelayCommand(async () => await LoadCustomersAsync(), () => !IsLoading);
            AddCustomerCommand = new RelayCommand(AddCustomer, () => !IsLoading);
            EditCustomerCommand = new RelayCommand(EditCustomer, () => SelectedCustomer != null && !IsLoading);
            DeleteCustomerCommand = new RelayCommand(async () => await DeleteCustomerAsync(), () => SelectedCustomer != null && !IsLoading);
            ViewOpportunitiesCommand = new RelayCommand(async () => await LoadOpportunitiesAsync(), () => SelectedCustomer != null && !IsLoading);
            SearchCommand = new RelayCommand(PerformSearch);
            ClearSearchCommand = new RelayCommand(ClearSearch, () => !string.IsNullOrEmpty(SearchText));
            ApplyFiltersCommand = new RelayCommand(PerformSearch);
            
            // Load initial data
            _ = LoadCustomersAsync();
        }

        #region Properties

        public ObservableCollection<Customer> Customers { get; }
        public ObservableCollection<Customer> FilteredCustomers { get; }
        public ObservableCollection<SalesOpportunity> CustomerOpportunities { get; }
        public ObservableCollection<CustomerStatus?> CustomerStatuses { get; }
        public ObservableCollection<CustomerType?> CustomerTypes { get; }

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

        public Customer? SelectedCustomer
        {
            get => _selectedCustomer;
            set
            {
                if (SetProperty(ref _selectedCustomer, value))
                {
                    _ = LoadOpportunitiesAsync();
                }
            }
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        public CustomerStatus? SelectedStatusFilter
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

        public CustomerType? SelectedTypeFilter
        {
            get => _selectedTypeFilter;
            set
            {
                if (SetProperty(ref _selectedTypeFilter, value))
                {
                    PerformSearch();
                }
            }
        }

        // Summary Properties for Dashboard-like functionality
        public int TotalCustomers => Customers.Count;
        public int ActiveCustomers => Customers.Count(c => c.Status == CustomerStatus.Active);
        public int ProspectCustomers => Customers.Count(c => c.Status == CustomerStatus.Prospect);
        public decimal TotalSalesValue => Customers.Sum(c => c.TotalSales);
        public decimal AverageCreditLimit => Customers.Any() ? Customers.Average(c => c.CreditLimit) : 0;

        #endregion

        #region Commands

        public ICommand LoadCustomersCommand { get; }
        public ICommand AddCustomerCommand { get; }
        public ICommand EditCustomerCommand { get; }
        public ICommand DeleteCustomerCommand { get; }
        public ICommand ViewOpportunitiesCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand ClearSearchCommand { get; }
        public ICommand ApplyFiltersCommand { get; }

        #endregion

        #region Methods

        private async Task LoadCustomersAsync()
        {
            try
            {
                IsLoading = true;
                StatusMessage = "Loading customers...";

                await Application.Current.Dispatcher.InvokeAsync(async () =>
                {
                    var customers = await _customerService.GetCustomersAsync();
                    
                    Customers.Clear();
                    foreach (var customer in customers)
                    {
                        Customers.Add(customer);
                    }
                    
                    PerformSearch();
                    StatusMessage = $"Loaded {customers.Count} customers";
                    
                    // Notify summary properties changed
                    OnPropertyChanged(nameof(TotalCustomers));
                    OnPropertyChanged(nameof(ActiveCustomers));
                    OnPropertyChanged(nameof(ProspectCustomers));
                    OnPropertyChanged(nameof(TotalSalesValue));
                    OnPropertyChanged(nameof(AverageCreditLimit));
                });
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error loading customers: {ex.Message}";
                MessageBox.Show($"Error loading customers: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void AddCustomer()
        {
            var dialog = new CustomerEditWindow();
            dialog.Owner = Application.Current.MainWindow;
            
            if (dialog.ShowDialog() == true && dialog.Result != null)
            {
                _ = SaveCustomerAsync(dialog.Result, isNew: true);
            }
        }

        private void EditCustomer()
        {
            if (SelectedCustomer == null) return;

            var dialog = new CustomerEditWindow(SelectedCustomer);
            dialog.Owner = Application.Current.MainWindow;
            
            if (dialog.ShowDialog() == true && dialog.Result != null)
            {
                _ = SaveCustomerAsync(dialog.Result, isNew: false);
            }
        }

        private async Task SaveCustomerAsync(Customer customer, bool isNew)
        {
            try
            {
                IsLoading = true;
                StatusMessage = isNew ? "Adding customer..." : "Updating customer...";

                Customer savedCustomer;
                if (isNew)
                {
                    savedCustomer = await _customerService.AddCustomerAsync(customer);
                    await Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        Customers.Add(savedCustomer);
                    });
                }
                else
                {
                    savedCustomer = await _customerService.UpdateCustomerAsync(customer);
                    await Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        var existingCustomer = Customers.FirstOrDefault(c => c.Id == savedCustomer.Id);
                        if (existingCustomer != null)
                        {
                            var index = Customers.IndexOf(existingCustomer);
                            Customers[index] = savedCustomer;
                        }
                    });
                }

                PerformSearch();
                StatusMessage = isNew ? "Customer added successfully" : "Customer updated successfully";
                
                // Update summary properties
                OnPropertyChanged(nameof(TotalCustomers));
                OnPropertyChanged(nameof(ActiveCustomers));
                OnPropertyChanged(nameof(ProspectCustomers));
                OnPropertyChanged(nameof(TotalSalesValue));
                OnPropertyChanged(nameof(AverageCreditLimit));
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error saving customer: {ex.Message}";
                MessageBox.Show($"Error saving customer: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task DeleteCustomerAsync()
        {
            if (SelectedCustomer == null) return;

            var result = MessageBox.Show(
                $"Are you sure you want to delete {SelectedCustomer.DisplayName}?",
                "Confirm Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes) return;

            try
            {
                IsLoading = true;
                StatusMessage = "Deleting customer...";

                var success = await _customerService.DeleteCustomerAsync(SelectedCustomer.Id);
                if (success)
                {
                    await Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        Customers.Remove(SelectedCustomer);
                        SelectedCustomer = null;
                    });
                    
                    PerformSearch();
                    StatusMessage = "Customer deleted successfully";
                    
                    // Update summary properties
                    OnPropertyChanged(nameof(TotalCustomers));
                    OnPropertyChanged(nameof(ActiveCustomers));
                    OnPropertyChanged(nameof(ProspectCustomers));
                    OnPropertyChanged(nameof(TotalSalesValue));
                    OnPropertyChanged(nameof(AverageCreditLimit));
                }
                else
                {
                    StatusMessage = "Failed to delete customer";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error deleting customer: {ex.Message}";
                MessageBox.Show($"Error deleting customer: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task LoadOpportunitiesAsync()
        {
            if (SelectedCustomer == null) return;

            try
            {
                StatusMessage = "Loading opportunities...";
                
                var opportunities = await _customerService.GetOpportunitiesByCustomerIdAsync(SelectedCustomer.Id);
                
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    CustomerOpportunities.Clear();
                    foreach (var opportunity in opportunities)
                    {
                        CustomerOpportunities.Add(opportunity);
                    }
                });
                
                StatusMessage = $"Loaded {opportunities.Count} opportunities for {SelectedCustomer.DisplayName}";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error loading opportunities: {ex.Message}";
            }
        }

        private void PerformSearch()
        {
            Application.Current.Dispatcher.InvokeAsync(() =>
            {
                FilteredCustomers.Clear();

                var searchResults = Customers.AsEnumerable();

                // Apply text search
                if (!string.IsNullOrWhiteSpace(SearchText))
                {
                    searchResults = searchResults.Where(c =>
                        c.DisplayName.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                        c.ContactFullName.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                        c.Email.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                        c.Phone.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                        c.City.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                        c.State.Contains(SearchText, StringComparison.OrdinalIgnoreCase));
                }

                // Apply status filter
                if (SelectedStatusFilter.HasValue)
                {
                    searchResults = searchResults.Where(c => c.Status == SelectedStatusFilter.Value);
                }

                // Apply type filter
                if (SelectedTypeFilter.HasValue)
                {
                    searchResults = searchResults.Where(c => c.Type == SelectedTypeFilter.Value);
                }

                foreach (var customer in searchResults.OrderBy(c => c.DisplayName))
                {
                    FilteredCustomers.Add(customer);
                }
            });
        }

        private void ClearSearch()
        {
            SearchText = string.Empty;
            SelectedStatusFilter = null;
            SelectedTypeFilter = null;
        }

        #endregion
    }
}