using MiniERPClient.Models;

namespace MiniERPClient.Services
{
    public interface IEmployeeService
    {
        Task<List<Employee>> GetEmployeesAsync();
        Task<Employee?> GetEmployeeByIdAsync(int id);
        Task<Employee> AddEmployeeAsync(Employee employee);
        Task<Employee> UpdateEmployeeAsync(Employee employee);
        Task<bool> DeleteEmployeeAsync(int id);
    }

    public class EmployeeService : IEmployeeService
    {
        private readonly List<Employee> _employees;
        private int _nextId = 1;

        public EmployeeService()
        {
            // Sample data
            _employees = new List<Employee>
            {
                new Employee { Id = _nextId++, FirstName = "John", LastName = "Doe", Email = "john.doe@company.com", Department = "IT", Salary = 75000, HireDate = DateTime.Now.AddYears(-2) },
                new Employee { Id = _nextId++, FirstName = "Jane", LastName = "Smith", Email = "jane.smith@company.com", Department = "HR", Salary = 65000, HireDate = DateTime.Now.AddYears(-1) },
                new Employee { Id = _nextId++, FirstName = "Bob", LastName = "Johnson", Email = "bob.johnson@company.com", Department = "Finance", Salary = 80000, HireDate = DateTime.Now.AddMonths(-6) },
                new Employee { Id = _nextId++, FirstName = "Alice", LastName = "Brown", Email = "alice.brown@company.com", Department = "Marketing", Salary = 70000, HireDate = DateTime.Now.AddMonths(-3) }
            };
        }

        public async Task<List<Employee>> GetEmployeesAsync()
        {
            // Simulate async operation
            await Task.Delay(100);
            return new List<Employee>(_employees);
        }

        public async Task<Employee?> GetEmployeeByIdAsync(int id)
        {
            await Task.Delay(50);
            return _employees.FirstOrDefault(e => e.Id == id);
        }

        public async Task<Employee> AddEmployeeAsync(Employee employee)
        {
            await Task.Delay(100);
            employee.Id = _nextId++;
            _employees.Add(employee);
            return employee;
        }

        public async Task<Employee> UpdateEmployeeAsync(Employee employee)
        {
            await Task.Delay(100);
            var existingEmployee = _employees.FirstOrDefault(e => e.Id == employee.Id);
            if (existingEmployee != null)
            {
                var index = _employees.IndexOf(existingEmployee);
                _employees[index] = employee;
            }
            return employee;
        }

        public async Task<bool> DeleteEmployeeAsync(int id)
        {
            await Task.Delay(100);
            var employee = _employees.FirstOrDefault(e => e.Id == id);
            if (employee != null)
            {
                _employees.Remove(employee);
                return true;
            }
            return false;
        }
    }
}