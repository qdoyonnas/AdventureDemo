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
        private static void AddMenuItem( ItemsControl menu, FormattedText label, ContextMenuAction action, bool enabled )
        {
            if( menu == null ) { return; }

            MenuItem newItem = new MenuItem();
            newItem.IsEnabled = enabled;
            newItem.Header = label.GetContent();
            if( action != null ) {
                newItem.Click += delegate {
                    action();
                };
            }

            menu.Items.Insert(0, newItem);
        }

        #region FrameworkElement - FormattedText Methods
        /// <summary>
        /// Add a context menu option to the control.
        /// </summary>
        /// <param name="control">FrameworkElement to add option to.</param>
        /// <param name="label">Span of the added option.</param>
        /// <param name="action">Action to be performed by option.</param>
        public static void AddContextMenuItem( FrameworkElement control, FormattedText label, ContextMenuAction action, bool enabled = true )
        {
            if( !CheckContextMenu(control) ) { return; }

            AddMenuItem( control.ContextMenu, label, action, enabled );
        }

        /// <summary>
        /// Add multiple option to context menu of the control.
        /// </summary>
        /// <param name="control">FrameworkElement to add options to.</param>
        /// <param name="items">Dictionary of Label, Action value pairs.</param>
        public static void AddContextMenuItems( FrameworkElement control, Dictionary<FormattedText, ContextMenuAction> items, bool enabled = true )
        {
            AddContextMenuItems( control, new FormattedText(), items, enabled );
        }
        /// <summary>
        /// Add multiple options to a submenu in the context menu of the control. If the submenu header does not exist it
        /// will be created first.
        /// </summary>
        /// <param name="control">FrameworkElement to add options to.</param>
        /// <param name="header">Submenu header name.</param>
        /// <param name="items">Dictionary of Label, Action value pairs</param>
        public static void AddContextMenuItems( FrameworkElement control, FormattedText header, Dictionary<FormattedText, ContextMenuAction> items, bool enabled = true )
        {
            if( !CheckContextMenu(control) ) { return; }

            MenuItem headerItem = null;
            if( header != null ) {
                foreach( MenuItem item in control.ContextMenu.Items ) {
                    TextBlock headBlock = item.Header as TextBlock;
                    string headString = FormattedText.ParseBlockForTextContent(headBlock);
                    if( string.Compare(headString.Trim(), header.GetText().Trim(), true) == 0 ) {
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
        public static void AddContextMenuItems( FrameworkElement control, MenuItem headerItem, Dictionary<FormattedText, ContextMenuAction> items, bool enabled = true )
        {
            if( !CheckContextMenu(control) ) { return; }
            
            FormattedText[] keys = items.Keys.ToArray();
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
        public static MenuItem AddContextMenuHeader( FrameworkElement control, FormattedText header, Dictionary<FormattedText, ContextMenuAction> items, bool enabled = true )
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
        public static MenuItem AddContextMenuHeader( FrameworkElement control, FormattedText header )
        {
            if( !CheckContextMenu(control) ) { return null; }

            foreach( MenuItem item in control.ContextMenu.Items ) {
                TextBlock itemHeaderBlock = item.Header as TextBlock;
                string itemHeader = FormattedText.ParseBlockForTextContent(itemHeaderBlock);
                if( itemHeader.Trim() == header.GetText().Trim() ) {
                    return item;
                }
            }

            MenuItem newHeader = null;
            if( header != null ) {
                newHeader = new MenuItem();
                newHeader.Header = header.GetContent();
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

        public static void ClearContextMenu( FrameworkElement control )
        {
            if( control == null ) { return; }

            control.ContextMenu = null;
        }
        #endregion

        #region FrameworkContentElement - FormattedText Methods
        /// <summary>
        /// Add a context menu option to the control.
        /// </summary>
        /// <param name="control">FrameworkContentElement to add option to.</param>
        /// <param name="label">Span of the added option.</param>
        /// <param name="action">Action to be performed by option.</param>
        public static void AddContextMenuItem( FrameworkContentElement control, FormattedText label, ContextMenuAction action, bool enabled = true )
        {
            if( !CheckContextMenu(control) ) { return; }

            AddMenuItem( control.ContextMenu, label, action, enabled );
        }

        /// <summary>
        /// Add multiple option to context menu of the control.
        /// </summary>
        /// <param name="control">FrameworkContentElement to add options to.</param>
        /// <param name="items">Dictionary of Label, Action value pairs.</param>
        public static void AddContextMenuItems( FrameworkContentElement control, Dictionary<FormattedText, ContextMenuAction> items, bool enabled = true )
        {
            AddContextMenuItems( control, new FormattedText(), items, enabled );
        }
        /// <summary>
        /// Add multiple options to a submenu in the context menu of the control. If the submenu header does not exist it
        /// will be created first.
        /// </summary>
        /// <param name="control">FrameworkContentElement to add options to.</param>
        /// <param name="header">Submenu header name.</param>
        /// <param name="items">Dictionary of Label, Action value pairs</param>
        public static void AddContextMenuItems( FrameworkContentElement control, FormattedText header, Dictionary<FormattedText, ContextMenuAction> items, bool enabled = true )
        {
            if( !CheckContextMenu(control) ) { return; }

            MenuItem headerItem = null;
            if( header != null ) {
                foreach( MenuItem item in control.ContextMenu.Items ) {
                    TextBlock headBlock = item.Header as TextBlock;
                    string headString = FormattedText.ParseBlockForTextContent(headBlock);
                    if( string.Compare(headString.Trim(), header.GetText().Trim(), true) == 0 ) {
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
        public static void AddContextMenuItems( FrameworkContentElement control, MenuItem headerItem, Dictionary<FormattedText, ContextMenuAction> items, bool enabled = true )
        {
            if( !CheckContextMenu(control) ) { return; }
            
            FormattedText[] keys = items.Keys.ToArray();
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
        public static MenuItem AddContextMenuHeader( FrameworkContentElement control, FormattedText header, Dictionary<FormattedText, ContextMenuAction> items, bool enabled = true )
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
        public static MenuItem AddContextMenuHeader( FrameworkContentElement control, FormattedText header )
        {
            if( !CheckContextMenu(control) ) { return null; }

            foreach( MenuItem item in control.ContextMenu.Items ) {
                TextBlock itemHeader = item.Header as TextBlock;
                string itemText = FormattedText.ParseBlockForTextContent(itemHeader);
                if( itemText.Trim() == header.GetText().Trim()) {
                    return item;
                }
            }

            MenuItem newHeader = null;
            if( header != null ) {
                newHeader = new MenuItem();
                newHeader.Header = header.GetContent();
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

        public static void ClearContextMenu( FrameworkContentElement control )
        {
            if( control == null ) { return; }

            control.ContextMenu = null;
        }
		#endregion

		#region FrameworkElement - TextBlock Methods
        /// <summary>
		/// Add a context menu option to the control.
		/// </summary>
		/// <param name="control">FrameworkElement to add option to.</param>
		/// <param name="label">Text of the added option.</param>
		/// <param name="action">Action to be performed by option.</param>
		public static void AddContextMenuItem( FrameworkElement control, TextBlock label, ContextMenuAction action, bool enabled = true )
        {
            if( label == null ) { return; }

            AddContextMenuItem( control, new FormattedText(label), action, enabled );
        }

        /// <summary>
        /// Add multiple option to context menu of the control.
        /// </summary>
        /// <param name="control">FrameworkElement to add options to.</param>
        /// <param name="items">Dictionary of Label, Action value pairs.</param>
        public static void AddContextMenuItems( FrameworkElement control, Dictionary<TextBlock, ContextMenuAction> items, bool enabled = true )
        {
            TextBlock block = null;
            AddContextMenuItems( control, block, items, enabled );
        }
        /// <summary>
        /// Add multiple options to a submenu in the context menu of the control. If the submenu header does not exist it
        /// will be created first.
        /// </summary>
        /// <param name="control">FrameworkElement to add options to.</param>
        /// <param name="header">Submenu header name.</param>
        /// <param name="items">Dictionary of Label, Action value pairs</param>
        public static void AddContextMenuItems( FrameworkElement control, TextBlock header, Dictionary<TextBlock, ContextMenuAction> items, bool enabled = true )
        {
            FormattedText headerBlock = header == null ? null : new FormattedText(header);
            Dictionary<FormattedText, ContextMenuAction> itemBlocks = MapDicBlockToFormatted(items);

            AddContextMenuItems( control, headerBlock, itemBlocks, enabled );
        }
        /// <summary>
        /// Add multiple options to a submenu in the context menu of the control. If the submenu header does not exist
        /// the options will be added to the root of the context menu.
        /// </summary>
        /// <param name="control">FrameworkElement to add options to.</param>
        /// <param name="headerItem">Label of submenu header.</param>
        /// <param name="items">Dictionary of Label, Action value pairs.</param>
        public static void AddContextMenuItems( FrameworkElement control, MenuItem headerItem, Dictionary<TextBlock, ContextMenuAction> items, bool enabled = true )
        {
            Dictionary<FormattedText, ContextMenuAction> itemBlocks = MapDicBlockToFormatted(items);

            AddContextMenuItems( control, headerItem, itemBlocks, enabled );
        }

        /// <summary>
        /// Adds a new submenu to the context menu of the control and populates it with the provided options.
        /// </summary>
        /// <param name="control">FrameworkElement to add options to.</param>
        /// <param name="header">Label of submenu header.</param>
        /// <param name="items">Dictionary of Label, Action value pairs.</param>
        /// <returns></returns>
        public static MenuItem AddContextMenuHeader( FrameworkElement control, TextBlock header, Dictionary<TextBlock, ContextMenuAction> items, bool enabled = true )
        {
            FormattedText headerBlock = header == null ? null : new FormattedText(header);
            Dictionary<FormattedText, ContextMenuAction> itemBlocks = MapDicBlockToFormatted(items);

            return AddContextMenuHeader( control, headerBlock, itemBlocks, enabled );
        }
        /// <summary>
        /// Adds a new submenu to the context menu of the control.
        /// </summary>
        /// <param name="control">FrameworkElement to add options to.</param>
        /// <param name="header">Label of submenu header.</param>
        /// <returns></returns>
        public static MenuItem AddContextMenuHeader( FrameworkElement control, TextBlock header )
        {
            FormattedText headerBlock = header == null ? null : new FormattedText(header);
            return AddContextMenuHeader( control, headerBlock );
        }
		#endregion

		#region FrameworkContentElement - TextBlock Methods
        /// <summary>
        /// Add a context menu option to the control.
        /// </summary>
        /// <param name="control">FrameworkContentElement to add option to.</param>
        /// <param name="label">Text of the added option.</param>
        /// <param name="action">Action to be performed by option.</param>
        public static void AddContextMenuItem( FrameworkContentElement control, TextBlock label, ContextMenuAction action, bool enabled = true )
        {
            if( label == null ) { return; }

            AddContextMenuItem( control, new FormattedText(label), action, enabled );
        }

        /// <summary>
        /// Add multiple option to context menu of the control.
        /// </summary>
        /// <param name="control">FrameworkContentElement to add options to.</param>
        /// <param name="items">Dictionary of Label, Action value pairs.</param>
        public static void AddContextMenuItems( FrameworkContentElement control, Dictionary<TextBlock, ContextMenuAction> items, bool enabled = true  )
        {
            TextBlock block = null;
            AddContextMenuItems( control, block, items, enabled );
        }
        /// <summary>
        /// Add multiple options to a submenu in the context menu of the control. If the submenu header does not exist it
        /// will be created first.
        /// </summary>
        /// <param name="control">FrameworkContentElement to add options to.</param>
        /// <param name="header">Submenu header name.</param>
        /// <param name="items">Dictionary of Label, Action value pairs</param>
        public static void AddContextMenuItems( FrameworkContentElement control, TextBlock header, Dictionary<TextBlock, ContextMenuAction> items, bool enabled = true  )
        {
            FormattedText headerBlock = header == null ? null : new FormattedText(header);
            Dictionary<FormattedText, ContextMenuAction> itemBlocks = MapDicBlockToFormatted(items);

            AddContextMenuItems( control, headerBlock, itemBlocks, enabled );
        }
        /// <summary>
        /// Add multiple options to a submenu in the context menu of the control. If the submenu header does not exist
        /// the options will be added to the root of the context menu.
        /// </summary>
        /// <param name="control">FrameworkContentElement to add options to.</param>
        /// <param name="headerItem">Label of submenu header.</param>
        /// <param name="items">Dictionary of Label, Action value pairs.</param>
        public static void AddContextMenuItems( FrameworkContentElement control, MenuItem headerItem, Dictionary<TextBlock, ContextMenuAction> items, bool enabled = true  )
        {
            Dictionary<FormattedText, ContextMenuAction> itemBlocks = MapDicBlockToFormatted(items);

            AddContextMenuItems( control, headerItem, itemBlocks, enabled );
        }

        /// <summary>
        /// Adds a new submenu to the context menu of the control and populates it with the provided options.
        /// </summary>
        /// <param name="control">FrameworkContentElement to add options to.</param>
        /// <param name="header">Label of submenu header.</param>
        /// <param name="items">Dictionary of Label, Action value pairs.</param>
        /// <returns></returns>
        public static MenuItem AddContextMenuHeader( FrameworkContentElement control, TextBlock header, Dictionary<TextBlock, ContextMenuAction> items, bool enabled = true  )
        {
            FormattedText headerBlock = header == null ? null : new FormattedText(header);
            Dictionary<FormattedText, ContextMenuAction> itemBlocks = MapDicBlockToFormatted(items);

            return AddContextMenuHeader( control, headerBlock, itemBlocks, enabled );
        }
        /// <summary>
        /// Adds a new submenu to the context menu of the control.
        /// </summary>
        /// <param name="control">FrameworkContentElement to add options to.</param>
        /// <param name="header">Label of submenu header.</param>
        /// <returns></returns>
        public static MenuItem AddContextMenuHeader( FrameworkContentElement control, TextBlock header )
        {
            FormattedText headerBlock = header == null ? null : new FormattedText(header);
            return AddContextMenuHeader( control, headerBlock );
        }
        #endregion

		#region FrameworkElement - string Methods
		/// <summary>
		/// Add a context menu option to the control.
		/// </summary>
		/// <param name="control">FrameworkElement to add option to.</param>
		/// <param name="label">Text of the added option.</param>
		/// <param name="action">Action to be performed by option.</param>
		public static void AddContextMenuItem( FrameworkElement control, string label, ContextMenuAction action, bool enabled = true )
        {
            if( string.IsNullOrEmpty(label) ) { return; }

            AddContextMenuItem( control, new FormattedText(label), action, enabled );
        }

        /// <summary>
        /// Add multiple option to context menu of the control.
        /// </summary>
        /// <param name="control">FrameworkElement to add options to.</param>
        /// <param name="items">Dictionary of Label, Action value pairs.</param>
        public static void AddContextMenuItems( FrameworkElement control, Dictionary<string, ContextMenuAction> items, bool enabled = true )
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
        public static void AddContextMenuItems( FrameworkElement control, string header, Dictionary<string, ContextMenuAction> items, bool enabled = true )
        {
            FormattedText headerBlock = string.IsNullOrEmpty(header) ? null : new FormattedText(header);
            Dictionary<FormattedText, ContextMenuAction> itemBlocks = MapDicStringToFormatted(items);

            AddContextMenuItems( control, headerBlock, itemBlocks, enabled );
        }
        /// <summary>
        /// Add multiple options to a submenu in the context menu of the control. If the submenu header does not exist
        /// the options will be added to the root of the context menu.
        /// </summary>
        /// <param name="control">FrameworkElement to add options to.</param>
        /// <param name="headerItem">Label of submenu header.</param>
        /// <param name="items">Dictionary of Label, Action value pairs.</param>
        public static void AddContextMenuItems( FrameworkElement control, MenuItem headerItem, Dictionary<string, ContextMenuAction> items, bool enabled = true )
        {
            Dictionary<FormattedText, ContextMenuAction> itemBlocks = MapDicStringToFormatted(items);

            AddContextMenuItems( control, headerItem, itemBlocks, enabled );
        }

        /// <summary>
        /// Adds a new submenu to the context menu of the control and populates it with the provided options.
        /// </summary>
        /// <param name="control">FrameworkElement to add options to.</param>
        /// <param name="header">Label of submenu header.</param>
        /// <param name="items">Dictionary of Label, Action value pairs.</param>
        /// <returns></returns>
        public static MenuItem AddContextMenuHeader( FrameworkElement control, string header, Dictionary<string, ContextMenuAction> items, bool enabled = true )
        {
            FormattedText headerBlock = string.IsNullOrEmpty(header) ? null : new FormattedText(header);
            Dictionary<FormattedText, ContextMenuAction> itemBlocks = MapDicStringToFormatted(items);

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
            FormattedText headerBlock = string.IsNullOrEmpty(header) ? null : new FormattedText(header);
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
        public static void AddContextMenuItem( FrameworkContentElement control, string label, ContextMenuAction action, bool enabled = true )
        {
            if( string.IsNullOrEmpty(label) ) { return; }

            AddContextMenuItem( control, new FormattedText(label), action, enabled );
        }

        /// <summary>
        /// Add multiple option to context menu of the control.
        /// </summary>
        /// <param name="control">FrameworkContentElement to add options to.</param>
        /// <param name="items">Dictionary of Label, Action value pairs.</param>
        public static void AddContextMenuItems( FrameworkContentElement control, Dictionary<string, ContextMenuAction> items, bool enabled = true  )
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
        public static void AddContextMenuItems( FrameworkContentElement control, string header, Dictionary<string, ContextMenuAction> items, bool enabled = true  )
        {
            FormattedText headerBlock = string.IsNullOrEmpty(header) ? null : new FormattedText(header);
            Dictionary<FormattedText, ContextMenuAction> itemBlocks = MapDicStringToFormatted(items);

            AddContextMenuItems( control, headerBlock, itemBlocks, enabled );
        }
        /// <summary>
        /// Add multiple options to a submenu in the context menu of the control. If the submenu header does not exist
        /// the options will be added to the root of the context menu.
        /// </summary>
        /// <param name="control">FrameworkContentElement to add options to.</param>
        /// <param name="headerItem">Label of submenu header.</param>
        /// <param name="items">Dictionary of Label, Action value pairs.</param>
        public static void AddContextMenuItems( FrameworkContentElement control, MenuItem headerItem, Dictionary<string, ContextMenuAction> items, bool enabled = true  )
        {
            Dictionary<FormattedText, ContextMenuAction> itemBlocks = MapDicStringToFormatted(items);

            AddContextMenuItems( control, headerItem, itemBlocks, enabled );
        }

        /// <summary>
        /// Adds a new submenu to the context menu of the control and populates it with the provided options.
        /// </summary>
        /// <param name="control">FrameworkContentElement to add options to.</param>
        /// <param name="header">Label of submenu header.</param>
        /// <param name="items">Dictionary of Label, Action value pairs.</param>
        /// <returns></returns>
        public static MenuItem AddContextMenuHeader( FrameworkContentElement control, string header, Dictionary<string, ContextMenuAction> items, bool enabled = true  )
        {
            FormattedText headerBlock = string.IsNullOrEmpty(header) ? null : new FormattedText(header);
            Dictionary<FormattedText, ContextMenuAction> itemBlocks = MapDicStringToFormatted(items);

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
            FormattedText headerBlock = string.IsNullOrEmpty(header) ? null : new FormattedText(header);
            return AddContextMenuHeader( control, headerBlock );
        }
        #endregion

        private static Dictionary<FormattedText, ContextMenuAction> MapDicStringToFormatted( Dictionary<string, ContextMenuAction> items )
        {
            Dictionary<FormattedText, ContextMenuAction> itemBlocks = new Dictionary<FormattedText, ContextMenuAction>();
            foreach( KeyValuePair<string, ContextMenuAction> pair in items ) {
                if( !string.IsNullOrEmpty(pair.Key) ) {
                    itemBlocks[new FormattedText(pair.Key)] = pair.Value;
                }
            }

            return itemBlocks;
        }
        private static Dictionary<FormattedText, ContextMenuAction> MapDicBlockToFormatted( Dictionary<TextBlock, ContextMenuAction> items )
        {
            Dictionary<FormattedText, ContextMenuAction> itemBlocks = new Dictionary<FormattedText, ContextMenuAction>();
            foreach( KeyValuePair<TextBlock, ContextMenuAction> pair in items ) {
                if( pair.Key != null ) {
                    itemBlocks[new FormattedText(pair.Key)] = pair.Value;
                }
            }

            return itemBlocks;
        }
    }

    public delegate bool ContextMenuAction();
}
