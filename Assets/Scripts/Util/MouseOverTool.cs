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
    private Vector2 location;

    [SerializeField]
    private string title;

    [SerializeField]
    [TextArea(15, 5)]
    private string flavor;

    [SerializeField]
    [TextArea(15, 5)]
    private string effect;

    private GameObject instantiatedObject;

    public void OnPointerEnter(PointerEventData eventData) {
        instantiatedObject = Instantiate(infoSlide, location, Quaternion.identity);
        instantiatedObject.GetComponent<Transform>().localScale = new Vector2(10, 10);

        instantiatedObject.transform.SetParent(canvas.transform, false);

        instantiatedObject.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().SetText(title);
        instantiatedObject.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().SetText(flavor);
        instantiatedObject.transform.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().SetText(effect);
    }

    public void OnPointerExit(PointerEventData eventData) {
        if (instantiatedObject != null) Destroy(instantiatedObject);
    }
}
