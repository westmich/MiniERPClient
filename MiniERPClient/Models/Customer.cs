using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MiniERPClient.Models
{
    public enum CustomerType
    {
        Individual,
        Business,
        Enterprise
    }

    public enum CustomerStatus
    {
        Prospect,
        Active,
        Inactive,
        Churned
    }

    public class Customer : INotifyPropertyChanged
    {
        private int _id;
        private string _companyName = string.Empty;
        private string _contactFirstName = string.Empty;
        private string _contactLastName = string.Empty;
        private string _email = string.Empty;
        private string _phone = string.Empty;
        private string _address = string.Empty;
        private string _city = string.Empty;
        private string _state = string.Empty;
        private string _zipCode = string.Empty;
        private string _country = string.Empty;
        private CustomerType _type = CustomerType.Business;
        private CustomerStatus _status = CustomerStatus.Prospect;
        private decimal _creditLimit;
        private decimal _totalSales;
        private DateTime _createdDate = DateTime.Now;
        private DateTime? _lastContactDate;
        private string _notes = string.Empty;
        private bool _isActive = true;

        public int Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        public string CompanyName
        {
            get => _companyName;
            set
            {
                if (SetProperty(ref _companyName, value))
                {
                    OnPropertyChanged(nameof(DisplayName));
                }
            }
        }

        public string ContactFirstName
        {
            get => _contactFirstName;
            set
            {
                if (SetProperty(ref _contactFirstName, value))
                {
                    OnPropertyChanged(nameof(ContactFullName));
                    OnPropertyChanged(nameof(DisplayName));
                }
            }
        }

        public string ContactLastName
        {
            get => _contactLastName;
            set
            {
                if (SetProperty(ref _contactLastName, value))
                {
                    OnPropertyChanged(nameof(ContactFullName));
                    OnPropertyChanged(nameof(DisplayName));
                }
            }
        }

        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        public string Phone
        {
            get => _phone;
            set => SetProperty(ref _phone, value);
        }

        public string Address
        {
            get => _address;
            set
            {
                if (SetProperty(ref _address, value))
                {
                    OnPropertyChanged(nameof(FullAddress));
                }
            }
        }

        public string City
        {
            get => _city;
            set
            {
                if (SetProperty(ref _city, value))
                {
                    OnPropertyChanged(nameof(FullAddress));
                }
            }
        }

        public string State
        {
            get => _state;
            set
            {
                if (SetProperty(ref _state, value))
                {
                    OnPropertyChanged(nameof(FullAddress));
                }
            }
        }

        public string ZipCode
        {
            get => _zipCode;
            set
            {
                if (SetProperty(ref _zipCode, value))
                {
                    OnPropertyChanged(nameof(FullAddress));
                }
            }
        }

        public string Country
        {
            get => _country;
            set => SetProperty(ref _country, value);
        }

        public CustomerType Type
        {
            get => _type;
            set
            {
                if (SetProperty(ref _type, value))
                {
                    OnPropertyChanged(nameof(TypeBadge));
                }
            }
        }

        public CustomerStatus Status
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

        public decimal CreditLimit
        {
            get => _creditLimit;
            set => SetProperty(ref _creditLimit, value);
        }

        public decimal TotalSales
        {
            get => _totalSales;
            set => SetProperty(ref _totalSales, value);
        }

        public DateTime CreatedDate
        {
            get => _createdDate;
            set => SetProperty(ref _createdDate, value);
        }

        public DateTime? LastContactDate
        {
            get => _lastContactDate;
            set => SetProperty(ref _lastContactDate, value);
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
        public string ContactFullName => $"{ContactFirstName} {ContactLastName}".Trim();
        public string DisplayName => string.IsNullOrEmpty(CompanyName) ? ContactFullName : CompanyName;
        public string FullAddress => $"{Address}, {City}, {State} {ZipCode}".Trim(new char[] { ',', ' ' });
        public string StatusBadge => Status.ToString();
        public string TypeBadge => Type.ToString();

        public Customer Clone()
        {
            return new Customer
            {
                Id = this.Id,
                CompanyName = this.CompanyName,
                ContactFirstName = this.ContactFirstName,
                ContactLastName = this.ContactLastName,
                Email = this.Email,
                Phone = this.Phone,
                Address = this.Address,
                City = this.City,
                State = this.State,
                ZipCode = this.ZipCode,
                Country = this.Country,
                Type = this.Type,
                Status = this.Status,
                CreditLimit = this.CreditLimit,
                TotalSales = this.TotalSales,
                CreatedDate = this.CreatedDate,
                LastContactDate = this.LastContactDate,
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
}