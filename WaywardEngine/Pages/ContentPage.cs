using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WaywardEngine
{
    public class ContentPage : Page
    {
        Label title;
        StackPanel contents;

        public ContentPage()
            : base("BlankPage")
        {
            title = Utilities.FindNode<Label>( element, "Title" );
            contents = Utilities.FindNode<StackPanel>( element, "Contents" );

            ContextMenuHelper.AddContextMenuItem( element, "Close", CloseAction );
        }

        public void SetTitle( string sTitle )
        {
            title.Content = sTitle;
        }
        public void SetTitleVisibility( bool state )
        {
            title.Visibility = state ? Visibility.Visible : Visibility.Collapsed;
        }
        /// <summary>
        /// Add FrameworkElement to Page StackPanel.
        /// </summary>
        /// <param name="content"></param>
        public void AddContent( FrameworkElement content )
        {
            contents.Children.Add( content );
        }
        /// <summary>
        /// Remove FrameworkElement from Page StackPanel.
        /// </summary>
        /// <param name="content"></param>
        public void RemoveContent( FrameworkElement content )
        {
            contents.Children.Remove(content);
        }

        public virtual void Clear()
        {
            contents.Children.Clear();
        }
    }
}
