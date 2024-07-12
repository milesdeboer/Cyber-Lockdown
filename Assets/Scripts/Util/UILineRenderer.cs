using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILineRenderer : MaskableGraphic
{
    [SerializeField]
    private Vector2[] points;

    protected override void OnFillVBO(List<UIVertex> vbo) {

    }
}
