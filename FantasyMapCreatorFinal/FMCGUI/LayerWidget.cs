using System;
using Gtk;

namespace FantasyMapCreatorFinal
{
    public class LayerWidget : DrawingArea
    {
        private String name;
        // TODO: Rearrange files so that these getter setters are after constructors?
        public bool NeedsRedraw{ get; set; }

        public bool IsSelected{ get; set; }
        
        public LayerWidget(string layertext) : base()
        {
            name = layertext;
            NeedsRedraw = true;
            IsSelected = false;
            this.SetSizeRequest(100, 18);
            
        }

        
        protected override bool OnExposeEvent(Gdk.EventExpose evnt)
        {
            // if (NeedsRedraw)
            // {
            using (Cairo.Context cr = Gdk.CairoHelper.Create(this.GdkWindow))
            {
                if (IsSelected)
                {
                    cr.SetSourceRGB(0.7, 0.5, 0.8);
                    cr.Paint();
                  
                }
                else
                {
                    //using (Cairo.Context cr = Gdk.CairoHelper.Create(this.GdkWindow))
                    // {
                    cr.SetSourceRGB(0.6, 0.64, 0.85);
                    cr.Paint();
                    //    cr.MoveTo(5, 11);
                    //}
                    //}
                }
                cr.SetSourceRGB(0, 0, 0);
                cr.MoveTo(5, 11);
                cr.ShowText(name);
               // cr.Stroke();
                
                ((IDisposable)cr.GetTarget()).Dispose();
                ((IDisposable)cr).Dispose();
             
            }
            return true;
            //return base.OnExposeEvent(evnt);
        }
    }
}

