using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(EnemyLayeredSpawner))]
public class EnemySpawnEditor : Editor
{
    public int focusedLayer = 0;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();

        EditorGUILayout.LabelField("Table For Spawning Pieces", EditorStyles.boldLabel);
        EnemyLayeredSpawner spawnManager = (EnemyLayeredSpawner)target;
        if (GUILayout.Button("Add Spawn Layer"))
        {
            spawnManager.AddSpawnLayer();
        }

        if (spawnManager.spawnLayers != null)
        {
            // Set Slider
            int maxSpawnLayer = 0;
            foreach (SpawnLayer layer in spawnManager.spawnLayers)
            {
                if (layer.loopInterval > maxSpawnLayer)
                    maxSpawnLayer = layer.loopInterval;
            }

            spawnManager.spawnIndexIndicator = EditorGUILayout.IntSlider(spawnManager.spawnIndexIndicator, 0, maxSpawnLayer);
            DrawUILine(Color.grey);
            for (int i = 0; i < spawnManager.spawnLayers.Count; i++)
            {
                SpawnLayer layer = spawnManager.spawnLayers[i];
                EditorGUILayout.LabelField("Spawn Layer", EditorStyles.boldLabel);
            DrawUILine(Color.grey, 1, 2);
                if (GUILayout.Button("Remove Spawn Layer"))
                {
                    spawnManager.RemoveLayerAt(i);
                }
                if (GUILayout.Button("Add Spawn"))
                {
                    layer.AddSpawn();
                }

                layer.loopInterval = EditorGUILayout.IntField("Interval", spawnManager.spawnLayers[i].loopInterval);
                DrawUILine(Color.grey, 1, 2);

                layer.inspectorShow = EditorGUILayout.Foldout(layer.inspectorShow, "Show Spawn Group (" + layer.spawns.Count + ")");
                EditorGUI.indentLevel = 2;
                // Handle The Spawn Areas
                if (layer.inspectorShow)
                {
                    for (int j = 0; j < layer.spawns.Count; j++)
                    {
                        PiecePositionPair piecePair = spawnManager.spawnLayers[i].spawns[j];

                        EditorGUILayout.LabelField("Spawns", EditorStyles.boldLabel);
                        DrawUILine(Color.gray, 1);
                        if (GUILayout.Button("Remove Spawn"))
                        {
                            spawnManager.spawnLayers[i].spawns.RemoveAt(j);
                        }


                        if (spawnManager.spawnLayers[i].spawns[j].piece == null)
                            EditorGUILayout.LabelField("New Spawn");
                        else
                            EditorGUILayout.LabelField(piecePair.piece.name);


                        piecePair.inspectorShow = EditorGUILayout.Foldout(piecePair.inspectorShow, "Spawn Variables");
                        if (piecePair.inspectorShow)
                        {
                            EditorGUI.indentLevel = 4;
                            // Handle The Spawn Areas
                            piecePair.piece = (PieceBase)EditorGUILayout.ObjectField(piecePair.piece, typeof(PieceBase), false);
                            piecePair.pos = EditorGUILayout.Vector3IntField("Tile Position", piecePair.pos);
                            piecePair.colour = EditorGUILayout.ColorField("Colour of Gizmo drawn", piecePair.colour);
                        }
                    }
                }
                DrawUILine(Color.grey);

            }

        }

        serializedObject.ApplyModifiedProperties();
    }

    public static void DrawUILine(Color color, int thickness = 2, int padding = 10)
    {
        Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
        r.height = thickness;
        r.y += padding / 2;
        r.x -= 2;
        r.width += 6;
        EditorGUI.DrawRect(r, color);
    }
}