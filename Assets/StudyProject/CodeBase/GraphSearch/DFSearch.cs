using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace StudyProject.CodeBase
{
    public class DFSearch : MonoBehaviour
    {
        [SerializeField] private List<Node> _nodes = new();

        public void Search(Node node)
        {
            bool[] visited = new bool[_nodes.Count];
            LinkedList<Node> nodes = new LinkedList<Node>();

            nodes.AddLast(node);

            while (nodes.Any())
            {
                Node current = nodes.LastOrDefault();

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
        }
    }
}