using System.Windows;
using MiniERPClient.ViewModels;
using MiniERPClient.Models;

namespace MiniERPClient.Views
{
    public partial class CustomerEditWindow : Window
    {
        public CustomerEditDialogViewModel ViewModel { get; private set; }
        public Customer? Result { get; private set; }

        public CustomerEditWindow(Customer? customer = null)
        {
            InitializeComponent();
            
            ViewModel = new CustomerEditDialogViewModel(customer);
            DataContext = ViewModel;
            
            // Subscribe to dialog events
            ViewModel.SaveRequested += OnSaveRequested;
            ViewModel.CancelRequested += OnCancelRequested;
            
            // Set window title based on operation
            Title = customer == null ? "Add New Customer" : "Edit Customer";
        }

        private void OnSaveRequested(object? sender, EventArgs e)
        {
            Result = ViewModel.Customer;
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