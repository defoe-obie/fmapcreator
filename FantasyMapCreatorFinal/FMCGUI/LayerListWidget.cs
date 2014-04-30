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
            this.Add(layerlistbox);
            
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
            lw = new LayerWidget(name + layerlistbox.Children.Length + " " + selectedIndex);
            lw.IsSelected = true;
            layerlistbox.PackEnd(lw, false, false, 0);
            selectedIndex = layerlistbox.Children.Length - 1 - FMCMainWindow.dm.CurrentLayerIndex;
            //layerlistbox.ReorderChild(lw, 0);
            // TODO: Add this back in when testing on such things can actually be done.
            layerlistbox.ShowAll();
            
        }
        //BLAH BLAH BLAH OVERRIDE IT SO THAT IT IS COUNTING INDICES FROM THE FUCKINGBOTTOM!
        protected override bool OnButtonPressEvent(Gdk.EventButton evnt)
        {
            System.Console.WriteLine("Yes, I know.");
            //double x_root = evnt.XRoot;
            //double y_root = evnt.YRoot;
            double x = evnt.X;
            double y = evnt.Y;
            LayerWidget lw = (LayerWidget)layerlistbox.Children[selectedIndex];
            lw.IsSelected = false;
            int layerListIndex = (int)(y / lw.HeightRequest);
            lw = (LayerWidget)layerlistbox.Children[layerListIndex];
            lw.IsSelected = true;
            selectedIndex = layerListIndex;
            FMCMainWindow.dm.CurrentLayerIndex = layerlistbox.Children.Length - 1 - layerListIndex;
            
            //selectedLayer = layerlistbox.Children.Length - 1 - FMCMainWindow.dm.CurrentLayerIndex;
            // Recall that the listbox packs the items in bottom to top.
            //FMCMainWindow.dm.CurrentLayerIndex = reverseSelectedLayer - layerlistbox.Children.Length + 1;
            int la = FMCMainWindow.dm.CurrentLayerIndex;
            this.QueueDraw();
            
            return base.OnButtonPressEvent(evnt);
        }
    }
}

