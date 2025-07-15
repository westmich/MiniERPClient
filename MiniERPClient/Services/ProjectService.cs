using MiniERPClient.Models;

namespace MiniERPClient.Services
{
    public interface IProjectService
    {
        Task<List<Project>> GetProjectsAsync();
        Task<Project?> GetProjectByIdAsync(int id);
        Task<Project> AddProjectAsync(Project project);
        Task<Project> UpdateProjectAsync(Project project);
        Task<bool> DeleteProjectAsync(int id);
        Task<List<ProjectTask>> GetTasksByProjectIdAsync(int projectId);
        Task<List<ProjectTask>> GetAllTasksAsync();
        Task<ProjectTask> AddTaskAsync(ProjectTask task);
        Task<ProjectTask> UpdateTaskAsync(ProjectTask task);
        Task<bool> DeleteTaskAsync(int id);
        Task<List<TimeEntry>> GetTimeEntriesAsync(int? projectId = null, int? employeeId = null);
        Task<TimeEntry> AddTimeEntryAsync(TimeEntry timeEntry);
        Task<List<ResourceAllocation>> GetResourceAllocationsAsync(int? projectId = null);
        Task<ResourceAllocation> AddResourceAllocationAsync(ResourceAllocation allocation);
    }

    public class ProjectService : IProjectService
    {
        private readonly List<Project> _projects;
        private readonly List<ProjectTask> _tasks;
        private readonly List<TimeEntry> _timeEntries;
        private readonly List<ResourceAllocation> _resourceAllocations;
        private int _nextProjectId = 1;
        private int _nextTaskId = 1;
        private int _nextTimeEntryId = 1;
        private int _nextAllocationId = 1;

        public ProjectService()
        {
            // Sample project data
            _projects = new List<Project>
            {
                new Project
                {
                    Id = _nextProjectId++,
                    Name = "ERP System Implementation",
                    Description = "Complete implementation of new ERP system for client",
                    ClientName = "Acme Corporation",
                    Status = ProjectStatus.InProgress,
                    Priority = ProjectPriority.High,
                    Budget = 500000,
                    ActualCost = 350000,
                    StartDate = DateTime.Now.AddMonths(-6),
                    EndDate = DateTime.Now.AddMonths(2),
                    ProjectManagerId = 1,
                    ProjectManagerName = "John Doe",
                    Notes = "Large scale ERP implementation with multiple modules",
                    CreatedDate = DateTime.Now.AddMonths(-7),
                    LastUpdated = DateTime.Now.AddDays(-2)
                },
                new Project
                {
                    Id = _nextProjectId++,
                    Name = "Customer Portal Development",
                    Description = "Development of web-based customer portal",
                    ClientName = "TechStart Inc",
                    Status = ProjectStatus.InProgress,
                    Priority = ProjectPriority.Medium,
                    Budget = 150000,
                    ActualCost = 95000,
                    StartDate = DateTime.Now.AddMonths(-3),
                    EndDate = DateTime.Now.AddMonths(1),
                    ProjectManagerId = 2,
                    ProjectManagerName = "Jane Smith",
                    Notes = "Modern web portal with responsive design",
                    CreatedDate = DateTime.Now.AddMonths(-4),
                    LastUpdated = DateTime.Now.AddDays(-1)
                },
                new Project
                {
                    Id = _nextProjectId++,
                    Name = "Mobile App Development",
                    Description = "iOS and Android mobile application",
                    ClientName = "Global Enterprises Ltd",
                    Status = ProjectStatus.Planning,
                    Priority = ProjectPriority.Medium,
                    Budget = 200000,
                    ActualCost = 25000,
                    StartDate = DateTime.Now.AddMonths(1),
                    EndDate = DateTime.Now.AddMonths(8),
                    ProjectManagerId = 3,
                    ProjectManagerName = "Bob Johnson",
                    Notes = "Cross-platform mobile application with native features",
                    CreatedDate = DateTime.Now.AddDays(-15),
                    LastUpdated = DateTime.Now.AddDays(-3)
                },
                new Project
                {
                    Id = _nextProjectId++,
                    Name = "Legacy System Migration",
                    Description = "Migration from legacy system to modern platform",
                    ClientName = "Industrial Solutions",
                    Status = ProjectStatus.Completed,
                    Priority = ProjectPriority.High,
                    Budget = 300000,
                    ActualCost = 285000,
                    StartDate = DateTime.Now.AddMonths(-12),
                    EndDate = DateTime.Now.AddMonths(-2),
                    CompletedDate = DateTime.Now.AddMonths(-2),
                    ProjectManagerId = 1,
                    ProjectManagerName = "John Doe",
                    Notes = "Successfully completed migration project",
                    CreatedDate = DateTime.Now.AddMonths(-14),
                    LastUpdated = DateTime.Now.AddMonths(-2)
                }
            };

            // Sample task data
            _tasks = new List<ProjectTask>
            {
                new ProjectTask
                {
                    Id = _nextTaskId++,
                    ProjectId = 1,
                    ProjectName = "ERP System Implementation",
                    Title = "Database Design",
                    Description = "Design and implement database schema",
                    Status = Models.TaskStatus.Completed,
                    Priority = TaskPriority.High,
                    AssignedToEmployeeId = 3,
                    AssignedToName = "Bob Johnson",
                    StartDate = DateTime.Now.AddMonths(-6),
                    DueDate = DateTime.Now.AddMonths(-5),
                    CompletedDate = DateTime.Now.AddMonths(-5),
                    EstimatedHours = 80,
                    ActualHours = 85,
                    Notes = "Database schema completed with all required tables"
                },
                new ProjectTask
                {
                    Id = _nextTaskId++,
                    ProjectId = 1,
                    ProjectName = "ERP System Implementation",
                    Title = "User Interface Development",
                    Description = "Develop main user interface components",
                    Status = Models.TaskStatus.InProgress,
                    Priority = TaskPriority.High,
                    AssignedToEmployeeId = 4,
                    AssignedToName = "Alice Brown",
                    StartDate = DateTime.Now.AddMonths(-4),
                    DueDate = DateTime.Now.AddDays(15),
                    EstimatedHours = 120,
                    ActualHours = 95,
                    Notes = "UI development 80% complete"
                },
                new ProjectTask
                {
                    Id = _nextTaskId++,
                    ProjectId = 2,
                    ProjectName = "Customer Portal Development",
                    Title = "Authentication System",
                    Description = "Implement secure user authentication",
                    Status = Models.TaskStatus.Completed,
                    Priority = TaskPriority.High,
                    AssignedToEmployeeId = 2,
                    AssignedToName = "Jane Smith",
                    StartDate = DateTime.Now.AddMonths(-3),
                    DueDate = DateTime.Now.AddMonths(-2),
                    CompletedDate = DateTime.Now.AddMonths(-2),
                    EstimatedHours = 40,
                    ActualHours = 38,
                    Notes = "Authentication system with OAuth integration"
                },
                new ProjectTask
                {
                    Id = _nextTaskId++,
                    ProjectId = 2,
                    ProjectName = "Customer Portal Development",
                    Title = "Dashboard Creation",
                    Description = "Create customer dashboard with analytics",
                    Status = Models.TaskStatus.InProgress,
                    Priority = TaskPriority.Medium,
                    AssignedToEmployeeId = 4,
                    AssignedToName = "Alice Brown",
                    StartDate = DateTime.Now.AddDays(-30),
                    DueDate = DateTime.Now.AddDays(10),
                    EstimatedHours = 60,
                    ActualHours = 45,
                    Notes = "Dashboard layout complete, working on charts"
                }
            };

            // Sample time entries
            _timeEntries = new List<TimeEntry>
            {
                new TimeEntry
                {
                    Id = _nextTimeEntryId++,
                    ProjectId = 1,
                    ProjectName = "ERP System Implementation",
                    TaskId = 1,
                    TaskTitle = "Database Design",
                    EmployeeId = 3,
                    EmployeeName = "Bob Johnson",
                    Date = DateTime.Now.AddDays(-1),
                    Hours = 8,
                    Description = "Completed database schema review and optimization",
                    HourlyRate = 75
                },
                new TimeEntry
                {
                    Id = _nextTimeEntryId++,
                    ProjectId = 1,
                    ProjectName = "ERP System Implementation",
                    TaskId = 2,
                    TaskTitle = "User Interface Development",
                    EmployeeId = 4,
                    EmployeeName = "Alice Brown",
                    Date = DateTime.Now.AddDays(-1),
                    Hours = 6.5m,
                    Description = "Worked on responsive design implementation",
                    HourlyRate = 70
                },
                new TimeEntry
                {
                    Id = _nextTimeEntryId++,
                    ProjectId = 2,
                    ProjectName = "Customer Portal Development",
                    TaskId = 4,
                    TaskTitle = "Dashboard Creation",
                    EmployeeId = 4,
                    EmployeeName = "Alice Brown",
                    Date = DateTime.Now.AddDays(-2),
                    Hours = 7,
                    Description = "Implemented chart components and data visualization",
                    HourlyRate = 70
                }
            };

            // Sample resource allocations
            _resourceAllocations = new List<ResourceAllocation>
            {
                new ResourceAllocation
                {
                    Id = _nextAllocationId++,
                    ProjectId = 1,
                    ProjectName = "ERP System Implementation",
                    EmployeeId = 1,
                    EmployeeName = "John Doe",
                    Role = "Project Manager",
                    AllocationPercentage = 50,
                    StartDate = DateTime.Now.AddMonths(-6),
                    EndDate = DateTime.Now.AddMonths(2),
                    HourlyRate = 85
                },
                new ResourceAllocation
                {
                    Id = _nextAllocationId++,
                    ProjectId = 1,
                    ProjectName = "ERP System Implementation",
                    EmployeeId = 3,
                    EmployeeName = "Bob Johnson",
                    Role = "Senior Developer",
                    AllocationPercentage = 75,
                    StartDate = DateTime.Now.AddMonths(-6),
                    EndDate = DateTime.Now.AddMonths(2),
                    HourlyRate = 75
                },
                new ResourceAllocation
                {
                    Id = _nextAllocationId++,
                    ProjectId = 2,
                    ProjectName = "Customer Portal Development",
                    EmployeeId = 2,
                    EmployeeName = "Jane Smith",
                    Role = "Lead Developer",
                    AllocationPercentage = 80,
                    StartDate = DateTime.Now.AddMonths(-3),
                    EndDate = DateTime.Now.AddMonths(1),
                    HourlyRate = 80
                }
            };
        }

        public async Task<List<Project>> GetProjectsAsync()
        {
            await Task.Delay(120);
            return new List<Project>(_projects);
        }

        public async Task<Project?> GetProjectByIdAsync(int id)
        {
            await Task.Delay(50);
            return _projects.FirstOrDefault(p => p.Id == id);
        }

        public async Task<Project> AddProjectAsync(Project project)
        {
            await Task.Delay(180);
            project.Id = _nextProjectId++;
            project.CreatedDate = DateTime.Now;
            project.LastUpdated = DateTime.Now;
            _projects.Add(project);
            return project;
        }

        public async Task<Project> UpdateProjectAsync(Project project)
        {
            await Task.Delay(150);
            var existingProject = _projects.FirstOrDefault(p => p.Id == project.Id);
            if (existingProject != null)
            {
                project.LastUpdated = DateTime.Now;
                var index = _projects.IndexOf(existingProject);
                _projects[index] = project;
            }
            return project;
        }

        public async Task<bool> DeleteProjectAsync(int id)
        {
            await Task.Delay(100);
            var project = _projects.FirstOrDefault(p => p.Id == id);
            if (project != null)
            {
                _projects.Remove(project);
                return true;
            }
            return false;
        }

        public async Task<List<ProjectTask>> GetTasksByProjectIdAsync(int projectId)
        {
            await Task.Delay(80);
            return _tasks.Where(t => t.ProjectId == projectId).ToList();
        }

        public async Task<List<ProjectTask>> GetAllTasksAsync()
        {
            await Task.Delay(100);
            return new List<ProjectTask>(_tasks);
        }

        public async Task<ProjectTask> AddTaskAsync(ProjectTask task)
        {
            await Task.Delay(120);
            task.Id = _nextTaskId++;
            task.CreatedDate = DateTime.Now;
            task.LastUpdated = DateTime.Now;
            _tasks.Add(task);
            return task;
        }

        public async Task<ProjectTask> UpdateTaskAsync(ProjectTask task)
        {
            await Task.Delay(100);
            var existingTask = _tasks.FirstOrDefault(t => t.Id == task.Id);
            if (existingTask != null)
            {
                task.LastUpdated = DateTime.Now;
                var index = _tasks.IndexOf(existingTask);
                _tasks[index] = task;
            }
            return task;
        }

        public async Task<bool> DeleteTaskAsync(int id)
        {
            await Task.Delay(80);
            var task = _tasks.FirstOrDefault(t => t.Id == id);
            if (task != null)
            {
                _tasks.Remove(task);
                return true;
            }
            return false;
        }

        public async Task<List<TimeEntry>> GetTimeEntriesAsync(int? projectId = null, int? employeeId = null)
        {
            await Task.Delay(90);
            var entries = _timeEntries.AsEnumerable();
            
            if (projectId.HasValue)
                entries = entries.Where(t => t.ProjectId == projectId.Value);
                
            if (employeeId.HasValue)
                entries = entries.Where(t => t.EmployeeId == employeeId.Value);
                
            return entries.OrderByDescending(t => t.Date).ToList();
        }

        public async Task<TimeEntry> AddTimeEntryAsync(TimeEntry timeEntry)
        {
            await Task.Delay(100);
            timeEntry.Id = _nextTimeEntryId++;
            timeEntry.CreatedDate = DateTime.Now;
            _timeEntries.Add(timeEntry);
            return timeEntry;
        }

        public async Task<List<ResourceAllocation>> GetResourceAllocationsAsync(int? projectId = null)
        {
            await Task.Delay(70);
            var allocations = _resourceAllocations.AsEnumerable();
            
            if (projectId.HasValue)
                allocations = allocations.Where(r => r.ProjectId == projectId.Value);
                
            return allocations.ToList();
        }

        public async Task<ResourceAllocation> AddResourceAllocationAsync(ResourceAllocation allocation)
        {
            await Task.Delay(110);
            allocation.Id = _nextAllocationId++;
            allocation.CreatedDate = DateTime.Now;
            _resourceAllocations.Add(allocation);
            return allocation;
        }
    }
}