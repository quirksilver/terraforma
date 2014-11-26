using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BuildingControl : MonoBehaviour 
{
    public CanvasGroup canvasGroup;
    public Text nameLabel;
    private Building building;

    public Tile touchStart;

    float alpha = 0;
    float targetAlpha = 0;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        alpha = Mathf.MoveTowards(alpha, targetAlpha, 0.1f);
        canvasGroup.alpha = alpha;

        if (Input.GetMouseButtonDown(0))
        {
            touchStart = Map.instance.GetTileOver();
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (Map.instance.GetTileOver() == touchStart)
            {
                if (Map.instance.GetTileOver().building != null)
                {
                    building = Map.instance.GetTileOver().building;
                    nameLabel.text = building.name;
                    targetAlpha = 1.0f;
                }
                else
                {
                    targetAlpha = 0;
                }
            }
        }
	}
}
