using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeColliderMaker : MonoBehaviour
{
    public float width;
    public float height;
    public Vector3 centerPos;

    private void Awake()
    {
        Terrain thisTerrain = this.GetComponent<Terrain>();
        MakeTreeColliders(thisTerrain, centerPos, height, width);
    }


    public static void MakeTreeColliders(Terrain terrain)
    {
        GameObject treeColliders = new GameObject("Tree Colliders");
        treeColliders.transform.parent = terrain.transform;
        TerrainData td = terrain.terrainData;
        TreeInstance[] tis = td.treeInstances;
        TreePrototype[] tps = td.treePrototypes;
        Bounds[] bounds = new Bounds[tps.Length];
        float[] radii = new float[tps.Length];
        //IEnumerable<World.Tree> worldTrees = Globals.instance.worldProperties.terrains.First(wt => wt.name == terrain.name).GetTrees();
        for (int i = 0; i < tps.Length; i++)
        {
            bounds[i] = tps[i].prefab.gameObject.GetComponent<MeshFilter>().sharedMesh.bounds;
            //radii[i] = worldTrees.First(wt => wt.name == tps[i].prefab.name).trunkSize;
        }
        int index = 0;
        foreach (TreeInstance ti in tis)
        {
            GameObject tc = new GameObject("TC" + string.Format("{0:00000}", index));
            CapsuleCollider cc = tc.AddComponent<CapsuleCollider>();
            cc.direction = 1;
            //cc.radius = radii[ti.prototypeIndex] * ti.widthScale;
            cc.height = bounds[ti.prototypeIndex].size.y * ti.heightScale;
            cc.center = Vector3.up * cc.height / 2f;
            tc.transform.parent = treeColliders.transform;
            tc.transform.position = TerrainExtras.WorldCoordinates(terrain, ti.position);
            index++;
        }
    }

    public static void MakeTreeColliders(Terrain terrain, Vector3 center, float height, float radius)
    {
        GameObject treeColliders = new GameObject("Tree Colliders");
        treeColliders.transform.parent = terrain.transform;
        TerrainData td = terrain.terrainData;
        TreeInstance[] tis = td.treeInstances;
        TreePrototype[] tps = td.treePrototypes;
        Bounds[] bounds = new Bounds[tps.Length];
        float[] radii = new float[tps.Length];
        //IEnumerable<World.Tree> worldTrees = Globals.instance.worldProperties.terrains.First(wt => wt.name == terrain.name).GetTrees();
        for (int i = 0; i < tps.Length; i++)
        {
            bounds[i] = tps[i].prefab.gameObject.GetComponent<MeshFilter>().sharedMesh.bounds;
            //radii[i] = worldTrees.First(wt => wt.name == tps[i].prefab.name).trunkSize;
        }
        int index = 0;
        foreach (TreeInstance ti in tis)
        {
            GameObject tc = new GameObject("TC" + string.Format("{0:00000}", index));
            CapsuleCollider cc = tc.AddComponent<CapsuleCollider>();
            cc.direction = 1;
            //cc.radius = radii[ti.prototypeIndex] * ti.widthScale;
            cc.height = height;//bounds[ti.prototypeIndex].size.y * ti.heightScale;
            cc.center = Vector3.up * cc.height / 2f;
            cc.radius = radius;
            tc.transform.parent = treeColliders.transform;
            tc.transform.position = TerrainExtras.WorldCoordinates(terrain, ti.position);
            index++;
        }
    }

    public static void MakeTreeCollidersBox(Terrain terrain, Vector3 size, Vector3 center)
    {
        GameObject treeColliders = new GameObject("Tree Colliders");
        treeColliders.transform.parent = terrain.transform;
        TerrainData td = terrain.terrainData;
        TreeInstance[] tis = td.treeInstances;
        TreePrototype[] tps = td.treePrototypes;
        Bounds[] bounds = new Bounds[tps.Length];
        float[] radii = new float[tps.Length];
        //IEnumerable<World.Tree> worldTrees = Globals.instance.worldProperties.terrains.First(wt => wt.name == terrain.name).GetTrees();
        for (int i = 0; i < tps.Length; i++)
        {
            bounds[i] = tps[i].prefab.gameObject.GetComponent<MeshFilter>().sharedMesh.bounds;
            //radii[i] = worldTrees.First(wt => wt.name == tps[i].prefab.name).trunkSize;
        }
        int index = 0;
        foreach (TreeInstance ti in tis)
        {
            GameObject tc = new GameObject("TC" + string.Format("{0:00000}", index));
            BoxCollider cc = tc.AddComponent<BoxCollider>();

            //cc.radius = radii[ti.prototypeIndex] * ti.widthScale;
            cc.size = size;
            cc.center = center;
            tc.transform.parent = treeColliders.transform;
            tc.transform.position = TerrainExtras.WorldCoordinates(terrain, ti.position);
            tc.transform.rotation = new Quaternion(0, 0, 0, 0);// Quaternion.Euler(0, 360 * ti.rotation,0);

            index++;
        }
    }
}

public class TerrainExtras
{
    public static Vector3 WorldCoordinates(Terrain terrain, Vector3 point)
    {
        Vector3 tdSize = terrain.terrainData.size;
        point.y = terrain.terrainData.GetHeight((int)(point.x * terrain.terrainData.heightmapResolution), (int)(point.z * terrain.terrainData.heightmapResolution));
        point.x *= tdSize.x;
        point.z *= tdSize.z;
        point += terrain.transform.position;
        return point;
    }
}