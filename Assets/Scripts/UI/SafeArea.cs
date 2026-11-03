using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class SafeArea : MonoBehaviour
{
    private RectTransform _rect;
    private Rect _lastSafeArea = new Rect(0, 0, 0, 0);
    private ScreenOrientation _lastOrientation = ScreenOrientation.AutoRotation;

    private void Awake()
    {
        _rect = GetComponent<RectTransform>();
        Apply();
    }

    private void Update()
    {
        // (rotation, foldables, editor resize).
        if (Screen.safeArea != _lastSafeArea || Screen.orientation != _lastOrientation)
        {
            Apply();
        }
    }

    private void Apply()
    {
        Rect safeArea = Screen.safeArea;
        _lastSafeArea = safeArea;
        _lastOrientation = Screen.orientation;

        if (Screen.width <= 0 || Screen.height <= 0) return;

        Vector2 anchorMin = safeArea.position;
        Vector2 anchorMax = safeArea.position + safeArea.size;

        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;

        _rect.anchorMin = anchorMin;
        _rect.anchorMax = anchorMax;
    }
}
