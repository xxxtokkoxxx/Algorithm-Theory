using UnityEngine;

namespace StudyProject.CodeBase
{
    public class UILineRenderer : MonoBehaviour
    {
        [SerializeField] private LineRenderer _lineRenderer;

        public void DrawLineBetweenUIElements(RectTransform start, RectTransform end)
        {
            _lineRenderer.positionCount = 2;

            Vector3 worldPosition1 = RectTransformUtility.WorldToScreenPoint(Camera.main, start.position);
            Vector3 worldPosition2 = RectTransformUtility.WorldToScreenPoint(Camera.main, end.position);

            Vector3 lineWorldPosition1 = Camera.main.ScreenToWorldPoint(new Vector3(worldPosition1.x, worldPosition1.y, -Camera.main.transform.position.z));
            Vector3 lineWorldPosition2 = Camera.main.ScreenToWorldPoint(new Vector3(worldPosition2.x, worldPosition2.y, -Camera.main.transform.position.z));

            _lineRenderer.SetPosition(0, lineWorldPosition1);
            _lineRenderer.SetPosition(1, lineWorldPosition2);

            _lineRenderer.startWidth = 0.050f;
            _lineRenderer.endWidth = 0.050f;
            _lineRenderer.startColor = Color.red;
            _lineRenderer.endColor = Color.red;
        }

        public void Clear()
        {
            _lineRenderer.positionCount = 0;
        }


        public void HighLight(bool isHighlighted)
        {
            throw new System.NotImplementedException();
        }
    }
}