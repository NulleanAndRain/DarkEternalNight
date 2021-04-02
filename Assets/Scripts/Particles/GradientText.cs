using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GradientText : BaseMeshEffect {

    public Color ColorLeft;
    public Color ColorMid;
    public Color ColorRight;

    public override void ModifyMesh(VertexHelper vh) {
        if (!this.IsActive())
            return;
        List<UIVertex> vertexList = new List<UIVertex>();
        vh.GetUIVertexStream(vertexList);
        ModifyVertices(vertexList);
        vh.Clear();
        vh.AddUIVertexTriangleStream(vertexList);
    }
    public void ModifyVertices(List<UIVertex> vertexList) {
        int count = vertexList.Count;
        float left  = vertexList[0].position.x;
        float right = vertexList[0].position.x;

        for (int i = 1; i < count; i++) {
            float x = vertexList[i].position.x;
            if (x > right) {
                right = x;
            } else if (x < left) {
                left = x;
            }
        }
        float width = right - left;
        float mid = left + width / 2;
        float halfwidth = width / 2;
        for (int i = 0; i < count; i++) {
            UIVertex v = vertexList[i];

            if(v.position.x < mid) {
                v.color = Color.Lerp(ColorLeft, ColorMid, (v.position.x - left) / halfwidth) * v.color;
			} else {
                v.color = Color.Lerp(ColorMid, ColorRight, (v.position.x - mid) / halfwidth) * v.color;

            }
            vertexList[i] = v;
        }
    }
}
