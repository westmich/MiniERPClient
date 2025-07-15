using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MiniERPClient.Behaviors
{
    public static class FocusBehavior
    {
        public static readonly DependencyProperty IsFocusedProperty =
            DependencyProperty.RegisterAttached("IsFocused", typeof(bool), typeof(FocusBehavior),
                new PropertyMetadata(false, OnIsFocusedChanged));

        public static bool GetIsFocused(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsFocusedProperty);
        }

        public static void SetIsFocused(DependencyObject obj, bool value)
        {
            obj.SetValue(IsFocusedProperty, value);
        }

        private static void OnIsFocusedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is UIElement element && (bool)e.NewValue)
            {
                element.Focus();
            }
        }
    }

    public static class ValidationBehavior
    {
        public static readonly DependencyProperty ShowValidationErrorsProperty =
            DependencyProperty.RegisterAttached("ShowValidationErrors", typeof(bool), typeof(ValidationBehavior),
                new PropertyMetadata(false, OnShowValidationErrorsChanged));

        public static bool GetShowValidationErrors(DependencyObject obj)
        {
            return (bool)obj.GetValue(ShowValidationErrorsProperty);
        }

        public static void SetShowValidationErrors(DependencyObject obj, bool value)
        {
            obj.SetValue(ShowValidationErrorsProperty, value);
        }

        private static void OnShowValidationErrorsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FrameworkElement element)
            {
                if ((bool)e.NewValue)
                {
                    element.Loaded += OnElementLoaded;
                }
                else
                {
                    element.Loaded -= OnElementLoaded;
                }
            }
        }

        private static void OnElementLoaded(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element)
            {
                ShowValidationErrorsRecursive(element);
            }
        }

        private static void ShowValidationErrorsRecursive(DependencyObject parent)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                
                if (child is TextBox textBox)
                {
                    textBox.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
                }
                else if (child is ComboBox comboBox)
                {
                    comboBox.GetBindingExpression(ComboBox.SelectedItemProperty)?.UpdateSource();
                }

                ShowValidationErrorsRecursive(child);
            }
        }
    }

    public static class AnimationBehavior
    {
        public static readonly DependencyProperty AnimateOnLoadProperty =
            DependencyProperty.RegisterAttached("AnimateOnLoad", typeof(bool), typeof(AnimationBehavior),
                new PropertyMetadata(false, OnAnimateOnLoadChanged));

        public static bool GetAnimateOnLoad(DependencyObject obj)
        {
            return (bool)obj.GetValue(AnimateOnLoadProperty);
        }

        public static void SetAnimateOnLoad(DependencyObject obj, bool value)
        {
            obj.SetValue(AnimateOnLoadProperty, value);
        }

        private static void OnAnimateOnLoadChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FrameworkElement element && (bool)e.NewValue)
            {
                element.Loaded += OnElementLoadedForAnimation;
            }
        }

        private static void OnElementLoadedForAnimation(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element)
            {
                element.RenderTransform = new TranslateTransform(0, 20);
                element.Opacity = 0;

                var storyboard = new System.Windows.Media.Animation.Storyboard();
                
                var opacityAnimation = new System.Windows.Media.Animation.DoubleAnimation
                {
                    From = 0,
                    To = 1,
                    Duration = TimeSpan.FromMilliseconds(300)
                };
                
                var translateAnimation = new System.Windows.Media.Animation.DoubleAnimation
                {
                    From = 20,
                    To = 0,
                    Duration = TimeSpan.FromMilliseconds(300)
                };

                System.Windows.Media.Animation.Storyboard.SetTarget(opacityAnimation, element);
                System.Windows.Media.Animation.Storyboard.SetTargetProperty(opacityAnimation, new PropertyPath("Opacity"));
                
                System.Windows.Media.Animation.Storyboard.SetTarget(translateAnimation, element);
                System.Windows.Media.Animation.Storyboard.SetTargetProperty(translateAnimation, new PropertyPath("RenderTransform.Y"));

                storyboard.Children.Add(opacityAnimation);
                storyboard.Children.Add(translateAnimation);
                storyboard.Begin();
            }
        }
    }
}