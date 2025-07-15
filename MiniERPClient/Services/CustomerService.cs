using MiniERPClient.Models;

namespace MiniERPClient.Services
{
    public interface ICustomerService
    {
        Task<List<Customer>> GetCustomersAsync();
        Task<Customer?> GetCustomerByIdAsync(int id);
        Task<Customer> AddCustomerAsync(Customer customer);
        Task<Customer> UpdateCustomerAsync(Customer customer);
        Task<bool> DeleteCustomerAsync(int id);
        Task<List<SalesOpportunity>> GetOpportunitiesByCustomerIdAsync(int customerId);
    }

    public class CustomerService : ICustomerService
    {
        private readonly List<Customer> _customers;
        private int _nextId = 1;

        public CustomerService()
        {
            // Sample customer data
            _customers = new List<Customer>
            {
                new Customer 
                { 
                    Id = _nextId++, 
                    CompanyName = "Acme Corporation", 
                    ContactFirstName = "John", 
                    ContactLastName = "Smith",
                    Email = "john.smith@acme.com", 
                    Phone = "(555) 123-4567",
                    Address = "123 Business Ave",
                    City = "New York",
                    State = "NY",
                    ZipCode = "10001",
                    Country = "USA",
                    Type = CustomerType.Enterprise,
                    Status = CustomerStatus.Active,
                    CreditLimit = 100000,
                    TotalSales = 250000,
                    CreatedDate = DateTime.Now.AddMonths(-12),
                    LastContactDate = DateTime.Now.AddDays(-5),
                    Notes = "Key enterprise customer with multiple departments"
                },
                new Customer 
                { 
                    Id = _nextId++, 
                    CompanyName = "TechStart Inc", 
                    ContactFirstName = "Sarah", 
                    ContactLastName = "Johnson",
                    Email = "sarah@techstart.com", 
                    Phone = "(555) 987-6543",
                    Address = "456 Startup Lane",
                    City = "San Francisco",
                    State = "CA",
                    ZipCode = "94105",
                    Country = "USA",
                    Type = CustomerType.Business,
                    Status = CustomerStatus.Active,
                    CreditLimit = 50000,
                    TotalSales = 75000,
                    CreatedDate = DateTime.Now.AddMonths(-6),
                    LastContactDate = DateTime.Now.AddDays(-2),
                    Notes = "Growing startup with expansion plans"
                },
                new Customer 
                { 
                    Id = _nextId++, 
                    CompanyName = "", 
                    ContactFirstName = "Michael", 
                    ContactLastName = "Brown",
                    Email = "michael.brown@email.com", 
                    Phone = "(555) 456-7890",
                    Address = "789 Residential St",
                    City = "Chicago",
                    State = "IL",
                    ZipCode = "60601",
                    Country = "USA",
                    Type = CustomerType.Individual,
                    Status = CustomerStatus.Prospect,
                    CreditLimit = 5000,
                    TotalSales = 0,
                    CreatedDate = DateTime.Now.AddDays(-15),
                    LastContactDate = DateTime.Now.AddDays(-10),
                    Notes = "Individual customer interested in small business solution"
                },
                new Customer 
                { 
                    Id = _nextId++, 
                    CompanyName = "Global Enterprises Ltd", 
                    ContactFirstName = "Lisa", 
                    ContactLastName = "Davis",
                    Email = "lisa.davis@global-ent.com", 
                    Phone = "(555) 321-0987",
                    Address = "321 Corporate Blvd",
                    City = "Houston",
                    State = "TX",
                    ZipCode = "77001",
                    Country = "USA",
                    Type = CustomerType.Enterprise,
                    Status = CustomerStatus.Active,
                    CreditLimit = 200000,
                    TotalSales = 500000,
                    CreatedDate = DateTime.Now.AddMonths(-18),
                    LastContactDate = DateTime.Now.AddDays(-1),
                    Notes = "Top tier customer with international operations"
                },
                new Customer 
                { 
                    Id = _nextId++, 
                    CompanyName = "Dormant Corp", 
                    ContactFirstName = "Robert", 
                    ContactLastName = "Wilson",
                    Email = "robert@dormant.com", 
                    Phone = "(555) 111-2222",
                    Address = "111 Old Street",
                    City = "Boston",
                    State = "MA",
                    ZipCode = "02101",
                    Country = "USA",
                    Type = CustomerType.Business,
                    Status = CustomerStatus.Inactive,
                    CreditLimit = 25000,
                    TotalSales = 15000,
                    CreatedDate = DateTime.Now.AddMonths(-24),
                    LastContactDate = DateTime.Now.AddMonths(-6),
                    Notes = "Previously active customer, needs re-engagement"
                }
            };
        }

        public async Task<List<Customer>> GetCustomersAsync()
        {
            // Simulate async operation
            await Task.Delay(150);
            return new List<Customer>(_customers);
        }

        public async Task<Customer?> GetCustomerByIdAsync(int id)
        {
            await Task.Delay(50);
            return _customers.FirstOrDefault(c => c.Id == id);
        }

        public async Task<Customer> AddCustomerAsync(Customer customer)
        {
            await Task.Delay(200);
            customer.Id = _nextId++;
            customer.CreatedDate = DateTime.Now;
            _customers.Add(customer);
            return customer;
        }

        public async Task<Customer> UpdateCustomerAsync(Customer customer)
        {
            await Task.Delay(150);
            var existingCustomer = _customers.FirstOrDefault(c => c.Id == customer.Id);
            if (existingCustomer != null)
            {
                var index = _customers.IndexOf(existingCustomer);
                _customers[index] = customer;
            }
            return customer;
        }

        public async Task<bool> DeleteCustomerAsync(int id)
        {
            await Task.Delay(100);
            var customer = _customers.FirstOrDefault(c => c.Id == id);
            if (customer != null)
            {
                _customers.Remove(customer);
                return true;
            }
            return false;
        }

        public async Task<List<SalesOpportunity>> GetOpportunitiesByCustomerIdAsync(int customerId)
        {
            await Task.Delay(100);
            
            // Sample opportunities for demonstration
            var opportunities = new List<SalesOpportunity>();
            
            if (customerId == 1) // Acme Corporation
            {
                opportunities.AddRange(new[]
                {
                    new SalesOpportunity 
                    { 
                        Id = 1, 
                        CustomerId = customerId, 
                        Title = "Enterprise Software License",
                        Description = "Annual software license renewal with expanded user base",
                        EstimatedValue = 150000,
                        ProbabilityPercent = 90,
                        Stage = SalesOpportunityStage.Negotiation,
                        ExpectedCloseDate = DateTime.Now.AddDays(15),
                        SalesRepresentative = "Jane Smith",
                        Notes = "Strong renewal opportunity with expansion"
                    },
                    new SalesOpportunity 
                    { 
                        Id = 2, 
                        CustomerId = customerId, 
                        Title = "Professional Services",
                        Description = "Implementation and training services",
                        EstimatedValue = 50000,
                        ProbabilityPercent = 70,
                        Stage = SalesOpportunityStage.Proposal,
                        ExpectedCloseDate = DateTime.Now.AddDays(30),
                        SalesRepresentative = "Jane Smith",
                        Notes = "Additional services for new deployment"
                    }
                });
            }
            else if (customerId == 2) // TechStart Inc
            {
                opportunities.Add(new SalesOpportunity 
                { 
                    Id = 3, 
                    CustomerId = customerId, 
                    Title = "Starter Package Upgrade",
                    Description = "Upgrade from basic to professional package",
                    EstimatedValue = 25000,
                    ProbabilityPercent = 80,
                    Stage = SalesOpportunityStage.Qualified,
                    ExpectedCloseDate = DateTime.Now.AddDays(20),
                    SalesRepresentative = "Bob Johnson",
                    Notes = "Growing company ready to expand"
                });
            }
            
            return opportunities;
        }
    }
}