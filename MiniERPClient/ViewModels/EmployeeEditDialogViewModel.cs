using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Windows;
using System.Windows.Input;
using MiniERPClient.Commands;
using MiniERPClient.Models;

namespace MiniERPClient.ViewModels
{
    public class EmployeeEditDialogViewModel : ViewModelBase
    {
        private Employee _employee;
        private bool _isNewEmployee;
        private bool _isSaving;
        private string _validationSummary = string.Empty;

        public EmployeeEditDialogViewModel(Employee? employee = null)
        {
            _isNewEmployee = employee == null;
            _employee = employee?.Clone() ?? new Employee
            {
                FirstName = string.Empty,
                LastName = string.Empty,
                Email = string.Empty,
                Department = "IT",
                Salary = 50000,
                HireDate = DateTime.Today,
                IsActive = true
            };

            // Initialize available departments
            AvailableDepartments = new ObservableCollection<string>
            {
                "IT", "HR", "Finance", "Marketing", "Sales", "Operations", "Legal", "R&D"
            };

            // Initialize commands
            SaveCommand = new RelayCommand(Save, CanSave);
            CancelCommand = new RelayCommand(Cancel);

            // Subscribe to property changes on the employee to trigger validation
            _employee.PropertyChanged += OnEmployeePropertyChanged;

            // Validate initially
            ValidateEmployee();
        }

        #region Properties

        public Employee Employee
        {
            get => _employee;
            set
            {
                if (_employee != null)
                {
                    _employee.PropertyChanged -= OnEmployeePropertyChanged;
                }
                
                if (SetProperty(ref _employee, value))
                {
                    if (_employee != null)
                    {
                        _employee.PropertyChanged += OnEmployeePropertyChanged;
                    }
                    ValidateEmployee();
                }
            }
        }

        public bool IsNewEmployee
        {
            get => _isNewEmployee;
            set => SetProperty(ref _isNewEmployee, value);
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

        public ObservableCollection<string> AvailableDepartments { get; }

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

        private void OnEmployeePropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            ValidateEmployee();
            // Refresh the CanExecute state of commands
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

        private void ValidateEmployee()
        {
            var validationErrors = new List<string>();

            // First Name validation
            if (string.IsNullOrWhiteSpace(Employee.FirstName))
            {
                validationErrors.Add("First Name is required.");
            }

            // Last Name validation
            if (string.IsNullOrWhiteSpace(Employee.LastName))
            {
                validationErrors.Add("Last Name is required.");
            }

            // Email validation
            if (string.IsNullOrWhiteSpace(Employee.Email))
            {
                validationErrors.Add("Email is required.");
            }
            else if (!IsValidEmail(Employee.Email))
            {
                validationErrors.Add("Email format is invalid.");
            }

            // Department validation
            if (string.IsNullOrWhiteSpace(Employee.Department))
            {
                validationErrors.Add("Department is required.");
            }

            // Salary validation
            if (Employee.Salary <= 0)
            {
                validationErrors.Add("Salary must be greater than 0.");
            }

            // Hire Date validation
            if (Employee.HireDate > DateTime.Today)
            {
                validationErrors.Add("Hire Date cannot be in the future.");
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
            if (_employee != null)
            {
                _employee.PropertyChanged -= OnEmployeePropertyChanged;
            }
        }

        #endregion
    }
}