using FireSafety.Models;
using System;
using System.Windows;
using System.Windows.Controls;

namespace FireSafety.Controls
{
    class ModifiedTreeView : TreeView
    {
        public Entity ModifiedSelectedItem
        {
            get { return (Entity)GetValue(mySelectedItemProperty); }
            set { SetValue(mySelectedItemProperty, value); }
        }
        public static readonly DependencyProperty mySelectedItemProperty = DependencyProperty.Register("ModifiedSelectedItem", typeof(Entity), typeof(ModifiedTreeView));

        public ModifiedTreeView()
        {

        }

        protected override void OnSelectedItemChanged(RoutedPropertyChangedEventArgs<object> e)
        {
            ModifiedSelectedItem = e.NewValue as Entity;
            base.OnSelectedItemChanged(e);
        }
    }
}
