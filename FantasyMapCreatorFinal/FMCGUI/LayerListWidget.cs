using System;
using Gtk;
using System.Collections.Generic;

namespace FantasyMapCreatorFinal
{
    public class LayerListWidget : EventBox
    {
        private int selectedIndex;
        private VBox layerlistbox;

        public LayerListWidget()
        {   
            layerlistbox = new VBox();
            ScrolledWindow sw = new ScrolledWindow();
            sw.HscrollbarPolicy = PolicyType.Always;
            sw.AddWithViewport(layerlistbox);
            
            this.Add(sw);
            
            selectedIndex = -1;
            string[] names = FMCMainWindow.dm.GetLayerTitles();
            LayerWidget lw;
            foreach (string s in names)
            {
                lw = new LayerWidget(s);
                layerlistbox.PackEnd(lw, false, false, 0);
            }
            selectedIndex = FMCMainWindow.dm.CurrentLayerIndex;
            lw = (LayerWidget)layerlistbox.Children[selectedIndex];
            lw.IsSelected = true;
            
        }

        public void AddLayerToWidget()
        {
            LayerWidget lw;
            if (selectedIndex >= 0)
            {
                lw = (LayerWidget)layerlistbox.Children[selectedIndex];
                lw.IsSelected = false;
            }
            string name = FMCMainWindow.dm.CurrentLayer.Name;
            lw = new LayerWidget(name);
            lw.IsSelected = true;
            layerlistbox.PackEnd(lw, false, false, 0);
            layerlistbox.ReorderChild(lw, FMCMainWindow.dm.CurrentLayerIndex);
            layerlistbox.ShowAll();
            
        }
        
        protected override bool OnButtonPressEvent(Gdk.EventButton evnt)
        {
            //int x, y;
            //layerlistbox.GetPointer(out x, out y);
            //double x = evnt.X;
            //double y = (this.Allocation.Height - evnt.Y);
            LayerWidget lw = (LayerWidget)layerlistbox.Children[selectedIndex];
            int y = (int)(evnt.Y - (layerlistbox.Allocation.Height - lw.HeightRequest * layerlistbox.Children.Length));
            if (y >= 0)
            {
                int newIndex = (int)(y / lw.HeightRequest);
                if ( newIndex >= layerlistbox.Children.Length){
                    return true;
                }
                lw.IsSelected = false;
                selectedIndex = newIndex;
                lw = (LayerWidget)layerlistbox.Children[selectedIndex];
                lw.IsSelected = true;
                FMCMainWindow.dm.CurrentLayerIndex = layerlistbox.Children.Length - 1 - selectedIndex;
                this.QueueDraw();
            }
            return true;
        }
    }
}

