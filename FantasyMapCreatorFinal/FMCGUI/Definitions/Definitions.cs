using System;
using System.Reflection;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using Gtk;

namespace FantasyMapCreatorFinal
{
    public struct DefaultLayerProperties
    {
        public string ID;
        public string Name;
        public Definitions.LType Layertype;
        private GUIRoutine guiRoutine;
        private CairoRoutine cairoRoutine;

        public DefaultLayerProperties(Definitions.LType ltype, string id, string name, CairoRoutine cr, GUIRoutine gr)
        {
            Layertype = ltype;
            ID = id;
            Name = name;
            cairoRoutine = cr;
            guiRoutine = gr;
        }

        public GUIRoutine GetGUIRoutine(){
            return guiRoutine;
        }
//        public VBox GetGUI()
//        {
//            guiRoutine.GenerateGUI();
//            return guiRoutine.GUI;           
//        }

        public void Render(Layer currentlayer, Cairo.Context cr, ArrayList data)
        {
            cairoRoutine.RenderCairo(currentlayer, cr, data);
        }
    }

    public class Definitions
    {
        private string definitions_dir;
        //private List<LayerPropertiesRedux> Oceans;
        //private List<DefaultLayerProperties> Islands;
        private List<DefaultLayerProperties> LayerDefinitions;
        public static string[] LayerTypes = { "Ocean", "Island", "Forest", "Mountain" };

        public enum LType
        {
            Ocean,
            Island,
            Forest,
            Mountain,
            Undefined
        }

        public Definitions()
        {
            string app_dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string prefix = Path.Combine(Path.Combine(app_dir, ".."), "..");
            string proj_dir = Path.GetFullPath(prefix);
            proj_dir = Path.Combine(proj_dir, "FMCGUI");
            proj_dir = Path.Combine(proj_dir, "Definitions");
            definitions_dir = Path.GetFullPath(proj_dir);
            //  Islands = new List<DefaultLayerProperties>();
            LayerDefinitions = new List<DefaultLayerProperties>();
            LoadDefinitions();
        }

        private LType ConvertToLType(string lname){
            for (int i = 0; i < LayerTypes.Length; ++i){
                if (LayerTypes[i] == lname){
                    return (LType)i;
                }
            }
            return LType.Undefined;
        }
        public string[] GetSubTypes(string layerTypeName)
        {
            LType ltn = ConvertToLType(layerTypeName);
            List<DefaultLayerProperties> toIterateOn = LayerDefinitions.FindAll(delegate(DefaultLayerProperties obj)
            {
                return (obj.Layertype == (LType)ltn);
            });
            string[] names = new string[toIterateOn.Count];
            for (int i = 0; i < toIterateOn.Count; ++i)
            {
                names[i] = toIterateOn[i].Name;
            }
            return names;
        }

        public string GetLayerPropertiesID(string layername)
        {//int ltype, int lsubtype){
            DefaultLayerProperties dlp = LayerDefinitions.Find(delegate(DefaultLayerProperties obj)
            {
                return obj.Name == layername;
            });
            return dlp.ID;
          
        }

        public void Render(Layer currentlayer, Cairo.Context cr, string layername, ArrayList data)
        {
            DefaultLayerProperties dlp = LayerDefinitions.Find(delegate(DefaultLayerProperties obj)
            {
                return obj.Name == layername;
            });
            if(dlp.Name != null){
                dlp.Render(currentlayer, cr, data);
            }
        }

        
        public GUIRoutine GetGUIRoutine(string layername){
            DefaultLayerProperties dlp = LayerDefinitions.Find(delegate(DefaultLayerProperties obj)
            {
                return obj.Name == layername;
            });
            if(dlp.Name == null){
                return new GUIRoutine();
            }
            return dlp.GetGUIRoutine();
               
        }
//        public VBox GetGUI(string layername)
//        {
//            DefaultLayerProperties dlp = LayerDefinitions.Find(delegate(DefaultLayerProperties obj)
//            {
//                return obj.Name == layername;
//            });
//            if(dlp.Name == null){
//                return new VBox();
//            }
//                
//            return dlp.GetGUI();
//        }

        private void LoadDefinitions()
        {
            string[] directories = Directory.GetDirectories(definitions_dir);
            foreach (string dir in directories)
            {
                string[] files = Directory.GetFiles(Path.Combine(definitions_dir, dir));
                foreach (string filename in files)
                {
                    try
                    {
                        using (StreamReader sr = new StreamReader(filename))
                        {
                            string layertype = sr.ReadLine().Trim();
                            LType layertypeid = LType.Undefined;
                            for (int i = 0; i < LayerTypes.Length; ++i)
                            {
                                if (layertype == LayerTypes[i])
                                {
                                    layertypeid = (LType)i;
                                }
                            }
                            if (layertypeid == LType.Undefined)
                            {
                                throw new Exception(string.Format("Warning: Line 1 of file {0} has undefined layertype.", Path.GetFileName(filename)));
                            }
                            string id = sr.ReadLine().Trim();
                            string name = sr.ReadLine().Trim();
                            GUIRoutine gr = new GUIRoutine();
                            int dataelements = Convert.ToInt32(sr.ReadLine().Trim());
                            for (int i = 0; i < dataelements; ++i)
                            {
                                gr.Routine.Add(sr.ReadLine().Trim());
                            }
                            CairoRoutine cr = new CairoRoutine();
                            string cairostring = sr.ReadLine().Trim();
                            if(cairostring == "cairobegin"){
                                cairostring = sr.ReadLine().Trim();
                                while(cairostring != "cairoend"){
                                    cr.Routine.Add(cairostring);
                                    cairostring = sr.ReadLine().Trim();
                                }
                            }
                            DefaultLayerProperties lpr = new DefaultLayerProperties(layertypeid, id, name, cr, gr);
                            LayerDefinitions.Add(lpr);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }   
                }
            }
                
        }
    }
}

