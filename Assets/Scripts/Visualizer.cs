using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Visualizer : MonoBehaviour
{
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

        List<Vector2> order = new List<Vector2>();
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

            order.Add(new Vector2((float)dataPoints[nearest].x, (float)dataPoints[nearest].y));

            dataPoints.RemoveAt(nearest); // TODO optimize using dictionary or boolean visited array
        }

        for(int i = 0; i < order.Count; i++)
            Debug.Log("order[" + i + "] = " + order[i]);
    }
}
