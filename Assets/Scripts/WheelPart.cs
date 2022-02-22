using TMPro;
using UnityEngine;

public class WheelPart : MonoBehaviour
{
    public TextMeshPro textMesh;
    public TextMeshPro textMeshEuros;

    public void SetValueEuros(string t, bool isJackpot)
    {
        SetValue(t, textMeshEuros, isJackpot ? Color.yellow : Color.black);
    }

    public void SetValueWheelValue(string t)
    {
        SetValue(t, textMesh, Color.black);
    }

    private void SetValue(string t, TextMeshPro label, Color color)
    {
        label.color = color;
        label.SetText(t);
    }
}