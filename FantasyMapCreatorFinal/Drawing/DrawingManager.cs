using System;
using System.Collections.Generic;
using Cairo;

namespace FantasyMapCreatorFinal
{
    public class DrawingManager
    {
        private List<Layer> layers;
        public int Width{ get; private set; }

        public int Height{ get; private set; }

        public Layer CurrentLayer
        {
            get{ return layers[CurrentLayerIndex]; } 
            set
            {
                if (layers.Contains(value))
                    CurrentLayerIndex = layers.IndexOf(value);
                
            }
        }

        public int CurrentLayerIndex{ get; set; }

        public DrawingManager(int width, int height)
        {
            layers = new List<Layer>();
            Width = width;
            Height = height;
            CurrentLayerIndex = -1;
            
        }


        public string[] GetLayerTitles()
        {
            string[] result = new string[layers.Count];
            for (int i = 0; i < layers.Count; ++i)
            {
                result[i] = layers[i].Name;
            }
            return result;
        }

        public bool RemoveCurrentLayer()
        {
            if (layers.Count > 1)
            {
                layers.Remove(CurrentLayer);
                CurrentLayerIndex = CurrentLayerIndex == 0 ? 0 : CurrentLayerIndex - 1;
                return true;
            }
            return false;
        }

        public void AddNewLayer(string newname, LayerProperties lp)
        {
            Layer newLayer = new Layer(this.Width, this.Height, newname, lp);
            CurrentLayerIndex = CurrentLayerIndex + 1;
            layers.Insert(CurrentLayerIndex, newLayer);
        }

        public ImageSurface RenderLayers()
        {
            ImageSurface finalSurface = new ImageSurface(Format.Argb32, Width, Height);
            using (Cairo.Context cr = new Cairo.Context(finalSurface))
            {
                foreach (Layer l in layers)
                {
                    if (l.Equals(CurrentLayer))
                    {
                        l.RenderLayer();
                    }
                    cr.SetSourceSurface(l, 0, 0);
                    cr.Paint();
                }
                ((IDisposable)cr.GetTarget()).Dispose();
                ((IDisposable)cr).Dispose();
            }
            return finalSurface;
        }
    }
}

