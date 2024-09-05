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

        public Dictionary<Node, List<Edge>> BuildMinimumSpanningTree()
        {
            HashSet<Node> inTree = new HashSet<Node>();
            Dictionary<Node, double> key = new Dictionary<Node, double>();
            Dictionary<Node, List<Edge>> parent = new Dictionary<Node, List<Edge>>();

            foreach (Node node in _nodes)
            {
                key[node] = double.MaxValue;
                parent[node] = new List<Edge>();
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
                        parent[v].Add(new Edge(v,u));;
                    }
                }
            }

            return parent;
        }

        public void DFS(Node node)
        {
            HashSet<Node> visited = new HashSet<Node>();
            Stack<Node> nodes = new Stack<Node>();

            nodes.Push(node);

            while (nodes.Any())
            {
                Node current = nodes.Pop();

                foreach (Edge connection in _adjacencyCollection[current])
                {
                    if (visited.Contains(connection.Destination))
                    {
                        continue;
                    }

                    Debug.Log("connected node " + connection.Destination.name);
                    nodes.Push(connection.Destination);
                    visited.Add(connection.Destination);
                }
            }
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