using MiniERPClient.ViewModels;
using MiniERPClient.Views;
using System.Windows;
using System.Windows.Controls;

namespace MiniERPClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private UserControl? _currentView;
        private MainViewModel _employeeViewModel;
        private EmployeeManagementView? _employeeView;

        public MainWindow()
        {
            InitializeComponent();
            _employeeViewModel = new MainViewModel();
            
            // Create and initialize employee view
            _employeeView = new EmployeeManagementView();
            _employeeView.DataContext = _employeeViewModel;
            
            // Set up the initial view
            ShowEmployeeManagement();
        }

        private void ShowEmployeeManagement()
        {
            ContentArea.Children.Clear();
            if (_employeeView != null)
            {
                ContentArea.Children.Add(_employeeView);
                Grid.SetColumnSpan(_employeeView, 3);
            }
            _currentView = _employeeView;
            Title = "Mini ERP Client - Employee Management";
            UpdateNavigationHighlight("Employee");
        }

        private void EmployeeButton_Click(object sender, RoutedEventArgs e)
        {
            ShowEmployeeManagement();
        }

        private void CustomerButton_Click(object sender, RoutedEventArgs e)
        {
            // Clear current content and show Customer Management
            ContentArea.Children.Clear();
            
            // Create and set Customer Management view
            var customerView = new CustomerManagementView();
            customerView.DataContext = new CustomerViewModel();
            
            // Add the customer view to the content area
            ContentArea.Children.Add(customerView);
            Grid.SetColumnSpan(customerView, 3); // Span all columns
            
            _currentView = customerView;
            
            // Update title
            Title = "Mini ERP Client - Customer Management";
            UpdateNavigationHighlight("Customer");
        }

        private void ProductButton_Click(object sender, RoutedEventArgs e)
        {
            // Clear current content and show Product Management
            ContentArea.Children.Clear();
            
            // Create and set Product Management view
            var productView = new ProductManagementView();
            productView.DataContext = new ProductViewModel();
            
            // Add the product view to the content area
            ContentArea.Children.Add(productView);
            Grid.SetColumnSpan(productView, 3); // Span all columns
            
            _currentView = productView;
            
            // Update title
            Title = "Mini ERP Client - Product Management";
            UpdateNavigationHighlight("Product");
        }

        private void ProjectButton_Click(object sender, RoutedEventArgs e)
        {
            // Clear current content and show Project Management
            ContentArea.Children.Clear();
            
            // Create and set Project Management view
            var projectView = new ProjectManagementView();
            projectView.DataContext = new ProjectViewModel();
            
            // Add the project view to the content area
            ContentArea.Children.Add(projectView);
            Grid.SetColumnSpan(projectView, 3); // Span all columns
            
            _currentView = projectView;
            
            // Update title
            Title = "Mini ERP Client - Project Management";
            UpdateNavigationHighlight("Project");
        }

        private async void DashboardButton_Click(object sender, RoutedEventArgs e)
        {
            // Clear current content and show Dashboard Analytics
            ContentArea.Children.Clear();
            
            // Create and set Dashboard view
            var dashboardView = new DashboardAnalyticsView();
            var dashboardViewModel = new DashboardViewModel();
            dashboardView.DataContext = dashboardViewModel;
            
            // Add the dashboard view to the content area
            ContentArea.Children.Add(dashboardView);
            Grid.SetColumnSpan(dashboardView, 3); // Span all columns
            
            _currentView = dashboardView;
            
            // Update title
            Title = "Mini ERP Client - Executive Dashboard";
            UpdateNavigationHighlight("Dashboard");
            
            // Initialize dashboard data after view is fully set up
            await dashboardViewModel.InitializeAsync();
        }

        private void UpdateNavigationHighlight(string activeView)
        {
            // Reset all buttons to inactive state
            EmployeeButton.Tag = null;
            CustomerButton.Tag = null;
            ProductButton.Tag = null;
            ProjectButton.Tag = null;
            DashboardButton.Tag = null;

            // Set the active button
            switch (activeView)
            {
                case "Employee":
                    EmployeeButton.Tag = "Active";
                    break;
                case "Customer":
                    CustomerButton.Tag = "Active";
                    break;
                case "Product":
                    ProductButton.Tag = "Active";
                    break;
                case "Project":
                    ProjectButton.Tag = "Active";
                    break;
                case "Dashboard":
                    DashboardButton.Tag = "Active";
                    break;
            }
        }
    }
}