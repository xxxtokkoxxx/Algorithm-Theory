using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace StudyProject.CodeBase
{
    public class Program : MonoBehaviour
    {
        [SerializeField] private Graph _graph;

        [SerializeField, ValueDropdown(nameof(GetNodes))]
        public Node _source;

        [SerializeField, ValueDropdown(nameof(GetNodes))]
        public Node _destination;

        [SerializeField] private TreeState _treeState;
        private Dictionary<Node, Node> _minimumSpanningTree;

        public IEnumerable GetNodes()
        {
            return _graph.Nodes;
        }

        public void InitAdjacencyCollection()
        {
            if (_graph.AdjacencyCollection.Any())
            {
                Debug.Log("THe collection is not empty, clear it before initializing a new one");
                return;
            }

            foreach (Node node in _graph.Nodes)
            {
                _graph.AdjacencyCollection.Add(node, new List<Edge>());
            }
        }

        public void AddEdge()
        {
            if (_graph != null)
            {
                if (_source == _destination)
                    return;

                _graph.AddEdge(_source, _destination);
            }
        }

        public void ClearAdjacencyCollection()
        {
            foreach (KeyValuePair<Node, List<Edge>> node in _graph.AdjacencyCollection)
            {
                node.Key.GetComponent<UILineRenderer>().Clear();
            }

            _graph.AdjacencyCollection.Clear();
        }

        [Button]
        public void SetDefaultState()
        {
            _treeState = TreeState.Default;
        }

        [Button]
        public void BuildMinimumSpanningTree()
        {
            _minimumSpanningTree = _graph.BuildMinimumSpanningTree();
            _treeState = TreeState.MST;
        }

        private void OnDrawGizmos()
        {
            if (_graph != null && _graph.AdjacencyCollection != null)
            {
                switch (_treeState)
                {
                    case TreeState.Default:
                        DrawEntireTree();
                        break;
                    case TreeState.MST:
                        DrawMST();
                        break;
                }
            }
        }

        private void DrawEntireTree()
        {
            foreach (KeyValuePair<Node, List<Edge>> connection in _graph.AdjacencyCollection)
            {
                foreach (Edge edge in connection.Value)
                {
                    if (edge != null && edge.Source != null && edge.Destination != null &&
                        edge.Destination.RectTransform != null)
                    {
                        Vector3 worldPosA = RectTransformUtility.PixelAdjustPoint(edge.Source.transform.position,
                            edge.Source.transform, null);
                        Vector3 worldPosB = RectTransformUtility.PixelAdjustPoint(edge.Destination.transform.position,
                            edge.Destination.transform, null);
                        Handles.color = new Color(0.0f, 0.0f, 1f, 0.5f);
                        Handles.DrawLine(worldPosA, worldPosB, 5.0f);

                        GUIStyle style = new GUIStyle();
                        style.normal.textColor = Color.black;
                        style.fontSize = 25;
                        style.alignment = TextAnchor.MiddleCenter;
                        Handles.Label((worldPosA + worldPosB) / 2,
                            Vector2.Distance(edge.Source.RectTransform.position, edge.Destination.RectTransform.position).ToString("F1"), style);
                    }
                }
            }
        }

        private void DrawMST()
        {
            if (_minimumSpanningTree == null)
                return;

            foreach (Node node in _graph.Nodes)
            {
                if (_minimumSpanningTree[node] != null)
                {
                    Node u = _minimumSpanningTree[node];

                    Vector3 worldPosA = RectTransformUtility.PixelAdjustPoint(u.transform.position, u.transform, null);
                    Vector3 worldPosB =
                        RectTransformUtility.PixelAdjustPoint(node.transform.position, node.transform, null);
                    Handles.color = Color.red;
                    Handles.DrawLine(worldPosA, worldPosB, 5.0f);
                    GUIStyle style = new GUIStyle();
                    style.normal.textColor = Color.black;
                    style.fontSize = 25;
                    style.alignment = TextAnchor.MiddleCenter;
                    Handles.Label((worldPosA + worldPosB) / 2,
                        Vector2.Distance(u.RectTransform.position, node.RectTransform.position).ToString("F1"), style);
                }
            }


        }
    }

    public enum TreeState
    {
        Default,
        MST,
        DFS,
        BFS
    }
}