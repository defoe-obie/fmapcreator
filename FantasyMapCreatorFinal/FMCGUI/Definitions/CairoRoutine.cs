using System;
using Cairo;
using System.Collections;
using System.Collections.Generic;

namespace FantasyMapCreatorFinal
{
    public class CairoRoutine
    {
        public ArrayList Routine;
        private ArrayList currentData;
        private Layer currentLayer;

        public CairoRoutine()
        {
            Routine = new ArrayList();
        }

        public void RenderCairo(Layer layer, Cairo.Context cr, ArrayList data)
        {
            currentLayer = layer;
            currentData = data;
            foreach (string s in Routine)
            {
                string[] commands = s.Split('=');
                switch (commands[0])
                {
                    case("using"):
                        ProcessUsing(cr, commands[1]);
                        break;
                    case("setcolor"):
                        ProcessSetColor(cr, commands[1]);
                        break;
                    case("paint"):
                        cr.Paint();
                        break;
                }
            }
        }

        private void ProcessSetColor(Cairo.Context cr, string s)
        {
            double[] rgbcolor;
            // Variable
            if (s.StartsWith("@"))
            {
                int varnumber = Convert.ToInt32(s.Substring(1));
                Gdk.Color setcolor = (Gdk.Color)currentData[varnumber];
                rgbcolor = ConvertColorToDoubles(setcolor);
                
            }
            // Otherwise
            else
            {
                string[] rgb = s.Split(',');
                rgbcolor = new double[]{ Convert.ToByte(rgb[0]) / 255.0, Convert.ToByte(rgb[1]) / 255.0, Convert.ToByte(rgb[2]) / 255.0 };
            }
            cr.SetSourceRGB(rgbcolor[0], rgbcolor[1], rgbcolor[2]);
        }

        private void ProcessUsing(Cairo.Context cr, string s)
        {
            string[] commands = s.Split(',');
            switch (commands[0])
            {
                case("special"):
                    ProcessSpecial(cr, commands[1]);
                    break;
                case("hardmask"):
                    break;
            }
        }

        private void ProcessSpecial(Cairo.Context cr, string s)
        {
            switch (s)
            {
                case("multiplecontour"):
                    RenderMultipleContour(cr);
                    break;
            }
        }

        private double[] ConvertColorToDoubles(Gdk.Color color)
        {
            double[] rgb = new double[3];
            rgb[0] = (color.Red * 1.0) / ushort.MaxValue;
            rgb[1] = (color.Green * 1.0) / ushort.MaxValue;
            rgb[2] = (color.Blue * 1.0) / ushort.MaxValue;
            return rgb;
        }

        private void CreateEdgePath(Cairo.Context cr)
        {
            currentLayer.GenerateEdgePath();
            currentLayer.CreateEdgeCairoPath(cr);
        }

        private void RenderMultipleContour(Cairo.Context cr)
        {
            CreateEdgePath(cr);
            
            int contournumber = Convert.ToInt32(currentData[0]);
            int contourwidth = Convert.ToInt32(currentData[1]) * 2;
            int contourspacing = Convert.ToInt32(currentData[2]);
            Gdk.Color color = (Gdk.Color)currentData[3];
            
            double[] rgb = ConvertColorToDoubles(color);
            cr.Antialias = Cairo.Antialias.Default;
            cr.LineCap = Cairo.LineCap.Round;
            cr.LineJoin = Cairo.LineJoin.Round;
                
            for (int i = contournumber; i > 0; --i)
            {
                cr.Operator = Cairo.Operator.Over;
                cr.LineWidth = i * (contourwidth + contourspacing) + contourwidth;
                double alpha = ((contournumber - i) * 1.0 / contournumber);
                cr.SetSourceRGBA(rgb[0], rgb[1], rgb[2], alpha);
                cr.StrokePreserve();
                
                cr.Operator = Cairo.Operator.Clear;
                cr.LineWidth = cr.LineWidth - contourwidth;
                cr.StrokePreserve();
            }
            
            cr.Operator = Cairo.Operator.Over;
            cr.LineWidth = Math.Max(contourwidth, 2.0);
            cr.SetSourceRGBA(rgb[0], rgb[1], rgb[2], 1);
            
            
            cr.Stroke();
            cr.SetSourceRGBA(1, 1, 1, 1);
            cr.MaskSurface(currentLayer.Mask, 0, 0);
           
        }
    }
}

//TODO: Add jitter to edge drawing
//TODO: Add preview in layer creation