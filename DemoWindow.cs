/***
 * Copyright (c) 2024 Red Games
 * 
 * Permission is hereby granted, free of charge, to any person
 * obtaining a copy of this software and associated documentation
 * files (the "Software"), to deal in the Software without
 * restriction, including without limitation the rights to use,
 * copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the
 * Software is furnished to do so, subject to the following
 * conditions:
 * 
 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
 * OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
 * NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
 * HOLRDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
 * WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USER OR
 * OTHER DEALINGS IN THE SOFTWARE.
 */

using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace RedGame.Framework.EditorTools
{
    // Demo data class
    public class LightItem
    {
        public bool enabled;
        public string name;
        public LightType type;
        public LightShape shape;
        public Color color;
        public float intensity;
    }
    
    // Demo editor window
    public class DemoWindow : EditorWindow
    {
        private SimpleEditorTableView<LightItem> _tableView;
        private LightItem[] _lightItems = Array.Empty<LightItem>();
 
        
        private LightType _newLightType = LightType.Directional;

        [MenuItem("My Tool/Show Table View Demo")]
        private static void ShowWindow()
        {
            var window = GetWindow<DemoWindow>();
            window.titleContent = new GUIContent("Lights Table");
            window.Show();
        }

        private SimpleEditorTableView<LightItem> CreateTable()
        {
            SimpleEditorTableView<LightItem> tableView = new SimpleEditorTableView<LightItem>();

            GUIStyle labelGUIStyle = new GUIStyle(GUI.skin.label)
            {
                padding = new RectOffset(left: 10, right: 10, top: 2, bottom: 2)
            };
            
            GUIStyle disabledLabelGUIStyle = new GUIStyle(labelGUIStyle)
            {
                normal = new GUIStyleState
                {
                    textColor = Color.gray
                }
            };

            tableView.AddColumn("E", 30, (rect, item) =>
            {
                rect.xMin += 10;
                item.enabled = EditorGUI.Toggle(
                    position: rect,
                    value: item.enabled
                );
            }).SetMaxWidth(30).SetTooltip("Enable/Disable Light");

            tableView.AddColumn("Name", 80, (rect, item) =>
            {
                GUIStyle style = item.enabled ? labelGUIStyle : disabledLabelGUIStyle;
                item.name = EditorGUI.TextField(
                    position: rect,
                    text: item.name,
                    style: style
                );
            }).SetAutoResize(true).SetTooltip("Light name")
                .SetSorting((a, b) => String.Compare(a.name, b.name, StringComparison.Ordinal));
            
            tableView.AddColumn("Type", 70, (rect, item) =>
            {
                float iconSize = rect.height;
                Rect iconRect = new Rect(rect.x, rect.y, iconSize, iconSize);
                Rect labelRect = new Rect(iconRect.xMax, rect.y, rect.width - iconSize, rect.height);

                string iconName = GetLightIcon(item.type);
                EditorGUI.LabelField(iconRect, EditorGUIUtility.IconContent(iconName));
                
                item.type = (LightType) EditorGUI.EnumPopup(
                    position: labelRect,
                    item.type
                );
            }).SetAllowToggleVisibility(true).SetSorting((a, b) => a.type - b.type);

            tableView.AddColumn("Shape", 70, (rect, item) =>
            {
                if (item.type != LightType.Directional)
                    item.shape = (LightShape)EditorGUI.EnumPopup(
                        position: rect,
                        item.shape
                    );
            }).SetAllowToggleVisibility(true).SetTooltip("Shape of light");

            tableView.AddColumn("Color", 100, (rect, item) =>
            {
                item.color = EditorGUI.ColorField(
                    position: rect,
                    item.color
                );
            }).SetAllowToggleVisibility(true);

            tableView.AddColumn("Intensity", 120, (rect, item) =>
            {
                item.intensity = EditorGUI.Slider(
                    position: rect,
                    value: item.intensity,
                    leftValue: 0,
                    rightValue: 3
                );
            }).SetAllowToggleVisibility(true).SetSorting((a, b) => a.intensity.CompareTo(b.intensity));

            tableView.AddColumn("Edit", 60, (rect, item) =>
            {
                if (GUI.Button(rect, "Delete"))
                {
                    _lightItems = _lightItems.Where(x => x != item).ToArray();
                }
            }).SetTooltip("Click to delete this light");
            
            return tableView;
        }

        private void OnGUI()
        {
            TitleGUI();
            
            if (_tableView == null)
                _tableView = CreateTable();
            
            _tableView.DrawTableGUI(_lightItems);
            
            StatusGUI();
        }

        private void TitleGUI()
        {
            EditorGUILayout.BeginHorizontal();

            _newLightType = (LightType)EditorGUILayout.EnumPopup("Light Type", _newLightType);
            
            // Create new light button
            if (GUILayout.Button("Create New"))
            {
                _lightItems = _lightItems.Concat(new[]
                {
                    new LightItem
                    {
                        enabled = true,
                        name = GetUniqueLightName(),
                        type = _newLightType,
                        shape = GetRandomLightShape(),
                        color = GetRandomColor(),
                        intensity = Mathf.Round(Random.Range(0.0f, 3.0f) * 100) * 0.01f
                    }
                }).ToArray();

                _newLightType = GetRandomLightType();
            }

            EditorGUILayout.EndHorizontal();
        }

        private void StatusGUI()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Total Light Count: " + _lightItems.Length);
            EditorGUILayout.EndHorizontal();
        }

        private string GetUniqueLightName()
        {
            int index = 1;
            string lightName = "Light " + index;
            while (_lightItems.Any(x => x.name == lightName))
            {
                index++;
                lightName = "Light " + index;
            }

            return lightName;
        }
        
        private Color GetRandomColor()
        {
            return new Color(
                Random.value,
                Random.value,
                Random.value
            );
        }
        
        private LightType GetRandomLightType()
        {
            return (LightType) Random.Range(0, 3);
        }
        
        private LightShape GetRandomLightShape()
        {
            return (LightShape) Random.Range(0, 3);
        }
        
        private static string GetLightIcon(LightType type)
        {
            switch (type)
            {
                case LightType.Directional:
                    return "DirectionalLight Icon";
                case LightType.Spot:
                    return "SpotLight Icon";
                case LightType.Area:
                    return "AreaLight Icon";
                case LightType.Disc:
                    return "DiscLight Icon";
                default:
                    return "d_Light Icon";
            }
        }
    }
}