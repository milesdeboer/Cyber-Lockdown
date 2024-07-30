using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;

public class Checkbox : MonoBehaviour
{
    [SerializeField]
    private Sprite selected;
    [SerializeField]
    private Sprite unselected;

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
                b.GetComponent<Image>().sprite = (b.GetComponent<Image>().sprite.Equals(selected)) ? unselected : selected;
            });
    }

    public void UpdateDisplay() {
        buttons = new List<GameObject>();
        foreach(Transform child_ in transform) {
            GameObject child = child_.gameObject;
            buttons.Add(child);
            child.GetComponent<Image>().sprite = unselected;
        }
    }

    public bool IsInitialized() {
        return buttons != null;
    }
}
