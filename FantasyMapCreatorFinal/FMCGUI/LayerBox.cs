using System;
using Gtk;
using Cairo;

namespace FantasyMapCreatorFinal
{
    public class LayerBox : VBox
    {
        //private VBox layerpanel;
        private LayerListWidget layernames;
        private Toolbar layertools;
        public int SelectedLayer{ get; set; }
        
        //public VBox GUI{ get { return layerpanel; } }

        public LayerBox(): base()
        {
            this.Homogeneous = false;
            this.Spacing = 3;
            this.SetSizeRequest(100, -1);
                
            
            layernames = new LayerListWidget();
            layernames.SelectionNotifyEvent += LayerNotifySelected;
            layernames.SelectionRequestEvent += LayerRequestSelected;
            Label layerlabel = new Label("Layers");
            layertools = new Toolbar();
            
            layerlabel.SetAlignment(0.0f, 0.5f);
            // TODO: Worry about this crap later... Background colors.
            //layerlabel.ModifyBase(StateType.Normal, new Gdk.Color(128, 128, 128));
            //layerlabel.ModifyBg(StateType.Normal, new Gdk.Color(128, 0, 0));
            this.PackStart(layerlabel, false, false, 5);
            
            //PopulateLayerNames();
            this.PackStart(layernames, false, false, 0);
           
            PopulateLayerTools();
            this.PackEnd(layertools, false, false, 0);
        }
        private void LayerNotifySelected(object sender, Gtk.SelectionNotifyEventArgs evnt)
        {
            Console.WriteLine("LayerGUI Notify Request");
            
        }
        
        private void LayerRequestSelected(object sender, Gtk.SelectionRequestEventArgs evnt)
        {
            Console.WriteLine("LayerGUI Selection Request");
            
        }
        
        private void PopulateLayerTools()
        {
            
            layertools.Orientation = Orientation.Horizontal;
            layertools.ToolbarStyle = ToolbarStyle.Icons;
            ToolButton addLayer = new ToolButton(Stock.Add);
            addLayer.Label = "New Layer";
            addLayer.TooltipText = "Create a new layer";
            addLayer.Clicked += OnAddLayer;
            ToolButton deleteLayer = new ToolButton(Stock.Remove);
            deleteLayer.Label = "Delete Layer";
            deleteLayer.TooltipText = "Delete current layer";
            ToolButton copyLayer = new ToolButton(Stock.Copy);
            copyLayer.Label = "Copy Layer";
            copyLayer.TooltipText = "Copy current layer";
            layertools.Insert(addLayer, 0);
            layertools.Insert(deleteLayer, 1);
            layertools.Insert(copyLayer, 2);
            
        }

        private void OnAddLayer(object sender, EventArgs args)
        {
            // TODO: Add in new layer dialog.
            FMCMainWindow.dm.AddNewLayer("UntitledX", new LayerProperties());
            layernames.AddLayerToWidget();
            layernames.QueueDraw();   
        }

//        protected override bool OnExposeEvent(Gdk.EventExpose evnt)
//        {
//            return base.OnExposeEvent(evnt);
//            
//        }
        
    }
}

