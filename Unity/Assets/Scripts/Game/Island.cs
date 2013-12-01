using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Island : MonoBehaviour
{
    public List<Color> colors;
    public List<GameObject> foods;

    public GameObject prefabRobot;

    private Material mainMat;

    private Bounds pieceBounds;
    private Transform parentFood;

    private Transform cachedTransform;
    private Vector3 posBase;

    private float timeSlow;
    private float bobAmount;

    private Robot robot;

    public Color hueBase;
    public Color hueLight;
    public Color hueDesaturated;

    private void Start()
    {
        mainMat = renderer.material;

        HSBColor color = HSBColor.FromColor(colors.GetRandom());
        color.b += Random.Range(-0.2f, 0f);

        hueBase = color.ToColor();

        color.b += 0.1f;

        hueLight = color.ToColor();

        color.b -= 0.1f;
        color.s = 0f;

        hueDesaturated = color.ToColor();

        mainMat.color = hueBase;

        foods = new List<GameObject>();

        cachedTransform = transform;
        cachedTransform.position += Vector3.up * Random.Range(-2f, 2f);
        posBase = transform.position;

        pieceBounds = collider.bounds;
        pieceBounds.extents *= 0.8f;

        parentFood = new GameObject("Food").transform;
        parentFood.parent = transform;

        GameObject robotGO = GameObject.Instantiate(prefabRobot) as GameObject;

        robotGO.transform.localPosition = new Vector3(Random.Range(pieceBounds.min.x, pieceBounds.max.x),
                                                      5f,
                                                      Random.Range(pieceBounds.min.z, pieceBounds.max.z));

        robotGO.transform.parent = transform;

        robot = robotGO.GetComponent<Robot>();

        timeSlow = Random.Range(2, 8);
        bobAmount = Random.Range(0.5f, 1f) * Mathf.Sign(Random.Range(-10, 10));
    }

    public Vector3 GetPositionForFood(Vector3 groundPoint)
    {
        groundPoint.x = Mathf.Clamp(groundPoint.x, pieceBounds.min.x, pieceBounds.max.x);
        groundPoint.y = pieceBounds.max.y + 1f;
        groundPoint.z = Mathf.Clamp(groundPoint.z, pieceBounds.min.z, pieceBounds.max.z);

        return groundPoint;
    }

    public void TrackFood(GameObject food)
    {
        food.transform.parent = parentFood;

        foods.Add(food);
    }

    private void Update()
    {
        if (foods.Contains(null))
            foods.RemoveAll(x => x == null);

        cachedTransform.position = posBase + (MathTools.CosineWave(Time.time / timeSlow) * Vector3.up * bobAmount);
        pieceBounds.center = cachedTransform.position;

        if (robot.isSilent)
            mainMat.color = hueDesaturated;
        else
            mainMat.color = Color.Lerp(hueBase, hueLight, robot.activePower);
    }
}
