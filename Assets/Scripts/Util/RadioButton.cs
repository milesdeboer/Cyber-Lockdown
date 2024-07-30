using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class RadioButton : MonoBehaviour
{
    [SerializeField]
    private Sprite selected;
    [SerializeField]
    private Sprite unselected;

    [SerializeField]
    private Material iconMat;

    private List<GameObject> buttons;

    public void Start() {
        buttons = new List<GameObject>();
        foreach(Transform child_ in transform) {
            GameObject child = child_.gameObject;
            buttons.Add(child);
            child.GetComponent<Button>().onClick.AddListener(delegate {
                OnClick(child);
            });
        }
    }

    public void OnClick(GameObject self) {// if passed null, unselect all
        Debug.Log("Running OnClick");
        buttons
            .Where(b => {
                b.GetComponent<Image>().sprite = unselected;
                if (b.transform.childCount > 0) {
                    Image image = b.transform.GetChild(0).gameObject.GetComponent<Image>();
                    if (image != null) image.material = iconMat;
                }
                return (self != null) ? b.name == self.name : false;
            })
            .ToList()
            .ForEach(b => {
                b.GetComponent<Image>().sprite = selected;
                if (b.transform.childCount > 0) {
                    Image image = b.transform.GetChild(0).gameObject.GetComponent<Image>();
                    if (image != null) image.material = null;
                }
            });
    }

}
