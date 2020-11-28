using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public static class SteepestDescent
{
    private static List<DataPoint> _dataPoints; 

    //  1 / (2 * pi * stretch_x * stretch_y)
    private static double K0(double stretch_x, double stretch_y) {return 1.0 / (2.0 * Math.PI * stretch_x * stretch_y);}

    // (cos(rotation) ** 2 / stretch_x ** 2) + (sin(rotation) ** 2 / stretch_y ** 2)
    private static double K1(double stretch_x, double stretch_y, double rotation) {
        return (Math.Pow(Math.Cos(rotation), 2.0) / (stretch_x * stretch_x)) +
               (Math.Pow(Math.Sin(rotation), 2.0) / (stretch_y * stretch_y));
    }

    // (sin(rotation) ** 2 / stretch_x ** 2) + (cos(rotation) ** 2 / stretch_y ** 2)
    private static double K2(double stretch_x, double stretch_y, double rotation) {
        return (Math.Pow(Math.Sin(rotation), 2.0) / (stretch_x * stretch_x)) +
               (Math.Pow(Math.Cos(rotation), 2.0) / (stretch_y * stretch_y));
    }

    // sin(2 * rotation) * (1 / stretch_x ** 2 - 1 / stretch_y ** 2)
    private static double K12(double stretch_x, double stretch_y, double rotation) {
        return Math.Sin(2.0 * rotation) * (1.0 / stretch_x * stretch_x - 1 / stretch_y * stretch_y);
    }

    /* k0(stretch_x, stretch_y) * 
     * (
     * 1+k1(stretch_x, stretch_y, rotation) * (x-data_point_x)**2 + 
     * k12(stretch_x, stretch_y, rotation) * (x-data_point_x)*(y-data_point_y) + 
     * k2(stretch_x, stretch_y, rotation) * 
     * (y-data_point_y)**2)**(-3/2)
     * 
     */
    private static double CauchyDensity(double x, double y, DataPoint datapoint)
    {
        return K0(datapoint.stretch_x, datapoint.stretch_y) *
               Math.Pow(
                           (
                           1 +   K1(datapoint.stretch_x, datapoint.stretch_y, datapoint.rotation)  * (x - datapoint.x)*(x - datapoint.x) +
                                 K12(datapoint.stretch_x, datapoint.stretch_y, datapoint.rotation) * (x - datapoint.x)*(y - datapoint.y) +
                                 K2(datapoint.stretch_x, datapoint.stretch_y, datapoint.rotation)  * (y - datapoint.y)*(y - datapoint.y)
                            ), 
                -3.0 / 2.0
                        );   

    }

    /*
     * (-3/2) * 
     * k0(stretch_x, stretch_y)**(-2/3) * 
     * cauchy_density(x, y, data_point_x, data_point_y, stretch_x, stretch_y, rotation)**(5/3) * 
     * (
     * 2 * k2(stretch_x, stretch_y, rotation)*(y-data_point_y)+
     * k12(stretch_x, stretch_y, rotation) * (x-data_point_x))
     */
    private static double CauchyGradientDensityY(double x, double y, DataPoint datapoint)
    {
        return (-3.0 / 2.0) *
               Math.Pow(K0(datapoint.stretch_x, datapoint.stretch_y), (-2.0 / 3.0)) *
               Math.Pow(CauchyDensity(x, y, datapoint), 5.0 / 3.0) *
               (
                    2.0 * K2(datapoint.stretch_x, datapoint.stretch_y, datapoint.rotation) * (y - datapoint.y) +
                    K12(datapoint.stretch_x, datapoint.stretch_y, datapoint.rotation) * (x - datapoint.x)
               );
    }

    /*
     * (-3/2) * 
     * k0(stretch_x, stretch_y)**(-2/3) * 
     * cauchy_density(x, y, data_point_x, data_point_y, stretch_x, stretch_y, rotation)**(5/3) * 
     * (
     * 2 * k1(stretch_x, stretch_y, rotation)*(x-data_point_x) + 
     * k12(stretch_x, stretch_y, rotation) * (y-data_point_y)
     * )
     */
    private static double CauchyGradientDensityX(double x, double y, DataPoint datapoint)
    {
        return (-3.0 / 2.0) *
               Math.Pow(K0(datapoint.stretch_x, datapoint.stretch_y), (-2.0 / 3.0)) *
               Math.Pow(CauchyDensity(x, y, datapoint), 5.0 / 3.0) *
               (
                    2.0 * K1(datapoint.stretch_x, datapoint.stretch_y, datapoint.rotation)  * (x - datapoint.x) +
                    K12(datapoint.stretch_x, datapoint.stretch_y, datapoint.rotation) * (y - datapoint.y)
               );
    }

    private static double CauchyTotalGradientPotentialY(double x, double y)
    {
        double totalGradientPotential = 0;
        foreach(DataPoint point in _dataPoints)
        {
            totalGradientPotential += -point.mass * CauchyGradientDensityY(x, y, point);
        }
        return totalGradientPotential;
    }

    private static double CauchyTotalGradientPotentialX(double x, double y)
    {
        double totalGradientPotential = 0;
        foreach(DataPoint point in _dataPoints)
        {
            totalGradientPotential += -point.mass * CauchyGradientDensityX(x, y, point);
        }
        return totalGradientPotential;
    }

    // runs steepest descent for {ucount} steps with initial coordinates {x, y} and learning rate equal to {steepPace}
    // returns a list of coordinates {Vector2[]} all the traversed points during the run of steepest descent
    public static Vector2[] Run(in List<DataPoint> dataPoints, double x, double y, double steepPace, uint count) 
    {
        _dataPoints = dataPoints;

        Vector2[] steps = new Vector2[count];
        for(uint i = 0; i < count; i++)
        {
            double totalGradientPotentialX = CauchyTotalGradientPotentialX(x, y);
            double totalGradientPotentialY = CauchyTotalGradientPotentialY(x, y);

            Assert.IsFalse(totalGradientPotentialX == 0 && totalGradientPotentialY == 0);

            x -= steepPace * totalGradientPotentialX / Math.Sqrt(totalGradientPotentialX * totalGradientPotentialX + totalGradientPotentialY * totalGradientPotentialY);
            y -= steepPace * totalGradientPotentialY / Math.Sqrt(totalGradientPotentialX * totalGradientPotentialX + totalGradientPotentialY * totalGradientPotentialY);

            steps[i] = new Vector2((float)x, (float)y);
        }
        return steps;
    }

    private static double CauchyPotential(double x, double y, DataPoint dataPoint)
    {
        return -dataPoint.mass * CauchyDensity(x, y, dataPoint);
    }

    public static double CauchyTotalPotential(in List<DataPoint> dataPoints, double x, double y) // returns the cauchy total potential at {x, y} with respect to a list of data points {dataPoints}
    {
        _dataPoints = dataPoints;
        double totalPotential = 0;
        foreach(DataPoint point in dataPoints)
        {
            totalPotential += CauchyPotential(x, y, point);
        }
        return totalPotential;
    }
}
