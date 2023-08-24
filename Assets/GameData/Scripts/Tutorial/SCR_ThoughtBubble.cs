using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SCR_ThoughtBubble : MonoBehaviour
{
    [SerializeField] private GameObject smallCloud;
    [SerializeField] private GameObject mediumCloud;
    [SerializeField] private GameObject largeCloud;

    [SerializeField] private TextMeshProUGUI prompt;

    [SerializeField] private float growSpeed = 2.0f;
    [SerializeField] private float textSpeed;

    [HideInInspector] public bool isRunning = false;
    private List<Instruction> instructions = new List<Instruction>();

    public IEnumerator DisplayText(string text, float displayTime)
    {
        gameObject.SetActive(true);
        isRunning = true;

        Vector3 smallScale = Vector3.one * 0.1f;
        smallCloud.transform.localScale = smallScale;
        mediumCloud.transform.localScale = smallScale;
        largeCloud.transform.localScale = smallScale;

        while (largeCloud.transform.localScale.y < 0.99f)
        {
            smallCloud.transform.localScale = Vector3.Lerp(smallCloud.transform.localScale, Vector3.one, growSpeed * Time.deltaTime);
            mediumCloud.transform.localScale = Vector3.Lerp(mediumCloud.transform.localScale, Vector3.one, growSpeed * Time.deltaTime);
            largeCloud.transform.localScale = Vector3.Lerp(largeCloud.transform.localScale, Vector3.one, growSpeed * Time.deltaTime);
            yield return null;
        }

        float currentCharacter = 0;
        while (prompt.text.Length != text.Length)
        {
            currentCharacter += Time.deltaTime * textSpeed;
            //currentCharacter = Mathf.Lerp(currentCharacter, text.Length, Time.deltaTime * textSpeed);
            if (text.Length - currentCharacter < 2.0f)
            {
                currentCharacter = text.Length;
            }

            prompt.text = text.Substring(0, (int)currentCharacter);
            yield return null;
        }

        Debug.Log("Finished Text");

        yield return new WaitForSecondsRealtime(displayTime);
        isRunning = false;
        prompt.text = "";
        gameObject.SetActive(false);

        if (instructions.Count > 0)
        {
            instructions[0].PlayInstruction(this);
            instructions.RemoveAt(0);
        }
    }

    public void ResetThoughtBubble()
    {
        StopAllCoroutines();
        smallCloud.transform.localScale = Vector3.one;
        mediumCloud.transform.localScale = Vector3.one;
        largeCloud.transform.localScale = Vector3.one;
        prompt.text = "";
        isRunning = false;
        gameObject.SetActive(false);
    }

    public void QueueThoughtBubble(string displayText, float displayTime)
    {
        Instruction newInstruction = new Instruction();
        newInstruction.displayText = displayText;
        newInstruction.displayTime = displayTime;

        instructions.Add(newInstruction);
    }

    private void Update()
    {
        transform.parent.LookAt(Camera.main.transform, Vector3.up);
    }
}

struct Instruction
{
    public string displayText;
    public float displayTime;

    public void PlayInstruction(SCR_ThoughtBubble bubble)
    {
        bubble.gameObject.SetActive(true);
        bubble.StartCoroutine(bubble.DisplayText(displayText, displayTime));
    }
}
