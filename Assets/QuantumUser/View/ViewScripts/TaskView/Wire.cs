using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Wire : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private LineRenderer lineRenderer;
    private RectTransform wireIn;
    private RectTransform wireEnd;
    private Vector3 originalWireInPosition;
    private Vector3 originalPosition;
    private Vector2 originalSize;

    [Header("Wire Settings")]
    public GameObject wireManager;
    public GameObject wire;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        lineRenderer = GetComponent<LineRenderer>();
        wireEnd = gameObject.GetComponent<Image>().rectTransform;
        wireIn = wire.transform.Find("WireIn").GetComponent<Image>().rectTransform;
        originalWireInPosition = wireIn.localPosition;
        originalPosition = rectTransform.localPosition;
        originalSize = rectTransform.sizeDelta;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
    }

    public void OnDrag(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform.parent as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 mousePosition
        );
        rectTransform.localPosition = mousePosition;

        var direction = mousePosition - (Vector2)wire.transform.Find("WireStart").GetComponent<Image>().rectTransform.localPosition;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        float distance = direction.magnitude;

        wireEnd.sizeDelta = new Vector2(distance, wireEnd.sizeDelta.y);
        transform.rotation = Quaternion.Euler(0, 0, angle);
        wireIn.rotation = Quaternion.Euler(0, 0, angle);
        wireIn.localPosition = mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        WiresEnergyTask wireTask = wireManager.GetComponent<WiresEnergyTask>();
        GameObject closestRightWire = null;
        float minDistance = Mathf.Infinity;

        foreach (GameObject rightWire in wireTask.rightWires)
        {
            float distance = Vector3.Distance(rectTransform.position, rightWire.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestRightWire = rightWire;
            }
        }

        if (closestRightWire != null && minDistance <= wireTask.connectThreshold)
        {
            wireTask.ConnectWires(wire, closestRightWire);
        }
        else
        {
            ResetPosition();
        }
    }

    private void ResetPosition()
    {
        rectTransform.localPosition = originalPosition;
        rectTransform.sizeDelta = originalSize;
        transform.rotation = Quaternion.Euler(0, 0, 0);
        wireIn.rotation = Quaternion.Euler(0, 0, 0);
        wireIn.localPosition = originalWireInPosition;
    }
}
