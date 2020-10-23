using System;
using System.Collections;
using System.Collections.Generic;

public class SteepestDescent
{
    public DataPoint[] dataPoints;
    
    //  1 / (2 * pi * stretch_x * stretch_y)
    private double K0(double stretch_x, double stretch_y) {return 1.0 / (2.0 * Math.PI * stretch_x * stretch_y);}

    // (cos(rotation) ** 2 / stretch_x ** 2) + (sin(rotation) ** 2 / stretch_y ** 2)
    private double K1(double stretch_x, double stretch_y, double rotation) {
        return (Math.Pow(Math.Cos(rotation), 2.0) / (stretch_x * stretch_x)) +
               (Math.Pow(Math.Sin(rotation), 2.0) / (stretch_y * stretch_y));
    }

    // (sin(rotation) ** 2 / stretch_x ** 2) + (cos(rotation) ** 2 / stretch_y ** 2)
    private double K2(double stretch_x, double stretch_y, double rotation) {
        return (Math.Pow(Math.Sin(rotation), 2.0) / (stretch_x * stretch_x)) +
               (Math.Pow(Math.Cos(rotation), 2.0) / (stretch_y * stretch_y));
    }

    // sin(2 * rotation) * (1 / stretch_x ** 2 - 1 / stretch_y ** 2)
    private double K12(double stretch_x, double stretch_y, double rotation) {
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
    private double CauchyDensity(double x, double y, DataPoint datapoint)
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
    public double CauchyGradientDensity_Y(double x, double y, DataPoint datapoint)
    {
        return (-3.0 / 2.0) *
               Math.Pow(K0(datapoint.stretch_x, datapoint.stretch_y), (-2.0 / 3.0)) *
               Math.Pow(CauchyDensity(x, y, datapoint), 5.0 / 3.0) *
               (
                    2.0 * K2(datapoint.stretch_x, datapoint.stretch_y, datapoint.rotation) * (y - datapoint.y) +
                    K12(datapoint.stretch_x, datapoint.stretch_y, datapoint.rotation) * (x - datapoint.x)
               );
    }
}
