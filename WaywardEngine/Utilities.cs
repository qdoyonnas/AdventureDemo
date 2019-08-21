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
    }
}