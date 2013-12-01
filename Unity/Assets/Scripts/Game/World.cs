using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class World : MonoBehaviour
{
    public Transform camHandle;
    public Camera cam;

    public GameObject prefabFood;
    public GameObject prefabIsland;

    public List<Track> tracks;
    public List<GameObject> islands;
    public List<Robot> robots;

    private List<Vector3> points;

    private float islandDist = 26f;

    private Track currentTrack;

    private IEnumerator Start()
    {
        currentTrack = tracks.GetRandom();
        int islandCount = currentTrack.clips.Length;

        points = new List<Vector3>();
        islands = new List<GameObject>();
        robots = new List<Robot>();

        for (int i = 0; i < islandCount; ++i)
        {
            Vector3 point = Vector3.zero;
        
            if (i > 0)
            {
                point = points[i - 1];

                while (true)
                {
                    if (Random.value < 0.5f)
                        point += Vector3.right * Mathf.Sign(Random.Range(-10f, 10f));
                    else
                        point += Vector3.forward * Mathf.Sign(Random.Range(-10f, 10f));

                    if (!points.Contains(point))
                    {
                        break;
                    }

                    //yield return null;
                }
            }

            GameObject island = GameObject.Instantiate(prefabIsland, point * islandDist, Quaternion.identity) as GameObject;
            island.transform.parent = transform;

            islands.Add(island);

            points.Add(point);
        }

        // place camera.

        Bounds boardBounds = new Bounds();

        for (int i = 0; i < islands.Count; ++i)
            boardBounds.Encapsulate(islands[i].collider.bounds);

        camHandle.position = Vector3.Scale(boardBounds.center, Vector3.right + Vector3.forward);
        camHandle.localRotation = Quaternion.Euler(Random.Range(30f, 75f), 0f, 0f);

        Vector3 min = cam.WorldToViewportPoint(boardBounds.min);
        Vector3 max = cam.WorldToViewportPoint(boardBounds.max);

        float x = Mathf.Abs(min.x) + Mathf.Abs(max.x);
        float y = Mathf.Abs(min.y) + Mathf.Abs(max.y);

        cam.orthographicSize *= 0.2f + (x > y ? x : y);

        // give them a frame to create the robots
        yield return null;

        // get robots to play music

        Debug.Log(currentTrack.name);

        double createTime = AudioSettings.dspTime;

        for (int i = 0; i < islandCount; ++i)
        {
            Robot robot = islands[i].GetComponentInChildren<Robot>();
            robot.createTime = createTime;

            robot.audio.clip = currentTrack.clips[i];
            robot.PlayAtTime();

            robots.Add(robot);
        }

        Title.ShowTitle(currentTrack.author, currentTrack.title);

        yield break;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hitInfo;
            Ray clickRay = cam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(clickRay, out hitInfo, 500f, Layers.MaskOnly(Layers.Ground)))
            {
                Island island = hitInfo.transform.GetComponent<Island>();

                if (island != null)
                {
                    Vector3 foodPos = island.GetPositionForFood(hitInfo.point);
                    GameObject food = GameObject.Instantiate(prefabFood, foodPos, Quaternion.identity) as GameObject;

                    island.TrackFood(food);
                }
            }
        }

        int notPlaying = 0;

        for (int i = 0; i < robots.Count; ++i)
        {
            if (!robots[i].audio.isPlaying)
                ++notPlaying;
        }

        if (notPlaying == currentTrack.clips.Length || Input.GetKeyDown(KeyCode.Escape))
        {
            Application.LoadLevel(Scenes.game);
        }
    }

}
