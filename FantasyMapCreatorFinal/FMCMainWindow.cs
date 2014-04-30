using System;
using Gtk;

namespace FantasyMapCreatorFinal
{
    public class FMCMainWindow: Gtk.Window
    {
        public FMCMainWindow() : base(Gtk.WindowType.Toplevel)
        {
            this.Title = "Fantasy Map Creator V.1.0";
            this.DeleteEvent += OnDeleteEvent;
        
            //this.Maximize();
            this.Resize(800, 600);
            Gtk.VBox vb = new VBox(false, 0);
            Gtk.ScrolledWindow sw = new ScrolledWindow();
            DrawingManager dm = new DrawingManager(400, 300);
            Canvas c = dm.GUI;
            sw.AddWithViewport(c);
           vb.Add(sw);
            this.Add(vb);
            this.ShowAll();
   
        }
        
        protected void OnDeleteEvent(object sender, DeleteEventArgs a)
        {
            Application.Quit();
            a.RetVal = true;
        }
    }
}
