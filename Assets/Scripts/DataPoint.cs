using UnityEngine;

public class DataPoint
{
    public double x, y;
    public double stretch_x, stretch_y;
    public double rotation;
    public double mass;

    public DataPoint() { }

    public DataPoint(double x, double y, double stretch_x, double stretch_y, double rotation, double mass)
    {
        this.x = x;
        this.y = y;

        this.stretch_x = stretch_x;
        this.stretch_y = stretch_y;

        this.rotation = rotation;
        this.mass = mass;
    }

    public override string ToString()
    {
        return $"x:{x}, y:{y}, stretch_x:{stretch_x}, stretch_y:{stretch_y}, rotation:{rotation}, mass:{mass}";
    }

    public Vector2 ToVector2()
    {
        return new Vector2((float)x, (float)y);
    }
}
