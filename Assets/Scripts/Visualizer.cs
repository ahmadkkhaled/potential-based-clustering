using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Visualizer : MonoBehaviour
{
    private int idx = 0;
    private List<GameObject> rocks;
    public GameObject rockPrefab;
    

    // returns the index of the dataPoint which is nearest to Vector2 coordinate
    private int GetNearest(List<DataPoint> dataPoints, Vector2 coordinate) // TODO optimize
    {
        int ret = 0;
        float minDistance = float.MaxValue;
        for(int i = 0; i < dataPoints.Count; i++)
        {
            float distance = Vector2.Distance(coordinate, dataPoints[i].ToVector2());
            if(distance < minDistance)
            {
                minDistance = distance;
                ret = i;
            }
        }
        Debug.Log("The minimum is: " + minDistance + " between " + coordinate + " and " + dataPoints[ret].ToVector2());
        return ret;
    }
    void Start()
    {
        List<DataPoint> dataPoints = new List<DataPoint>();
        string dataPath = Application.dataPath + "/Datasets/iris.csv";
        DataReader.FillData(dataPoints, dataPath);

        List<Vector2> order = new List<Vector2>(); // TODO change from list to array because size is known beforehand
        while (dataPoints.Any()) // FIXME SteepestDescent returns NaN when there are no unique coordinates in dataPoints
        {
            if(dataPoints.Count == 1)
            {
                order.Add(dataPoints.Last().ToVector2());
                break;
            }

            double init_x = dataPoints[0].x, init_y = dataPoints[0].y; // initial point is first point

            Vector2[] steps = SteepestDescent.Run(dataPoints, init_x, init_y, 0.03, 300);

            int nearest = GetNearest(dataPoints, steps.Last());

            order.Add(dataPoints[nearest].ToVector2());

            dataPoints.RemoveAt(nearest); // TODO optimize using dictionary or boolean visited array
        }

        for(int i = 0; i < order.Count; i++)
            Debug.Log("order[" + i + "] = " + order[i]);

        rocks = new List<GameObject>();
        foreach(Vector2 coordinate in order)
        {
            rocks.Add(Instantiate(rockPrefab, 15.0f * new Vector3(coordinate.x, coordinate.y, UnityEngine.Random.Range(0.5f, 5.0f)), Quaternion.identity));
        }

        StartCoroutine(DestroyCoroutine());
    }

    private System.Collections.IEnumerator DestroyCoroutine()
    {
        while(idx < rocks.Count)
        {
            Debug.Log(idx);
            Destroy(rocks[idx]);
            idx++;
            yield return new WaitForSeconds(0.5f);
        }
    }
}
