using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TerrainGenerator))]
public class TerrainGeneratorEditor : Editor {
    public Shape shape;
    public Platform plat;

    public override void OnInspectorGUI()
    {
        TerrainGenerator terrain = (TerrainGenerator)target;

        shape = (Shape) terrain.shape;
        shape = (Shape)EditorGUILayout.EnumPopup("Shape", shape);
        plat = (Platform) terrain.plat;
        plat = (Platform)EditorGUILayout.EnumPopup("Platform type", plat);

        switch (shape) {
            case Shape.Plane:
                terrain.shape = Shape.Plane;
                break;
            case Shape.Sphere:
                terrain.shape = Shape.Sphere;
                break;
        }

        switch (plat) {
            case Platform.Square:
                terrain.Square = (GameObject)EditorGUILayout.ObjectField("Square", terrain.Square, typeof(GameObject), true);
                terrain.SquareWidth = EditorGUILayout.FloatField("Square Width", terrain.SquareWidth);
                terrain.plat = Platform.Square;
                break;
            case Platform.Spike:
                terrain.Spike = (GameObject)EditorGUILayout.ObjectField("Spike", terrain.Spike, typeof(GameObject), true);
                terrain.invertSpikes = EditorGUILayout.Toggle("Invert Spikes", terrain.invertSpikes);
                terrain.plat = Platform.Spike;
                break;
        }

        terrain.squareMaterial = (Material)EditorGUILayout.ObjectField("Material", terrain.squareMaterial, typeof(Material), true);

        if (GUILayout.Button("Build")) {
            terrain.Generate();
        }

        if (GUI.changed) {
            EditorUtility.SetDirty(terrain);
        }

        DrawDefaultInspector();
    }
}
