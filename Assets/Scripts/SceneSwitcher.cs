using UnityEngine;
using System.Collections;

public class SceneSwitcher : MonoBehaviour
{
    private int scene=-1;
    private CanvasGroup group;

    private float alpha = 1.0f;
    private float targetAlpha = 0.0f;

	// Use this for initialization
	void Start () 
    {
        group = GetComponent<CanvasGroup>();
        group.alpha = 1.0f;
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (Application.isLoadingLevel)
        {
            return;
        }

        alpha = Mathf.MoveTowards(alpha, targetAlpha, Time.deltaTime);
        group.alpha = alpha;

        if (alpha == 1 && scene != -1)
        {
            Application.LoadLevel(scene);
        }
	}

    public static void ChangeScene(int i)
    {
        SceneSwitcher switcher = FindObjectOfType<SceneSwitcher>();
        if (switcher != null)
        {
            switcher.SwitchScene(i);
        }
        else
        {
            Application.LoadLevel(i);
        }
    }

    public void SwitchScene(int i)
    {
        scene = i;
        targetAlpha = 1.0f;
    }
}
