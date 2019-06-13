using System;
using Assets.UltimateIsometricToolkit.Scripts.Core;
using UltimateIsometricToolkit.physics;
using UnityEngine;

[ExecuteInEditMode]
public class BaseFloorGenerator : MonoBehaviour
{
    public String Prefix = "Floor Tile";

    public GameObject Prefab;

    public int SizeX = 16;

    public int SizeZ = 16;

    public int Y;

    private void Awake()
    {
        this.GetOrAddComponent<IsoTransform>();
    }

    public void RePaint()
    {
        while (transform.childCount != 0)
        {
            foreach (Transform child in transform)
            {
                DestroyImmediate(child.gameObject);
            }
        }

        GenerateFloor();
    }

    private void GenerateFloor()
    {
        if (!Prefab)
        {
            Debug.Log("Select a prefab");
            return;
        }

        var isoTransform = GetComponent<IsoTransform>();
        var parentX = isoTransform.Position.x;
        var parentY = isoTransform.Position.y;
        var parentZ = isoTransform.Position.z;

        for (var dx = 0; dx <= SizeX; dx++)
        {
            for (var dz = 0; dz <= SizeZ; dz++)
            {
                var x = parentX + dx;
                var z = parentZ + dz;
                Insert(Prefab, Prefix + " (" + x + ", " + z + ")", x, Y + parentY, z);
            }
        }
    }

    private GameObject Insert(GameObject prefab, String name, float x, float y, float z)
    {
        GameObject gameObject = Instantiate(prefab);
        gameObject.transform.parent = transform;
        gameObject.name = name;

        gameObject.AddComponent<IsoBoxCollider>();
        var isoTransform = gameObject.GetComponent<IsoTransform>();
        isoTransform.Position = new Vector3(x, y, z);
        isoTransform.ShowBounds = name.StartsWith("Collider");

        return gameObject;
    }
}