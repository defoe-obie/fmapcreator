using System;
using Gtk;
using Cairo;

namespace FantasyMapCreatorFinal
{
    public class CanvasBox : ScrolledWindow
    {
        private Cairo.Pattern checkers;
        private DrawingArea background;
        private int width, height;
        private int x_offset, y_offset;
        private bool canvasCreated, layerRenderNeeded;
        private Layer currentLayer;
        private ImageSurface workingSurface, layersSurface;
        private PointD firstPoint, previousPoint;

        public CanvasBox() : base()
        {
            width = 0;
            height = 0;
            canvasCreated = false;
            background = new DrawingArea();
            background.ExposeEvent += BackgroundExposeEvent;
            ImageSurface check = new Cairo.ImageSurface(Cairo.Format.Argb32, 20, 20);
            using (Cairo.Context checkContext = new Cairo.Context(check))
            {
                checkContext.SetSourceRGBA(0, 0, 0, 0.1);
                checkContext.Paint();
                checkContext.Rectangle(0, 0, 10, 10);
                checkContext.Rectangle(10, 10, 20, 20);
                checkContext.SetSourceRGBA(0, 0, 0, 0.2);
                checkContext.Fill();
                ((IDisposable)checkContext.GetTarget()).Dispose();
                ((IDisposable)checkContext).Dispose();
                checkers = new SurfacePattern(check);
                checkers.Extend = Cairo.Extend.Repeat;
            }
            
            this.AddWithViewport(background);
         
        }

        private void BackgroundExposeEvent(object sender, ExposeEventArgs evnt)
        {
            using (Cairo.Context thisContext = Gdk.CairoHelper.Create(background.GdkWindow))
            {
                thisContext.SetSource(checkers);
                thisContext.Paint();
                if (canvasCreated)
                {
                    int parentWidth = this.Allocation.Width;
                    int parentHeight = this.Allocation.Height;
                    this.x_offset = parentWidth > width ? (parentWidth - width) / 2 : 0;
                    this.y_offset = parentHeight > height ? (parentHeight - height) / 2 : 0;
            
                    thisContext.Rectangle(x_offset - 1, y_offset - 1, width + 2, height + 2);
                    thisContext.SetSourceRGBA(0, 0, 0, 0.9);
                    thisContext.Fill();
                    thisContext.SetSourceRGBA(0, 0, 0, 0.2);
                    thisContext.Rectangle(x_offset + 2, y_offset + 2, width, height);
                    thisContext.FillPreserve();
                    thisContext.Rectangle(x_offset + 4, y_offset + 4, width, height);
                    thisContext.FillPreserve();
                    thisContext.Rectangle(x_offset + 6, y_offset + 6, width, height);
                    thisContext.Fill();
                
                    thisContext.Rectangle(x_offset, y_offset, width, height);
                    thisContext.SetSourceRGB(1, 1, 1);
                    thisContext.Fill();
                    
                    if (layerRenderNeeded)
                    {
                        layersSurface = FMCMainWindow.dm.RenderLayers();
                        workingSurface = new ImageSurface(Format.Argb32, width, height);
                        layerRenderNeeded = false;
                    }
                    thisContext.SetSourceSurface(layersSurface, x_offset, y_offset);
                    thisContext.Paint();
                    thisContext.SetSourceSurface(workingSurface, x_offset, y_offset);
                    thisContext.Paint();
                }
                ((IDisposable)thisContext.GetTarget()).Dispose();
                ((IDisposable)thisContext).Dispose();
            }  
        }

        public void CreateCanvasBackground(int width, int height)
        {
            this.width = width;
            this.height = height;
            canvasCreated = true;
            layerRenderNeeded = true;
            currentLayer = FMCMainWindow.dm.CurrentLayer;
            
            workingSurface = new ImageSurface(Format.Argb32, width, height);
            layersSurface = new ImageSurface(Format.Argb32, width, height);
            background.AddEvents((int)Gdk.EventMask.ButtonPressMask);
            background.ButtonPressEvent += OnDrawingClick;
            background.AddEvents((int)Gdk.EventMask.Button1MotionMask);
            background.AddEvents((int)Gdk.EventMask.Button3MotionMask);
            background.MotionNotifyEvent += OnDrawing;
            background.AddEvents((int)Gdk.EventMask.ButtonReleaseMask);
            background.ButtonReleaseEvent += OnDrawingClickRelease;
            
            background.QueueDraw();
        }

        private void OnDrawingClick(object sender, ButtonPressEventArgs args)
        {
            currentLayer = FMCMainWindow.dm.CurrentLayer;
            Gdk.EventButton evnt = args.Event;
            System.Console.WriteLine("On button press event");
            firstPoint = new PointD(evnt.X - x_offset + 0.5, evnt.Y - y_offset + 0.5);
            previousPoint = firstPoint;
            //layerRenderNeeded = false;
            
            if (evnt.Button == 1) // Left Mouse Button : creating
            {
                currentLayer.AddToPath(firstPoint, Layer.NextPointFunction.MoveTo);
            }
            else if (evnt.Button == 3) // Right Mouse Button : erasing
            {
                currentLayer.AddToErasePath(firstPoint, Layer.NextPointFunction.MoveTo);
            }
        }

        private void OnDrawingClickRelease(object sender, ButtonReleaseEventArgs args)
        {
            Gdk.EventButton evnt = args.Event;
            
            System.Console.WriteLine("Button Released");
            if (evnt.Button == 1) // Left Mouse Button : creating
            {
                currentLayer.AddToPath(firstPoint, Layer.NextPointFunction.LineTo);
            }
            else if (evnt.Button == 3) // Right Mouse Button : erasing
            {
                currentLayer.AddToErasePath(firstPoint, Layer.NextPointFunction.LineTo);
            }
            layerRenderNeeded = true;
            background.GdkWindow.InvalidateRect(new Gdk.Rectangle(x_offset, y_offset, width, height), false);
            
        }

        private void OnDrawing(object sender, MotionNotifyEventArgs args)
        {
            Gdk.EventMotion evnt = args.Event;
            PointD current = new PointD(evnt.X - x_offset + 0.5, evnt.Y - y_offset + 0.5);
            using (Cairo.Context cr = new Context(workingSurface))
            {   
                if (evnt.State == Gdk.ModifierType.Button1Mask)
                {
                    currentLayer.AddToPath(current, Layer.NextPointFunction.LineTo);
                }
                else
                {
                    currentLayer.AddToErasePath(current, Layer.NextPointFunction.LineTo);
                }
                cr.MoveTo(previousPoint.X, previousPoint.Y);
                cr.LineTo(current.X, current.Y);
                cr.LineJoin = Cairo.LineJoin.Round;
                cr.LineWidth = 1;
                cr.SetSourceRGBA(1, 0, 0, 0.8);
                cr.Stroke();
                
                ((IDisposable)cr.GetTarget()).Dispose();
                ((IDisposable)cr).Dispose();
            
                int leftx = (int)Math.Min(current.X, previousPoint.X) - 1 + x_offset;
                int upx = (int)Math.Min(current.Y, previousPoint.Y) - 1 + y_offset;
                int boundingx = (int)Math.Abs(current.X - previousPoint.X) + 2;
                int boundingy = (int)Math.Abs(current.Y - previousPoint.Y) + 2;
                background.GdkWindow.InvalidateRect(new Gdk.Rectangle(leftx, upx, boundingx, boundingy), false);
            
                previousPoint = current;
            }
            
                
        }
    }
}

