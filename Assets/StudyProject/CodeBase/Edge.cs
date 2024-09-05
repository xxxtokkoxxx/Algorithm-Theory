using System;
using UnityEngine;

namespace StudyProject.CodeBase
{
    [Serializable]
    public class Edge
    {
        public Node Source;
        public Node Destination;

        public readonly float Weight;

        public Edge(Node source, Node destination)
        {
            Source = source;
            Destination = destination;
            Weight = Vector2.Distance(source.RectTransform.position, destination.RectTransform.position);
        }
    }
}