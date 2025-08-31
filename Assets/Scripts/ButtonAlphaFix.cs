using UnityEngine;
using UnityEngine.UI;

public class ButtonAlphaFix : MonoBehaviour
{
    void Start()
    {
        Image img = GetComponent<Image>();
        if (img != null)
        {
            img.alphaHitTestMinimumThreshold = 0.5f;
            Debug.Log($"{gameObject.name} : alphaHitTestMinimumThreshold 적용됨");
        }
    }
}
