/*
        ------------------------------------------------------------------------------------------------------------------------------
        ------------------------------------------------------------------------------------------------------------------------------
                                                                                           
             ((((                                                                          
            ((((     _______       _______.     ___                                        
             ))))   |       \     /       |    /   \                                       
          _ .---.   |  .--.  |   |   (----`   /  ^  \                                      
         ( |`---'|  |  |  |  |    \   \      /  /_\  \    _                                
          \|     |  |  '--'  |.----)   |    /  _____  \  /   _   _|  _        _  ._ |   _  
          : .___, : |_______/ |_______/    /__/     \__\ \_ (_) (_| (/_ \/\/ (_) |  |< _>  
           `-----'                                                                         
                                                                                           
        ------------------------------------------------------------------------------------------------------------------------------
        ------------------------------------------------------------------------------------------------------------------------------
*/

// FMOD EventRefDrawer.cs Patcher
// by Made Indrayana - Double Shot Audio
// Execute from Menu "Double Shot/FMOD/Patcher/Event to GUID Swapper"

using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace DoubleShot.Editor
{
    public class EventRefDrawerMod
    {
        [MenuItem("Double Shot/FMOD/Patcher/Event to GUID Swapper", false, 0)]
        public static void EventRefDrawerPatcher()
        {
            string path = Path.Combine(Application.dataPath, "Plugins/FMOD/src/Editor/EventRefDrawer.cs");
            string injectionPath = Path.GetFullPath("Packages/com.doubleshot.fmodeventguidswapper/Editor/EventRefDrawerPatchContent.md");

            // Null checker for FMOD
            if (!File.Exists(path))
            {
                Debug.LogWarning("FMOD src folder does not exist!");
                return;
            }

            // Null checker for PatchContent, accounting for either Package install or .unitypackage install
            if (!File.Exists(injectionPath))
            {
                bool checkIfNotPackage = File.Exists(Path.Combine(Application.dataPath, "DoubleShot/Editor/EventRefDrawerPatchContent.md"));
                if (!checkIfNotPackage)
                {
                    Debug.LogWarning("Patch content not found!");
                    return;
                }
                else
                    injectionPath = Path.Combine(Application.dataPath, "DoubleShot/Editor/EventRefDrawerPatchContent.md");
            }

            StringBuilder tempNewScript = new StringBuilder();
            StreamReader file = new StreamReader(path, true);
            string line;

            if (EditorUtility.DisplayDialog("Double Shot Audio Patcher",
                "This FMOD patch will add a \"Swap\" button to [FMODUnity.EventRef] property which facilitates swapping Event Path with GUID. \n\n" +
                "Patch has been tested and verified in FMOD Version 2.00.08 up to 2.01.07. \n\n" +
                "Do you want to continue?", "Yes", "No"))
            {
                // Checks if patch is already there
                while ((line = file.ReadLine()) != null)
                {
                    if (line.Contains("#region Added the swap GUID/Path functionality"))
                    {
                        Debug.Log("EventRefDrawer.cs GUID Patch already applied.");
                        file.DiscardBufferedData();
                        file.Close();
                        return;
                    }
                }

                file.DiscardBufferedData();
                file.BaseStream.Seek(0, SeekOrigin.Begin);

                #region NEW IMPLEMENTATION HERE:
                /*

                // reading all the file and then put it into temp string
                string[] tempTxt = File.ReadAllLines(path);

                // search the string array for the one you want to search 
                int pos = ArrayUtility.IndexOf<string>(tempTxt, "                    pathProperty.stringValue = ((EditorEventRef)DragAndDrop.objectReferences[0]).Path;");
                
                if(pos != -1) // if found, do replace stuffs
                {
                    tempTxt[pos] = "                    Debug.Log(\"Success\");";
                }

                // and then write all changes at once.
                File.WriteAllLines(path, tempTxt);

                */
                #endregion

                do
                {
                    line = file.ReadLine();
                    tempNewScript.AppendLine(line);
                } while (!line.Contains("if (!string.IsNullOrEmpty(pathProperty.stringValue) && EventManager.EventFromPath(pathProperty.stringValue) != null)"));

                line = file.ReadLine();
                tempNewScript.AppendLine(line);


                StreamReader fileToInject = new StreamReader(injectionPath, true);
                tempNewScript.Append(fileToInject.ReadToEnd());
                tempNewScript.Append(file.ReadToEnd());
                file.Close();
                fileToInject.Close();

                StreamWriter writer = new StreamWriter(path);
                writer.Write(tempNewScript);
                writer.Close();

                AssetDatabase.ImportAsset("Assets/Plugins/FMOD/src/Editor/EventRefDrawer.cs");

                Debug.Log("EventRefDrawer.cs GUID Patch applied successfully.");
            }
        }
    }

    public class EnableDisableTweak
    {
        private const string MenuName = "Double Shot/FMOD/Tweak Enabled";
        private const string SettingName = "DoubleShot.FMODGUIDTweak.Enabled";
        
        public static bool isTweakEnabled
        {
            get { return EditorPrefs.GetBool(SettingName, true); }
            set { EditorPrefs.SetBool(SettingName, value); }
        }

        [MenuItem(MenuName)]
        private static void ToggleTweak()
        {
            isTweakEnabled = !isTweakEnabled;
        }

        [MenuItem(MenuName, true)]
        private static bool ToggleTweakValidate()
        {
            Menu.SetChecked(MenuName, isTweakEnabled);
            return true;
        }
    
}
