using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class RadioButton : MonoBehaviour
{
    [SerializeField]
    private Color selectedColor;
    [SerializeField]
    private Color unselectedColor;

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

    public void OnClick(GameObject self) {
        buttons
            .Where(b => {
                b.GetComponent<Image>().color = unselectedColor;
                return b.name == self.name;
            })
            .ToList()
            .ForEach(b => {
                b.GetComponent<Image>().color = selectedColor;
            });
    }
}
