using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SpriteSlicer : EditorWindow
{
    string folderLocation;
    bool isSquare = true;
    int x;
    int y;
    int pixelPerUnit;
    float pivotX = 0.5f;
    float pivotY = 0.5f;

    [MenuItem("Tools/Sprite Slicer")]
    public static void ShowWindow()
    {
        GetWindow(typeof(SpriteSlicer));
    }

    private void OnGUI()
    {
        GUILayout.Label("Slice Sprites", EditorStyles.boldLabel);
        pixelPerUnit = EditorGUILayout.IntField("Pixel Per Unit", pixelPerUnit);

        EditorGUILayout.LabelField("Size of the sprite", EditorStyles.boldLabel);
        isSquare = EditorGUILayout.Toggle("Is Sprite Sheet One Strip?", isSquare);
        
        if (!isSquare)
        {
        EditorGUILayout.LabelField("Size of the sprite", EditorStyles.boldLabel);
            x = EditorGUILayout.IntField("X", x);
            y = EditorGUILayout.IntField("X", y);
        }
        EditorGUILayout.LabelField("Sprites Location", EditorStyles.boldLabel);
        folderLocation = EditorGUILayout.TextField("Folder:", folderLocation);

        EditorGUILayout.LabelField("Size of the sprite", EditorStyles.boldLabel);
        pivotX = EditorGUILayout.FloatField("Pivot X", pivotX);
        pivotY = EditorGUILayout.FloatField("Pivot Y", pivotY);

        if (GUILayout.Button("Slice"))
        {
            if (isSquare)
            {
                SliceSprites(folderLocation.TrimStart("Assets/Resources".ToCharArray()));
            }
            else
            {
                SliceSprites(folderLocation.TrimStart("Assets/Resources".ToCharArray()), x, y);
            }
        }
    }

    private void SliceSprites(string spriteLocations)
    {
        // Change the below for the path to the folder containing the sprite sheets (warning: not tested on folders containing anything other than just spritesheets!)
        // Ensure the folder is within 'Assets/Resources/' (the below example folder's full path within the project is 'Assets/Resources/ToSlice')
        string folderPath = spriteLocations;

        Object[] spriteSheets = Resources.LoadAll(folderPath, typeof(Texture2D));
        Debug.Log("spriteSheets.Length: " + spriteSheets.Length);

        for (int z = 0; z < spriteSheets.Length; z++)
        {
            Debug.Log("z: " + z + " spriteSheets[z]: " + spriteSheets[z]);

            string path = AssetDatabase.GetAssetPath(spriteSheets[z]);
            TextureImporter ti = AssetImporter.GetAtPath(path) as TextureImporter;
            ti.isReadable = true;
            ti.spriteImportMode = SpriteImportMode.Multiple;
            ti.spritesheet = new List<SpriteMetaData>().ToArray();
            ti.spritePixelsPerUnit = pixelPerUnit;
            ti.filterMode = FilterMode.Point;
            ti.textureCompression = TextureImporterCompression.Uncompressed;
            List<SpriteMetaData> newData = new List<SpriteMetaData>();

            Texture2D spriteSheet = spriteSheets[z] as Texture2D;

            for (int i = 0; i < spriteSheet.width; i += spriteSheet.height)
            {
                for (int j = spriteSheet.height; j > 0; j -= spriteSheet.height)
                {
                    SpriteMetaData smd = new SpriteMetaData();
                    smd.pivot = new Vector2(0.5f, 0.5f);
                    smd.alignment = 9;
                    smd.name = (spriteSheet.height - j) / spriteSheet.height + ", " + i / spriteSheet.height;
                    smd.rect = new Rect(i, j - spriteSheet.height, spriteSheet.height, spriteSheet.height);
                    smd.pivot = new Vector2(pivotX, pivotY);
                    newData.Add(smd);
                }
            }

            ti.spritesheet = newData.ToArray();
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        }
        Debug.Log("Done Slicing!");
    }

    private void SliceSprites(string spriteLocations, int x, int y)
    {
        // Change the below for the with and height dimensions of each sprite within the spritesheets
        int sliceWidth = x;
        int sliceHeight = y;

        // Change the below for the path to the folder containing the sprite sheets (warning: not tested on folders containing anything other than just spritesheets!)
        // Ensure the folder is within 'Assets/Resources/' (the below example folder's full path within the project is 'Assets/Resources/ToSlice')
        string folderPath = spriteLocations;

        Object[] spriteSheets = Resources.LoadAll(folderPath, typeof(Texture2D));
        Debug.Log("spriteSheets.Length: " + spriteSheets.Length);

        for (int z = 0; z < spriteSheets.Length; z++)
        {
            Debug.Log("z: " + z + " spriteSheets[z]: " + spriteSheets[z]);

            string path = AssetDatabase.GetAssetPath(spriteSheets[z]);
            TextureImporter ti = AssetImporter.GetAtPath(path) as TextureImporter;
            ti.isReadable = true;
            ti.spriteImportMode = SpriteImportMode.Multiple;
            ti.spritePixelsPerUnit = pixelPerUnit;
            ti.filterMode = FilterMode.Point;
            ti.textureCompression = TextureImporterCompression.Uncompressed;

            List<SpriteMetaData> newData = new List<SpriteMetaData>();

            Texture2D spriteSheet = spriteSheets[z] as Texture2D;

            for (int i = 0; i < spriteSheet.width; i += sliceWidth)
            {
                for (int j = spriteSheet.height; j > 0; j -= sliceHeight)
                {
                    SpriteMetaData smd = new SpriteMetaData();
                    smd.pivot = new Vector2(0.5f, 0.5f);
                    smd.alignment = 9;
                    smd.name = (spriteSheet.height - j) / sliceHeight + ", " + i / sliceWidth;
                    smd.rect = new Rect(i, j - sliceHeight, sliceWidth, sliceHeight);
                    smd.pivot = new Vector2(pivotX, pivotY);

                    newData.Add(smd);
                }
            }

            ti.spritesheet = newData.ToArray();
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        }
        Debug.Log("Done Slicing!");
    }
}
