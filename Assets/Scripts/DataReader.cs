using System.Collections.Generic;
using System.IO;

public static class DataReader
{
    public static void FillData(List<DataPoint> dataPoints, string path)
    {
        string[] lines = File.ReadAllLines(path);
        foreach(string line in lines){
            string[] parsedLine = line.Split(',');

            double sepal_length = double.Parse(parsedLine[0]);
            double sepal_width = double.Parse(parsedLine[1]);
            string type = parsedLine[4];

            /*
             * Rotation = 0 
             * Stretch (both x and y) = 1 
             * Mass = 1
             */
            dataPoints.Add(new DataPoint(sepal_length, sepal_width, 1, 1, 0, 1, type));
        }
    } 
}
