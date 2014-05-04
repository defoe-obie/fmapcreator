using System;
using Gtk;

namespace FantasyMapCreatorFinal
{
    public class FMCMainWindow: Gtk.Window
    {
       // private VBox mainVBox;
       // private HBox mainHBox;
       // private HPaned drawingLayersPaned;
        
        //TODO: Move this to a more appropriate location
        public static DrawingManager dm { get; private set; }
        public static Definitions dr{ get; private set;}
        private LayerBox layerbox;
        private CanvasBox canvasbox;
        
        public FMCMainWindow() : base(Gtk.WindowType.Toplevel)
        {
            this.Title = "Fantasy Map Creator V.1.0";
            this.DeleteEvent += OnDeleteEvent;
            this.Resize(800, 500);       
            dm = new DrawingManager(400,300);
            dr = new Definitions();
            canvasbox = new CanvasBox();
            layerbox = new LayerBox();
            layerbox.CreateInitialLayer();
            canvasbox.CreateCanvasBackground(dm.Width, dm.Height);
            layerbox.PopulateLayerBox();
            
            VBox mainVBox = new VBox(false, 1);
            HBox mainHBox = new HBox(false, 1);
            HPaned drawingLayersPaned = new HPaned();
            
            //ScrolledWindow sw = new ScrolledWindow();
            //sw.AddWithViewport(dm.GUI);
            drawingLayersPaned.Pack1(canvasbox, true, true);
            drawingLayersPaned.Pack2(layerbox, false, false);
            
            mainHBox.PackStart(new Label("DrawingStuff"), false, false, 8);
            mainHBox.PackStart(drawingLayersPaned);
            
            
            mainVBox.PackStart(new Label("Top Toolbar goes here"), false, false, 10);
            mainVBox.PackStart(mainHBox);
            mainVBox.PackEnd(new Label("Bottom Status Bar goes here"), false, false, 10);
            
            this.Add(mainVBox);
            this.ShowAll();
            
            drawingLayersPaned.Position = drawingLayersPaned.Allocation.Width - drawingLayersPaned.Child2.WidthRequest;
            
        }

        protected void OnDeleteEvent(object sender, DeleteEventArgs a)
        {
            Application.Quit();
            a.RetVal = true;
        }
    }
}

