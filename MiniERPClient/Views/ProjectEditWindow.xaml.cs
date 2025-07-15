using System.Windows;
using MiniERPClient.ViewModels;
using MiniERPClient.Models;

namespace MiniERPClient.Views
{
    public partial class ProjectEditWindow : Window
    {
        public ProjectEditDialogViewModel ViewModel { get; private set; }
        public Project? Result { get; private set; }

        public ProjectEditWindow(Project? project = null)
        {
            InitializeComponent();
            
            ViewModel = new ProjectEditDialogViewModel(project);
            DataContext = ViewModel;
            
            // Subscribe to dialog events
            ViewModel.SaveRequested += OnSaveRequested;
            ViewModel.CancelRequested += OnCancelRequested;
            
            // Set window title based on operation
            Title = project == null ? "Add New Project" : "Edit Project";
        }

        private void OnSaveRequested(object? sender, EventArgs e)
        {
            Result = ViewModel.Project;
            DialogResult = true;
            Close();
        }

        private void OnCancelRequested(object? sender, EventArgs e)
        {
            Result = null;
            DialogResult = false;
            Close();
        }

        protected override void OnClosed(EventArgs e)
        {
            // Unsubscribe from events to prevent memory leaks
            ViewModel.SaveRequested -= OnSaveRequested;
            ViewModel.CancelRequested -= OnCancelRequested;
            ViewModel.Cleanup();
            base.OnClosed(e);
        }
    }
}