using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

//prevents text from overlapping outside the text box
public class SCR_FitText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    IEnumerator ReduceFontSize()
    {
        while (CheckTextHeight())
        {
            Debug.Log("Reduce font size");
            text.fontSize--;
            yield return null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(text.text.IsNormalized());
        StartCoroutine(ReduceFontSize());
    }

    bool CheckTextHeight()
    {
        float textHeight = LayoutUtility.GetPreferredHeight(text.rectTransform);

        float textBoxHeight = GetComponent<RectTransform>().rect.height;

        return textHeight > textBoxHeight;
    }
}
