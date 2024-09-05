using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class Node : MonoBehaviour
{
    public int Index;
    public string Name;
    public float Distance;
    public List<Node> Connections = new List<Node>();

    [SerializeField] public RectTransform RectTransform;
    private UILineRenderer _lineRenderer;

    private void Start()
    {
        if (RectTransform == null)
            RectTransform = GetComponent<RectTransform>();
    }

    public void SetIndex(int index)
    {
        Index = index;
    }

    public void AddConnection(Node node, float distance)
    {
        Connections.Add(node);
        Distance = distance;
    }

    public void AddLineRenderer(Node endNode)
    {
        _lineRenderer = gameObject.AddComponent<UILineRenderer>();
        _lineRenderer.DrawLineBetweenUIElements(RectTransform, endNode.RectTransform);
    }

    public void HighLight(bool isHighlighted)
    {
        if (_lineRenderer == null)
            return;

        _lineRenderer.HighLight(isHighlighted);
    }
}