using MiniERPClient.Models;

namespace MiniERPClient.Services
{
    public interface IDashboardService
    {
        Task<DashboardMetrics> GetDashboardMetricsAsync();
        Task<List<ChartDataPoint>> GetRevenueChartDataAsync(int months = 12);
        Task<List<ChartDataPoint>> GetExpenseChartDataAsync(int months = 12);
        Task<List<ChartDataPoint>> GetProjectStatusChartDataAsync();
        Task<List<ChartDataPoint>> GetCustomerTypeChartDataAsync();
        Task<List<KPICard>> GetKPICardsAsync();
        Task<List<AlertItem>> GetAlertsAsync();
        Task<bool> MarkAlertAsReadAsync(string alertId);
    }

    public class DashboardService : IDashboardService
    {
        private readonly IEmployeeService _employeeService;
        private readonly ICustomerService _customerService;
        private readonly IProductService _productService;
        private readonly IProjectService _projectService;
        private readonly List<AlertItem> _alerts;

        public DashboardService() : this(
            new EmployeeService(), 
            new CustomerService(), 
            new ProductService(), 
            new ProjectService())
        {
        }

        public DashboardService(
            IEmployeeService employeeService,
            ICustomerService customerService,
            IProductService productService,
            IProjectService projectService)
        {
            _employeeService = employeeService;
            _customerService = customerService;
            _productService = productService;
            _projectService = projectService;

            // Sample alerts
            _alerts = new List<AlertItem>
            {
                new AlertItem
                {
                    Title = "Low Stock Alert",
                    Description = "Database Server Hardware is critically low (3 units remaining)",
                    Type = AlertType.Warning,
                    Priority = AlertPriority.High,
                    Module = "Inventory",
                    CreatedDate = DateTime.Now.AddMinutes(-15)
                },
                new AlertItem
                {
                    Title = "Project Overdue",
                    Description = "ERP System Implementation is 5 days past deadline",
                    Type = AlertType.Error,
                    Priority = AlertPriority.Critical,
                    Module = "Projects",
                    CreatedDate = DateTime.Now.AddHours(-2)
                },
                new AlertItem
                {
                    Title = "New Customer Added",
                    Description = "Global Enterprises Ltd has been added as a new customer",
                    Type = AlertType.Success,
                    Priority = AlertPriority.Low,
                    Module = "CRM",
                    CreatedDate = DateTime.Now.AddDays(-1)
                },
                new AlertItem
                {
                    Title = "Budget Variance Alert",
                    Description = "Customer Portal Development is 15% over budget",
                    Type = AlertType.Warning,
                    Priority = AlertPriority.Medium,
                    Module = "Projects",
                    CreatedDate = DateTime.Now.AddHours(-4)
                },
                new AlertItem
                {
                    Title = "System Backup Complete",
                    Description = "Daily system backup completed successfully",
                    Type = AlertType.Success,
                    Priority = AlertPriority.Low,
                    Module = "System",
                    CreatedDate = DateTime.Now.AddHours(-8)
                }
            };
        }

        public async Task<DashboardMetrics> GetDashboardMetricsAsync()
        {
            await Task.Delay(200); // Simulate async operation

            var employees = await _employeeService.GetEmployeesAsync();
            var customers = await _customerService.GetCustomersAsync();
            var products = await _productService.GetProductsAsync();
            var projects = await _projectService.GetProjectsAsync();

            return new DashboardMetrics
            {
                // Employee Metrics
                TotalEmployees = employees.Count,
                ActiveEmployees = employees.Count(e => e.IsActive),
                TotalSalaryExpense = employees.Sum(e => e.Salary),
                AverageSalary = employees.Any() ? employees.Average(e => e.Salary) : 0,

                // Customer Metrics
                TotalCustomers = customers.Count,
                ActiveCustomers = customers.Count(c => c.Status == CustomerStatus.Active),
                ProspectCustomers = customers.Count(c => c.Status == CustomerStatus.Prospect),
                TotalCustomerValue = customers.Sum(c => c.TotalSales),
                AverageCustomerValue = customers.Any() ? customers.Average(c => c.TotalSales) : 0,

                // Product Metrics
                TotalProducts = products.Count,
                ActiveProducts = products.Count(p => p.Status == ProductStatus.Active),
                LowStockProducts = products.Count(p => p.StockAlert == StockAlertLevel.Low || p.StockAlert == StockAlertLevel.Critical),
                OutOfStockProducts = products.Count(p => p.StockAlert == StockAlertLevel.OutOfStock),
                TotalInventoryValue = products.Sum(p => p.StockQuantity * p.CostPrice),
                AverageProductMargin = products.Any() ? products.Average(p => p.ProfitMargin) : 0,

                // Project Metrics
                TotalProjects = projects.Count,
                ActiveProjects = projects.Count(p => p.Status == ProjectStatus.InProgress),
                CompletedProjects = projects.Count(p => p.Status == ProjectStatus.Completed),
                OverdueProjects = projects.Count(p => p.IsOverdue),
                TotalProjectBudget = projects.Sum(p => p.Budget),
                TotalProjectCosts = projects.Sum(p => p.ActualCost),
                AverageProjectProgress = projects.Any() ? projects.Average(p => p.ProgressPercentage) : 0,

                // Financial Metrics (simulated)
                TotalRevenue = 2500000m,
                TotalExpenses = 1800000m,

                LastUpdated = DateTime.Now
            };
        }

        public async Task<List<ChartDataPoint>> GetRevenueChartDataAsync(int months = 12)
        {
            await Task.Delay(100);

            var data = new List<ChartDataPoint>();
            var random = new Random();

            for (int i = months - 1; i >= 0; i--)
            {
                var date = DateTime.Now.AddMonths(-i);
                var baseAmount = 180000 + (random.Next(-30000, 50000));
                
                data.Add(new ChartDataPoint
                {
                    Label = date.ToString("MMM yyyy"),
                    Value = baseAmount,
                    Category = "Revenue",
                    Date = date
                });
            }

            return data;
        }

        public async Task<List<ChartDataPoint>> GetExpenseChartDataAsync(int months = 12)
        {
            await Task.Delay(100);

            var data = new List<ChartDataPoint>();
            var random = new Random();

            for (int i = months - 1; i >= 0; i--)
            {
                var date = DateTime.Now.AddMonths(-i);
                var baseAmount = 130000 + (random.Next(-20000, 40000));
                
                data.Add(new ChartDataPoint
                {
                    Label = date.ToString("MMM yyyy"),
                    Value = baseAmount,
                    Category = "Expense",
                    Date = date
                });
            }

            return data;
        }

        public async Task<List<ChartDataPoint>> GetProjectStatusChartDataAsync()
        {
            await Task.Delay(80);

            var projects = await _projectService.GetProjectsAsync();
            
            var result = projects.GroupBy(p => p.Status)
                          .Select(g => new ChartDataPoint
                          {
                              Label = g.Key.ToString(),
                              Value = g.Count(),
                              Category = "Projects"
                          })
                          .ToList();
                          
            // Ensure we have some data even if no projects exist
            if (!result.Any())
            {
                result.Add(new ChartDataPoint
                {
                    Label = "No Projects",
                    Value = 0,
                    Category = "Projects"
                });
            }
            
            return result;
        }

        public async Task<List<ChartDataPoint>> GetCustomerTypeChartDataAsync()
        {
            await Task.Delay(80);

            var customers = await _customerService.GetCustomersAsync();
            
            var result = customers.GroupBy(c => c.Type)
                           .Select(g => new ChartDataPoint
                           {
                               Label = g.Key.ToString(),
                               Value = g.Count(),
                               Category = "Customers"
                           })
                           .ToList();
                          
            // Ensure we have some data even if no customers exist
            if (!result.Any())
            {
                result.Add(new ChartDataPoint
                {
                    Label = "No Customers",
                    Value = 0,
                    Category = "Customers"
                });
            }
            
            return result;
        }

        public async Task<List<KPICard>> GetKPICardsAsync()
        {
            await Task.Delay(150);

            var metrics = await GetDashboardMetricsAsync();

            return new List<KPICard>
            {
                new KPICard
                {
                    Title = "Total Revenue",
                    Value = metrics.TotalRevenue.ToString("C0"),
                    Subtitle = "This Month",
                    Icon = "??",
                    Color = "#27AE60",
                    TrendDirection = "up",
                    TrendValue = "+12.5%"
                },
                new KPICard
                {
                    Title = "Active Projects",
                    Value = metrics.ActiveProjects.ToString(),
                    Subtitle = $"{metrics.TotalProjects} Total",
                    Icon = "??",
                    Color = "#3498DB",
                    TrendDirection = "up",
                    TrendValue = "+3"
                },
                new KPICard
                {
                    Title = "Customer Satisfaction",
                    Value = "94.2%",
                    Subtitle = "Average Rating",
                    Icon = "?",
                    Color = "#F39C12",
                    TrendDirection = "up",
                    TrendValue = "+2.1%"
                },
                new KPICard
                {
                    Title = "Team Utilization",
                    Value = "87.5%",
                    Subtitle = $"{metrics.ActiveEmployees} Active",
                    Icon = "??",
                    Color = "#9B59B6",
                    TrendDirection = "neutral",
                    TrendValue = "0%"
                },
                new KPICard
                {
                    Title = "Profit Margin",
                    Value = metrics.ProfitMargin.ToString("F1") + "%",
                    Subtitle = "Net Profit",
                    Icon = "??",
                    Color = "#E67E22",
                    TrendDirection = "up",
                    TrendValue = "+1.8%"
                },
                new KPICard
                {
                    Title = "Inventory Value",
                    Value = metrics.TotalInventoryValue.ToString("C0"),
                    Subtitle = $"{metrics.LowStockProducts} Low Stock",
                    Icon = "??",
                    Color = "#1ABC9C",
                    TrendDirection = "down",
                    TrendValue = "-3.2%"
                }
            };
        }

        public async Task<List<AlertItem>> GetAlertsAsync()
        {
            await Task.Delay(80);
            return _alerts.OrderByDescending(a => a.CreatedDate).ToList();
        }

        public async Task<bool> MarkAlertAsReadAsync(string alertId)
        {
            await Task.Delay(50);
            var alert = _alerts.FirstOrDefault(a => a.Id == alertId);
            if (alert != null)
            {
                alert.IsRead = true;
                return true;
            }
            return false;
        }
    }
}