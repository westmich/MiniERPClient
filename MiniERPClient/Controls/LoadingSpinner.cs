using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MiniERPClient.Controls
{
    public class LoadingSpinner : Control
    {
        static LoadingSpinner()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LoadingSpinner), 
                new FrameworkPropertyMetadata(typeof(LoadingSpinner)));
        }

        public static readonly DependencyProperty IsLoadingProperty =
            DependencyProperty.Register(nameof(IsLoading), typeof(bool), typeof(LoadingSpinner),
                new PropertyMetadata(false, OnIsLoadingChanged));

        public static readonly DependencyProperty LoadingTextProperty =
            DependencyProperty.Register(nameof(LoadingText), typeof(string), typeof(LoadingSpinner),
                new PropertyMetadata("Loading..."));

        public static readonly DependencyProperty SpinnerSizeProperty =
            DependencyProperty.Register(nameof(SpinnerSize), typeof(double), typeof(LoadingSpinner),
                new PropertyMetadata(24.0));

        public static readonly DependencyProperty SpinnerColorProperty =
            DependencyProperty.Register(nameof(SpinnerColor), typeof(Brush), typeof(LoadingSpinner),
                new PropertyMetadata(new SolidColorBrush(Colors.Blue)));

        public bool IsLoading
        {
            get => (bool)GetValue(IsLoadingProperty);
            set => SetValue(IsLoadingProperty, value);
        }

        public string LoadingText
        {
            get => (string)GetValue(LoadingTextProperty);
            set => SetValue(LoadingTextProperty, value);
        }

        public double SpinnerSize
        {
            get => (double)GetValue(SpinnerSizeProperty);
            set => SetValue(SpinnerSizeProperty, value);
        }

        public Brush SpinnerColor
        {
            get => (Brush)GetValue(SpinnerColorProperty);
            set => SetValue(SpinnerColorProperty, value);
        }

        private static void OnIsLoadingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is LoadingSpinner spinner)
            {
                spinner.UpdateVisualState();
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            UpdateVisualState();
        }

        private void UpdateVisualState()
        {
            VisualStateManager.GoToState(this, IsLoading ? "Loading" : "NotLoading", true);
        }
    }
}