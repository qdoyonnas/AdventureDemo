using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Controls;
using System.Threading.Tasks;

namespace WaywardEngine
{
    public static class ContextMenuHelper
    {
        /// <summary>
        /// Add a context menu option to the control.
        /// </summary>
        /// <param name="control">FrameworkElement to add option to.</param>
        /// <param name="label">Text of the added option.</param>
        /// <param name="action">Action to be performed by option.</param>
        public static void AddContextMenuItem( FrameworkElement control, string label, RoutedEventHandler action )
        {
            if( !CheckContextMenu(control) ) { return; }

            AddMenuItem( control.ContextMenu, label, action );
        }
        private static void AddMenuItem( ItemsControl menu, string label, RoutedEventHandler action )
        {
            if( menu == null ) { return; }

            MenuItem newItem = new MenuItem();
            newItem.Header = label;
            if( action != null ) {
                newItem.Click += action;
            }

            menu.Items.Insert(0, newItem);
        }

        /// <summary>
        /// Add multiple option to context menu of the control.
        /// </summary>
        /// <param name="control">FrameworkElement to add options to.</param>
        /// <param name="items">Dictionary of Label, Action value pairs.</param>
        public static void AddContextMenuItems( FrameworkElement control, Dictionary<string, RoutedEventHandler> items )
        {
            AddContextMenuItems( control, string.Empty, items );
        }
        /// <summary>
        /// Add multiple options to a submenu in the context menu of the control. If the submenu header does not exist it
        /// will be created first.
        /// </summary>
        /// <param name="control">FrameworkElement to add options to.</param>
        /// <param name="header">Submenu header name.</param>
        /// <param name="items">Dictionary of Label, Action value pairs</param>
        public static void AddContextMenuItems( FrameworkElement control, string header, Dictionary<string, RoutedEventHandler> items )
        {
            if( !CheckContextMenu(control) ) { return; }

            MenuItem headerItem = null;
            if( !string.IsNullOrEmpty(header) ) {
                foreach( MenuItem item in control.ContextMenu.Items ) {
                    if( (string)item.Header == header ) {
                        headerItem = item;
                    }
                }
                if( headerItem == null ) {
                    AddContextMenuHeader( control, header, items );
                    return;
                }
            }

            AddContextMenuItems( control, headerItem, items );
        }
        /// <summary>
        /// Add multiple options to a submenu in the context menu of the control. If the submenu header does not exist
        /// the options will be added to the root of the context menu.
        /// </summary>
        /// <param name="control">FrameworkElement to add options to.</param>
        /// <param name="headerItem">Label of submenu header.</param>
        /// <param name="items">Dictionary of Label, Action value pairs.</param>
        public static void AddContextMenuItems( FrameworkElement control, MenuItem headerItem, Dictionary<string, RoutedEventHandler> items )
        {
            if( !CheckContextMenu(control) ) { return; }
            
            string[] keys = items.Keys.ToArray();
            for( int i = keys.Length-1; i >= 0; i-- ) {
                if( headerItem == null ) {
                    AddMenuItem( control.ContextMenu, keys[i], items[keys[i]] );
                } else {
                    AddMenuItem( headerItem, keys[i], items[keys[i]] );
                }
            }
        }

        /// <summary>
        /// Adds a new submenu to the context menu of the control and populates it with the provided options.
        /// </summary>
        /// <param name="control">FrameworkElement to add options to.</param>
        /// <param name="header">Label of submenu header.</param>
        /// <param name="items">Dictionary of Label, Action value pairs.</param>
        /// <returns></returns>
        public static MenuItem AddContextMenuHeader( FrameworkElement control, string header, Dictionary<string, RoutedEventHandler> items )
        {
            if( !CheckContextMenu(control) ) { return null; }

            MenuItem newHeader = AddContextMenuHeader( control, header );
            AddContextMenuItems( control, newHeader, items );

            return newHeader;
        }
        /// <summary>
        /// Adds a new submenu to the context menu of the control.
        /// </summary>
        /// <param name="control">FrameworkElement to add options to.</param>
        /// <param name="header">Label of submenu header.</param>
        /// <returns></returns>
        public static MenuItem AddContextMenuHeader( FrameworkElement control, string header )
        {
            if( !CheckContextMenu(control) ) { return null; }

            MenuItem newHeader = null;
            if( !string.IsNullOrEmpty(header) ) {
                newHeader = new MenuItem();
                newHeader.Header = header;
                control.ContextMenu.Items.Insert(0, newHeader);
            }

            return newHeader;
        }

        /// <summary>
        /// Checks for the presence of a context menu on the control, and, if there isn't one, adds one.
        /// Returns false only in the case that the provided control is null.
        /// </summary>
        /// <param name="control">FrameworkElement to be checked.</param>
        /// <returns></returns>
        private static bool CheckContextMenu( FrameworkElement control )
        {
            if( control == null ) { return false; }

            if( control.ContextMenu == null ) {
                control.ContextMenu = new ContextMenu();
            }

            return true;
        }

        /// <summary>
        /// Add a context menu option to the control.
        /// </summary>
        /// <param name="control">FrameworkContentElement to add option to.</param>
        /// <param name="label">Text of the added option.</param>
        /// <param name="action">Action to be performed by option.</param>
        public static void AddContextMenuItem( FrameworkContentElement control, string label, RoutedEventHandler action )
        {
            if( !CheckContextMenu(control) ) { return; }

            AddMenuItem( control.ContextMenu, label, action );
        }

        /// <summary>
        /// Add multiple option to context menu of the control.
        /// </summary>
        /// <param name="control">FrameworkContentElement to add options to.</param>
        /// <param name="items">Dictionary of Label, Action value pairs.</param>
        public static void AddContextMenuItems( FrameworkContentElement control, Dictionary<string, RoutedEventHandler> items )
        {
            AddContextMenuItems( control, string.Empty, items );
        }
        /// <summary>
        /// Add multiple options to a submenu in the context menu of the control. If the submenu header does not exist it
        /// will be created first.
        /// </summary>
        /// <param name="control">FrameworkContentElement to add options to.</param>
        /// <param name="header">Submenu header name.</param>
        /// <param name="items">Dictionary of Label, Action value pairs</param>
        public static void AddContextMenuItems( FrameworkContentElement control, string header, Dictionary<string, RoutedEventHandler> items )
        {
            if( !CheckContextMenu(control) ) { return; }

            MenuItem headerItem = null;
            if( !string.IsNullOrEmpty(header) ) {
                foreach( MenuItem item in control.ContextMenu.Items ) {
                    if( (string)item.Header == header ) {
                        headerItem = item;
                    }
                }
                if( headerItem == null ) {
                    AddContextMenuHeader( control, header, items );
                    return;
                }
            }

            AddContextMenuItems( control, headerItem, items );
        }
        /// <summary>
        /// Add multiple options to a submenu in the context menu of the control. If the submenu header does not exist
        /// the options will be added to the root of the context menu.
        /// </summary>
        /// <param name="control">FrameworkContentElement to add options to.</param>
        /// <param name="headerItem">Label of submenu header.</param>
        /// <param name="items">Dictionary of Label, Action value pairs.</param>
        public static void AddContextMenuItems( FrameworkContentElement control, MenuItem headerItem, Dictionary<string, RoutedEventHandler> items )
        {
            if( !CheckContextMenu(control) ) { return; }
            
            string[] keys = items.Keys.ToArray();
            for( int i = keys.Length-1; i >= 0; i-- ) {
                if( headerItem == null ) {
                    AddMenuItem( control.ContextMenu, keys[i], items[keys[i]] );
                } else {
                    AddMenuItem( headerItem, keys[i], items[keys[i]] );
                }
            }
        }

        /// <summary>
        /// Adds a new submenu to the context menu of the control and populates it with the provided options.
        /// </summary>
        /// <param name="control">FrameworkContentElement to add options to.</param>
        /// <param name="header">Label of submenu header.</param>
        /// <param name="items">Dictionary of Label, Action value pairs.</param>
        /// <returns></returns>
        public static MenuItem AddContextMenuHeader( FrameworkContentElement control, string header, Dictionary<string, RoutedEventHandler> items )
        {
            if( !CheckContextMenu(control) ) { return null; }

            MenuItem newHeader = AddContextMenuHeader( control, header );
            AddContextMenuItems( control, newHeader, items );

            return newHeader;
        }
        /// <summary>
        /// Adds a new submenu to the context menu of the control.
        /// </summary>
        /// <param name="control">FrameworkContentElement to add options to.</param>
        /// <param name="header">Label of submenu header.</param>
        /// <returns></returns>
        public static MenuItem AddContextMenuHeader( FrameworkContentElement control, string header )
        {
            if( !CheckContextMenu(control) ) { return null; }

            MenuItem newHeader = null;
            if( !string.IsNullOrEmpty(header) ) {
                newHeader = new MenuItem();
                newHeader.Header = header;
                control.ContextMenu.Items.Insert(0, newHeader);
            }

            return newHeader;
        }


        /// <summary>
        /// Checks for the presence of a context menu on the control, and, if there isn't one, adds one.
        /// Returns false only in the case that the provided control is null.
        /// </summary>
        /// <param name="control">FrameworkContentElement to be checked.</param>
        /// <returns></returns>
        private static bool CheckContextMenu( FrameworkContentElement control )
        {
            if( control == null ) { return false; }

            if( control.ContextMenu == null ) {
                control.ContextMenu = new ContextMenu();
            }

            return true;
        }
    }
}
