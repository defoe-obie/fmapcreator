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
        private List<PathNode> queuedPaths;
        private List<PathNode> erasePaths;
        private List<PathNode> surfacePaths;
        private ImageSurface mask;
        //private LayerProperties layerproperties;
        //private LayerType layertype;
        private bool layerHasChanged;
        
        public string Name{get;set;}
        //public LayerType LType{ get;set;}
        public LayerProperties Properties{ get; set; }
        public ImageSurface Mask{ get { return mask; } }
        //public LayerProperties LayerProperites{ get { return layerproperties; } set { layerproperties = value; } }

        //public LayerType LType{ get { return layertype; } set { layertype = value; } }

        // E-e-e-e-e-e-e-numerations!
        public enum LayerType
        {
            Ocean,
            Island,
            Mountain,
            Forest,
            Road,
            Glyph,
            Otherterrain
        }
        
        public enum NextPointFunction
        {
            MoveTo,
            LineTo
        }
        
        // Constructors
        public Layer(int width, int height, string name, LayerProperties lp): base(Cairo.Format.Argb32, width, height){
            mask = new ImageSurface(Cairo.Format.Argb32, this.Width, this.Height);
            Name = name;
            queuedPaths = new List<PathNode>();
            erasePaths = new List<PathNode>();
            surfacePaths = new List<PathNode>();
            layerHasChanged = false;
        }
//        public Layer(string filename) : base(filename)
//        { 
//            initialize();
//        }
//
//        public Layer(int width, int height) : base(Cairo.Format.Argb32, width, height)
//        {
//            initialize();
//        }
//
//        public Layer(Cairo.Format format, int width, int height) : base(format, width, height)
//        { 
//            initialize();
//        }
        // Private Methods
        
        // Public methods
        public void AddToPath(PointD nextPoint, NextPointFunction npf)
        {
            PathNode newNode = new PathNode(nextPoint, npf);
            layerHasChanged = true;
            queuedPaths.Add(newNode);
        }

        public void AddToErasePath(PointD nextPoint, NextPointFunction npf)
        {
            PathNode newNode = new PathNode(nextPoint, npf);
            layerHasChanged = true;
            erasePaths.Add(newNode);
        }

        public void RenderMask()
        {
            Cairo.Context cr2 = new Context(mask);
            // Draw new paths
            if (queuedPaths.Count > 0)
            {
                ImageSurface maskSurface = new Cairo.ImageSurface(Cairo.Format.Argb32, Width, Height);
                Cairo.Context cr = new Context(maskSurface);
                
                foreach (PathNode pn in queuedPaths)
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
                cr.SetSourceRGBA(0, 0, 0, 1);
                cr.Fill();
                
                ((IDisposable)cr.GetTarget()).Dispose();
                ((IDisposable)cr).Dispose(); 
                
                cr2.SetSourceRGBA(0, 0, 0, 1);
                cr2.MaskSurface(maskSurface, 0, 0);
                queuedPaths.Clear();
            }
            // Erase new erase paths
            if (erasePaths.Count > 0)
            {
                Console.WriteLine("Erasing");
                foreach (PathNode pn in erasePaths)
                {
                    switch (pn.PointFunction)
                    {
                        case(NextPointFunction.MoveTo):
                            cr2.MoveTo(pn.Point);
                            break;
                        case(NextPointFunction.LineTo):
                            cr2.LineTo(pn.Point);
                            break;
                            
                    }
                }
                cr2.Operator = Cairo.Operator.Clear;
                cr2.Fill();
                erasePaths.Clear();
            }  
                
            ((IDisposable)cr2.GetTarget()).Dispose();
            ((IDisposable)cr2).Dispose(); 
        }

        public void RenderLayer()
        {
            if ( !layerHasChanged){
                return;
            }
            Console.WriteLine("Rendering Layer");
            RenderMask();
            Console.WriteLine("Doing EdgeGen");
            mask.Flush();
                
            Bitmap bitmap = new Bitmap(mask);
            BitmapManipulator bm = new BitmapManipulator();
                
            // TODO: make one static/global copy of BitmapManipulator
            surfacePaths = bm.FindEdgePaths(bitmap, 55);
            mask.MarkDirty();
            
            Console.WriteLine("Finished EdgeGen");
            
            if (surfacePaths.Count > 0)
            {
                ImageSurface renderSurface = new Cairo.ImageSurface(this.Format, Width, Height);
                Cairo.Context cr = new Context(renderSurface);
                
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
                // TODO: ContourWidth is actually twice what it will be displayed as.
                int contourspacing = 5;
                int contournumber = 3;
                int contourwidth = 2;
                cr.LineWidth = contourwidth;
                cr.Antialias = Cairo.Antialias.Default;
                cr.LineCap = Cairo.LineCap.Round;
                cr.LineJoin = Cairo.LineJoin.Round;
                
                for (int i = contournumber; i > 0; --i)
                {
                    cr.LineWidth = i * (contourwidth + contourspacing) + contourwidth;
                    cr.SetSourceRGBA(0, 0.2, 0.7, 1.0 / (i + (1 / contournumber)));
                    cr.StrokePreserve();
                    cr.LineWidth = cr.LineWidth - contourwidth;
                    cr.SetSourceRGBA(1, 1, 1, 1);
                    cr.StrokePreserve();
                }
                
                //cr.SetSourceRGBA(0.8, 0.7, 0.11, 0.5);
                cr.LineWidth = Math.Max(contourwidth, 2);
                cr.SetSourceRGBA(0, 0, 0, 1);
                cr.StrokePreserve();
                
                cr.SetSourceRGBA(1, 1, 1, 1);
                cr.MaskSurface(mask, 0, 0);
                
                ((IDisposable)cr.GetTarget()).Dispose();
                ((IDisposable)cr).Dispose();   
                
                Cairo.Context cr3 = new Cairo.Context(this);
                var la = cr3.Operator;
                cr3.Operator = Operator.Clear;
                //cr3.SetSourceRGBA(1, 1, 1, 0);
                cr3.Paint();
                cr3.Operator = Operator.Over;
                cr3.SetSourceSurface(renderSurface, 0, 0);
                cr3.Paint();
                ((IDisposable)cr3.GetTarget()).Dispose();
                ((IDisposable)cr3).Dispose();
           
            }
            Console.WriteLine("Done Rendering Layer");
            layerHasChanged = false;
        }
    }
}

