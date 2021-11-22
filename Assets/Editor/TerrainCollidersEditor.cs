using UnityEngine;
using UnityEditor;

public class TreeCollidersEditor
{
    [MenuItem("GameObject/Make Tree Colliders")]
    static void MakeTreeCollidersMenuItem()
    {
        foreach (Transform selection in Selection.transforms)
        {

            Terrain terrain = selection.GetComponent<Terrain>();
            if (terrain)
                TreeColliderMaker.MakeTreeColliders(terrain);
        }
    }


    [MenuItem("GameObject/Make Tree Colliders Box")]
    static void MakeTreeCollidersMenuItemBox()
    {
        foreach (Transform selection in Selection.transforms)
        {

            Terrain terrain = selection.GetComponent<Terrain>();
            if (terrain)
                TreeColliderMaker.MakeTreeColliders(terrain);//, new Vector3(1, 1, 1), new Vector3(0, 0, 0));
        }
    }

    [MenuItem("GameObject/Make Tree Colliders Box", true)]
    static bool ValidateMakeTreeCollidersMenuItemBox()
    {
        if (Selection.activeTransform)
        {
            foreach (Transform selection in Selection.transforms)
            {
                Terrain terrain = selection.GetComponent<Terrain>();
                if (terrain)
                    return true;
            }
            return false;
        }
        else
            return false;
    }

    [MenuItem("GameObject/Make Tree Colliders", true)]
    static bool ValidateMakeTreeCollidersMenuItem()
    {
        if (Selection.activeTransform)
        {
            foreach (Transform selection in Selection.transforms)
            {
                Terrain terrain = selection.GetComponent<Terrain>();
                if (terrain)
                    return true;
            }
            return false;
        }
        else
            return false;
    }

}
