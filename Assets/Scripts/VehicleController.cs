using Assets.UltimateIsometricToolkit.Scripts.Core;
using IronPython.Modules;
using Misc;
using UnityEditor;
using UnityEngine;

public class VehicleController : MonoBehaviour
{
   
    private CustomAStarAgent _astarAgent;

    
    // Start is called before the first frame update
    void Awake()
    {
        _astarAgent = this.GetOrAddComponent<CustomAStarAgent>();
    }

    public void test()
    {
        _astarAgent.MoveTo(new Vector3(5, 1, 4));
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}

[CustomEditor(typeof(VehicleController))]
public class CustomGridGraphEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        VehicleController myScript = (VehicleController) target;
        if (GUILayout.Button("test"))
        {
            myScript.test();
        }
    }
}
