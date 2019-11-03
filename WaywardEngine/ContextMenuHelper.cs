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
        #region FrameworkElement - TextBlock Methods
        /// <summary>
        /// Add a context menu option to the control.
        /// </summary>
        /// <param name="control">FrameworkElement to add option to.</param>
        /// <param name="label">Span of the added option.</param>
        /// <param name="action">Action to be performed by option.</param>
        public static void AddContextMenuItem( FrameworkElement control, TextBlock label, RoutedEventHandler action, bool enabled = true )
        {
            if( !CheckContextMenu(control) ) { return; }

            AddMenuItem( control.ContextMenu, label, action, enabled );
        }
        private static void AddMenuItem( ItemsControl menu, TextBlock label, RoutedEventHandler action, bool enabled )
        {
            if( menu == null ) { return; }

            MenuItem newItem = new MenuItem();
            newItem.IsEnabled = enabled;
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
        public static void AddContextMenuItems( FrameworkElement control, Dictionary<TextBlock, RoutedEventHandler> items, bool enabled = true )
        {
            AddContextMenuItems( control, new TextBlock(), items, enabled );
        }
        /// <summary>
        /// Add multiple options to a submenu in the context menu of the control. If the submenu header does not exist it
        /// will be created first.
        /// </summary>
        /// <param name="control">FrameworkElement to add options to.</param>
        /// <param name="header">Submenu header name.</param>
        /// <param name="items">Dictionary of Label, Action value pairs</param>
        public static void AddContextMenuItems( FrameworkElement control, TextBlock header, Dictionary<TextBlock, RoutedEventHandler> items, bool enabled = true )
        {
            if( !CheckContextMenu(control) ) { return; }

            MenuItem headerItem = null;
            if( header != null ) {
                foreach( MenuItem item in control.ContextMenu.Items ) {
                    TextBlock headBlock = item.Header as TextBlock;
                    if( string.Compare(headBlock.Text.Trim(), header.Text.Trim(), true) == 0 ) {
                        headerItem = item;
                    }
                }
                if( headerItem == null ) {
                    AddContextMenuHeader( control, header, items, enabled );
                    return;
                }
            }

            AddContextMenuItems( control, headerItem, items, enabled );
        }
        /// <summary>
        /// Add multiple options to a submenu in the context menu of the control. If the submenu header does not exist
        /// the options will be added to the root of the context menu.
        /// </summary>
        /// <param name="control">FrameworkElement to add options to.</param>
        /// <param name="headerItem">Label of submenu header.</param>
        /// <param name="items">Dictionary of Label, Action value pairs.</param>
        public static void AddContextMenuItems( FrameworkElement control, MenuItem headerItem, Dictionary<TextBlock, RoutedEventHandler> items, bool enabled = true )
        {
            if( !CheckContextMenu(control) ) { return; }
            
            TextBlock[] keys = items.Keys.ToArray();
            for( int i = keys.Length-1; i >= 0; i-- ) {
                if( headerItem == null ) {
                    AddMenuItem( control.ContextMenu, keys[i], items[keys[i]], enabled );
                } else {
                    AddMenuItem( headerItem, keys[i], items[keys[i]], enabled );
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
        public static MenuItem AddContextMenuHeader( FrameworkElement control, TextBlock header, Dictionary<TextBlock, RoutedEventHandler> items, bool enabled = true )
        {
            if( !CheckContextMenu(control) ) { return null; }

            MenuItem newHeader = AddContextMenuHeader( control, header );
            AddContextMenuItems( control, newHeader, items, enabled );

            return newHeader;
        }
        /// <summary>
        /// Adds a new submenu to the context menu of the control.
        /// </summary>
        /// <param name="control">FrameworkElement to add options to.</param>
        /// <param name="header">Label of submenu header.</param>
        /// <returns></returns>
        public static MenuItem AddContextMenuHeader( FrameworkElement control, TextBlock header )
        {
            if( !CheckContextMenu(control) ) { return null; }

            foreach( MenuItem item in control.ContextMenu.Items ) {
                string itemHeader = item.Header as string;
                if( itemHeader == header.Text ) {
                    return item;
                }
            }

            MenuItem newHeader = null;
            if( header != null ) {
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
        #endregion

        #region FrameworkContentElement - TextBlock Methods
        /// <summary>
        /// Add a context menu option to the control.
        /// </summary>
        /// <param name="control">FrameworkContentElement to add option to.</param>
        /// <param name="label">Span of the added option.</param>
        /// <param name="action">Action to be performed by option.</param>
        public static void AddContextMenuItem( FrameworkContentElement control, TextBlock label, RoutedEventHandler action, bool enabled = true )
        {
            if( !CheckContextMenu(control) ) { return; }

            AddMenuItem( control.ContextMenu, label, action, enabled );
        }

        /// <summary>
        /// Add multiple option to context menu of the control.
        /// </summary>
        /// <param name="control">FrameworkContentElement to add options to.</param>
        /// <param name="items">Dictionary of Label, Action value pairs.</param>
        public static void AddContextMenuItems( FrameworkContentElement control, Dictionary<TextBlock, RoutedEventHandler> items, bool enabled = true )
        {
            AddContextMenuItems( control, new TextBlock(), items, enabled );
        }
        /// <summary>
        /// Add multiple options to a submenu in the context menu of the control. If the submenu header does not exist it
        /// will be created first.
        /// </summary>
        /// <param name="control">FrameworkContentElement to add options to.</param>
        /// <param name="header">Submenu header name.</param>
        /// <param name="items">Dictionary of Label, Action value pairs</param>
        public static void AddContextMenuItems( FrameworkContentElement control, TextBlock header, Dictionary<TextBlock, RoutedEventHandler> items, bool enabled = true )
        {
            if( !CheckContextMenu(control) ) { return; }

            MenuItem headerItem = null;
            if( header != null ) {
                foreach( MenuItem item in control.ContextMenu.Items ) {
                    TextBlock headBlock = item.Header as TextBlock;
                    if( string.Compare(headBlock.Text.Trim(), header.Text.Trim(), true) == 0 ) {
                        headerItem = item;
                    }
                }
                if( headerItem == null ) {
                    AddContextMenuHeader( control, header, items, enabled );
                    return;
                }
            }

            AddContextMenuItems( control, headerItem, items, enabled );
        }
        /// <summary>
        /// Add multiple options to a submenu in the context menu of the control. If the submenu header does not exist
        /// the options will be added to the root of the context menu.
        /// </summary>
        /// <param name="control">FrameworkContentElement to add options to.</param>
        /// <param name="headerItem">Label of submenu header.</param>
        /// <param name="items">Dictionary of Label, Action value pairs.</param>
        public static void AddContextMenuItems( FrameworkContentElement control, MenuItem headerItem, Dictionary<TextBlock, RoutedEventHandler> items, bool enabled = true )
        {
            if( !CheckContextMenu(control) ) { return; }
            
            TextBlock[] keys = items.Keys.ToArray();
            for( int i = keys.Length-1; i >= 0; i-- ) {
                if( headerItem == null ) {
                    AddMenuItem( control.ContextMenu, keys[i], items[keys[i]], enabled );
                } else {
                    AddMenuItem( headerItem, keys[i], items[keys[i]], enabled );
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
        public static MenuItem AddContextMenuHeader( FrameworkContentElement control, TextBlock header, Dictionary<TextBlock, RoutedEventHandler> items, bool enabled = true )
        {
            if( !CheckContextMenu(control) ) { return null; }

            MenuItem newHeader = AddContextMenuHeader( control, header );
            AddContextMenuItems( control, newHeader, items, enabled );

            return newHeader;
        }
        /// <summary>
        /// Adds a new submenu to the context menu of the control.
        /// </summary>
        /// <param name="control">FrameworkContentElement to add options to.</param>
        /// <param name="header">Label of submenu header.</param>
        /// <returns></returns>
        public static MenuItem AddContextMenuHeader( FrameworkContentElement control, TextBlock header )
        {
            if( !CheckContextMenu(control) ) { return null; }

            foreach( MenuItem item in control.ContextMenu.Items ) {
                TextBlock itemHeader = item.Header as TextBlock;
                if( itemHeader.Text == header.Text ) {
                    return item;
                }
            }

            MenuItem newHeader = null;
            if( header != null ) {
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
        #endregion

        #region FrameworkElement - string Methods
        /// <summary>
        /// Add a context menu option to the control.
        /// </summary>
        /// <param name="control">FrameworkElement to add option to.</param>
        /// <param name="label">Text of the added option.</param>
        /// <param name="action">Action to be performed by option.</param>
        public static void AddContextMenuItem( FrameworkElement control, string label, RoutedEventHandler action, bool enabled = true )
        {
            if( string.IsNullOrEmpty(label) ) { return; }

            AddContextMenuItem( control, new TextBlock(new Run(label)), action, enabled );
        }

        /// <summary>
        /// Add multiple option to context menu of the control.
        /// </summary>
        /// <param name="control">FrameworkElement to add options to.</param>
        /// <param name="items">Dictionary of Label, Action value pairs.</param>
        public static void AddContextMenuItems( FrameworkElement control, Dictionary<string, RoutedEventHandler> items, bool enabled = true )
        {
            AddContextMenuItems( control, string.Empty, items, enabled );
        }
        /// <summary>
        /// Add multiple options to a submenu in the context menu of the control. If the submenu header does not exist it
        /// will be created first.
        /// </summary>
        /// <param name="control">FrameworkElement to add options to.</param>
        /// <param name="header">Submenu header name.</param>
        /// <param name="items">Dictionary of Label, Action value pairs</param>
        public static void AddContextMenuItems( FrameworkElement control, string header, Dictionary<string, RoutedEventHandler> items, bool enabled = true )
        {
            TextBlock headerBlock = string.IsNullOrEmpty(header) ? null : new TextBlock( new Run(header) );
            Dictionary<TextBlock, RoutedEventHandler> itemBlocks = MapDicStringToBlock(items);

            AddContextMenuItems( control, headerBlock, itemBlocks, enabled );
        }
        /// <summary>
        /// Add multiple options to a submenu in the context menu of the control. If the submenu header does not exist
        /// the options will be added to the root of the context menu.
        /// </summary>
        /// <param name="control">FrameworkElement to add options to.</param>
        /// <param name="headerItem">Label of submenu header.</param>
        /// <param name="items">Dictionary of Label, Action value pairs.</param>
        public static void AddContextMenuItems( FrameworkElement control, MenuItem headerItem, Dictionary<string, RoutedEventHandler> items, bool enabled = true )
        {
            Dictionary<TextBlock, RoutedEventHandler> itemBlocks = MapDicStringToBlock(items);

            AddContextMenuItems( control, headerItem, itemBlocks, enabled );
        }

        /// <summary>
        /// Adds a new submenu to the context menu of the control and populates it with the provided options.
        /// </summary>
        /// <param name="control">FrameworkElement to add options to.</param>
        /// <param name="header">Label of submenu header.</param>
        /// <param name="items">Dictionary of Label, Action value pairs.</param>
        /// <returns></returns>
        public static MenuItem AddContextMenuHeader( FrameworkElement control, string header, Dictionary<string, RoutedEventHandler> items, bool enabled = true )
        {
            TextBlock headerBlock = string.IsNullOrEmpty(header) ? null : new TextBlock( new Run(header) );
            Dictionary<TextBlock, RoutedEventHandler> itemBlocks = MapDicStringToBlock(items);

            return AddContextMenuHeader( control, headerBlock, itemBlocks, enabled );
        }
        /// <summary>
        /// Adds a new submenu to the context menu of the control.
        /// </summary>
        /// <param name="control">FrameworkElement to add options to.</param>
        /// <param name="header">Label of submenu header.</param>
        /// <returns></returns>
        public static MenuItem AddContextMenuHeader( FrameworkElement control, string header )
        {
            TextBlock headerBlock = string.IsNullOrEmpty(header) ? null : new TextBlock( new Run(header) );
            return AddContextMenuHeader( control, headerBlock );
        }
        #endregion

        #region FrameworkContentElement - string Methods
        /// <summary>
        /// Add a context menu option to the control.
        /// </summary>
        /// <param name="control">FrameworkContentElement to add option to.</param>
        /// <param name="label">Text of the added option.</param>
        /// <param name="action">Action to be performed by option.</param>
        public static void AddContextMenuItem( FrameworkContentElement control, string label, RoutedEventHandler action, bool enabled = true )
        {
            if( string.IsNullOrEmpty(label) ) { return; }

            AddContextMenuItem( control, new TextBlock( new Run(label) ), action, enabled );
        }

        /// <summary>
        /// Add multiple option to context menu of the control.
        /// </summary>
        /// <param name="control">FrameworkContentElement to add options to.</param>
        /// <param name="items">Dictionary of Label, Action value pairs.</param>
        public static void AddContextMenuItems( FrameworkContentElement control, Dictionary<string, RoutedEventHandler> items, bool enabled = true  )
        {
            AddContextMenuItems( control, string.Empty, items, enabled );
        }
        /// <summary>
        /// Add multiple options to a submenu in the context menu of the control. If the submenu header does not exist it
        /// will be created first.
        /// </summary>
        /// <param name="control">FrameworkContentElement to add options to.</param>
        /// <param name="header">Submenu header name.</param>
        /// <param name="items">Dictionary of Label, Action value pairs</param>
        public static void AddContextMenuItems( FrameworkContentElement control, string header, Dictionary<string, RoutedEventHandler> items, bool enabled = true  )
        {
            TextBlock headerBlock = string.IsNullOrEmpty(header) ? null : new TextBlock( new Run(header) );
            Dictionary<TextBlock, RoutedEventHandler> itemBlocks = MapDicStringToBlock(items);

            AddContextMenuItems( control, headerBlock, itemBlocks, enabled );
        }
        /// <summary>
        /// Add multiple options to a submenu in the context menu of the control. If the submenu header does not exist
        /// the options will be added to the root of the context menu.
        /// </summary>
        /// <param name="control">FrameworkContentElement to add options to.</param>
        /// <param name="headerItem">Label of submenu header.</param>
        /// <param name="items">Dictionary of Label, Action value pairs.</param>
        public static void AddContextMenuItems( FrameworkContentElement control, MenuItem headerItem, Dictionary<string, RoutedEventHandler> items, bool enabled = true  )
        {
            Dictionary<TextBlock, RoutedEventHandler> itemBlocks = MapDicStringToBlock(items);

            AddContextMenuItems( control, headerItem, itemBlocks, enabled );
        }

        /// <summary>
        /// Adds a new submenu to the context menu of the control and populates it with the provided options.
        /// </summary>
        /// <param name="control">FrameworkContentElement to add options to.</param>
        /// <param name="header">Label of submenu header.</param>
        /// <param name="items">Dictionary of Label, Action value pairs.</param>
        /// <returns></returns>
        public static MenuItem AddContextMenuHeader( FrameworkContentElement control, string header, Dictionary<string, RoutedEventHandler> items, bool enabled = true  )
        {
            TextBlock headerBlock = string.IsNullOrEmpty(header) ? null : new TextBlock( new Run(header) );
            Dictionary<TextBlock, RoutedEventHandler> itemBlocks = MapDicStringToBlock(items);

            return AddContextMenuHeader( control, headerBlock, itemBlocks, enabled );
        }
        /// <summary>
        /// Adds a new submenu to the context menu of the control.
        /// </summary>
        /// <param name="control">FrameworkContentElement to add options to.</param>
        /// <param name="header">Label of submenu header.</param>
        /// <returns></returns>
        public static MenuItem AddContextMenuHeader( FrameworkContentElement control, string header )
        {
            TextBlock headerBlock = string.IsNullOrEmpty(header) ? null : new TextBlock( new Run(header) );
            return AddContextMenuHeader( control, headerBlock );
        }
        #endregion

        private static Dictionary<TextBlock, RoutedEventHandler> MapDicStringToBlock( Dictionary<string, RoutedEventHandler> items )
        {
            Dictionary<TextBlock, RoutedEventHandler> itemBlocks = new Dictionary<TextBlock, RoutedEventHandler>();
            foreach( KeyValuePair<string, RoutedEventHandler> pair in items ) {
                if( !string.IsNullOrEmpty(pair.Key) ) {
                    itemBlocks[new TextBlock( new Run(pair.Key) )] = pair.Value;
                }
            }

            return itemBlocks;
        }
    }
}
