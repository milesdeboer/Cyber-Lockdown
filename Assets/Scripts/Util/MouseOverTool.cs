using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class MouseOverTool : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private GameObject infoSlide;
    [SerializeField]
    private Canvas canvas;
    [SerializeField]
    private string title;

    [SerializeField]
    [TextArea(15, 10)]
    private string flavor;
    [SerializeField]
    [TextArea(15, 10)]
    private string effect;

    private GameObject instantiatedObject;

    public void OnPointerEnter(PointerEventData eventData) {
        Thread.Sleep(100);
        Vector2 coords = Input.mousePosition;
        coords.x += 10 * infoSlide.GetComponent<RectTransform>().sizeDelta.x / 2;
        coords.y -= 10 * infoSlide.GetComponent<RectTransform>().sizeDelta.y / 2;

        instantiatedObject = Instantiate(infoSlide, coords, Quaternion.identity);
        instantiatedObject.GetComponent<Transform>().localScale = new Vector2(10, 10);

        instantiatedObject.transform.SetParent(canvas.transform, true);

        instantiatedObject.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().SetText(title);
        instantiatedObject.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().SetText(flavor);
        instantiatedObject.transform.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().SetText(effect);
    }

    public void OnPointerExit(PointerEventData eventData) {
        Thread.Sleep(1000);
        if (instantiatedObject != null) Destroy(instantiatedObject);
    }
}
