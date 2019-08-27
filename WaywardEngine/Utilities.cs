﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WaywardEngine
{
    public static class Utilities
    {
        /// <summary>
        /// Returns highest zIndex value of elements inside Canvas Parent.
        /// </summary>
        /// <param name="parent">The Canvas to search.</param>
        /// <returns></returns>
        public static int GetMaxZOfCanvas( Canvas parent )
        {
            int zIndex = 0;
            int maxZ = 0;
            for( int i = 0; i < parent.Children.Count; i++ ) {
                zIndex = Canvas.GetZIndex(parent.Children[i]);
                maxZ = ( maxZ >= zIndex ? maxZ : zIndex );
            }

            return maxZ;
        }

        /// <summary>
        /// Places FrameworkElement target at highest zIndex value in Canvas parent.
        /// </summary>
        /// <param name="parent">Containing Canvas.</param>
        /// <param name="target">FramworkElement to be moved to front.</param>
        public static void BringToFrontOfCanvas( Canvas parent, FrameworkElement target )
        {
            int currentIndex = Canvas.GetZIndex(target);
            int zIndex = 0;
            int maxZ = 0;
            FrameworkElement child;
            for( int i = 0; i < parent.Children.Count; i++ ) {
                child = parent.Children[i] as FrameworkElement;
                if( child != target ) {
                    zIndex = Canvas.GetZIndex(child);
                    if( zIndex > currentIndex ) {
                        zIndex--;
                        Canvas.SetZIndex(child, zIndex);
                    }
                    maxZ = ( maxZ >= zIndex ? maxZ : zIndex );
                }
            }
            Canvas.SetZIndex(target, maxZ+1);
        }

        public static T FindNode<T>( DependencyObject parent, string name )
            where T : class
        {
            if( parent == null || string.IsNullOrEmpty(name) ) { return null; }

            T node = LogicalTreeHelper.FindLogicalNode(parent, name) as T;
            if( node == null ) {
                throw new System.NullReferenceException($"Could not find {typeof(T).Name} '{name}' in {parent}");
            }

            return node;
        }

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

        public static void AddContextMenuItems( FrameworkElement control, Dictionary<string, RoutedEventHandler> items )
        {
            AddContextMenuItems( control, string.Empty, items );
        }
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
        public static void AddContextMenuItems( FrameworkElement control, MenuItem headerItem, Dictionary<string, RoutedEventHandler> items )
        {
            if( !CheckContextMenu(control) ) { return; }

            items.Reverse();
            foreach( KeyValuePair<string, RoutedEventHandler> item in items ) {
                if( headerItem == null ) {
                    AddMenuItem( control.ContextMenu, item.Key, item.Value );
                } else {
                    AddMenuItem( headerItem, item.Key, item.Value );
                }
            }
        }

        public static MenuItem AddContextMenuHeader( FrameworkElement control, string header, Dictionary<string, RoutedEventHandler> items )
        {
            if( !CheckContextMenu(control) ) { return null; }

            MenuItem newHeader = AddContextMenuHeader( control, header );
            AddContextMenuItems( control, newHeader, items );

            return newHeader;
        }
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

        private static bool CheckContextMenu( FrameworkElement control )
        {
            if( control == null ) { return false; }

            if( control.ContextMenu == null ) {
                control.ContextMenu = new ContextMenu();
            }

            return true;
        }
    }
}
