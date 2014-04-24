using System;
using Gtk;
using Cairo;
using System.Collections;
using System.Collections.Generic;

namespace FantasyMapCreatorFinal
{
    public class Canvas : Gtk.DrawingArea
    {
        private int height, width;
        private int x_offset, y_offset;
       
        private ImageSurface workingSurface;
        private PointD lastpoint;
        private bool layerRenderNeeded;
        private List<Layer> layers;
        
        //Temporary
        private Queue q;
        private Layer testLayer;
        //Properties
        public int X{ get { return x_offset; } }
        public int Y{ get { return y_offset; } }
        
        //Constructor
        public Canvas(int width, int height) : base()
        {
            this.width = width;
            this.height = height;
            this.x_offset = 0;
            this.y_offset = 0;
            this.SetSizeRequest(width, height);
            
            //base.ModifyBg(StateType.Normal, new Gdk.Color(128, 128, 128));
            
            workingSurface = new Cairo.ImageSurface(Cairo.Format.ARGB32, width, height);
            
            q = new Queue();
            layers = new List<Layer>();
            testLayer = new Layer(width, height);
            layerRenderNeeded = false;
            
            
            this.AddEvents((int)Gdk.EventMask.ButtonPressMask);
            this.AddEvents((int)Gdk.EventMask.Button1MotionMask);
            this.AddEvents((int)Gdk.EventMask.ButtonReleaseMask);
        }

        private void drawOnSurface(ImageSurface surface)
        {
            Cairo.Context cr = new Context(surface);
            cr.SetSourceRGB(0, 0.2, 0.6);
            cr.LineJoin = Cairo.LineJoin.Round;
            
            if (q.Count > 2)
            {
                PointD previous = (PointD)q.Dequeue();
                PointD current; //= (PointD)q.Dequeue();
                PointD next;
                cr.MoveTo(previous);
                while (q.Count > 1)
                {
                    current = (PointD)q.Dequeue();
                    next = (PointD)q.Dequeue();
                    cr.CurveTo(previous,current, next);
                    previous = next;
                    //current = next;
                }
                if (q.Count > 0)
                    cr.LineTo((PointD)q.Dequeue());
            }
            
            cr.Stroke();
            ((IDisposable)cr.GetTarget()).Dispose();
            ((IDisposable)cr).Dispose();
            
            q.Clear();
        }

        protected override bool OnExposeEvent(Gdk.EventExpose evnt)
        {
            base.OnExposeEvent(evnt);
            
            int lawidth = this.Parent.Allocation.Width;
            int laheight = this.Parent.Allocation.Height;
            this.x_offset = lawidth > width ? (lawidth - width) / 2 : 0;
            this.y_offset = laheight > height ? (laheight - height) / 2 : 0;
            //int finalwidth = Math.Min(lawidth, width);
            ImageSurface surface = new Cairo.ImageSurface(Cairo.Format.ARGB32, width, height);
            
            //drawOnSurface(surface);
            
            Console.WriteLine("EXPOSE EVENT");
            using (Cairo.Context gr = Gdk.CairoHelper.Create(this.GdkWindow))
            {
                // Rectangle r = new Rectangle(0, 0, Math.Min(lawidth, width), Math.Min(laheight, height));
                // gr.Rectangle(r);
                // gr.Clip();)
                
                gr.Rectangle(x_offset - 2, y_offset - 2, width + 4, height + 4);
                gr.SetSourceRGB(0, 0, 0);
                gr.Fill();
                //Temporary  white backgroundage.
                gr.Rectangle(x_offset, y_offset, width, height);
                gr.SetSourceRGB(1, 1, 1);
                gr.Fill();
                if (layerRenderNeeded)
                {
                    testLayer.RenderLayer(surface);
                    gr.SetSourceSurface(surface, x_offset, y_offset);
                    gr.Paint();
                    layerRenderNeeded = false;
                }
                else{
                    gr.SetSourceSurface(workingSurface, x_offset, y_offset);
                    gr.Paint();
                }
                ((IDisposable)gr.GetTarget()).Dispose();
                ((IDisposable)gr).Dispose();
                
                
            }
            return true;
        }

        protected override bool OnButtonPressEvent(Gdk.EventButton evnt)
        {
            base.OnButtonPressEvent(evnt);
            System.Console.WriteLine("On button press event");
            lastpoint = new PointD(evnt.X - x_offset, evnt.Y - y_offset);
                 testLayer.AddToPath(lastpoint, Layer.NextPointFunction.MoveTo);
           
            return true;
        }

        protected override bool OnButtonReleaseEvent(Gdk.EventButton evnt)
        {
            base.OnButtonReleaseEvent(evnt);
            layerRenderNeeded = true;
            this.QueueDraw();
            return true;
        }

        protected override bool OnMotionNotifyEvent(Gdk.EventMotion evnt)
        {
            base.OnMotionNotifyEvent(evnt);
            //double drawx = evnt.X - x;
            //double drawy = evnt.Y - y;
            
            PointD current = new PointD(evnt.X-x_offset, evnt.Y-y_offset);
            Cairo.Context cr = new Context(workingSurface);
            
            testLayer.AddToPath(current, Layer.NextPointFunction.LineTo);
            
            cr.MoveTo(lastpoint.X + 0.5, lastpoint.Y+0.5);
            
            cr.LineTo(current.X + 0.5, current.Y + 0.5);
            cr.LineJoin = Cairo.LineJoin.Round;
            
            //cr.SetSourceRGBA(1, 0, 0,0.3);
            //cr.LineWidth = 3;
            //cr.StrokePreserve();
            
            cr.SetSourceRGBA(1, 0, 0,0.8);
            cr.LineWidth = 1;
            cr.Stroke();
            ((IDisposable)cr.GetTarget()).Dispose();
            ((IDisposable)cr).Dispose();
            
            int leftx = (int)Math.Min(current.X, lastpoint.X) - 1 + x_offset;
            int upx = (int)Math.Min(current.Y, lastpoint.Y) - 1 + y_offset;
            int boundingx = (int)Math.Abs(current.X - lastpoint.X) + 2;
            int boundingy = (int)Math.Abs(current.Y - lastpoint.Y) + 2;
            GdkWindow.InvalidateRect(new Gdk.Rectangle(leftx, upx, boundingx, boundingy),false);
            
            //System.Console.WriteLine(String.Format("({0},{1}) is better.", evnt.X-x_offset, evnt.Y-y_offset));
            
            lastpoint = current;
            q.Enqueue(lastpoint);
            return true;
        }
    }
}


