using System;
using System.Collections.Generic;
using Gongulus.MapEditor;
using UnityEditor;
using UnityEngine;

namespace Gongulus.Editor
{
    public class GridMapCreator : EditorWindow
    {
        private Vector2 offset;
        private Vector2 drag;

        private List<List<Node>> nodes;
        private List<List<PartScript>> parts;

        private GUIStyle empty;
        private GUIStyle currentStyle;
        
        private Vector2 nodePos;
        private StyleManager styleManager;

        private bool isErasing;

        private Rect menuBar;

        private GameObject map;
        [MenuItem(("Window/Grid map creator"))]
        private static void OpenWindow()
        {
            GridMapCreator window = GetWindow<GridMapCreator>();
            window.titleContent = new GUIContent("Grid map creator");
        }

        private void OnEnable()
        {
            SetupeStyles();
            SetupNodesAndParts();
            SetupMap();
        }

        private void SetupMap()
        {
            try
            {
                map = GameObject.FindGameObjectWithTag("Map");
                //RestoreMap(map);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            if (map == null)
            {
                map = new GameObject("Map");
                map.tag = "Map";
            }
        }

        private void RestoreMap(GameObject map)
        {
            if (map.transform.childCount > 0)
            {
                for (int i = 0; i < map.transform.childCount; i++)
                {
                    int ii = map.transform.GetChild(i).GetComponent<PartScript>().Row + styleManager.Offset.y;
                    int jj = map.transform.GetChild(i).GetComponent<PartScript>().Column+ styleManager.Offset.x;
                    GUIStyle theStyle = map.transform.GetChild(i).GetComponent<PartScript>().Style;
                    nodes[ii][jj].SetStyle(theStyle);
                    parts[ii][jj] = map.transform.GetChild(i).GetComponent<PartScript>();
                    parts[ii][jj].Part = map.transform.GetChild(i).gameObject;
                    parts[ii][jj].name = map.transform.GetChild(i).name;
                    parts[ii][jj].Row = ii;
                    parts[ii][jj].Column = jj;
                }
            }
        }

        private void SetupeStyles()
        {
            try
            {
                styleManager = GameObject.FindGameObjectWithTag("StyleManager").GetComponent<StyleManager>();
                for (int i = 0; i < styleManager.ButtonStyles.Length; i++)
                {
                    styleManager.ButtonStyles[i].NodeStyle = new GUIStyle();
                    styleManager.ButtonStyles[i].NodeStyle.normal.background = styleManager.ButtonStyles[i].Icon;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            
            empty = styleManager.ButtonStyles[0].NodeStyle;
            
            currentStyle = styleManager.ButtonStyles[1].NodeStyle;
        }

        private void SetupNodesAndParts()
        {
            nodes = new List<List<Node>>();
            parts = new List<List<PartScript>>();
            for (int i = 0; i < 25; i++)
            {
                nodes.Add(new List<Node>());
                parts.Add(new List<PartScript>());
                for (int j = 0; j < 25; j++)
                {
                    nodePos.Set(i * 30, j * 30);
                    nodes[i].Add(new Node(nodePos, 30, 30, empty));
                    
                    parts[i].Add(null);

                }
            }
        }

        private void OnGUI()
        {
            DrawGrid();
            DrawNodes();
            DrawMenuBar();
            ProcessNodes(Event.current);
            ProcessGrid(Event.current);
            
            if (GUI.changed)
            {
                Repaint();
            }
        }

        private void DrawMenuBar()
        {
            menuBar = new Rect(0, 0, position.width, 20);
            GUILayout.BeginArea(menuBar, EditorStyles.toolbar);
            GUILayout.BeginHorizontal();
            for (int i = 0; i < styleManager.ButtonStyles.Length; i++)
            {
                if (GUILayout.Toggle((currentStyle == styleManager.ButtonStyles[i].NodeStyle), new GUIContent(styleManager.ButtonStyles[i].ButtonText), EditorStyles.toolbarButton, GUILayout.Width(80)))
                {
                    currentStyle = styleManager.ButtonStyles[i].NodeStyle;
                }
            }
            
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }

        private void ProcessNodes(Event e)
        {
            int Row = (int)((e.mousePosition.x - offset.x)/30);
            int Column = (int)((e.mousePosition.y - offset.y)/30);
            if ((e.mousePosition.x - offset.x) < 0 || (e.mousePosition.x - offset.x) > 750 ||
                (e.mousePosition.y - offset.y) < 0 || (e.mousePosition.y - offset.y) > 750)
            {
            }
            else
            {
                if (e.type == EventType.MouseDown)
                {
                    if (nodes[Row][Column].Style.normal.background.name == "Empty")
                    {
                        isErasing = false;
                    }
                    else
                    {
                        isErasing = true;
                    }

                    PaintNodes(Row, Column);
                }

                if (e.type == EventType.MouseDrag)
                {
                    PaintNodes(Row, Column);
                    e.Use();
                }
            }
        }

        private void PaintNodes(int row, int column)
        {
            if (isErasing)
            {
                if (parts[row][column] != null)
                {
                    nodes[row][column].SetStyle(empty);
                    
                    DestroyImmediate(parts[row][column].gameObject);
                    
                    GUI.changed = true;
                }

                parts[row][column] = null;
            }
            else
            {
                if (parts[row][column] == null)
                {
                    nodes[row][column].SetStyle(currentStyle);
                    Debug.Log(currentStyle.normal.background.name);
                    GameObject g = Instantiate(Resources.Load("MapParts/" + currentStyle.normal.background.name)) as GameObject;
                    g.name = currentStyle.normal.background.name;
                    g.transform.position = new Vector3(column + styleManager.Offset.x, 0, row + styleManager.Offset.y);
                    g.transform.parent = map.transform;
                    parts[row][column] = g.GetComponent<PartScript>();
                    parts[row][column].Part = g;
                    parts[row][column].name = g.name;
                    parts[row][column].Row = row;
                    parts[row][column].Column = column;
                    parts[row][column].Style = currentStyle;
                    
                    
                    
                    GUI.changed = true;
                }       
            }
        }


        private void DrawNodes()
        {
            for (int i = 0; i < 25; i++)
            {
                for (int j = 0; j < 25; j++)
                {
                    nodes[i][j].Draw();
                }
            }
        }

        private void ProcessGrid(Event e)
        {
            drag = Vector2.zero;
            switch (e.type)
            {
                case EventType.MouseDrag:
                    if (e.button == 0)
                    {
                        OnMouseDrag(e.delta);
                    }
                    break;
            }
        }

        private void OnMouseDrag(Vector2 delta)
        {
            drag = delta;
            
            for (int i = 0; i < 25; i++)
            {
                for (int j = 0; j < 25; j++)
                {
                    nodes[i][j].Drag(delta);
                }
            }
            
            GUI.changed = true;
            
        }

        private void DrawGrid()
        {
            int widthDivider = Mathf.CeilToInt(position.width / 20);
            int heightDivider = Mathf.CeilToInt(position.height / 20);
            
            Handles.BeginGUI();
            Handles.color = new Color(0.5f, 0.5f, 0.5f, 0.2f);
            offset += drag;
            
            Vector3 newOffset = new Vector3(offset.x % 20, offset.y % 20, 0);

            for (int i = 0; i < widthDivider; i++)
            {
                Handles.DrawLine(new Vector3(20 * i, -20, 0) + newOffset, new Vector3(20*i, position.height, 0) + newOffset);
            }
            for (int i = 0; i < heightDivider; i++)
            {
                
                Handles.DrawLine(new Vector3(-20, i*20, 0) + newOffset, new Vector3(position.width, 20*i, 0) + newOffset);
            }

            Handles.color = Color.white;
            Handles.EndGUI();
        }
    }
}