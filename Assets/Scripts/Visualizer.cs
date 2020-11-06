using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Visualizer : MonoBehaviour
{
    private int idx = 0;
    private List<GameObject> vDataPoints;
    private GameObject trail;
    private List<DataPoint> dataPoints;

    public GameObject rockPrefab, trailPrefab;
    public float distanceRatio = 1.0f;
    public float trailSpeed = 25f;
    public Material[] colors; // After building the project as an exe, the number of colors is gonna be restricted to at most X amount of colors
    public float animationSpeed = 1.0f;
    public Canvas canvas;

    void Visualize()
    {
        idx = 0;
        vDataPoints = new List<GameObject>(); // vDataPoints is a list of GameOjbects (prefix 'v' is for visualized)

        // mapping each type to a material index assuming number of unique types is less than number of materials
        int materialIndex = 0;
        Dictionary<string, int> typeToColor = new Dictionary<string, int>();
        foreach(DataPoint dataPoint in dataPoints)
        {
            if (!typeToColor.ContainsKey(dataPoint.type))
            {
                typeToColor.Add(dataPoint.type, materialIndex);
                materialIndex++;
            }
        }

        /*
         * order is a list of datapoints in the order of removal by steepest descent
         * it's easier to imagine order as dataPoints but with a different ordering of elements
         * this order of elements matters because it's the order of the traversal of the visualization method.
         * 
         * TODO change from list to array because size is known beforehand
         */
        List<DataPoint> order = new List<DataPoint>(); 
        while (dataPoints.Any()) // FIXME SteepestDescent returns NaN when there are no unique coordinates in dataPoints
        {
            if(dataPoints.Count == 1)
            {
                order.Add(dataPoints.Last());
                break;
            }

            double init_x = dataPoints[0].x, init_y = dataPoints[0].y; // initial point is first point

            Vector2[] steps = SteepestDescent.Run(dataPoints, init_x, init_y, 0.03, 300);

            int nearest = GetNearest(dataPoints, steps.Last());
            order.Add(dataPoints[nearest]);
            dataPoints.RemoveAt(nearest); // TODO optimize using dictionary or boolean visited array
        }

        // instantiate datapoints
        foreach (DataPoint dataPoint in order)
        {
            GameObject vPoint = Instantiate(rockPrefab, distanceRatio * new Vector3((float)dataPoint.x, 0, (float)dataPoint.y), Quaternion.identity);

            int colorIndex = typeToColor[dataPoint.type];
            vPoint.GetComponent<MeshRenderer>().material = colors[colorIndex];

            vDataPoints.Add(vPoint);
        }

        // instaniate trail
        trail = Instantiate(trailPrefab, Vector3.zero, Quaternion.identity);

        StartCoroutine(Animate());
    }

    private void Update()
    {
        if (GetComponent<DataReader>().isDataReady)
        {
            dataPoints = new List<DataPoint>();
            GetComponent<DataReader>().Populate(dataPoints);
            Visualize();
            GetComponent<DataReader>().isDataReady = false;
        }
    }
    private System.Collections.IEnumerator Animate()
    {
        while(idx < vDataPoints.Count)
        { 
            float distance = Vector3.Distance(trail.transform.position, vDataPoints[idx].transform.position);
            Debug.Log("Distance between trail and next data point: " + distance);
            if (distance <= 1.0f)
            {
                yield return new WaitForSeconds(1.0f / animationSpeed);
                Destroy(vDataPoints[idx]);
                Debug.Log("DESTROYED::vDataPoints[" + (idx++) + "]");
            }
            else
            {
                trail.transform.position = Vector3.MoveTowards(trail.transform.position, vDataPoints[idx].transform.position, trailSpeed * Time.deltaTime);
                yield return null;
            }
        }
        if (idx >= vDataPoints.Count) // destroy trail if animation is finished
            Destroy(trail);
    }


    private void Orbit(Transform current, Transform target) /// current orbits around target
    {
        current.RotateAround(target.position, new Vector3(0.1f, 0.1f, 0.1f), 500.0f * Time.deltaTime);
    }

    // returns the index of the dataPoint which is nearest to Vector2 coordinate
    private int GetNearest(List<DataPoint> dataPoints, Vector2 coordinate) // TODO optimize
    {
        int ret = 0;
        float minDistance = float.MaxValue;
        for (int i = 0; i < dataPoints.Count; i++)
        {
            float distance = Vector2.Distance(coordinate, dataPoints[i].ToVector2());
            if (distance < minDistance)
            {
                minDistance = distance;
                ret = i;
            }
        }
        Debug.Log("The minimum is: " + minDistance + " between " + coordinate + " and " + dataPoints[ret].ToVector2());
        return ret;
    }
}
