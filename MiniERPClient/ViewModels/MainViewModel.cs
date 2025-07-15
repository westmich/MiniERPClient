using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using MiniERPClient.Commands;
using MiniERPClient.Models;
using MiniERPClient.Services;
using MiniERPClient.Views;

namespace MiniERPClient.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IEmployeeService _employeeService;
        private bool _isLoading;
        private string _searchText = string.Empty;
        private Employee? _selectedEmployee;
        private string _statusMessage = "Ready";

        public MainViewModel() : this(new EmployeeService())
        {
        }

        public MainViewModel(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
            
            // Initialize collections
            Employees = new ObservableCollection<Employee>();
            FilteredEmployees = new ObservableCollection<Employee>();
            
            // Initialize commands
            LoadEmployeesCommand = new RelayCommand(async () => await LoadEmployeesAsync(), () => !IsLoading);
            AddEmployeeCommand = new RelayCommand(AddEmployee, () => !IsLoading);
            EditEmployeeCommand = new RelayCommand(EditEmployee, () => SelectedEmployee != null && !IsLoading);
            DeleteEmployeeCommand = new RelayCommand(async () => await DeleteEmployeeAsync(), () => SelectedEmployee != null && !IsLoading);
            SearchCommand = new RelayCommand(PerformSearch);
            ClearSearchCommand = new RelayCommand(ClearSearch, () => !string.IsNullOrEmpty(SearchText));
            
            // Load initial data
            _ = LoadEmployeesAsync();
        }

        #region Properties

        public ObservableCollection<Employee> Employees { get; }
        public ObservableCollection<Employee> FilteredEmployees { get; }

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

        public Employee? SelectedEmployee
        {
            get => _selectedEmployee;
            set => SetProperty(ref _selectedEmployee, value);
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        #endregion

        #region Commands

        public ICommand LoadEmployeesCommand { get; }
        public ICommand AddEmployeeCommand { get; }
        public ICommand EditEmployeeCommand { get; }
        public ICommand DeleteEmployeeCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand ClearSearchCommand { get; }

        #endregion

        #region Methods

        private async Task LoadEmployeesAsync()
        {
            try
            {
                IsLoading = true;
                StatusMessage = "Loading employees...";

                // Use Dispatcher to ensure UI updates happen on UI thread
                await Application.Current.Dispatcher.InvokeAsync(async () =>
                {
                    var employees = await _employeeService.GetEmployeesAsync();
                    
                    Employees.Clear();
                    foreach (var employee in employees)
                    {
                        Employees.Add(employee);
                    }
                    
                    PerformSearch();
                    StatusMessage = $"Loaded {employees.Count} employees";
                });
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error loading employees: {ex.Message}";
                MessageBox.Show($"Error loading employees: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void AddEmployee()
        {
            var dialog = new EmployeeEditWindow();
            dialog.Owner = Application.Current.MainWindow;
            
            if (dialog.ShowDialog() == true && dialog.Result != null)
            {
                _ = SaveEmployeeAsync(dialog.Result, isNew: true);
            }
        }

        private void EditEmployee()
        {
            if (SelectedEmployee == null) return;

            var dialog = new EmployeeEditWindow(SelectedEmployee);
            dialog.Owner = Application.Current.MainWindow;
            
            if (dialog.ShowDialog() == true && dialog.Result != null)
            {
                _ = SaveEmployeeAsync(dialog.Result, isNew: false);
            }
        }

        private async Task SaveEmployeeAsync(Employee employee, bool isNew)
        {
            try
            {
                IsLoading = true;
                StatusMessage = isNew ? "Adding employee..." : "Updating employee...";

                Employee savedEmployee;
                if (isNew)
                {
                    savedEmployee = await _employeeService.AddEmployeeAsync(employee);
                    await Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        Employees.Add(savedEmployee);
                    });
                }
                else
                {
                    savedEmployee = await _employeeService.UpdateEmployeeAsync(employee);
                    await Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        var existingEmployee = Employees.FirstOrDefault(e => e.Id == savedEmployee.Id);
                        if (existingEmployee != null)
                        {
                            var index = Employees.IndexOf(existingEmployee);
                            Employees[index] = savedEmployee;
                        }
                    });
                }

                PerformSearch();
                StatusMessage = isNew ? "Employee added successfully" : "Employee updated successfully";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error saving employee: {ex.Message}";
                MessageBox.Show($"Error saving employee: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task DeleteEmployeeAsync()
        {
            if (SelectedEmployee == null) return;

            var result = MessageBox.Show(
                $"Are you sure you want to delete {SelectedEmployee.FullName}?",
                "Confirm Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes) return;

            try
            {
                IsLoading = true;
                StatusMessage = "Deleting employee...";

                var success = await _employeeService.DeleteEmployeeAsync(SelectedEmployee.Id);
                if (success)
                {
                    await Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        Employees.Remove(SelectedEmployee);
                        SelectedEmployee = null;
                    });
                    
                    PerformSearch();
                    StatusMessage = "Employee deleted successfully";
                }
                else
                {
                    StatusMessage = "Failed to delete employee";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error deleting employee: {ex.Message}";
                MessageBox.Show($"Error deleting employee: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
                FilteredEmployees.Clear();

                var searchResults = string.IsNullOrWhiteSpace(SearchText)
                    ? Employees
                    : Employees.Where(e =>
                        e.FullName.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                        e.Department.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                        e.Email.Contains(SearchText, StringComparison.OrdinalIgnoreCase));

                foreach (var employee in searchResults)
                {
                    FilteredEmployees.Add(employee);
                }
            });
        }

        private void ClearSearch()
        {
            SearchText = string.Empty;
        }

        #endregion
    }
}