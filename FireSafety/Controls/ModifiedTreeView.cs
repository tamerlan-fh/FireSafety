using FireSafety.Models;
using System.Windows;
using System.Windows.Controls;

namespace FireSafety.Controls
{
    class ModifiedTreeView : TreeView
    {
        public Entity ModifiedSelectedItem
        {
            get { return (Entity)GetValue(ModifiedSelectedItemProperty); }
            set { SetValue(ModifiedSelectedItemProperty, value); }
        }
        public static readonly DependencyProperty ModifiedSelectedItemProperty = DependencyProperty.Register("ModifiedSelectedItem", typeof(Entity), typeof(ModifiedTreeView));

        public ModifiedTreeView() { }

        protected override void OnSelectedItemChanged(RoutedPropertyChangedEventArgs<object> e)
        {
            ModifiedSelectedItem = e.NewValue as Entity;
            base.OnSelectedItemChanged(e);
        }
    }
}
