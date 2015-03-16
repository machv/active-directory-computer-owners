using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Mach.Wpf.Mvvm
{
    public class TreeViewSelectionBehavior<T> where T : class
    {
        // Declare our attached property, it needs to be a DependencyProperty so
        // we can bind to it from oout ViewMode.
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.RegisterAttached(
            "SelectedItem",
            typeof(T),
            typeof(TreeViewSelectionBehavior<T>),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                new PropertyChangedCallback(SelectedItemChanged)));

        // We need a Get method for our new property
        public static T GetSelectedItem(DependencyObject dependencyObject)
        {
            return (T)dependencyObject.GetValue(SelectedItemProperty);
        }

        // As well as a Set method for our new property
        public static void SetSelectedItem(DependencyObject dependencyObject, T value)
        {
            dependencyObject.SetValue(SelectedItemProperty, value);
        }

        // This is the handler for when our new property's value changes
        // When our property is set to a non null value we need to add an event handler
        // for the TreeView's SelectedItemChanged event
        private static void SelectedItemChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            TreeView tv = dependencyObject as TreeView;

            if (e.NewValue == null && e.OldValue != null)
            {
                tv.SelectedItemChanged -= new RoutedPropertyChangedEventHandler<object>(tv_SelectedItemChanged);
            }
            else if (e.NewValue != null && e.OldValue == null)
            {
                tv.SelectedItemChanged += new RoutedPropertyChangedEventHandler<object>(tv_SelectedItemChanged);
            }

            var command = (ICommand)(tv as DependencyObject).GetValue(SelectedItemChangedProperty);
            if (command != null)
            {
                if (command.CanExecute(null))
                    command.Execute(new DependencyPropertyEventArgs(e));
            }
        }

        // When TreeView.SelectedItemChanged fires, set our new property to the value
        static void tv_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            SetSelectedItem((DependencyObject)sender, (T)(e.NewValue));
        }

        #region Selected Item Changed

        public static ICommand GetSelectedItemChanged(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(SelectedItemProperty);
        }

        public static void SetSelectedItemChanged(DependencyObject obj, ICommand value)
        {
            obj.SetValue(SelectedItemProperty, value);
        }

        public static readonly DependencyProperty SelectedItemChangedProperty =
            DependencyProperty.RegisterAttached("SelectedItemChanged", typeof(ICommand), typeof(TreeViewSelectionBehavior<T>));

        #endregion

        #region Event Args
        public class DependencyPropertyEventArgs : EventArgs
        {
            public DependencyPropertyChangedEventArgs DependencyPropertyChangedEventArgs { get; private set; }

            public DependencyPropertyEventArgs(DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
            {
                this.DependencyPropertyChangedEventArgs = dependencyPropertyChangedEventArgs;
            }
        }
        #endregion
    }
}
