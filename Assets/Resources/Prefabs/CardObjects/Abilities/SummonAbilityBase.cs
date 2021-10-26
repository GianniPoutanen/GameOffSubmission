using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "New Ability", menuName = "Summon Ability")]
[SerializeField]
public class SummonAbilityBase : AbilityBase
{
    [Header("The Piece To Summon")]
    [SerializeField]
    public PieceBase summonedPiece;
    [Header("The Piece To Summon")]
    public PieceStats Stats;
    public Vector3Int? target;
    private FieldManager _fm;

    private FieldManager fieldManager
    {
        get
        {
            if (_fm == null)
                _fm = GameObject.Find("Grid").GetComponent<FieldManager>();
            return _fm;
        }
    }

    public override bool CanCast()
    {
        target = fieldManager.fieldTilemap.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        return fieldManager.CheckSpaceFree((Vector3Int)target) && fieldManager.CheckInField((Vector3Int)target);
    }

    public override List<Vector3Int> GetTileTargets()
    {
        List<Vector3Int> targets = new List<Vector3Int>();
        target = fieldManager.fieldTilemap.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        if (target != null && fieldManager.CheckInField((Vector3Int)target))
            targets.Add((Vector3Int)target);
        return targets;
    }

    public override void Cancel()
    {
        target = null;
    }

    public override void Cast()
    {
        PieceBase piece = Instantiate(summonedPiece);
        Vector3 summonPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        piece.transform.position = new Vector3(summonPos.x, summonPos.y, 0);
        piece.allegiance = PieceBase.Team.ally;
        fieldManager.ClearHighlight();
    }
}

[CustomEditor(typeof(SummonAbilityBase)), CanEditMultipleObjects]
public class SummonAbilityBaseEditor : Editor
{
    SummonAbilityBase sab;

    SerializedProperty stats;
    SerializedProperty summon;
    SerializedProperty targetingType;
    SerializedProperty cost;

    private void OnEnable()
    {
        sab = (SummonAbilityBase)target;
        stats = serializedObject.FindProperty("Stats");
        summon = serializedObject.FindProperty("summonedPiece");
        targetingType = serializedObject.FindProperty("targetingType");
        cost = serializedObject.FindProperty("cost");
    }

    public override void OnInspectorGUI()
    {

        PieceBase prevPeice = sab.summonedPiece;
        //WIDTH - HEIGHT


        // Load changed values
        serializedObject.Update();
        EditorGUI.BeginChangeCheck();

        EditorGUILayout.PropertyField(cost);
        EditorGUILayout.PropertyField(targetingType);
        EditorGUILayout.PropertyField(summon);
        EditorGUILayout.PropertyField(stats);

        serializedObject.ApplyModifiedProperties();
        // Check if value has changed
        if (prevPeice != sab.summonedPiece && sab.summonedPiece != null)
        {
            Debug.Log("Piece Changed");
            sab.Stats = sab.summonedPiece.Stats;
        }
    }
}