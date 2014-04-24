using System;
using Cairo;
using System.Collections.Generic;

namespace FantasyMapCreatorFinal
{
    // Struct
    public struct PathNode
    {
        private PointD point;
        private Layer.NextPointFunction pointFunction;

        public PathNode(PointD newPoint, Layer.NextPointFunction newPointFunction)
        {
            point = newPoint;
            pointFunction = newPointFunction;
        }

        public PointD Point{ get { return point; } }

        public Layer.NextPointFunction PointFunction{ get { return pointFunction; } }
    }

    public class Layer : Cairo.ImageSurface
    {
        // Properties
        private List<PathNode> surfacePaths;
        private Surface mask;
        // Enums
        public enum NextPointFunction
        {
            MoveTo,
            LineTo}
        ;
        // Constructors
        public Layer(string filename) : base(filename)
        { 
            initialize();
        }

        public Layer(int width, int height) : base(Cairo.Format.Argb32, width, height)
        {
            initialize();
        }

        public Layer(Cairo.Format format, int width, int height) : base(format, width, height)
        { 
            initialize();
        }
        // Private Methods
        private void initialize()
        {
            mask = new ImageSurface(Cairo.Format.A8, this.Width, this.Height);
            surfacePaths = new List<PathNode>();
        }
        // Public methods
        public void AddToPath(PointD nextPoint, NextPointFunction npf)
        {
            PathNode newNode = new PathNode(nextPoint, npf);
            
            surfacePaths.Add(newNode);
        }

        public void RenderLayer()
        {
            
            if (surfacePaths.Count > 0)
            {
                ImageSurface renderSurface = new Cairo.ImageSurface(this.Format, Width, Height);
                
                Cairo.Context cr = new Context(renderSurface);
                Console.WriteLine("HErE");
                cr.LineWidth = 2;
                cr.SetSourceRGB(1, 0, 0);
                foreach (PathNode pn in surfacePaths)
                {
                    switch (pn.PointFunction)
                    {
                        case(NextPointFunction.MoveTo):
                            cr.MoveTo(pn.Point);
                            break;       
                        case(NextPointFunction.LineTo):
                            cr.LineTo(pn.Point);
                            break;
                    }
                    
                    
                }
                int contourspacing = 1;
                int contournumber = 3;
                int contourwidth = 2;
                
                // contourWidth = width of the actual contour lines
                // contourNumber = number of contours outside the island edges
                // contourSpacing = spacing between contours
                
                // must be drawn from largest to smallest
                // most intuitive
                //cr.LineWidth = 10;
                //cr.SetSourceRGBA(0, 0, 0, 1);
                //cr.StrokePreserve();
                cr.Antialias = Cairo.Antialias.None;
                cr.SetSourceRGBA(1, 1, 1, 1);
                cr.FillPreserve();
                
                Cairo.Context cr2 = new Cairo.Context(mask);
                cr2.SetSourceRGBA(0, 0, 0, 1);
                cr2.MaskSurface(renderSurface, 0, 0);
                
                ((IDisposable)cr2.GetTarget()).Dispose();
                ((IDisposable)cr2).Dispose();
                
                cr.LineWidth = contourwidth;
                cr.Antialias = Cairo.Antialias.Default;
                cr.LineCap = Cairo.LineCap.Round;
                cr.LineJoin = Cairo.LineJoin.Round;
                
                for (int i = contournumber; i > 0; --i)
                {
                    cr.LineWidth = i * (contourwidth * 2 + contourspacing) + contourwidth;
                    cr.SetSourceRGBA(0, 0.2, 0.7, 1.0 / (i + (1 / contournumber)));
                    cr.StrokePreserve();
                    cr.LineWidth = cr.LineWidth - contourwidth;
                    cr.SetSourceRGBA(1, 1, 1, 1);
                    cr.StrokePreserve();
                    
                }
                
                //cr.SetSourceRGBA(0.8, 0.7, 0.11, 0.5);
                cr.LineWidth = contourwidth;
                cr.SetSourceRGBA(0, 0, 0, 1);
                cr.Stroke();
                
                cr.SetSourceRGBA(1, 1, 1, 1);
                cr.MaskSurface(mask, 0, 0);
                
                Cairo.Context cr3 = new Cairo.Context(this);
                cr3.SetSourceSurface(renderSurface, 0, 0);
                cr3.Paint();
                ((IDisposable)cr3.GetTarget()).Dispose();
                ((IDisposable)cr3).Dispose();
                
                ((IDisposable)cr.GetTarget()).Dispose();
                ((IDisposable)cr).Dispose(); 
            }
        }
    }
}

