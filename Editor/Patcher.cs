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

// FMOD Event to GUID Workflow Swapper Patcher
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
        public const string EventRefDrawerPath = "Plugins/FMOD/src/Editor/EventRefDrawer.cs";
        public const string EventBrowserPath = "Plugins/FMOD/src/Editor/EventBrowser.cs";

        public const string PatchReference = "Using System;";
        public const string PatchReferenceAppend = "// Patched by Made Indrayana using FMOD EventRefDrawer.cs Patcher - https://github.com/made-indrayana/fmod-unity-event-guid-swapper \r\n";

        public const string DrawerSearch = "pathProperty.stringValue = ((EditorEventRef)DragAndDrop.objectReferences[0]).Path;";
        public const string DrawerReplace = @"
                    if (EditorPrefs.GetBool(""DoubleShot.FMODGUIDTweak.Enabled""))
                        pathProperty.stringValue = ((EditorEventRef) DragAndDrop.objectReferences[0]).Guid.ToString(""b"");
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

        public const string BrowserSearchOne = "string path = (data as EditorEventRef).Path;";
        public const string BrowserReplaceOne = @"
                    string path;
                    if (EditorPrefs.GetBool(""DoubleShot.FMODGUIDTweak.Enabled""))
                        path = (data as EditorEventRef).Guid.ToString(""b"");
                    else
                        path = (data as EditorEventRef).Path;";

        public const string BrowserSearchTwo = "emitter.Event = (data as EditorEventRef).Path;";
        public const string BrowserReplaceTwo = @"
                        if (EditorPrefs.GetBool(""DoubleShot.FMODGUIDTweak.Enabled""))
                            emitter.Event = (data as EditorEventRef).Guid.ToString(""b"");
                        else
                            emitter.Event = (data as EditorEventRef).Path;";

        public const string BrowserSearchThree = @"                if (itemIDs.TryGetValue(path, out itemID))
                {
                    SetSelection(new List<int> { itemID },
                        TreeViewSelectionOptions.RevealAndFrame | TreeViewSelectionOptions.FireSelectionChanged);
                }
                else
                {
                    SetSelection(new List<int>());
                }";
        public const string BrowserReplaceThree = @"
                if (itemIDs.TryGetValue(path, out itemID))
                {
                    SetSelection(new List<int> { itemID },
                        TreeViewSelectionOptions.RevealAndFrame | TreeViewSelectionOptions.FireSelectionChanged);
                }
                else if (itemIDs.TryGetValue(EventManager.EventFromPath(path).name.ToString(), out itemID))
                {
                    SetSelection(new List<int> { itemID },
                        TreeViewSelectionOptions.RevealAndFrame | TreeViewSelectionOptions.FireSelectionChanged);
                }
                else
                {
                    SetSelection(new List<int>());
                }";

    }

    [InitializeOnLoad]
    public class EditorStart
    {
        static EditorStart()
        {
            string eventRefDrawerPath = Path.Combine(Application.dataPath, PatchContent.EventRefDrawerPath);

            // Check if swap patch is already applied
            string check = File.ReadAllText(eventRefDrawerPath);
            if (!check.Contains(PatchContent.PatchReferenceAppend))
            {
                Debug.LogWarning("Event to GUID Patch is not active! To apply/reapply, go to Double Shot > FMOD > Patcher > Event to GUID");
            }
            else if (check.Contains(PatchContent.PatchReferenceAppend) && !EditorPrefs.HasKey("DoubleShot.FMODGUIDTweak.Enabled"))
            {
                if (EditorUtility.DisplayDialog("Double Shot Audio Patcher", "Event to GUID patch detected, but registry key for settings not found.\n\n" +
                        "Do you want to enable using GUID by default when selecting events on FMOD Event Browser?\n\n" +
                        "You can always change the setting in Double Shot > FMOD > Use GUID as Event Path.", "I want to use GUID as reference", "I want to use Path as reference"))
                {
                    EditorPrefs.SetBool("DoubleShot.FMODGUIDTweak.Enabled", true);
                }
                else
                    EditorPrefs.SetBool("DoubleShot.FMODGUIDTweak.Enabled", false);

            }
        }
    }
    public class Patcher
    {
        [MenuItem("Double Shot/FMOD/Patcher/Event to GUID Swapper", false, 0)]
        public static void EventToGUIDPatcher()
        {
            string eventRefDrawerPath = Path.Combine(Application.dataPath, PatchContent.EventRefDrawerPath);
            string eventBrowserPath = Path.Combine(Application.dataPath, PatchContent.EventBrowserPath);

            // Null checker for FMOD
            if (!File.Exists(eventRefDrawerPath))
            {
                Debug.LogWarning("FMOD src folder does not exist!");
                return;
            }

            // Check if swap patch is already applied
            string check = File.ReadAllText(eventRefDrawerPath);
            if (check.Contains(PatchContent.PatchReferenceAppend))
            {
                Debug.Log("Event to GUID already patched!");
                return;
            }

            if (EditorUtility.DisplayDialog("Double Shot Audio Patcher",
                "This FMOD patch will add a \"Swap\" button to [FMODUnity.EventRef] property which facilitates swapping Event Path with GUID. \n\n" +
                "In addition, this patch will allow you to use GUID by default when selecting events on FMOD Event Browser. \n\n" +
                "Patch has been tested and verified in FMOD Version 2.00.08 up to 2.01.07. \n\n" +
                "Do you want to continue?", "Yes", "No"))
            {
                // Check if swap patch is already applied
                if (check.Contains(PatchContent.PatchReferenceAppend))
                {
                    Debug.Log("EventRefDrawer.cs already patched!");
                    return;
                }

                // reading all the file and then put it into temp string
                string temp = File.ReadAllText(eventRefDrawerPath);
                temp = temp.Insert(0, PatchContent.PatchReferenceAppend); // Putting reference that the file has been patched
                temp = temp.Replace(PatchContent.DrawerSearch, PatchContent.DrawerReplace);
                temp = temp.Replace(PatchContent.SwapperSearch, PatchContent.SwapperReplace);
                File.WriteAllText(eventRefDrawerPath, temp);

                // Check if EventBrowser.cs patch is already applied
                check = File.ReadAllText(eventBrowserPath);
                if (check.Contains(PatchContent.PatchReferenceAppend))
                {
                    Debug.Log("EventBrowser.cs already patched!");
                    return;
                }

                temp = File.ReadAllText(eventBrowserPath);
                temp = temp.Insert(0, PatchContent.PatchReferenceAppend); // Putting reference that the file has been patched
                temp = temp.Replace(PatchContent.BrowserSearchOne, PatchContent.BrowserReplaceOne);
                temp = temp.Replace(PatchContent.BrowserSearchTwo, PatchContent.BrowserReplaceTwo);
                temp = temp.Replace(PatchContent.BrowserSearchThree, PatchContent.BrowserReplaceThree);
                File.WriteAllText(eventBrowserPath, temp);

                EditorPrefs.SetBool("DoubleShot.FMODGUIDTweak.Enabled", true);

                AssetDatabase.ImportAsset("Assets/Plugins/FMOD/src/Editor/EventRefDrawer.cs");
                AssetDatabase.ImportAsset("Assets/Plugins/FMOD/src/Editor/EventBrowser.cs");

                Debug.Log("EventRefDrawer.cs GUID Patch applied successfully.");
            }
        }
    }

    public class EnableDisableTweak
    {
        private const string MenuName = "Double Shot/FMOD/Use GUID as Event Path";
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
            if (!EditorPrefs.HasKey(SettingName))
                return false;
            else
            {
                Menu.SetChecked(MenuName, isTweakEnabled);
                return true;
            }
        }

        // For debugging only
        [MenuItem("Double Shot/Delete Key")]
        private static void DeleteKey()
        {
            EditorPrefs.DeleteKey(SettingName);
        }
        [MenuItem("Double Shot/Set Key")]
        private static void SetKey()
        {
            EditorPrefs.SetBool(SettingName, true);
        }
        [MenuItem("Double Shot/Force Editor Reload")]
        private static void ForceReload()
        {
            AssetDatabase.ImportAsset("Assets/Plugins/FMOD/src/Editor/EventRefDrawer.cs");
        }
    }

}