using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// On-screen joystick for mobile: drag inside the background circle, the
/// handle follows (clamped to handleRange), and Horizontal/Vertical report
/// back values in -1..1. PlayerController2D reads Horizontal every
/// FixedUpdate. Put this on the joystick's background Image; it needs
/// an Image raycast target so touches land on it.
/// </summary>
public class VirtualJoystick : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private RectTransform background;
    [SerializeField] private RectTransform handle;
    [Tooltip("How far (in UI pixels) the handle can move from the center.")]
    [SerializeField] private float handleRange = 70f;

    public float Horizontal { get; private set; }
    public float Vertical { get; private set; }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            background, eventData.position, eventData.pressEventCamera, out pos);

        pos = Vector2.ClampMagnitude(pos, handleRange);
        if (handle != null) handle.anchoredPosition = pos;

        Horizontal = pos.x / handleRange;
        Vertical = pos.y / handleRange;
    }

    public void OnPointerDown(PointerEventData eventData) => OnDrag(eventData);

    public void OnPointerUp(PointerEventData eventData)
    {
        if (handle != null) handle.anchoredPosition = Vector2.zero;
        Horizontal = 0f;
        Vertical = 0f;
    }
}
