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
            PrintMST(parent);
            return parent;
        }

        private void PrintMST(Dictionary<Node, Node> parent)
        {
            foreach (var node in _nodes)
            {
                if (parent[node] != null)
                {
                    Node u = parent[node];
                    double weight = _adjacencyCollection[u].Find(e => e.Source == node || e.Destination == node).Weight;
                    Debug.Log($"{u} - {node} \t{weight}");
                }
            }
        }

        public void BFS(Node node, Dictionary<Node, List<Edge>> adjasentCollection)
        {
            HashSet<Node> visited = new HashSet<Node>();
            Queue<Node> nodes = new Queue<Node>();

            nodes.Enqueue(node);
            visited.Add(node);

            while (nodes.Any())
            {
                Node current = nodes.Dequeue();
                foreach (Edge connection in adjasentCollection[current])
                {
                    if (visited.Contains(connection.Destination))
                    {
                        continue;
                    }

                    Debug.Log("connected node " + connection.Destination.name);
                    nodes.Enqueue(connection.Destination);
                    visited.Add(connection.Destination);
                }
            }
        }

        // public void DFS(Node node, Dictionary<Node, List<Edge>> adjasentCollection)
        // {
        //     HashSet<Node> visited = new HashSet<Node>();
        //     Stack<Node> nodes = new Stack<Node>();
        //
        //     nodes.Push(node);
        //
        //     while (nodes.Any())
        //     {
        //         Node current = nodes.Pop();
        //
        //         if (!visited.Contains(current))
        //         {
        //             visited.Add(current);
        //
        //             foreach (Edge connection in adjasentCollection[current])
        //             {
        //                 if (!visited.Contains(connection.Destination))
        //                 {
        //                     Debug.Log("connected node " + connection.Destination.name);
        //                     nodes.Push(connection.Destination);
        //                 }
        //             }
        //         }
        //     }
        // }

        private void DFSUtil(Node v, HashSet<Node> visited, Dictionary<Node, List<Edge>> adjasentCollectio)
        {
            visited.Add(v);
            Debug.Log(v + " ");

            foreach (Edge edge in adjasentCollectio[v])
            {
                if (!visited.Contains(edge.Destination))
                {
                    DFSUtil(edge.Destination, visited,adjasentCollectio);
                }
            }
        }

        public void DFS(Node startNode, Dictionary<Node, List<Edge>> adjasentCollectio)
        {
            HashSet<Node> visited = new HashSet<Node>();

            DFSUtil(startNode, visited, adjasentCollectio);
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