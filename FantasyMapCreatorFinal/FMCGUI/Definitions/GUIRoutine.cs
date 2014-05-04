using System;
using Gtk;
using System.Collections;

namespace FantasyMapCreatorFinal
{
    public class GUIRoutine
    {
        public ArrayList Data { get; private set; }

        public ArrayList Routine{ get; private set; }

        public VBox GUI{ get; private set; }

        private enum GUIType
        {
            HScale,
            ColorChooser
        }

        public GUIRoutine()
        {
            Routine = new ArrayList();
        }

        public void GenerateGUI()
        {
            Data = new ArrayList();
       
            GUI = new VBox(false, 5);
            GUI.PackStart(new HSeparator());
           
            foreach (string s in Routine)
            {
                AddToGUI(s);
            }
        }

        public void AddToGUI(string s)
        {
            string[] commands = s.Split('=');
            int dataindex = Convert.ToInt32(commands[0]);
            commands = commands[1].Split(new char[]{ ':' }, 2);
            string guitype = commands[0];
            switch (guitype)
            {
                case("hslider"):
                    GUI.PackStart(HSlider(commands[1], dataindex));  
                    break;
                case("colorchooser"):
                    GUI.PackStart(ColorChooser(commands[1], dataindex));
                    break;
            }
        }

        private HBox ColorChooser(string s, int dataindex)
        {
            HBox returnbox = new HBox(false, 5);
                string[] commands = s.Split(',');
            
            try
            {
                string name = commands[0];
                byte r = Convert.ToByte(commands[1]);
                byte g = Convert.ToByte(commands[2]);
                byte b = Convert.ToByte(commands[3]);
                Gdk.Color defaultcolor = new Gdk.Color(r, g, b);
                Data.Insert(dataindex, defaultcolor);
                ColorButton cb = new ColorButton(defaultcolor);
                cb.ColorSet += (object sender, EventArgs e) =>
                {
                    Gdk.Color nowcolor = ((ColorButton)sender).Color;
                    Data[dataindex] = nowcolor;
                };
                returnbox.PackStart(new Label(name));
                returnbox.PackEnd(cb);
                
            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine(String.Format("Line \"{0}\" has wrong number of arguments. Found {1}, Expected 4.", s, commands.Length));    
            }
            return returnbox;
            
        }

        private HBox HSlider(string s, int dataindex)
        {
            string[] commands = s.Split(',');
            string name = commands[0];
            double min = Convert.ToDouble(commands[1]);
            double max = Convert.ToDouble(commands[2]);
            double step = Convert.ToDouble(commands[3]);
            double defaultvalue = Convert.ToDouble(commands[4]);
            HScale hs = new HScale(min, max, step);
            hs.Value = defaultvalue;
            Data.Insert(dataindex, defaultvalue);
            hs.ValueChanged += (object sender, EventArgs e) =>
            {
                Data[dataindex] = ((HScale)sender).Value;
            };            
            HBox returnbox = new HBox();
            returnbox.PackStart(new Label(name));
            returnbox.PackEnd(hs);
            return returnbox;
        }
    }
}

