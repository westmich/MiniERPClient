using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using MiniERPClient.Commands;
using MiniERPClient.Models;

namespace MiniERPClient.ViewModels
{
    public class ProjectEditDialogViewModel : ViewModelBase
    {
        private Project _project;
        private bool _isNewProject;
        private bool _isSaving;
        private string _validationSummary = string.Empty;

        public ProjectEditDialogViewModel(Project? project = null)
        {
            _isNewProject = project == null;
            _project = project?.Clone() ?? new Project
            {
                Name = string.Empty,
                Description = string.Empty,
                ClientName = string.Empty,
                Status = ProjectStatus.Planning,
                Priority = ProjectPriority.Medium,
                Budget = 100000,
                ActualCost = 0,
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddMonths(3),
                ProjectManagerId = 1,
                ProjectManagerName = "John Doe",
                Notes = string.Empty
            };

            // Initialize available options
            AvailableStatuses = new ObservableCollection<ProjectStatus>
            {
                ProjectStatus.Planning,
                ProjectStatus.InProgress,
                ProjectStatus.OnHold,
                ProjectStatus.Completed,
                ProjectStatus.Cancelled
            };

            AvailablePriorities = new ObservableCollection<ProjectPriority>
            {
                ProjectPriority.Low,
                ProjectPriority.Medium,
                ProjectPriority.High,
                ProjectPriority.Critical
            };

            AvailableProjectManagers = new ObservableCollection<string>
            {
                "John Doe", "Jane Smith", "Bob Johnson", "Alice Williams", "Mike Davis"
            };

            // Initialize commands
            SaveCommand = new RelayCommand(Save, CanSave);
            CancelCommand = new RelayCommand(Cancel);

            // Subscribe to property changes on the project to trigger validation
            _project.PropertyChanged += OnProjectPropertyChanged;

            // Validate initially
            ValidateProject();
        }

        #region Properties

        public Project Project
        {
            get => _project;
            set
            {
                if (_project != null)
                {
                    _project.PropertyChanged -= OnProjectPropertyChanged;
                }
                
                if (SetProperty(ref _project, value))
                {
                    if (_project != null)
                    {
                        _project.PropertyChanged += OnProjectPropertyChanged;
                    }
                    ValidateProject();
                }
            }
        }

        public bool IsNewProject
        {
            get => _isNewProject;
            set => SetProperty(ref _isNewProject, value);
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

        public ObservableCollection<ProjectStatus> AvailableStatuses { get; }
        public ObservableCollection<ProjectPriority> AvailablePriorities { get; }
        public ObservableCollection<string> AvailableProjectManagers { get; }

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

        private void OnProjectPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            ValidateProject();
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

        private void ValidateProject()
        {
            var validationErrors = new List<string>();

            // Project Name validation
            if (string.IsNullOrWhiteSpace(Project.Name))
            {
                validationErrors.Add("Project Name is required.");
            }

            // Description validation
            if (string.IsNullOrWhiteSpace(Project.Description))
            {
                validationErrors.Add("Description is required.");
            }

            // Client Name validation
            if (string.IsNullOrWhiteSpace(Project.ClientName))
            {
                validationErrors.Add("Client Name is required.");
            }

            // Project Manager validation
            if (string.IsNullOrWhiteSpace(Project.ProjectManagerName))
            {
                validationErrors.Add("Project Manager is required.");
            }

            // Budget validation
            if (Project.Budget <= 0)
            {
                validationErrors.Add("Budget must be greater than 0.");
            }

            // Date validation
            if (Project.StartDate >= Project.EndDate)
            {
                validationErrors.Add("End Date must be after Start Date.");
            }

            if (Project.StartDate < DateTime.Today.AddYears(-10))
            {
                validationErrors.Add("Start Date cannot be more than 10 years in the past.");
            }

            if (Project.EndDate > DateTime.Today.AddYears(10))
            {
                validationErrors.Add("End Date cannot be more than 10 years in the future.");
            }

            // Actual Cost validation
            if (Project.ActualCost < 0)
            {
                validationErrors.Add("Actual Cost cannot be negative.");
            }

            ValidationSummary = string.Join(" ", validationErrors);
            OnPropertyChanged(nameof(HasValidationErrors));
        }

        public void Cleanup()
        {
            if (_project != null)
            {
                _project.PropertyChanged -= OnProjectPropertyChanged;
            }
        }

        #endregion
    }
}