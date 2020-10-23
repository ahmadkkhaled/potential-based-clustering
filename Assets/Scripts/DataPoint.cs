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


}
