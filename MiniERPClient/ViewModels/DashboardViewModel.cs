using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using MiniERPClient.Commands;
using MiniERPClient.Models;
using MiniERPClient.Services;

namespace MiniERPClient.ViewModels
{
    public class DashboardViewModel : ViewModelBase
    {
        private readonly IDashboardService _dashboardService;
        private bool _isLoading;
        private string _statusMessage = "Ready";
        private DashboardMetrics? _metrics;
        private AlertItem? _selectedAlert;

        public DashboardViewModel() : this(new DashboardService())
        {
        }

        public DashboardViewModel(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
            
            // Initialize collections
            KPICards = new ObservableCollection<KPICard>();
            RevenueChartData = new ObservableCollection<ChartDataPoint>();
            ExpenseChartData = new ObservableCollection<ChartDataPoint>();
            ProjectStatusChartData = new ObservableCollection<ChartDataPoint>();
            CustomerTypeChartData = new ObservableCollection<ChartDataPoint>();
            Alerts = new ObservableCollection<AlertItem>();
            
            // Initialize commands
            LoadDashboardCommand = new RelayCommand(async () => await LoadDashboardAsync(), () => !IsLoading);
            RefreshDataCommand = new RelayCommand(async () => await RefreshAllDataAsync(), () => !IsLoading);
            MarkAlertReadCommand = new RelayCommand<AlertItem>(async (alert) => await MarkAlertAsReadAsync(alert), (alert) => alert != null);
            ClearAlertsCommand = new RelayCommand(ClearAllAlerts);
        }

        #region Properties

        public ObservableCollection<KPICard> KPICards { get; }
        public ObservableCollection<ChartDataPoint> RevenueChartData { get; }
        public ObservableCollection<ChartDataPoint> ExpenseChartData { get; }
        public ObservableCollection<ChartDataPoint> ProjectStatusChartData { get; }
        public ObservableCollection<ChartDataPoint> CustomerTypeChartData { get; }
        public ObservableCollection<AlertItem> Alerts { get; }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        public DashboardMetrics? Metrics
        {
            get => _metrics;
            set => SetProperty(ref _metrics, value);
        }

        public AlertItem? SelectedAlert
        {
            get => _selectedAlert;
            set => SetProperty(ref _selectedAlert, value);
        }

        // Quick Stats Properties
        public string TotalRevenue => Metrics?.TotalRevenue.ToString("C0") ?? "$0";
        public string TotalExpenses => Metrics?.TotalExpenses.ToString("C0") ?? "$0";
        public string NetProfit => Metrics?.NetProfit.ToString("C0") ?? "$0";
        public string ProfitMargin => Metrics?.ProfitMargin.ToString("F1") + "%" ?? "0%";
        public string LastUpdated => Metrics?.LastUpdated.ToString("MM/dd/yyyy HH:mm") ?? "Never";

        // Alert Summary
        public int TotalAlerts => Alerts.Count;
        public int UnreadAlerts => Alerts.Count(a => !a.IsRead);
        public int CriticalAlerts => Alerts.Count(a => a.Priority == AlertPriority.Critical);
        public int WarningAlerts => Alerts.Count(a => a.Type == AlertType.Warning);

        #endregion

        #region Commands

        public ICommand LoadDashboardCommand { get; }
        public ICommand RefreshDataCommand { get; }
        public ICommand MarkAlertReadCommand { get; }
        public ICommand ClearAlertsCommand { get; }

        #endregion

        #region Public Methods
        
        public async Task InitializeAsync()
        {
            await LoadDashboardAsync();
        }
        
        #endregion

        #region Methods

        private async Task LoadDashboardAsync()
        {
            try
            {
                IsLoading = true;
                StatusMessage = "Loading dashboard...";

                // Load all dashboard data in parallel
                var metricsTask = _dashboardService.GetDashboardMetricsAsync();
                var kpiTask = _dashboardService.GetKPICardsAsync();
                var revenueTask = _dashboardService.GetRevenueChartDataAsync(6);
                var expenseTask = _dashboardService.GetExpenseChartDataAsync(6);
                var projectStatusTask = _dashboardService.GetProjectStatusChartDataAsync();
                var customerTypeTask = _dashboardService.GetCustomerTypeChartDataAsync();
                var alertsTask = _dashboardService.GetAlertsAsync();

                await Task.WhenAll(metricsTask, kpiTask, revenueTask, expenseTask, 
                                 projectStatusTask, customerTypeTask, alertsTask);

                // Update UI with results - these operations should be on the UI thread
                Metrics = await metricsTask;
                
                KPICards.Clear();
                var kpiResults = await kpiTask;
                foreach (var kpi in kpiResults)
                {
                    KPICards.Add(kpi);
                }
                
                RevenueChartData.Clear();
                var revenueResults = await revenueTask;
                foreach (var data in revenueResults)
                {
                    RevenueChartData.Add(data);
                }
                
                ExpenseChartData.Clear();
                var expenseResults = await expenseTask;
                foreach (var data in expenseResults)
                {
                    ExpenseChartData.Add(data);
                }
                
                ProjectStatusChartData.Clear();
                var projectResults = await projectStatusTask;
                foreach (var data in projectResults)
                {
                    ProjectStatusChartData.Add(data);
                }
                
                CustomerTypeChartData.Clear();
                var customerResults = await customerTypeTask;
                foreach (var data in customerResults)
                {
                    CustomerTypeChartData.Add(data);
                }
                
                Alerts.Clear();
                var alertResults = await alertsTask;
                foreach (var alert in alertResults)
                {
                    Alerts.Add(alert);
                }

                StatusMessage = "Dashboard loaded successfully";
                
                // Notify computed properties
                OnPropertyChanged(nameof(TotalRevenue));
                OnPropertyChanged(nameof(TotalExpenses));
                OnPropertyChanged(nameof(NetProfit));
                OnPropertyChanged(nameof(ProfitMargin));
                OnPropertyChanged(nameof(LastUpdated));
                OnPropertyChanged(nameof(TotalAlerts));
                OnPropertyChanged(nameof(UnreadAlerts));
                OnPropertyChanged(nameof(CriticalAlerts));
                OnPropertyChanged(nameof(WarningAlerts));
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error loading dashboard: {ex.Message}";
                MessageBox.Show($"Error loading dashboard: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task RefreshAllDataAsync()
        {
            await LoadDashboardAsync();
        }

        private async Task MarkAlertAsReadAsync(AlertItem? alert)
        {
            if (alert == null) return;

            try
            {
                StatusMessage = "Marking alert as read...";
                
                var success = await _dashboardService.MarkAlertAsReadAsync(alert.Id);
                if (success)
                {
                    alert.IsRead = true;
                    OnPropertyChanged(nameof(UnreadAlerts));
                    
                    StatusMessage = "Alert marked as read";
                }
                else
                {
                    StatusMessage = "Failed to mark alert as read";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error marking alert as read: {ex.Message}";
            }
        }

        private void ClearAllAlerts()
        {
            var result = MessageBox.Show(
                "Are you sure you want to clear all alerts?",
                "Confirm Clear",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                Alerts.Clear();
                OnPropertyChanged(nameof(TotalAlerts));
                OnPropertyChanged(nameof(UnreadAlerts));
                OnPropertyChanged(nameof(CriticalAlerts));
                OnPropertyChanged(nameof(WarningAlerts));
                
                StatusMessage = "All alerts cleared";
            }
        }

        #endregion
    }
}