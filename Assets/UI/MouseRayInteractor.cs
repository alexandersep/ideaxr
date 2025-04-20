using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MouseRayInteractor : MonoBehaviour
{
    public GraphicRaycaster raycaster;
    public EventSystem eventSystem;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Mouse click detected");
            PointerEventData pointerData = new PointerEventData(eventSystem)
            {
                position = Input.mousePosition
            };

            var results = new List<RaycastResult>();
            raycaster.Raycast(pointerData, results);

            foreach (RaycastResult result in results)
            {
                Button button = result.gameObject.GetComponent<Button>();
                if (button != null)
                {
                    button.onClick.Invoke();
                    return;
                }

                Slider slider = result.gameObject.GetComponent<Slider>();
                if (slider != null)
                {
                    Vector2 localPoint;
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(
                        slider.GetComponent<RectTransform>(),
                        Input.mousePosition,
                        null,
                        out localPoint
                    );

                    float pct = Mathf.InverseLerp(
                        slider.GetComponent<RectTransform>().rect.xMin,
                        slider.GetComponent<RectTransform>().rect.xMax,
                        localPoint.x
                    );

                    slider.value = Mathf.Lerp(slider.minValue, slider.maxValue, pct);
                    return;
                }
            }
        }
    }
}
