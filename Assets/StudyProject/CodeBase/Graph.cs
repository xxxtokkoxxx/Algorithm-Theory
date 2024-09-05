using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace StudyProject.CodeBase
{
    public class Graph : SerializedMonoBehaviour
    {
        [SerializeField] private Dictionary<Node, List<Edge>> _adjacencyCollection = new();
        [SerializeField] private List<Node> _nodes;
        public Dictionary<Node, List<Edge>> AdjacencyCollection => _adjacencyCollection;
        public List<Node> Nodes => _nodes;

        public void AddEdge(Node source, Node destination)
        {
            if (source == destination)
                return;

            if (source == null || destination == null)
                return;

            Edge edge = new Edge(source, destination);

            if (_adjacencyCollection[source].Contains(edge))
            {
                return;
            }

            _adjacencyCollection[source].Add(edge);
            _adjacencyCollection[destination].Add(edge);
        }

        public Dictionary<Node, Node> BuildMinimumSpanningTree()
        {
            HashSet<Node> inTree = new HashSet<Node>();
            Dictionary<Node, double> key = new Dictionary<Node, double>();
            Dictionary<Node, Node> parent = new Dictionary<Node, Node>();

            foreach (Node node in _nodes)
            {
                key[node] = double.MaxValue;
                parent[node] = null;
            }

            Node startNode = _nodes[0];
            key[startNode] = 0;

            while (inTree.Count < _nodes.Count)
            {
                Node u = MinKey(key, inTree);
                inTree.Add(u);

                foreach (Edge edge in _adjacencyCollection[u])
                {
                    Node v = edge.Source == u ? edge.Destination : edge.Source;
                    if (!inTree.Contains(v) && edge.Weight < key[v])
                    {
                        key[v] = edge.Weight;
                        parent[v] = u;
                    }
                }
            }

            return parent;
        }

        public void DFS(Node no)
        {
            // HashSet<Node> visited = new bool[_nodes.Count];
            // Stack<Node> nodes = new Stack<Node>();
            //
            // nodes.Push(node);
            //
            // Node current = nodes.LastOrDefault();
            //
            // foreach (Edge connection in current.Connections)
            // {
            //     if (visited[connection.Destination.Index])
            //     {
            //         continue;
            //     }
            //
            //     Debug.Log("connected node " + connection.Destination.name);
            //     nodes.AddLast(connection.Destination);
            //     visited[connection.Destination.Index] = true;
            // }
        }

        private Node MinKey(Dictionary<Node, double> key, HashSet<Node> insideTree)
        {
            double min = double.MaxValue;
            Node minNode = null;

            foreach (var node in _nodes)
            {
                if (!insideTree.Contains(node) && key[node] < min)
                {
                    min = key[node];
                    minNode = node;
                }
            }

            return minNode;
        }
    }
}