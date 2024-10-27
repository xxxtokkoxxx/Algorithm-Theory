using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

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
        [SerializeField] private int _index;
        [FormerlySerializedAs("_cityButtonRef")] [SerializeField] private CityButton cityCityButtonRef;
        [SerializeField] private Transform _scrollTransform;
        [SerializeField] private TextMeshProUGUI _chosenCityText;

        private Dictionary<Node, Node> _minimumSpanningTree;
        private Dictionary<Node, List<Edge>> _shrinkedList;
        private HashSet<Node> _searchedNodes;
        private List<Edge> _sortedEdges;
        private Node _chosenCityNode;

        public IEnumerable GetNodes()
        {
            return _graph.Nodes;
        }

        private void Start()
        {
            foreach (Node node in _graph.Nodes)
            {
                CityButton cityButton = Instantiate(cityCityButtonRef, _scrollTransform);
                cityButton.Initialize(node.name, SetCity);
            }
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
            _shrinkedList = new Dictionary<Node, List<Edge>>();
            foreach (Node node in _graph.Nodes)
            {
                if (_minimumSpanningTree[node] != null)
                {
                    Node u = _minimumSpanningTree[node];

                    if (!_shrinkedList.ContainsKey(u))
                    {
                        _shrinkedList[u] = new List<Edge>();
                    }

                    if (!_shrinkedList.ContainsKey(node))
                    {
                        _shrinkedList[node] = new List<Edge>();
                    }

                    _shrinkedList[u].Add(new Edge(u, node));
                    _shrinkedList[node].Add(new Edge(node, u));
                }
            }
            _treeState = TreeState.MST;
        }

        [Button]
        public void SearchBfs()
        {
            Node node = _chosenCityNode == null ? _graph.Nodes[0] : _chosenCityNode;

            _sortedEdges = _graph.SearchBFS(node, _shrinkedList);
            _treeState = TreeState.BFS;
        }

        [Button]
        public void SearchDfs()
        {
            Node node = _chosenCityNode == null ? _graph.Nodes[0] : _chosenCityNode;

            _sortedEdges = _graph.SearchDFS(node, _shrinkedList);
            _treeState = TreeState.DFS;
        }

        private void OnDrawGizmos()
        {
            if (_graph != null && _graph.AdjacencyCollection != null)
            {
                switch (_treeState)
                {
                    case TreeState.Default:
                        DrawGizmo(_graph.AdjacencyCollection, Color.red);
                        break;
                    case TreeState.MST:
                        DrawGizmo(_shrinkedList, Color.yellow);
                        break;
                    case TreeState.DFS:
                        DrawEdges(_sortedEdges, Color.blue);
                        break;
                    case TreeState.BFS:
                        DrawEdges(_sortedEdges, Color.green);
                        break;
                }
            }
        }

        private void DrawGizmo(Dictionary<Node, List<Edge>> collection, Color color)
        {
            if (collection == null)
                return;

            foreach (KeyValuePair<Node, List<Edge>> connection in collection)
            {
                DrawEdges(connection.Value, color);
            }
        }

        private void DrawEdges(List<Edge> connection, Color color)
        {
            int index = 0;

            foreach (Edge edge in connection)
            {
                if(_index <= index)
                    continue;

                if (edge != null && edge.Source != null && edge.Destination != null &&
                    edge.Destination.RectTransform != null)
                {
                    index++;

                    Vector3 worldPosA = RectTransformUtility.PixelAdjustPoint(edge.Source.transform.position,
                        edge.Source.transform, null);
                    Vector3 worldPosB = RectTransformUtility.PixelAdjustPoint(edge.Destination.transform.position,
                        edge.Destination.transform, null);
                    Handles.color = color;
                    Handles.DrawLine(worldPosA, worldPosB, 7.0f);

                    GUIStyle style = new GUIStyle();
                    style.normal.textColor = Color.black;
                    style.fontSize = 25;
                    style.alignment = TextAnchor.MiddleCenter;
                    Handles.Label((worldPosA + worldPosB) / 2,
                        Vector2.Distance(edge.Source.RectTransform.position, edge.Destination.RectTransform.position).ToString("F1"), style);
                }
            }
        }

        private void SetCity(string cityName)
        {
            _chosenCityText.text = cityName;
            _chosenCityNode = _graph.Nodes.First(a => a.name == cityName);
        }
    }
}