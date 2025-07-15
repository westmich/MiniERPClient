using System.Windows.Controls;
using MiniERPClient.ViewModels;

namespace MiniERPClient.Views
{
    /// <summary>
    /// Interaction logic for ProjectManagementView.xaml
    /// </summary>
    public partial class ProjectManagementView : UserControl
    {
        public ProjectManagementView()
        {
            InitializeComponent();
            
            // Set the DataContext to the ProjectViewModel for design-time support
            if (DataContext == null)
            {
                DataContext = new ProjectViewModel();
            }
        }

        private void Btn_Close_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            // This button is likely used to close the view or navigate away
            // The actual implementation depends on how navigation is handled in the main window
            // For now, we'll leave this empty as it may be handled by the parent container
            
            // If the parent window implements a navigation interface, it could be called here
            // Example: NavigationService?.GoBack() or Parent.Hide()
        }
    }
}