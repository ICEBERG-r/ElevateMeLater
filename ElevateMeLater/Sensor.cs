namespace ElevateMeLater;

public class Sensor
{
    public int CurrentFloor { get; set; } = 1;
    public int NextFloor { get; set; }
    public int CurrentWeight { get; set; }
    public bool GoingUp { get; set; }
    public bool InMotion { get; set; }
    public bool AtMaxWeight { get; set; }
}