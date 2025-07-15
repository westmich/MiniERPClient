using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using MiniERPClient.Commands;
using MiniERPClient.Models;
using MiniERPClient.Services;
using MiniERPClient.Views;

namespace MiniERPClient.ViewModels
{
    public class ProjectViewModel : ViewModelBase
    {
        private readonly IProjectService _projectService;
        private bool _isLoading;
        private string _searchText = string.Empty;
        private Project? _selectedProject;
        private ProjectTask? _selectedTask;
        private string _statusMessage = "Ready";
        private ProjectStatus? _selectedStatusFilter;
        private ProjectPriority? _selectedPriorityFilter;

        public ProjectViewModel() : this(new ProjectService())
        {
        }

        public ProjectViewModel(IProjectService projectService)
        {
            _projectService = projectService;
            
            // Initialize collections
            Projects = new ObservableCollection<Project>();
            FilteredProjects = new ObservableCollection<Project>();
            ProjectTasks = new ObservableCollection<ProjectTask>();
            TimeEntries = new ObservableCollection<TimeEntry>();
            ResourceAllocations = new ObservableCollection<ResourceAllocation>();
            
            // Initialize filter collections
            ProjectStatuses = new ObservableCollection<ProjectStatus?>
            {
                null, // All
                ProjectStatus.Planning,
                ProjectStatus.InProgress,
                ProjectStatus.OnHold,
                ProjectStatus.Completed,
                ProjectStatus.Cancelled
            };
            
            ProjectPriorities = new ObservableCollection<ProjectPriority?>
            {
                null, // All
                ProjectPriority.Low,
                ProjectPriority.Medium,
                ProjectPriority.High,
                ProjectPriority.Critical
            };
            
            // Initialize commands
            LoadProjectsCommand = new RelayCommand(async () => await LoadProjectsAsync(), () => !IsLoading);
            AddProjectCommand = new RelayCommand(AddProject, () => !IsLoading);
            EditProjectCommand = new RelayCommand(EditProject, () => SelectedProject != null && !IsLoading);
            DeleteProjectCommand = new RelayCommand(async () => await DeleteProjectAsync(), () => SelectedProject != null && !IsLoading);
            ViewTasksCommand = new RelayCommand(async () => await LoadTasksAsync(), () => SelectedProject != null && !IsLoading);
            ViewTimeEntriesCommand = new RelayCommand(async () => await LoadTimeEntriesAsync(), () => SelectedProject != null && !IsLoading);
            ViewResourcesCommand = new RelayCommand(async () => await LoadResourceAllocationsAsync(), () => SelectedProject != null && !IsLoading);
            AddTaskCommand = new RelayCommand(AddTask, () => SelectedProject != null && !IsLoading);
            SearchCommand = new RelayCommand(PerformSearch);
            ClearSearchCommand = new RelayCommand(ClearSearch, () => !string.IsNullOrEmpty(SearchText));
            
            // Load initial data
            _ = LoadProjectsAsync();
        }

        #region Properties

        public ObservableCollection<Project> Projects { get; }
        public ObservableCollection<Project> FilteredProjects { get; }
        public ObservableCollection<ProjectTask> ProjectTasks { get; }
        public ObservableCollection<TimeEntry> TimeEntries { get; }
        public ObservableCollection<ResourceAllocation> ResourceAllocations { get; }
        public ObservableCollection<ProjectStatus?> ProjectStatuses { get; }
        public ObservableCollection<ProjectPriority?> ProjectPriorities { get; }

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

        public Project? SelectedProject
        {
            get => _selectedProject;
            set
            {
                if (SetProperty(ref _selectedProject, value))
                {
                    _ = LoadTasksAsync();
                    _ = LoadTimeEntriesAsync();
                    _ = LoadResourceAllocationsAsync();
                }
            }
        }

        public ProjectTask? SelectedTask
        {
            get => _selectedTask;
            set => SetProperty(ref _selectedTask, value);
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        public ProjectStatus? SelectedStatusFilter
        {
            get => _selectedStatusFilter;
            set
            {
                if (SetProperty(ref _selectedStatusFilter, value))
                {
                    PerformSearch();
                }
            }
        }

        public ProjectPriority? SelectedPriorityFilter
        {
            get => _selectedPriorityFilter;
            set
            {
                if (SetProperty(ref _selectedPriorityFilter, value))
                {
                    PerformSearch();
                }
            }
        }

        // Dashboard Properties
        public int TotalProjects => Projects.Count;
        public int ActiveProjects => Projects.Count(p => p.Status == ProjectStatus.InProgress);
        public int CompletedProjects => Projects.Count(p => p.Status == ProjectStatus.Completed);
        public int OverdueProjects => Projects.Count(p => p.IsOverdue);
        public decimal TotalBudget => Projects.Sum(p => p.Budget);
        public decimal TotalActualCost => Projects.Sum(p => p.ActualCost);
        public decimal TotalBudgetVariance => TotalActualCost - TotalBudget;
        public double AverageProgress => Projects.Any() ? Projects.Average(p => p.ProgressPercentage) : 0;

        #endregion

        #region Commands

        public ICommand LoadProjectsCommand { get; }
        public ICommand AddProjectCommand { get; }
        public ICommand EditProjectCommand { get; }
        public ICommand DeleteProjectCommand { get; }
        public ICommand ViewTasksCommand { get; }
        public ICommand ViewTimeEntriesCommand { get; }
        public ICommand ViewResourcesCommand { get; }
        public ICommand AddTaskCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand ClearSearchCommand { get; }

        #endregion

        #region Methods

        private async Task LoadProjectsAsync()
        {
            try
            {
                IsLoading = true;
                StatusMessage = "Loading projects...";

                await Application.Current.Dispatcher.InvokeAsync(async () =>
                {
                    var projects = await _projectService.GetProjectsAsync();
                    
                    Projects.Clear();
                    foreach (var project in projects)
                    {
                        Projects.Add(project);
                    }
                    
                    PerformSearch();
                    StatusMessage = $"Loaded {projects.Count} projects";
                    
                    // Notify dashboard properties changed
                    OnPropertyChanged(nameof(TotalProjects));
                    OnPropertyChanged(nameof(ActiveProjects));
                    OnPropertyChanged(nameof(CompletedProjects));
                    OnPropertyChanged(nameof(OverdueProjects));
                    OnPropertyChanged(nameof(TotalBudget));
                    OnPropertyChanged(nameof(TotalActualCost));
                    OnPropertyChanged(nameof(TotalBudgetVariance));
                    OnPropertyChanged(nameof(AverageProgress));
                });
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error loading projects: {ex.Message}";
                MessageBox.Show($"Error loading projects: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void AddProject()
        {
            var dialog = new ProjectEditWindow();
            dialog.Owner = Application.Current.MainWindow;
            
            if (dialog.ShowDialog() == true && dialog.Result != null)
            {
                _ = SaveProjectAsync(dialog.Result, isNew: true);
            }
        }

        private void EditProject()
        {
            if (SelectedProject == null) return;

            var dialog = new ProjectEditWindow(SelectedProject);
            dialog.Owner = Application.Current.MainWindow;
            
            if (dialog.ShowDialog() == true && dialog.Result != null)
            {
                _ = SaveProjectAsync(dialog.Result, isNew: false);
            }
        }

        private async Task SaveProjectAsync(Project project, bool isNew)
        {
            try
            {
                IsLoading = true;
                StatusMessage = isNew ? "Adding project..." : "Updating project...";

                Project savedProject;
                if (isNew)
                {
                    savedProject = await _projectService.AddProjectAsync(project);
                    await Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        Projects.Add(savedProject);
                    });
                }
                else
                {
                    savedProject = await _projectService.UpdateProjectAsync(project);
                    await Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        var existingProject = Projects.FirstOrDefault(p => p.Id == savedProject.Id);
                        if (existingProject != null)
                        {
                            var index = Projects.IndexOf(existingProject);
                            Projects[index] = savedProject;
                        }
                    });
                }

                PerformSearch();
                StatusMessage = isNew ? "Project added successfully" : "Project updated successfully";
                
                // Update dashboard properties
                OnPropertyChanged(nameof(TotalProjects));
                OnPropertyChanged(nameof(ActiveProjects));
                OnPropertyChanged(nameof(CompletedProjects));
                OnPropertyChanged(nameof(OverdueProjects));
                OnPropertyChanged(nameof(TotalBudget));
                OnPropertyChanged(nameof(TotalActualCost));
                OnPropertyChanged(nameof(TotalBudgetVariance));
                OnPropertyChanged(nameof(AverageProgress));
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error saving project: {ex.Message}";
                MessageBox.Show($"Error saving project: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task DeleteProjectAsync()
        {
            if (SelectedProject == null) return;

            var result = MessageBox.Show(
                $"Are you sure you want to delete {SelectedProject.Name}?",
                "Confirm Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes) return;

            try
            {
                IsLoading = true;
                StatusMessage = "Deleting project...";

                var success = await _projectService.DeleteProjectAsync(SelectedProject.Id);
                if (success)
                {
                    await Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        Projects.Remove(SelectedProject);
                        SelectedProject = null;
                    });
                    
                    PerformSearch();
                    StatusMessage = "Project deleted successfully";
                    
                    // Update dashboard properties
                    OnPropertyChanged(nameof(TotalProjects));
                    OnPropertyChanged(nameof(ActiveProjects));
                    OnPropertyChanged(nameof(CompletedProjects));
                    OnPropertyChanged(nameof(OverdueProjects));
                    OnPropertyChanged(nameof(TotalBudget));
                    OnPropertyChanged(nameof(TotalActualCost));
                    OnPropertyChanged(nameof(TotalBudgetVariance));
                    OnPropertyChanged(nameof(AverageProgress));
                }
                else
                {
                    StatusMessage = "Failed to delete project";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error deleting project: {ex.Message}";
                MessageBox.Show($"Error deleting project: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task LoadTasksAsync()
        {
            if (SelectedProject == null) return;

            try
            {
                StatusMessage = "Loading tasks...";
                
                var tasks = await _projectService.GetTasksByProjectIdAsync(SelectedProject.Id);
                
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    ProjectTasks.Clear();
                    foreach (var task in tasks)
                    {
                        ProjectTasks.Add(task);
                    }
                });
                
                StatusMessage = $"Loaded {tasks.Count} tasks for {SelectedProject.Name}";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error loading tasks: {ex.Message}";
            }
        }

        private async Task LoadTimeEntriesAsync()
        {
            if (SelectedProject == null) return;

            try
            {
                StatusMessage = "Loading time entries...";
                
                var timeEntries = await _projectService.GetTimeEntriesAsync(SelectedProject.Id);
                
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    TimeEntries.Clear();
                    foreach (var entry in timeEntries)
                    {
                        TimeEntries.Add(entry);
                    }
                });
                
                StatusMessage = $"Loaded {timeEntries.Count} time entries for {SelectedProject.Name}";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error loading time entries: {ex.Message}";
            }
        }

        private async Task LoadResourceAllocationsAsync()
        {
            if (SelectedProject == null) return;

            try
            {
                StatusMessage = "Loading resource allocations...";
                
                var allocations = await _projectService.GetResourceAllocationsAsync(SelectedProject.Id);
                
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    ResourceAllocations.Clear();
                    foreach (var allocation in allocations)
                    {
                        ResourceAllocations.Add(allocation);
                    }
                });
                
                StatusMessage = $"Loaded {allocations.Count} resource allocations for {SelectedProject.Name}";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error loading resource allocations: {ex.Message}";
            }
        }

        private void AddTask()
        {
            if (SelectedProject == null) return;

            var newTask = new ProjectTask
            {
                ProjectId = SelectedProject.Id,
                ProjectName = SelectedProject.Name,
                Title = "New Task",
                Description = "Task description",
                Status = Models.TaskStatus.NotStarted,
                Priority = TaskPriority.Medium,
                AssignedToEmployeeId = 1,
                AssignedToName = "John Doe",
                StartDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(7),
                EstimatedHours = 8
            };

            _ = SaveTaskAsync(newTask);
        }

        private async Task SaveTaskAsync(ProjectTask task)
        {
            try
            {
                IsLoading = true;
                StatusMessage = "Adding task...";

                var savedTask = await _projectService.AddTaskAsync(task);
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    ProjectTasks.Add(savedTask);
                });

                StatusMessage = "Task added successfully";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error saving task: {ex.Message}";
                MessageBox.Show($"Error saving task: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
                FilteredProjects.Clear();

                var searchResults = Projects.AsEnumerable();

                // Apply text search
                if (!string.IsNullOrWhiteSpace(SearchText))
                {
                    searchResults = searchResults.Where(p =>
                        p.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                        p.Description.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                        p.ClientName.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                        p.ProjectManagerName.Contains(SearchText, StringComparison.OrdinalIgnoreCase));
                }

                // Apply status filter
                if (SelectedStatusFilter.HasValue)
                {
                    searchResults = searchResults.Where(p => p.Status == SelectedStatusFilter.Value);
                }

                // Apply priority filter
                if (SelectedPriorityFilter.HasValue)
                {
                    searchResults = searchResults.Where(p => p.Priority == SelectedPriorityFilter.Value);
                }

                foreach (var project in searchResults.OrderBy(p => p.Name))
                {
                    FilteredProjects.Add(project);
                }
            });
        }

        private void ClearSearch()
        {
            SearchText = string.Empty;
            SelectedStatusFilter = null;
            SelectedPriorityFilter = null;
        }

        #endregion
    }
}