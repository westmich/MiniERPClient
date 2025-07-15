using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MiniERPClient.Models
{
    public class Employee : INotifyPropertyChanged
    {
        private int _id;
        private string _firstName = string.Empty;
        private string _lastName = string.Empty;
        private string _email = string.Empty;
        private string _department = string.Empty;
        private decimal _salary;
        private DateTime _hireDate;
        private bool _isActive = true;

        public int Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        public string FirstName
        {
            get => _firstName;
            set
            {
                if (SetProperty(ref _firstName, value))
                {
                    OnPropertyChanged(nameof(FullName));
                    OnPropertyChanged(nameof(DisplayText));
                }
            }
        }

        public string LastName
        {
            get => _lastName;
            set
            {
                if (SetProperty(ref _lastName, value))
                {
                    OnPropertyChanged(nameof(FullName));
                    OnPropertyChanged(nameof(DisplayText));
                }
            }
        }

        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        public string Department
        {
            get => _department;
            set
            {
                if (SetProperty(ref _department, value))
                {
                    OnPropertyChanged(nameof(DisplayText));
                }
            }
        }

        public decimal Salary
        {
            get => _salary;
            set => SetProperty(ref _salary, value);
        }

        public DateTime HireDate
        {
            get => _hireDate;
            set => SetProperty(ref _hireDate, value);
        }

        public bool IsActive
        {
            get => _isActive;
            set => SetProperty(ref _isActive, value);
        }

        public string FullName => $"{FirstName} {LastName}";
        public string DisplayText => $"{FullName} - {Department}";

        public Employee Clone()
        {
            return new Employee
            {
                Id = this.Id,
                FirstName = this.FirstName,
                LastName = this.LastName,
                Email = this.Email,
                Department = this.Department,
                Salary = this.Salary,
                HireDate = this.HireDate,
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
}