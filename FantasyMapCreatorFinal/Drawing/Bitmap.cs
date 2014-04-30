using System;
using Cairo;

namespace FantasyMapCreatorFinal
{
    public class Bitmap
    {
        //Properties
        private byte[] alpha, red, green, blue;
        private int width, height, stride;
        private Cairo.Format format;
        // REMINDER: stride refers to how many Bytes there are per row in the image.
        
        public byte[] Alpha{ get{return alpha;} set{alpha = value;}}
        public byte[] Red{get{return red;} set{red = value;}}
        public byte[] Green{ get { return green; }set{green = value;}}
        public byte[] Blue{ get { return blue; }set{blue = value;}}
        public int Width{ get{return width;}}
        public int Height{get{return height;}}
        public Cairo.Format Format{get{return format;}}
        
        private enum Channel
        {
            Alpha,
            Red,
            Green,
            Blue
        }
        
        public Bitmap(ImageSurface sourceSurface)
        {
            width = sourceSurface.Width;
            height = sourceSurface.Height;
            stride = sourceSurface.Stride;
            format = sourceSurface.Format;
            
            byte[] data = sourceSurface.Data;
            alpha = new byte[width * height];
            red = new byte[width * height];
            green = new byte[width * height];
            blue = new byte[width * height];
            ConvertDataToBitmap(data);
        }

        private void ConvertDataToBitmap(byte[] data)
        {
            for (int i = 0; i < data.Length; i += 4)
            {
                byte[] currentARGB = { data[i], data[i + 1], data[i + 2], data[i + 3] };
                // Reverse order for little Endian machines
                if (BitConverter.IsLittleEndian)
                {
                    Array.Reverse(currentARGB);
                }
                alpha[i / 4] = currentARGB[0];
                red[i / 4] = currentARGB[1];
                green[i / 4] = currentARGB[2];
                blue[i / 4] = currentARGB[3];
            }
        }
        
        
        private byte GetChannelDataXY(Channel channel, int x, int y, byte defaultReturnValue=0)
        {
            if ((x < 0) || (x >= width))
                return defaultReturnValue;
            if ((y < 0) || (y >= height))
                return defaultReturnValue;
            int index = x + y * width;
            switch (channel)
            {
                case(Channel.Red):
                    return red[index];
                case(Channel.Blue):
                    return blue[index];
                case(Channel.Green):
                    return green[index];
                case(Channel.Alpha):
                    return alpha[index];
            }
            return defaultReturnValue;
        }
       
        public byte R(int x, int y, byte defaultReturnValue=0)
        {
            return GetChannelDataXY(Channel.Red, x, y, defaultReturnValue);
        }
        public byte G(int x, int y, byte defaultReturnValue=0)
        {
            return GetChannelDataXY(Channel.Green, x, y);
        }
        public byte B(int x, int y, byte defaultReturnValue=0)
        {
            return GetChannelDataXY(Channel.Blue, x, y);
        }
        public byte A(int x, int y, byte defaultReturnValue=0)
        {
            return GetChannelDataXY(Channel.Alpha, x, y);
        }
        public byte[] RGB(int x, int y, byte defaultReturnValue=0)
        {
            return new byte[]{ R(x, y), G(x, y), B(x, y) };
        }
        public byte[] ARGB(int x, int y, byte defaultReturnValue=0)
        {
            return new byte[]{ A(x, y), R(x, y), G(x, y), B(x, y) };
        }
        
        
        public ImageSurface ConvertToSurface()
        {
            // TODO: Figure out why this particular method is giving a memory leak error in Cairo Surface
            byte[] data = new byte[stride * height];
            for (int i = 0; i < data.Length; i += 4)
            {
                byte[] currentARGB = { alpha[i / 4], red[i / 4], green[i / 4], blue[i / 4] };
                if (BitConverter.IsLittleEndian)
                    {
                    Array.Reverse(currentARGB);
                }
                data[i] = currentARGB[0];
                data[i + 1] = currentARGB[1];
                data[i + 2] = currentARGB[2];
                data[i + 3] = currentARGB[3];
                
            }
            return new ImageSurface(data, format, width, height, stride);     
        }
    }
}

