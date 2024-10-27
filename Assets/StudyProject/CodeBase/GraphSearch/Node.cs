using UnityEngine;

namespace StudyProject.CodeBase
{
    [RequireComponent(typeof(RectTransform))]
    public class Node : MonoBehaviour
    {
        public string Name;
        public RectTransform RectTransform;

        private void Start()
        {
            if (RectTransform == null)
                RectTransform = GetComponent<RectTransform>();
        }
    }
}