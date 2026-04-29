using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaitText : MonoBehaviour
{
    [SerializeField] private float waitTime;
    [SerializeField] private List<string> waitTexts;

    private float waitTimeCount;
    private int textIndex;

    private Text text;

    private void Start()
    {
        text = GetComponent<Text>();
    }

    void Update()
    {
        waitTimeCount += Time.deltaTime;
        if (waitTimeCount >= waitTime)
        {
            waitTimeCount = 0;
            int index = textIndex + 1 >= waitTexts.Count ? 0 : textIndex + 1;
            textIndex = index;
            text.text = waitTexts[textIndex];
        }
    }
}
