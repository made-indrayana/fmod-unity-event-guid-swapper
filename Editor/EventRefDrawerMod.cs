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
    public class PatchContent
    {
        public const string EventRefDrawerSearchString = "pathProperty.stringValue = ((EditorEventRef)DragAndDrop.objectReferences[0]).Path;";
        public const string EventRefDrawerReplaceString = @"
                    if(EditorPrefs.GetBool(""DoubleShot.FMODGUIDTweak.Enabled"", false))
                        pathProperty.stringValue = ""{"" + ((EditorEventRef) DragAndDrop.objectReferences[0]).Guid.ToString() + ""}"";
                    else
                        pathProperty.stringValue = ((EditorEventRef) DragAndDrop.objectReferences[0]).Path;";



        public const string SwapperSearch = "Rect foldoutRect = new Rect(position.x + 10, position.y + baseHeight, position.width, baseHeight);";
        public const string SwapperReplace = @"

                #region Added the swap GUID/Path functionality
                
                // Tested and verified with FMOD 2.00.08 to 2.01.07
                Rect swapRect = new Rect(searchRect.x, position.y + baseHeight, searchRect.width * 3 + 5, baseHeight);
                if (GUI.Button(swapRect, new GUIContent(""Swap""), buttonStyle) && !string.IsNullOrEmpty(pathProperty.stringValue) && ((EventManager.EventFromPath(pathProperty.stringValue) != null) || EventManager.EventFromPath(pathProperty.stringValue) != null))
                {
                    EditorEventRef eventRef = EventManager.EventFromPath(pathProperty.stringValue);
                    if (pathProperty.stringValue.StartsWith(""{""))
                    {
                        property.stringValue = eventRef.Path;
                        property.serializedObject.ApplyModifiedProperties();
                        GUI.changed = true;
                    }
                    else
                    {
                        property.stringValue = eventRef.Guid.ToString(""b"");
                        property.serializedObject.ApplyModifiedProperties();
                        GUI.changed = true;
                    }
                }
                
                #endregion


                Rect foldoutRect = new Rect(position.x + 10, position.y + baseHeight, position.width, baseHeight);";
    }

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

            /*
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
            */

            if (EditorUtility.DisplayDialog("Double Shot Audio Patcher",
                "This FMOD patch will add a \"Swap\" button to [FMODUnity.EventRef] property which facilitates swapping Event Path with GUID. \n\n" +
                "Patch has been tested and verified in FMOD Version 2.00.08 up to 2.01.07. \n\n" +
                "Do you want to continue?", "Yes", "No"))
            {
                /*
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
                file.Close();
                */

                #region NEW IMPLEMENTATION HERE:

                EditorPrefs.SetBool("DoubleShot.FMODGUIDTweak.Enabled", false);

                // reading all the file and then put it into temp string
                string temp = File.ReadAllText(path);
                temp = temp.Replace(PatchContent.EventRefDrawerSearchString, PatchContent.EventRefDrawerReplaceString);
                temp = temp.Replace(PatchContent.SwapperSearch, PatchContent.SwapperReplace);
                Debug.Log(temp);

                File.WriteAllText(path, temp);

                #endregion

                /*
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
                */

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
            get { return EditorPrefs.GetBool(SettingName, false); }
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
}