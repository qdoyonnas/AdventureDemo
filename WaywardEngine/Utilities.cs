using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WaywardEngine
{
    public static class Utilities
    {
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

        public static void BringToFrontOfCanvas( Canvas parent, FrameworkElement target )
        {
            Console.WriteLine("IN BringToFrontOfCanvas");
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
