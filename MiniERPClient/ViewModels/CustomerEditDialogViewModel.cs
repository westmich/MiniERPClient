using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using MiniERPClient.Commands;
using MiniERPClient.Models;

namespace MiniERPClient.ViewModels
{
    public class CustomerEditDialogViewModel : ViewModelBase
    {
        private Customer _customer;
        private bool _isNewCustomer;
        private bool _isSaving;
        private string _validationSummary = string.Empty;

        public CustomerEditDialogViewModel(Customer? customer = null)
        {
            _isNewCustomer = customer == null;
            _customer = customer?.Clone() ?? new Customer
            {
                CompanyName = string.Empty,
                ContactFirstName = string.Empty,
                ContactLastName = string.Empty,
                Email = string.Empty,
                Phone = string.Empty,
                Address = string.Empty,
                City = string.Empty,
                State = string.Empty,
                ZipCode = string.Empty,
                Country = "USA",
                Type = CustomerType.Business,
                Status = CustomerStatus.Prospect,
                CreditLimit = 10000,
                TotalSales = 0,
                IsActive = true
            };

            // Initialize available types and statuses
            AvailableTypes = new ObservableCollection<CustomerType>
            {
                CustomerType.Individual,
                CustomerType.Business,
                CustomerType.Enterprise
            };

            AvailableStatuses = new ObservableCollection<CustomerStatus>
            {
                CustomerStatus.Prospect,
                CustomerStatus.Active,
                CustomerStatus.Inactive,
                CustomerStatus.Churned
            };

            AvailableCountries = new ObservableCollection<string>
            {
                "USA", "Canada", "Mexico", "United Kingdom", "Germany", "France", "Spain", "Italy", "Australia", "Japan", "China", "India", "Brazil"
            };

            // Initialize commands
            SaveCommand = new RelayCommand(Save, CanSave);
            CancelCommand = new RelayCommand(Cancel);

            // Subscribe to property changes on the customer to trigger validation
            _customer.PropertyChanged += OnCustomerPropertyChanged;

            // Validate initially
            ValidateCustomer();
        }

        #region Properties

        public Customer Customer
        {
            get => _customer;
            set
            {
                if (_customer != null)
                {
                    _customer.PropertyChanged -= OnCustomerPropertyChanged;
                }
                
                if (SetProperty(ref _customer, value))
                {
                    if (_customer != null)
                    {
                        _customer.PropertyChanged += OnCustomerPropertyChanged;
                    }
                    ValidateCustomer();
                }
            }
        }

        public bool IsNewCustomer
        {
            get => _isNewCustomer;
            set => SetProperty(ref _isNewCustomer, value);
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

        public ObservableCollection<CustomerType> AvailableTypes { get; }
        public ObservableCollection<CustomerStatus> AvailableStatuses { get; }
        public ObservableCollection<string> AvailableCountries { get; }

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

        private void OnCustomerPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            ValidateCustomer();
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

        private void ValidateCustomer()
        {
            var validationErrors = new List<string>();

            // Contact name validation (at least one name is required)
            if (string.IsNullOrWhiteSpace(Customer.ContactFirstName) && string.IsNullOrWhiteSpace(Customer.ContactLastName))
            {
                validationErrors.Add("At least First Name or Last Name is required.");
            }

            // For business customers, company name should be provided
            if (Customer.Type != CustomerType.Individual && string.IsNullOrWhiteSpace(Customer.CompanyName))
            {
                validationErrors.Add("Company Name is required for business customers.");
            }

            // Email validation
            if (string.IsNullOrWhiteSpace(Customer.Email))
            {
                validationErrors.Add("Email is required.");
            }
            else if (!IsValidEmail(Customer.Email))
            {
                validationErrors.Add("Email format is invalid.");
            }

            // Phone validation
            if (string.IsNullOrWhiteSpace(Customer.Phone))
            {
                validationErrors.Add("Phone number is required.");
            }

            // Credit limit validation
            if (Customer.CreditLimit < 0)
            {
                validationErrors.Add("Credit Limit cannot be negative.");
            }

            // Total sales validation
            if (Customer.TotalSales < 0)
            {
                validationErrors.Add("Total Sales cannot be negative.");
            }

            // Country validation
            if (string.IsNullOrWhiteSpace(Customer.Country))
            {
                validationErrors.Add("Country is required.");
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
            if (_customer != null)
            {
                _customer.PropertyChanged -= OnCustomerPropertyChanged;
            }
        }

        #endregion
    }
}