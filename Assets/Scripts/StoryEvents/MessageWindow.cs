﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class MessageWindow : MonoSingleton<MessageWindow> 
{
    private List<StoryMessage> messages;
    private int index = 0;
    private bool complete = false;
    private GraphicRaycaster graphicRaycaster;

    private float alpha=0;
    private float targetAlpha=0;
    public Text nameLabel;
    public Text messageLabel;
	public Image profilePic;
	

    void Start()
    {
        graphicRaycaster = transform.parent.GetComponent<GraphicRaycaster>();
        graphicRaycaster.enabled = false;
    }

    void Update()
    {
        alpha = Mathf.MoveTowards(alpha, targetAlpha, 0.1f);
        GetComponent<CanvasGroup>().alpha = alpha;

        if (graphicRaycaster.enabled == false && alpha == 1)
        {
            graphicRaycaster.enabled = true;
        }
    }

    public void StartMessages(List<StoryMessage> m,bool c)
    {
        complete = c;
        Map.instance.Pause = true;
        index = 0;
        messages = m;
        ShowMessage();
        targetAlpha = 1.0f;
    }

    public void NextMessage()
    {
        if (index + 1 >= messages.Count)
        {
            graphicRaycaster.enabled = false;
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
		if (messages [index].character.Contains ("_")) 
		{
			profilePic.gameObject.SetActive(true);
			string profilePath = "Profiles/" + messages [index].character;
			profilePic.sprite = Resources.Load<Sprite> (profilePath);
			nameLabel.text = char.ToUpper(messages[index].character[0]) + messages [index].character.Substring (1, messages [index].character.IndexOf ('_')-1);
		} 
		else 
		{
			profilePic.gameObject.SetActive(false);
			nameLabel.text = messages [index].character;
		}
        messageLabel.text = messages[index].message;
    }
}
