using UnityEngine;
using UnityEngine.UI;

public class LineDrawer : MonoBehaviour
{
    [SerializeField] private Transform lineContainer;
    [SerializeField] private Color lineColor = Color.white;
    [SerializeField] private float lineThickness = 5f;

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
        GameObject lineObj = new GameObject("Line", typeof(Image));
        lineObj.transform.SetParent(lineContainer, false);

        Image img = lineObj.GetComponent<Image>();
        img.color = lineColor;

        RectTransform rt = lineObj.GetComponent<RectTransform>();

        Vector2 startPos = from.anchoredPosition;
        Vector2 endPos = to.anchoredPosition;

        Vector2 direction = endPos - startPos;
        float distance = direction.magnitude;

        rt.sizeDelta = new Vector2(distance, lineThickness);
        rt.pivot = new Vector2(0, 0.5f);
        rt.anchoredPosition = startPos;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        rt.rotation = Quaternion.Euler(0, 0, angle);
    }
}
