using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class MessageWindow : MonoSingleton<MessageWindow> 
{
    private List<StoryMessage> messages;
    private int index = 0;
    private bool complete = false;
    private Button button;

    private float alpha=0;
    private float targetAlpha=0;
    public Text nameLabel;
    public Text messageLabel;

    void Start()
    {
        button = GetComponent<Button>();
        button.enabled = false;
    }

    void Update()
    {
        alpha = Mathf.MoveTowards(alpha, targetAlpha, 0.1f);
        GetComponent<CanvasGroup>().alpha = alpha;
    }

    public void StartMessages(List<StoryMessage> m,bool c)
    {
        complete = c;
        Map.instance.Pause = true;
        index = 0;
        messages = m;
        ShowMessage();
        targetAlpha = 1.0f;
        button.enabled = true;
    }

    public void NextMessage()
    {
        if (index + 1 >= messages.Count)
        {
            button.enabled = false;
            targetAlpha = 0.0f;
            Map.instance.Pause = false;
            if (complete)
            {
                Map.instance.CompleteLevel();
            }
        }
        else
        {
            index++;
            ShowMessage();
        }
    }

    public void ShowMessage()
    {
        nameLabel.text = messages[index].character;
        messageLabel.text = messages[index].message;
    }
}
