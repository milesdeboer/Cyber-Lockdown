using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;

public class Checkbox : MonoBehaviour
{
    [SerializeField]
    private Color selectedColor;
    [SerializeField]
    private Color unselectedColor;

    private List<GameObject> buttons;

    public void Start() {
        if (!IsInitialized()) UpdateDisplay();
        foreach(GameObject button in buttons) {
            button.GetComponent<Button>().onClick.AddListener(delegate {
                OnClick(button);
            });
        }
    }

    public void OnClick(GameObject self) {
        buttons
            .Where(b => b.name == self.name)
            .ToList()
            .ForEach(b => {
                b.GetComponent<Image>().color = (b.GetComponent<Image>().color.Equals(selectedColor)) ? unselectedColor : selectedColor;
            });
    }

    public void UpdateDisplay() {
        buttons = new List<GameObject>();
        foreach(Transform child_ in transform) {
            GameObject child = child_.gameObject;
            buttons.Add(child);
            child.GetComponent<Image>().color = unselectedColor;
        }
    }

    public bool IsInitialized() {
        return buttons != null;
    }
}
