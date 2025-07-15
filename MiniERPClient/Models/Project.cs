using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MiniERPClient.Models
{
    public enum ProjectStatus
    {
        Planning,
        InProgress,
        OnHold,
        Completed,
        Cancelled
    }

    public enum ProjectPriority
    {
        Low,
        Medium,
        High,
        Critical
    }

    public enum TaskStatus
    {
        NotStarted,
        InProgress,
        Completed,
        Blocked,
        Cancelled
    }

    public enum TaskPriority
    {
        Low,
        Medium,
        High,
        Critical
    }

    public class Project : INotifyPropertyChanged
    {
        private int _id;
        private string _name = string.Empty;
        private string _description = string.Empty;
        private string _clientName = string.Empty;
        private ProjectStatus _status = ProjectStatus.Planning;
        private ProjectPriority _priority = ProjectPriority.Medium;
        private decimal _budget;
        private decimal _actualCost;
        private DateTime _startDate = DateTime.Now;
        private DateTime _endDate = DateTime.Now.AddMonths(3);
        private DateTime? _completedDate;
        private int _projectManagerId;
        private string _projectManagerName = string.Empty;
        private string _notes = string.Empty;
        private DateTime _createdDate = DateTime.Now;
        private DateTime? _lastUpdated;

        public int Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        public string ClientName
        {
            get => _clientName;
            set => SetProperty(ref _clientName, value);
        }

        public ProjectStatus Status
        {
            get => _status;
            set
            {
                if (SetProperty(ref _status, value))
                {
                    OnPropertyChanged(nameof(StatusBadge));
                    OnPropertyChanged(nameof(IsOverdue));
                    OnPropertyChanged(nameof(RemainingDaysDisplay));
                }
            }
        }

        public ProjectPriority Priority
        {
            get => _priority;
            set
            {
                if (SetProperty(ref _priority, value))
                {
                    OnPropertyChanged(nameof(PriorityBadge));
                }
            }
        }

        public decimal Budget
        {
            get => _budget;
            set
            {
                if (SetProperty(ref _budget, value))
                {
                    OnPropertyChanged(nameof(BudgetVariance));
                    OnPropertyChanged(nameof(IsOverBudget));
                }
            }
        }

        public decimal ActualCost
        {
            get => _actualCost;
            set
            {
                if (SetProperty(ref _actualCost, value))
                {
                    OnPropertyChanged(nameof(BudgetVariance));
                    OnPropertyChanged(nameof(IsOverBudget));
                }
            }
        }

        public DateTime StartDate
        {
            get => _startDate;
            set
            {
                if (SetProperty(ref _startDate, value))
                {
                    OnPropertyChanged(nameof(ProgressPercentage));
                    OnPropertyChanged(nameof(DurationDisplay));
                }
            }
        }

        public DateTime EndDate
        {
            get => _endDate;
            set
            {
                if (SetProperty(ref _endDate, value))
                {
                    OnPropertyChanged(nameof(ProgressPercentage));
                    OnPropertyChanged(nameof(IsOverdue));
                    OnPropertyChanged(nameof(DurationDisplay));
                    OnPropertyChanged(nameof(RemainingDaysDisplay));
                }
            }
        }

        public DateTime? CompletedDate
        {
            get => _completedDate;
            set => SetProperty(ref _completedDate, value);
        }

        public int ProjectManagerId
        {
            get => _projectManagerId;
            set => SetProperty(ref _projectManagerId, value);
        }

        public string ProjectManagerName
        {
            get => _projectManagerName;
            set => SetProperty(ref _projectManagerName, value);
        }

        public string Notes
        {
            get => _notes;
            set => SetProperty(ref _notes, value);
        }

        public DateTime CreatedDate
        {
            get => _createdDate;
            set => SetProperty(ref _createdDate, value);
        }

        public DateTime? LastUpdated
        {
            get => _lastUpdated;
            set => SetProperty(ref _lastUpdated, value);
        }

        // Computed Properties
        public decimal BudgetVariance => ActualCost - Budget;
        public double ProgressPercentage
        {
            get
            {
                var totalDays = (EndDate - StartDate).TotalDays;
                var elapsedDays = (DateTime.Now - StartDate).TotalDays;
                if (totalDays <= 0) return 0;
                var progress = (elapsedDays / totalDays) * 100;
                return Math.Max(0, Math.Min(100, progress));
            }
        }
        public string StatusBadge => Status.ToString();
        public string PriorityBadge => Priority.ToString();
        public bool IsOverBudget => ActualCost > Budget;
        public bool IsOverdue => DateTime.Now > EndDate && Status != ProjectStatus.Completed;
        public string DurationDisplay => $"{(EndDate - StartDate).Days} days";
        public string RemainingDaysDisplay
        {
            get
            {
                if (Status == ProjectStatus.Completed) return "Completed";
                var remaining = (EndDate - DateTime.Now).Days;
                return remaining > 0 ? $"{remaining} days remaining" : $"{Math.Abs(remaining)} days overdue";
            }
        }

        public Project Clone()
        {
            return new Project
            {
                Id = this.Id,
                Name = this.Name,
                Description = this.Description,
                ClientName = this.ClientName,
                Status = this.Status,
                Priority = this.Priority,
                Budget = this.Budget,
                ActualCost = this.ActualCost,
                StartDate = this.StartDate,
                EndDate = this.EndDate,
                CompletedDate = this.CompletedDate,
                ProjectManagerId = this.ProjectManagerId,
                ProjectManagerName = this.ProjectManagerName,
                Notes = this.Notes,
                CreatedDate = this.CreatedDate,
                LastUpdated = this.LastUpdated
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

    public class ProjectTask
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public TaskStatus Status { get; set; } = TaskStatus.NotStarted;
        public TaskPriority Priority { get; set; } = TaskPriority.Medium;
        public int AssignedToEmployeeId { get; set; }
        public string AssignedToName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; } = DateTime.Now;
        public DateTime DueDate { get; set; } = DateTime.Now.AddDays(7);
        public DateTime? CompletedDate { get; set; }
        public decimal EstimatedHours { get; set; }
        public decimal ActualHours { get; set; }
        public string Notes { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? LastUpdated { get; set; }

        // Computed Properties
        public string StatusBadge => Status.ToString();
        public string PriorityBadge => Priority.ToString();
        public bool IsOverdue => DateTime.Now > DueDate && Status != TaskStatus.Completed;
        public decimal HoursVariance => ActualHours - EstimatedHours;
        public string DurationDisplay => $"{(DueDate - StartDate).Days} days";
        public string RemainingDaysDisplay
        {
            get
            {
                if (Status == TaskStatus.Completed) return "Completed";
                var remaining = (DueDate - DateTime.Now).Days;
                return remaining > 0 ? $"{remaining} days left" : $"{Math.Abs(remaining)} days overdue";
            }
        }
    }

    public class TimeEntry
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public int TaskId { get; set; }
        public string TaskTitle { get; set; } = string.Empty;
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public DateTime Date { get; set; } = DateTime.Now;
        public decimal Hours { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal HourlyRate { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Computed Properties
        public decimal TotalCost => Hours * HourlyRate;
        public string FormattedDate => Date.ToString("MM/dd/yyyy");
        public string FormattedHours => $"{Hours:F1} hrs";
    }

    public class ResourceAllocation
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public decimal AllocationPercentage { get; set; }
        public DateTime StartDate { get; set; } = DateTime.Now;
        public DateTime EndDate { get; set; } = DateTime.Now.AddMonths(1);
        public decimal HourlyRate { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Computed Properties
        public string AllocationDisplay => $"{AllocationPercentage:F0}%";
        public string DurationDisplay => $"{(EndDate - StartDate).Days} days";
    }
}