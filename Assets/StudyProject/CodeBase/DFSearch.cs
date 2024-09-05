using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DFSearch : MonoBehaviour
{
    [SerializeField] private List<Node> _nodes = new();

    public void Add(Node a, Node b, float distance)
    {
        a.AddConnection(b, distance);
        b.AddConnection(a, distance);
    }

    public void Search(Node node)
    {
        bool[] visited = new bool[_nodes.Count];
        LinkedList<Node> nodes = new LinkedList<Node>();

        nodes.AddLast(node);
        
        while (nodes.Any())
        {
            Node current = nodes.LastOrDefault();

            foreach (Node connection in current.Connections)
            {
                if (visited[connection.Index])
                {
                    continue;
                }
                
                Debug.Log("connected node " + connection.name);
                nodes.AddLast(connection);
                visited[connection.Index] = true;
            }
        }
    }
    public class UILineRenderer : MonoBehaviour
    {
        public RectTransform uiElement1; // First UI element (assign in inspector)
        public RectTransform uiElement2; // Second UI element (assign in inspector)
        public LineRenderer lineRenderer; // LineRenderer component (assign in inspector)

        public void DrawLineBetweenUIElements()
        {
            if (lineRenderer == null || uiElement1 == null || uiElement2 == null)
            {
                Debug.LogError("LineRenderer or UI elements not assigned.");
                return;
            }

            // Set the LineRenderer to have 2 positions
            lineRenderer.positionCount = 2;

            // Convert UI positions to world positions
            Vector3 worldPosition1 = RectTransformUtility.WorldToScreenPoint(Camera.main, uiElement1.position);
            Vector3 worldPosition2 = RectTransformUtility.WorldToScreenPoint(Camera.main, uiElement2.position);

            // Convert screen positions to world positions
            Vector3 lineWorldPosition1 = Camera.main.ScreenToWorldPoint(new Vector3(worldPosition1.x, worldPosition1.y, -Camera.main.transform.position.z));
            Vector3 lineWorldPosition2 = Camera.main.ScreenToWorldPoint(new Vector3(worldPosition2.x, worldPosition2.y, -Camera.main.transform.position.z));

            // Set positions for the LineRenderer
            lineRenderer.SetPosition(0, lineWorldPosition1);
            lineRenderer.SetPosition(1, lineWorldPosition2);

            // Optional: Adjust LineRenderer settings
            lineRenderer.startWidth = 0.1f; // Set the width of the line
            lineRenderer.endWidth = 0.1f; // Set the width of the line
            lineRenderer.startColor = Color.red; // Set the start color of the line
            lineRenderer.endColor = Color.red; // Set the end color of the line
        }
    }

}



