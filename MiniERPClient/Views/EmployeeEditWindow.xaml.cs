using System.Windows;
using MiniERPClient.ViewModels;
using MiniERPClient.Models;

namespace MiniERPClient.Views
{
    public partial class EmployeeEditWindow : Window
    {
        public EmployeeEditDialogViewModel ViewModel { get; private set; }
        public Employee? Result { get; private set; }

        public EmployeeEditWindow(Employee? employee = null)
        {
            InitializeComponent();
            
            ViewModel = new EmployeeEditDialogViewModel(employee);
            DataContext = ViewModel;
            
            // Subscribe to dialog events
            ViewModel.SaveRequested += OnSaveRequested;
            ViewModel.CancelRequested += OnCancelRequested;
            
            // Set window title based on operation
            Title = employee == null ? "Add New Employee" : "Edit Employee";
        }

        private void OnSaveRequested(object? sender, EventArgs e)
        {
            Result = ViewModel.Employee;
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