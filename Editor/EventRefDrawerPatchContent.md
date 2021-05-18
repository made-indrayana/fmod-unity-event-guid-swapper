                #region Added the swap GUID/Path functionality
                
                // Tested and verified with FMOD 2.00.08 to 2.01.07
                Rect swapRect = new Rect(searchRect.x, position.y + baseHeight, searchRect.width * 3 + 5, baseHeight);
                if (GUI.Button(swapRect, new GUIContent("Swap"), buttonStyle) && !string.IsNullOrEmpty(pathProperty.stringValue) && ((EventManager.EventFromPath(pathProperty.stringValue) != null) || EventManager.EventFromPath(pathProperty.stringValue) != null))
                {
                    EditorEventRef eventRef = EventManager.EventFromPath(pathProperty.stringValue);
                    if (pathProperty.stringValue.StartsWith("{"))
                    {
                        property.stringValue = eventRef.Path;
                        property.serializedObject.ApplyModifiedProperties();
                        GUI.changed = true;
                    }
                    else
                    {
                        property.stringValue = eventRef.Guid.ToString("b");
                        property.serializedObject.ApplyModifiedProperties();
                        GUI.changed = true;
                    }
                }
                
                #endregion

