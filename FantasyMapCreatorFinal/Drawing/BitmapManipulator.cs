using System;
using Cairo;
using System.Collections.Generic;
using System.Collections;

namespace FantasyMapCreatorFinal
{
    public class BitmapManipulator
    {
       
        public BitmapManipulator()
        {
           
        }

        private ImageSurface CreateMaskFromAlpha(Bitmap bitmap)
        {
            
             ImageSurface maskSurface = bitmap.ConvertToSurface();
            
            Cairo.Context cr = new Cairo.Context(maskSurface);
            cr.SetSourceRGB(0, 0, 0);
            
            cr.MaskSurface(maskSurface, 0, 0);
            
            ((IDisposable)cr.GetTarget()).Dispose();
            ((IDisposable)cr).Dispose();
            return maskSurface;
             
        }

        /// <summary>
        /// Creates a mask of strict 0 or 255 values using the alpha channel. Values become 0 if the original alpha value is less than the threshold.
        /// </summary>
        /// <returns>The bit mask as a new Cairo.ImageSurface.</returns>
        /// <param name="bitmap">Bitmap.</param>
        /// <param name="threshold">Threshold for alpha values.</param>
        public ImageSurface CreateBitMask(Bitmap bitmap, byte threshold = 128)
        {
            byte[] alpha = bitmap.Alpha;
            for (int i = 0; i < alpha.Length; ++i)
            {
                if (alpha[i] < threshold)
                    alpha[i] = 0;
                else
                    alpha[i] = 255;                        
            }          
            return CreateMaskFromAlpha(bitmap);
        }

        public ImageSurface CreateEdgeMask(Bitmap bitmap, byte threshold = 128)
        {
            byte[] alpha = (byte[])bitmap.Alpha.Clone();
           
            for (int y = 0; y < bitmap.Height; ++y)
            {
                for (int x = 0; x < bitmap.Width; ++x)
                {
                    if (bitmap.A(x, y) >= threshold)
                    {
                        bool surroundedFlag = true;
                        surroundedFlag = surroundedFlag && (bitmap.A(x - 1, y, 255) >= threshold);
                        surroundedFlag = surroundedFlag && (bitmap.A(x, y - 1, 255) >= threshold);
                        surroundedFlag = surroundedFlag && (bitmap.A(x + 1, y, 255) >= threshold);
                        surroundedFlag = surroundedFlag && (bitmap.A(x, y + 1, 255) >= threshold);
                        if (surroundedFlag)
                        {
                            alpha[x + y * bitmap.Width] = 0;
                        }
                        else
                        {
                            alpha[x + y * bitmap.Width] = 255;
                        }
                    }
                    else
                    {
                        alpha[x + y * bitmap.Width] = 0;
                    }
                    
                }
            }
            bitmap.Alpha = alpha;
            
            return CreateMaskFromAlpha(bitmap);     
            
           
        }


        private PointD GetNextPoint(Bitmap bitmap, int x, int y, byte threshold = 128)
        {
            int[] checkpoints = { -1, 0, 1 };
            
            foreach (int i in checkpoints)
            {
                foreach (int j in checkpoints)
                {
                    if (bitmap.A(x + j, y + i) >= threshold)
                    {
                        return (new PointD(x + j + 0.5, y + i + 0.5));
                    }
                }
            }
            return new PointD(-1,-1);
        }
        //TODO: Find a better (read:logical) place for this method. It doesn't really belong here.
        public List<PathNode> FindEdgePaths(Bitmap bitmap, byte threshold = 128)
        {
            ImageSurface edgeSurface = CreateEdgeMask(bitmap, threshold);
            
            Bitmap edgeBitmap = new Bitmap(edgeSurface);
         
            byte[] alpha = edgeBitmap.Alpha;
           // DebugDraw(alpha, edgeBitmap.Width, edgeBitmap.Height);
            
            List<PathNode> paths = new List<PathNode>();
          
            for (int y = 0; y < edgeBitmap.Height; ++y)
            {
                for (int x = 0; x < edgeBitmap.Width; ++x)
                {
                    //path does not start here
                    if (alpha[x + y * edgeBitmap.Width] < threshold)
                        continue;
                    
                    int curr_x = x;
                    int curr_y = y;
                    PointD currPoint = new PointD(curr_x + 0.5, curr_y+ 0.5);
                    Stack originPoints = new Stack();
                    originPoints.Push(currPoint);
                    bool newPathFlag = true;
                    bool endOfPathFlag = false;
                    while (originPoints.Count > 0)
                    {
                        if (newPathFlag){
                            paths.Add(new PathNode(currPoint, Layer.NextPointFunction.MoveTo));
                            newPathFlag = false;
                        }
                        else if (!endOfPathFlag){
                            paths.Add(new PathNode(currPoint, Layer.NextPointFunction.LineTo));
                        }
                        curr_x = (int)(currPoint.X - 0.5);
                        curr_y = (int)(currPoint.Y - 0.5);
                        alpha[curr_x + curr_y * edgeBitmap.Width] = 0;
                        PointD nextPoint = GetNextPoint(edgeBitmap, curr_x, curr_y, threshold);
                        // Reached End of a Path
                        if (nextPoint.X == -1){
                            currPoint = (PointD)originPoints.Pop();
                            endOfPathFlag = true;                            
                        }
                        else{
                            originPoints.Push(nextPoint);
                            currPoint = nextPoint;
                            if (endOfPathFlag){
                                newPathFlag = true;
                                endOfPathFlag = false;
                            }
                        }
                    }
                }
            }
            
            return paths;
        }
        
        private void DebugDraw(byte[] data, int width, int height){
            for (int y = 0; y < height; ++y){
                System.Console.WriteLine();
                for (int x = 0; x < width; ++x){
                    int index = x + y * width;
                    if (data[index] > 128){
                        System.Console.Write("##");
                    }
                    else{
                        System.Console.Write("--");
                    }
                }
            }
            System.Console.WriteLine();
        }
    }
}

