using System;
using System.Collections;
using Gtk;

namespace FantasyMapCreatorFinal
{
    public class CreateLayerDialog : Dialog
    {
        public string NewLayerName{ get; private set; }

        public string LayerTypeName{ get; private set; }

        public ArrayList NewData{ get; private set; }
        //public int LType{ get; private set;}
        //public int LSubType{ get; private set; }
        private Entry nameEntry;
        private ComboBox ltype, lsubtype;
        private HBox ltypebox, lsubtypebox;
        private VBox currentPropertiesBox;
        private GUIRoutine gr;
        private string[] allowedlayertypes;

        public CreateLayerDialog(string initiallayername="Untitled New Layer")
        {
            allowedlayertypes = Definitions.LayerTypes;
            CreateDialog(initiallayername);
        }

        public CreateLayerDialog(Definitions.LType[] allowedTypes, string initiallayername="Untitled New Layer")
        {
            allowedlayertypes = new string[allowedTypes.Length];
            for (int i = 0; i < allowedlayertypes.Length; ++i)
            {
                Definitions.LType x = allowedTypes[i];
                allowedlayertypes[i] = x.ToString();
                Console.WriteLine(x.ToString());
            }
            CreateDialog(initiallayername);
        }

        private void CreateDialog(string initiallayername)
        {
            
            this.Title = "Set New Layer Properties";
            this.SetPosition(WindowPosition.CenterOnParent);
            this.DestroyWithParent = true;
            this.KeepAbove = true;
            this.Modal = true;
            this.Resizable = false;
            this.TransientFor = (Gtk.Window)this.Parent;
            
            HBox entrybox = new HBox(false, 5);
            entrybox.PackStart(new Label("New Layer Name:"));
            nameEntry = new Entry(initiallayername);
            entrybox.PackEnd(nameEntry);
            
            ltypebox = new HBox(false, 5);
            ltype = new ComboBox(allowedlayertypes);
            ltype.Active = 0;
            ltype.Changed += OnLayerTypeChange;
            ltypebox.PackStart(new Label("Layer Type:"));
            ltypebox.PackEnd(ltype);
            
            lsubtypebox = new HBox(false, 5);
            lsubtype = new ComboBox(FMCMainWindow.dr.GetSubTypes(ltype.ActiveText));
            lsubtype.Active = 0;
            lsubtype.Changed += OnLayerSubTypeChange;
            lsubtypebox.PackStart(new Label("Sub Type:"));
            lsubtypebox.PackEnd(lsubtype);
            
            this.VBox.PackStart(entrybox);
            this.VBox.PackStart(ltypebox);
            this.VBox.PackStart(lsubtypebox);
           
            gr = FMCMainWindow.dr.GetGUIRoutine(lsubtype.ActiveText);
            gr.GenerateGUI();
            currentPropertiesBox = gr.GUI;
            this.VBox.PackStart(currentPropertiesBox);
            
            this.AddButton("Cancel", ResponseType.Cancel);
            this.AddButton("Create", ResponseType.Accept);
            this.ShowAll();
            
            
            
        }

        private void OnLayerTypeChange(object o, EventArgs args)
        {
            ComboBox cb = (ComboBox)o;
            
            this.VBox.Remove(lsubtypebox);
            
            lsubtypebox = new HBox(false, 5);
            lsubtype = new ComboBox(FMCMainWindow.dr.GetSubTypes(cb.ActiveText));
            lsubtype.Changed += OnLayerSubTypeChange;
            
            lsubtypebox.PackStart(new Label("Sub Type:"));
            lsubtypebox.PackEnd(lsubtype);
            
            this.VBox.PackStart(lsubtypebox);
            lsubtype.Active = 0;   
        }

        private void OnLayerSubTypeChange(object o, EventArgs args)
        {
            ComboBox cb = (ComboBox)o;
            
            this.VBox.Remove(currentPropertiesBox);
            gr = FMCMainWindow.dr.GetGUIRoutine(cb.ActiveText);
            gr.GenerateGUI();
            currentPropertiesBox = gr.GUI;
            this.VBox.PackStart(currentPropertiesBox);
            
            this.ShowAll();
        }

        protected override void OnResponse(ResponseType response_id)
        {
            if (response_id == ResponseType.Accept)
            {
                NewLayerName = nameEntry.Text;
                LayerTypeName = lsubtype.ActiveText;
                NewData = gr.Data;
            }
            
            
        }
    }
}

