using System;
using Cairo;
using System.Collections;
using System.Collections.Generic;

namespace FantasyMapCreatorFinal
{
    // Struct
    public struct PathNode
    {
        private Cairo.PointD point;
        private Layer.NextPointFunction pointFunction;

        public PathNode(Cairo.PointD nPoint, Layer.NextPointFunction npf)
        {
            point = nPoint;
            pointFunction = npf;
        }

        public PointD Point()
        {
            return point;
        }

        public Layer.NextPointFunction PointFunction()
        {
            return pointFunction;
        }
    }

    public struct LayerProperties
    {
        public string LayerTypeName;
        public ArrayList Data;

        public LayerProperties(string ltn, ArrayList newdata)
        {
            LayerTypeName = ltn;
            Data = newdata;
        }
    }

    public class Layer : Cairo.ImageSurface
    {
        // Properties
        private List<PathNode> queuedPaths;
        private List<PathNode> erasePaths;

        public List<PathNode> surfacePaths{ get; set; }

        private ImageSurface mask;
        public bool layerHasChanged;

        public string Name{ get; set; }

        private LayerProperties lp;

        public ImageSurface Mask{ get { return mask; } }

        public enum NextPointFunction
        {
            MoveTo,
            LineTo
        }
        // Constructors
        public Layer(int width, int height, string name, LayerProperties lp) : base(Cairo.Format.Argb32, width, height)
        {
            mask = new ImageSurface(Cairo.Format.Argb32, this.Width, this.Height);
            Name = name;
            this.lp = lp;
            queuedPaths = new List<PathNode>();
            erasePaths = new List<PathNode>();
            surfacePaths = new List<PathNode>();
            layerHasChanged = true;
        }

        public void AddToPath(PointD nextPoint, Layer.NextPointFunction npf)
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
            using (Cairo.Context cr2 = new Context(mask))
            {
                // Draw new paths
                if (queuedPaths.Count > 0)
                {
                    ImageSurface maskSurface = new Cairo.ImageSurface(Cairo.Format.Argb32, Width, Height);
                    using (Cairo.Context cr = new Context(maskSurface))
                    {
                
                        foreach (PathNode pn in queuedPaths)
                        {
                            switch (pn.PointFunction())
                            {
                       
                                case(NextPointFunction.MoveTo):
                                    cr.MoveTo(pn.Point());
                                    break;       
                                case(NextPointFunction.LineTo):
                                    cr.LineTo(pn.Point());
                                    break;
                            }
                        }
                        cr.SetSourceRGBA(0, 0, 0, 1);
                        cr.Fill();
                
                        ((IDisposable)cr.GetTarget()).Dispose();
                        ((IDisposable)cr).Dispose(); 
                    }
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
                        switch (pn.PointFunction())
                        {
                            case(NextPointFunction.MoveTo):
                                cr2.MoveTo(pn.Point());
                                break;
                            case(NextPointFunction.LineTo):
                                cr2.LineTo(pn.Point());
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
        }

        public void GenerateEdgePath()
        {
            
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
            
        }

        public void CreateEdgeCairoPath(Cairo.Context cr)
        {
            if (surfacePaths.Count > 0)
            {
                //ImageSurface renderSurface = new Cairo.ImageSurface(this.Format, Width, Height);
                //Cairo.Context cr = new Context(renderSurface);
                
                foreach (PathNode pn in surfacePaths)
                {
                    switch (pn.PointFunction())
                    {
                       
                        case(NextPointFunction.MoveTo):
                            cr.MoveTo(pn.Point());
                            break;       
                        case(NextPointFunction.LineTo):
                            cr.LineTo(pn.Point());
                            break;
                    }
                }
            }
        }

        public void RenderLayer()
        {
            if (!layerHasChanged)
            {
                return;
            }
//            if (surfacePaths.Count > 0)
//            {
            ImageSurface renderSurface = new Cairo.ImageSurface(this.Format, Width, Height);
            using (Cairo.Context cr = new Context(renderSurface))
            {
//                
//                foreach (PathNode pn in surfacePaths)
//                {
//                    switch (pn.PointFunction())
//                    {
//                       
//                        case(NextPointFunction.MoveTo):
//                            cr.MoveTo(pn.Point());
//                            break;       
//                        case(NextPointFunction.LineTo):
//                            cr.LineTo(pn.Point());
//                            break;
//                    }
//                }
//                //This is the proper call here:
                FMCMainWindow.dr.Render(this, cr, lp.LayerTypeName, lp.Data);
                
                
                ((IDisposable)cr.GetTarget()).Dispose();
                ((IDisposable)cr).Dispose();   
            }
            using (Cairo.Context cr3 = new Cairo.Context(this))
            {
                cr3.Operator = Operator.Clear;
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

