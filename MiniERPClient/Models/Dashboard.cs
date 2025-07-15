namespace MiniERPClient.Models
{
    public class DashboardMetrics
    {
        // Employee Metrics
        public int TotalEmployees { get; set; }
        public int ActiveEmployees { get; set; }
        public decimal TotalSalaryExpense { get; set; }
        public decimal AverageSalary { get; set; }
        
        // Customer Metrics
        public int TotalCustomers { get; set; }
        public int ActiveCustomers { get; set; }
        public int ProspectCustomers { get; set; }
        public decimal TotalCustomerValue { get; set; }
        public decimal AverageCustomerValue { get; set; }
        
        // Product Metrics
        public int TotalProducts { get; set; }
        public int ActiveProducts { get; set; }
        public int LowStockProducts { get; set; }
        public int OutOfStockProducts { get; set; }
        public decimal TotalInventoryValue { get; set; }
        public decimal AverageProductMargin { get; set; }
        
        // Project Metrics
        public int TotalProjects { get; set; }
        public int ActiveProjects { get; set; }
        public int CompletedProjects { get; set; }
        public int OverdueProjects { get; set; }
        public decimal TotalProjectBudget { get; set; }
        public decimal TotalProjectCosts { get; set; }
        public decimal ProjectBudgetVariance => TotalProjectCosts - TotalProjectBudget;
        public double AverageProjectProgress { get; set; }
        
        // Financial Metrics
        public decimal TotalRevenue { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal NetProfit => TotalRevenue - TotalExpenses;
        public decimal ProfitMargin => TotalRevenue > 0 ? (NetProfit / TotalRevenue) * 100 : 0;
        
        // Performance Indicators
        public DateTime LastUpdated { get; set; } = DateTime.Now;
    }

    public class ChartDataPoint
    {
        public string Label { get; set; } = string.Empty;
        public decimal Value { get; set; }
        public string Category { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string FormattedValue => Value.ToString("C0");
        public string FormattedDate => Date.ToString("MM/dd");
        
        // Helper property for integer values (like counts)
        public int IntValue => (int)Value;
    }

    public class KPICard
    {
        public string Title { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string Subtitle { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public string Color { get; set; } = "#3498DB";
        public string TrendDirection { get; set; } = "neutral"; // up, down, neutral
        public string TrendValue { get; set; } = string.Empty;
        public string TrendColor => TrendDirection switch
        {
            "up" => "#27AE60",
            "down" => "#E74C3C",
            _ => "#7F8C8D"
        };
    }

    public class AlertItem
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public AlertType Type { get; set; } = AlertType.Info;
        public AlertPriority Priority { get; set; } = AlertPriority.Medium;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public bool IsRead { get; set; }
        public string Module { get; set; } = string.Empty;
        
        public string TypeBadge => Type.ToString();
        public string PriorityBadge => Priority.ToString();
        public string TimeAgo
        {
            get
            {
                var timeSpan = DateTime.Now - CreatedDate;
                if (timeSpan.TotalMinutes < 1) return "Just now";
                if (timeSpan.TotalMinutes < 60) return $"{(int)timeSpan.TotalMinutes}m ago";
                if (timeSpan.TotalHours < 24) return $"{(int)timeSpan.TotalHours}h ago";
                return $"{(int)timeSpan.TotalDays}d ago";
            }
        }
        
        public string TypeColor => Type switch
        {
            AlertType.Success => "#27AE60",
            AlertType.Warning => "#F39C12",
            AlertType.Error => "#E74C3C",
            AlertType.Critical => "#8E44AD",
            _ => "#3498DB"
        };
    }

    public enum AlertType
    {
        Info,
        Success,
        Warning,
        Error,
        Critical
    }

    public enum AlertPriority
    {
        Low,
        Medium,
        High,
        Critical
    }

    public class RevenueData
    {
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public string Source { get; set; } = string.Empty;
        public string FormattedAmount => Amount.ToString("C0");
        public string FormattedDate => Date.ToString("MMM dd");
    }

    public class ExpenseData
    {
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public string Category { get; set; } = string.Empty;
        public string FormattedAmount => Amount.ToString("C0");
        public string FormattedDate => Date.ToString("MMM dd");
    }
}