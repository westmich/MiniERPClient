using System.Windows;
using MiniERPClient.ViewModels;
using MiniERPClient.Models;

namespace MiniERPClient.Views
{
    public partial class ProductEditWindow : Window
    {
        public ProductEditDialogViewModel ViewModel { get; private set; }
        public Product? Result { get; private set; }

        public ProductEditWindow(Product? product = null)
        {
            InitializeComponent();
            
            ViewModel = new ProductEditDialogViewModel(product);
            DataContext = ViewModel;
            
            // Subscribe to dialog events
            ViewModel.SaveRequested += OnSaveRequested;
            ViewModel.CancelRequested += OnCancelRequested;
            
            // Set window title based on operation
            Title = product == null ? "Add New Product" : "Edit Product";
        }

        private void OnSaveRequested(object? sender, EventArgs e)
        {
            Result = ViewModel.Product;
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