using System;
using Gtk;
using Cairo;

namespace FantasyMapCreatorFinal
{
    public class LayerBox : VBox
    {
        private LayerListWidget layernames;
        private Toolbar layertools;

        public int SelectedLayer{ get; set; }
        public LayerBox() : base()
        {
            this.Homogeneous = false;
            this.Spacing = 0;
            this.SetSizeRequest(120, -1);
        }

        public void PopulateLayerBox()
        {
            
            layernames = new LayerListWidget();
            layernames.SelectionNotifyEvent += LayerNotifySelected;
            layernames.SelectionRequestEvent += LayerRequestSelected;
            Label layerlabel = new Label("Layers");
            layertools = new Toolbar();
            
            layerlabel.SetAlignment(0.0f, 0.5f);
            this.PackStart(layerlabel, false, false, 5);
            
            this.Add(layernames);
           
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

        public void CreateInitialLayer()
        {
                   
            CreateLayerDialog cld = new CreateLayerDialog(new Definitions.LType[]{Definitions.LType.Ocean},"Ocean Layer");
            cld.Response += delegate(object o, ResponseArgs responseargs)
            {
                ResponseType rt = responseargs.ResponseId;
                if (rt == ResponseType.Accept)
                {
                    FMCMainWindow.dm.AddNewLayer(cld.NewLayerName, new LayerProperties(cld.LayerTypeName, cld.NewData));
             //       layernames.AddLayerToWidget();
               //     layernames.QueueDraw();           
                }
            };
            cld.Run();
            cld.Destroy();
            
        }

        private void OnAddLayer(object sender, EventArgs args)
        {
            CreateLayerDialog cld = new CreateLayerDialog();
            cld.Response += delegate(object o, ResponseArgs responseargs)
            {
                ResponseType rt = responseargs.ResponseId;
                if (rt == ResponseType.Accept)
                {
                    FMCMainWindow.dm.AddNewLayer(cld.NewLayerName, new LayerProperties(cld.LayerTypeName, cld.NewData));
                    layernames.AddLayerToWidget();
                    layernames.QueueDraw();           
                }
            };
            cld.Run();
            cld.Destroy();
            
        }
    }
}

