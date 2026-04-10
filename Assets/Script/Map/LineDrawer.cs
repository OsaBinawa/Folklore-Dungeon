using UnityEngine;
using UnityEngine.UI;

public class LineDrawer : MonoBehaviour
{
    [SerializeField] private Transform lineContainer;
    [SerializeField] private Color lineColor = Color.white;
    [SerializeField] private float lineThickness = 5f;

    [SerializeField] private bool useCurve = false;
    [SerializeField] private float curveOffset = 100f; // controls how strong the bend is
    [SerializeField] private int curveResolution = 20; // smoothness

    public void Draw(MapRun run)
    {
        ClearLines();

        for (int depthIndex = 0; depthIndex < run.DeptNodes.Count; depthIndex++)
        {
            var depth = run.DeptNodes[depthIndex];

            foreach (var node in depth)
            {
                if (node.Button == null)
                    continue;

                // SPECIAL RULE: Start depth draws to ALL nodes in depth 1
                if (node.Depth == 0 && run.DeptNodes.Count > 1)
                {
                    foreach (var next in run.DeptNodes[1])
                    {
                        if (next.Button == null)
                            continue;

                        DrawLine(
                            node.Button.GetComponent<RectTransform>(),
                            next.Button.GetComponent<RectTransform>()
                        );
                    }

                    continue; // Skip normal NextNodes logic
                }

                // Normal connection drawing
                foreach (var next in node.NextNodes)
                {
                    if (next.Button == null)
                        continue;

                    DrawLine(
                        node.Button.GetComponent<RectTransform>(),
                        next.Button.GetComponent<RectTransform>()
                    );
                }
            }
        }
    }


    private void ClearLines()
    {
        foreach (Transform child in lineContainer)
        {
            Destroy(child.gameObject);
        }
    }

    private void DrawLine(RectTransform from, RectTransform to)
    {
        if (!useCurve)
        {
            DrawStraightLine(from, to);
        }
        else
        {
            DrawCurvedLine(from, to);
        }
    }

    private void DrawStraightLine(RectTransform from, RectTransform to)
    {
        GameObject lineObj = new GameObject("Line", typeof(Image));
        lineObj.transform.SetParent(lineContainer, false);

        Image img = lineObj.GetComponent<Image>();
        img.color = lineColor;
        img.sprite = Sprite.Create(Texture2D.whiteTexture, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f));

        RectTransform rt = lineObj.GetComponent<RectTransform>();

        Vector3 worldStart = from.TransformPoint(from.rect.center);
        Vector3 worldEnd = to.TransformPoint(to.rect.center);

        Vector2 start = lineContainer.InverseTransformPoint(worldStart);
        Vector2 end = lineContainer.InverseTransformPoint(worldEnd);

        Vector2 direction = end - start;
        float distance = direction.magnitude;

        if (distance < 0.1f) return;

        rt.sizeDelta = new Vector2(distance, lineThickness);
        rt.pivot = new Vector2(0, 0.5f);
        rt.anchoredPosition = start;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        rt.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void DrawCurvedLine(RectTransform from, RectTransform to)
    {
        Vector3 worldStart = from.TransformPoint(from.rect.center);
        Vector3 worldEnd = to.TransformPoint(to.rect.center);

        Vector2 start = lineContainer.InverseTransformPoint(worldStart);
        Vector2 end = lineContainer.InverseTransformPoint(worldEnd);

        // Create control points (this makes the S shape)
        Vector2 mid = (start + end) / 2f;

        Vector2 control1 = new Vector2(mid.x, start.y + curveOffset);
        Vector2 control2 = new Vector2(mid.x, end.y - curveOffset);

        Vector2 previousPoint = start;

        for (int i = 1; i <= curveResolution; i++)
        {
            float t = i / (float)curveResolution;

            Vector2 point = GetCubicBezierPoint(t, start, control1, control2, end);

            DrawSegment(previousPoint, point);

            previousPoint = point;
        }
    }
    private Vector2 GetCubicBezierPoint(float t, Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3)
    {
        float u = 1 - t;

        return u * u * u * p0 +
               3 * u * u * t * p1 +
               3 * u * t * t * p2 +
               t * t * t * p3;
    }

    private void DrawSegment(Vector2 start, Vector2 end)
    {
        GameObject lineObj = new GameObject("CurveSegment", typeof(Image));
        lineObj.transform.SetParent(lineContainer, false);

        Image img = lineObj.GetComponent<Image>();
        img.color = lineColor;
        img.sprite = Sprite.Create(Texture2D.whiteTexture, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f));

        RectTransform rt = lineObj.GetComponent<RectTransform>();

        Vector2 direction = end - start;
        float distance = direction.magnitude;

        if (distance < 0.1f) return;

        rt.sizeDelta = new Vector2(distance, lineThickness);
        rt.pivot = new Vector2(0, 0.5f);
        rt.anchoredPosition = start;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        rt.rotation = Quaternion.Euler(0, 0, angle);
    }

    /*private void DrawLine(RectTransform from, RectTransform to)
    {
        GameObject lineObj = new GameObject("Line", typeof(Image));
        lineObj.transform.SetParent(lineContainer, false);

        Image img = lineObj.GetComponent<Image>();
        img.color = lineColor;

        // IMPORTANT: give the image a visible sprite
        img.sprite = Sprite.Create(
            Texture2D.whiteTexture,
            new Rect(0, 0, 1, 1),
            new Vector2(0.5f, 0.5f)
        );

        RectTransform rt = lineObj.GetComponent<RectTransform>();

        // Convert positions to SAME SPACE
        Vector3 worldStart = from.TransformPoint(from.rect.center);
        Vector3 worldEnd = to.TransformPoint(to.rect.center);

        Vector2 start = lineContainer.InverseTransformPoint(worldStart);
        Vector2 end = lineContainer.InverseTransformPoint(worldEnd);

        Vector2 direction = end - start;
        float distance = direction.magnitude;

        if (distance < 0.1f) return;

        rt.sizeDelta = new Vector2(distance, lineThickness);
        rt.pivot = new Vector2(0, 0.5f);
        rt.anchoredPosition = start;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        rt.rotation = Quaternion.Euler(0, 0, angle);

        // Optional: ensure visibility order
        rt.SetAsFirstSibling(); // or SetAsLastSibling()
    }*/
}
