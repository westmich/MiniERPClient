using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MiniERPClient.Controls
{
    public class SearchBox : Control
    {
        static SearchBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SearchBox),
                new FrameworkPropertyMetadata(typeof(SearchBox)));
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(nameof(Text), typeof(string), typeof(SearchBox),
                new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnTextChanged));

        public static readonly DependencyProperty PlaceholderTextProperty =
            DependencyProperty.Register(nameof(PlaceholderText), typeof(string), typeof(SearchBox),
                new PropertyMetadata("Search..."));

        public static readonly DependencyProperty ClearCommandProperty =
            DependencyProperty.Register(nameof(ClearCommand), typeof(ICommand), typeof(SearchBox));

        public static readonly DependencyProperty SearchCommandProperty =
            DependencyProperty.Register(nameof(SearchCommand), typeof(ICommand), typeof(SearchBox));

        public static readonly RoutedEvent TextChangedEvent =
            EventManager.RegisterRoutedEvent(nameof(TextChanged), RoutingStrategy.Bubble,
                typeof(RoutedEventHandler), typeof(SearchBox));

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public string PlaceholderText
        {
            get => (string)GetValue(PlaceholderTextProperty);
            set => SetValue(PlaceholderTextProperty, value);
        }

        public ICommand ClearCommand
        {
            get => (ICommand)GetValue(ClearCommandProperty);
            set => SetValue(ClearCommandProperty, value);
        }

        public ICommand SearchCommand
        {
            get => (ICommand)GetValue(SearchCommandProperty);
            set => SetValue(SearchCommandProperty, value);
        }

        public event RoutedEventHandler TextChanged
        {
            add => AddHandler(TextChangedEvent, value);
            remove => RemoveHandler(TextChangedEvent, value);
        }

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SearchBox searchBox)
            {
                searchBox.RaiseEvent(new RoutedEventArgs(TextChangedEvent));
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (GetTemplateChild("PART_ClearButton") is Button clearButton)
            {
                clearButton.Click += (s, e) =>
                {
                    Text = string.Empty;
                    ClearCommand?.Execute(null);
                };
            }

            if (GetTemplateChild("PART_SearchButton") is Button searchButton)
            {
                searchButton.Click += (s, e) => SearchCommand?.Execute(Text);
            }

            if (GetTemplateChild("PART_TextBox") is TextBox textBox)
            {
                textBox.KeyDown += (s, e) =>
                {
                    if (e.Key == Key.Enter)
                    {
                        SearchCommand?.Execute(Text);
                    }
                };
            }
        }
    }
}