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

        public List<Edge> SearchBFS(Node startNode, Dictionary<Node, List<Edge>> adjacencyCollection)
        {
            HashSet<Node> visited = new HashSet<Node>();
            Queue<Node> nodes = new Queue<Node>();
            List<Edge> sortedAdjacencyCollection = new List<Edge>();

            visited.Add(startNode);
            nodes.Enqueue(startNode);

            while (nodes.Count > 0)
            {
                Node current = nodes.Dequeue();

                foreach (Edge edge in adjacencyCollection[current])
                {
                    if (!visited.Contains(edge.Destination))
                    {
                        visited.Add(edge.Destination);
                        nodes.Enqueue(edge.Destination);
                        sortedAdjacencyCollection.Add(edge);
                    }
                }
            }

            return sortedAdjacencyCollection;
        }

        public List<Edge> SearchDFS(Node startNode, Dictionary<Node, List<Edge>> adjacencyCollection)
        {
            HashSet<Node> visited = new HashSet<Node>();
            List<Edge> connections = new List<Edge>();

            SearchDFSRecurcevly(startNode, visited, adjacencyCollection, connections);
            return connections;
        }

        private void SearchDFSRecurcevly(Node node, HashSet<Node> visited,
            Dictionary<Node, List<Edge>> adjacencyCollection, List<Edge> connections)
        {
            visited.Add(node);

            foreach (Edge edge in adjacencyCollection[node])
            {
                if (!visited.Contains(edge.Destination))
                {
                    connections.Add(new Edge(node, edge.Destination));
                    SearchDFSRecurcevly(edge.Destination, visited, adjacencyCollection, connections);
                }
            }
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