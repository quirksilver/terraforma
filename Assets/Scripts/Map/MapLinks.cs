using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Link : MonoBehaviour
{
    private List<RaycastHit> hits;
    private List<Vector3> direction;
    private List<Vector2> UVdirection;
    public static Texture2D texture;
    public static Color[] track;
    int buildingIndex = 0;

    private float timeSpan = 0.01f;
    private float timer = 0;

    private int trainPoint=0;
    private int trainDir = 1;
    private int lastDir = 1;
    public Transform trainRoot;

    private float trainDelay = 1.0f;
    private float trainTimer = 0;

    public void setup(Vector3 pos1, Vector3 pos2, Globe globe)
    {
        hits = new List<RaycastHit>();
        direction = new List<Vector3>();
        UVdirection = new List<Vector2>();

        float d = Vector3.Distance(pos2, pos1);
        Vector3 vd = pos2 - pos1;
        vd.Normalize();
        for (float i = 0; i < d; i += 0.5f)
        {
            hits.Add(globe.RotAtPoint(pos1 + vd * i));
            Debug.DrawRay(hits[hits.Count - 1].point, hits[hits.Count - 1].normal, Color.red, 120.0f);

            Vector2 texPos = Vector2.Scale(hits[hits.Count - 1].textureCoord, new Vector2(texture.width, texture.height));

            if (i != 0)
            {
                direction.Add((hits[hits.Count - 1].point - hits[hits.Count - 2].point).normalized);
                Debug.DrawRay(hits[hits.Count - 1].point, direction[direction.Count - 1]*30, Color.blue, 120.0f);

                UVdirection.Add(
                    (Vector2.Scale(hits[hits.Count-1].textureCoord, new Vector2(texture.width, texture.height)) - 
                    Vector2.Scale(hits[hits.Count-2].textureCoord, new Vector2(texture.width, texture.height))).normalized);
                Debug.Log(UVdirection[UVdirection.Count - 1].ToString());
            }
        }
        texture.Apply();
    }

    public void Update()
    {
        if (buildingIndex >= hits.Count)
        {
            //Train update

            trainTimer += Time.deltaTime;
            if (trainTimer < trainDelay)
            {
                return;
            }

            Vector3 newPos = trainRoot.position;
            newPos = Vector3.MoveTowards(newPos, hits[trainPoint].point, 4.0f*Time.deltaTime);
            if(newPos==hits[trainPoint].point)
            {
                trainPoint += trainDir;

                if (trainDir != lastDir)
                {
                    trainTimer = 0;
                }

                lastDir = trainDir;

                if (trainPoint <= 0
                    || trainPoint >= hits.Count - 2)
                {
                    trainDir = 0 - trainDir;
                }
                trainRoot.transform.rotation = Quaternion.LookRotation(direction[trainPoint], hits[trainPoint].normal);
            }

            trainRoot.transform.localPosition = newPos;
            //trainRoot.transform.rotation = Quaternion.FromToRotation(Vector3.up, direction[trainPoint]);// *Quaternion.FromToRotation(Vector3.up, hits[trainPoint].normal);

            return;
        }

        timer += Time.deltaTime;
//        Debug.Log(timer);
        if (timer > timeSpan)
        {
            timer = 0;

            Vector2 texPos = Vector2.Scale(hits[buildingIndex].textureCoord, new Vector2(texture.width, texture.height));

            for (int x = -2; x < 2; x++)
            {
                for (int y = -2; y < 2; y++)
                {
                    texture.SetPixel(
                        (int)(texPos.x + x),
                        (int)(texPos.y + y),
                        Color.red);
                }
            }

            texture.Apply();

            buildingIndex++;
        }
    }
}

public class MapLinks : MonoBehaviour
{
    public Globe globe;
    public Texture2D track;
    public GameObject trainObj;

    private Texture2D texture;

    private bool drawing = false;
    private float currentLength;
    private float length;
    private float lastLength;
    private Vector2 start;
    private Vector2 diff;

    private Color trackColor = new Color(0.2f, 0.2f, 0.2f, 1.0f);

    List<Link> links = new List<Link>();

    void Start()
    {
        texture = new Texture2D(1024, 1024);
        //texture.filterMode = FilterMode.Point;
        renderer.material.mainTexture = texture;

        Link.texture = texture;
        Link.track = texture.GetPixels();

        int y = 0;
        while (y < texture.height)
        {
            int x = 0;
            while (x < texture.width)
            {
                Color color = Color.clear;
                texture.SetPixel(x, y, color);
                ++x;
            }
            ++y;
        }

        texture.Apply();
    }

    public void Update()
    {
        foreach (Link link in links)
        {
            link.Update();
        }
    }

    public void drawLine(Vector3 pos1,Vector3 pos2)
    {
        Link newLink = new Link();

        GameObject train = GameObject.Instantiate(trainObj) as GameObject;
        newLink.trainRoot = train.transform;
        train.transform.position = pos1;

        newLink.setup(pos1, pos2, globe);
        links.Add(newLink);
    }
}
