using MiniERPClient.ViewModels;
using System.Windows;

namespace MiniERPClient
{
    /// <summary>
    /// Interaction logic for CustomerTestWindow.xaml
    /// </summary>
    public partial class CustomerTestWindow : Window
    {
        public CustomerTestWindow()
        {
            InitializeComponent();
            DataContext = new CustomerViewModel();
        }
    }
}