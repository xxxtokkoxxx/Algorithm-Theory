using UnityEngine;

namespace StudyProject.CodeBase
{
    [RequireComponent(typeof(RectTransform))]
    public class Node : MonoBehaviour
    {
        public string Name;
        public RectTransform RectTransform;

        [SerializeField] private UILineRenderer _lineRenderer;
        private UILineRenderer LineRenderer => _lineRenderer;

        private void Start()
        {
            if (RectTransform == null)
                RectTransform = GetComponent<RectTransform>();
        }

        public void HighLight(bool isHighlighted)
        {
            if (_lineRenderer == null)
                return;

            _lineRenderer.HighLight(isHighlighted);
        }
    }
}