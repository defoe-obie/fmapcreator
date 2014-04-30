using System;
using Gtk;

namespace FantasyMapCreatorFinal
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            Application.Init();
            FMCMainWindow win = new FMCMainWindow();
            win.ShowAll();
            Application.Run();
        }
    }
}
