using PathCreation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellPathCastIndicator : MonoBehaviour
{
    private PathCreator pathCreator;
    private VertexPath path;
    public GameObject pathPoint;
    [Range(0f, 2f)]
    public float curveAmount;
    [Range(0f, 5f)]
    public float starterCurveDistance;

    private void Start()
    {
        pathCreator = this.GetComponent<PathCreator>();
    }

    public void Update()
    {
        CreateLineMarkers();
    }

    public void CreateLineMarkers()
    {
        path = this.GetComponent<PathCreator>().path;
        foreach (Transform child in this.transform)
        {
            child.gameObject.SetActive(false);
        }

        for (int i = 1; i < path.NumPoints - 1; i++)
        {
            GameObject child = GameAssets.Instance.GetObject(pathPoint, this.transform);
            child.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            child.transform.position = path.GetPoint(i);
            Vector3 dir = child.transform.position - path.GetPoint(i + 1);
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;  // Assuming you want degrees not radians?
            child.transform.rotation = Quaternion.Euler(0f, 0f, angle + 90f);
        }
    }

    public void ChangePoints(Vector3 firstPoint, Vector3 secondPoint)
    {
        Vector3 curveAnchor = new Vector3(firstPoint.x /* + ((firstPoint.x - secondPoint.x) / 2f)*/, firstPoint.y - ((firstPoint.y - secondPoint.y) / starterCurveDistance));
        //Vector3 curveAnchor = new Vector3(firstPoint.x, firstPoint.y - Mathf.Sqrt(firstPoint.y - secondPoint.y));
        pathCreator.bezierPath = new BezierPath(new Vector2[] { firstPoint, curveAnchor, secondPoint }, false);
        pathCreator.bezierPath.AutoControlLength = curveAmount;
        pathCreator.TriggerPathUpdate();
    }
}
