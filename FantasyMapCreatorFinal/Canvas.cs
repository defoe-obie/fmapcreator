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
        private PointD lastpoint, firstpoint;
        private bool layerRenderNeeded;
        private List<Layer> layers;
        //Temporary
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
            
            layers = new List<Layer>();
            testLayer = new Layer(width, height);
            layerRenderNeeded = true;
            
            
            this.AddEvents((int)Gdk.EventMask.ButtonPressMask);
            this.AddEvents((int)Gdk.EventMask.Button1MotionMask);
            this.AddEvents((int)Gdk.EventMask.Button3MotionMask);
            this.AddEvents((int)Gdk.EventMask.ButtonReleaseMask);
        }

//        private void drawOnSurface(ImageSurface surface)
//        {
//            Cairo.Context cr = new Context(surface);
//            cr.SetSourceRGB(0, 0.2, 0.6);
//            cr.LineJoin = Cairo.LineJoin.Round;
//            
//            if (q.Count > 2)
//            {
//                PointD previous = (PointD)q.Dequeue();
//                PointD current; //= (PointD)q.Dequeue();
//                PointD next;
//                cr.MoveTo(previous);
//                while (q.Count > 1)
//                {
//                    current = (PointD)q.Dequeue();
//                    next = (PointD)q.Dequeue();
//                    cr.CurveTo(previous, current, next);
//                    previous = next;
//                    //current = next;
//                }
//                if (q.Count > 0)
//                    cr.LineTo((PointD)q.Dequeue());
//            }
//            
//            cr.Stroke();
//            ((IDisposable)cr.GetTarget()).Dispose();
//            ((IDisposable)cr).Dispose();
//            
//            q.Clear();
//        }

        protected override bool OnExposeEvent(Gdk.EventExpose evnt)
        {
            base.OnExposeEvent(evnt);
            
            int lawidth = this.Parent.Allocation.Width;
            int laheight = this.Parent.Allocation.Height;
            this.x_offset = lawidth > width ? (lawidth - width) / 2 : 0;
            this.y_offset = laheight > height ? (laheight - height) / 2 : 0;
            
            Console.WriteLine("EXPOSE EVENT");
            using (Cairo.Context gr = Gdk.CairoHelper.Create(this.GdkWindow))
            {
                // Rectangle r = new Rectangle(0, 0, Math.Min(lawidth, width), Math.Min(laheight, height));
                // gr.Rectangle(r);
                // gr.Clip();)
                ImageSurface slfjds = new Cairo.ImageSurface(Cairo.Format.A8, 20, 20);
                Cairo.Context la = new Cairo.Context(slfjds);
                la.SetSourceRGBA(0, 0, 0, 0.1);
                la.Paint();
                la.Rectangle(0, 0, 10, 10);
                la.Rectangle(10, 10, 20, 20);
                la.SetSourceRGBA(0, 0, 0, 0.2);
                la.Fill();
                ((IDisposable)la.GetTarget()).Dispose();
                ((IDisposable)la).Dispose();
                Pattern checkers = new SurfacePattern(slfjds);
                checkers.Extend = Cairo.Extend.Repeat;
                //gr.SetSource(slfjds);
                gr.Mask(checkers);
                //gr.Paint();
                gr.Rectangle(x_offset - 1, y_offset - 1, width + 2, height + 2);
                gr.SetSourceRGBA(0, 0, 0, 0.9);
                gr.Fill();
                gr.SetSourceRGBA(0, 0, 0, 0.2);
                gr.Rectangle(x_offset + 2, y_offset + 2, width, height);
                gr.FillPreserve();
                gr.Rectangle(x_offset + 4, y_offset + 4, width, height);
                gr.FillPreserve();
                gr.Rectangle(x_offset + 6, y_offset + 6, width, height);
                gr.Fill();
                
                //Temporary  white backgroundage.
                gr.Rectangle(x_offset, y_offset, width, height);
                gr.SetSourceRGB(1, 1, 1);
                gr.Fill();
                if (layerRenderNeeded)
                {
                    testLayer.RenderLayer();
                    gr.SetSourceSurface(testLayer, x_offset, y_offset);
                    gr.Paint();
                    workingSurface = new Cairo.ImageSurface(Cairo.Format.Argb32, width, height);
                }
                else
                {
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
            //base.OnButtonPressEvent(evnt);
           // Left Mouse Button
            if (evnt.Button == 1)
            {
                System.Console.WriteLine("On button press event");
                firstpoint = new PointD(evnt.X - x_offset + 0.5, evnt.Y - y_offset + 0.5);
                lastpoint = firstpoint;
                testLayer.AddToPath(firstpoint, Layer.NextPointFunction.MoveTo);
                layerRenderNeeded = false;
            }
            return true;
        }

        protected override bool OnButtonReleaseEvent(Gdk.EventButton evnt)
        {
            if (evnt.Button == 1)
            {
                testLayer.AddToPath(firstpoint, Layer.NextPointFunction.LineTo);
                layerRenderNeeded = true;
                this.QueueDraw();
            }
            return true;
        }

        protected override bool OnMotionNotifyEvent(Gdk.EventMotion evnt)
        {
            // base.OnMotionNotifyEvent(evnt);
            //double drawx = evnt.X - x;
            //double drawy = evnt.Y - y;
            
            if (evnt.State == Gdk.ModifierType.Button1Mask)
            {
           
                PointD current = new PointD(evnt.X - x_offset + 0.5, evnt.Y - y_offset + 0.5);
                Cairo.Context cr = new Context(workingSurface);
            
                testLayer.AddToPath(current, Layer.NextPointFunction.LineTo);
            
                cr.MoveTo(lastpoint.X, lastpoint.Y);
                cr.LineTo(current.X, current.Y);
                cr.LineJoin = Cairo.LineJoin.Round;
                cr.LineWidth = 1;
                cr.SetSourceRGBA(1, 0, 0, 0.8);
                cr.Stroke();
                
                ((IDisposable)cr.GetTarget()).Dispose();
                ((IDisposable)cr).Dispose();
            
                int leftx = (int)Math.Min(current.X, lastpoint.X) - 1 + x_offset;
                int upx = (int)Math.Min(current.Y, lastpoint.Y) - 1 + y_offset;
                int boundingx = (int)Math.Abs(current.X - lastpoint.X) + 2;
                int boundingy = (int)Math.Abs(current.Y - lastpoint.Y) + 2;
                GdkWindow.InvalidateRect(new Gdk.Rectangle(leftx, upx, boundingx, boundingy), false);
            
                //System.Console.WriteLine(String.Format("({0},{1}) is better.", evnt.X-x_offset, evnt.Y-y_offset));
            
                lastpoint = current;
            }
            else if(evnt.State == Gdk.ModifierType.Button3Mask)
                {
                Console.WriteLine("RIGHT CLICK!");
            }
            return true;
                
        }
    }
}


