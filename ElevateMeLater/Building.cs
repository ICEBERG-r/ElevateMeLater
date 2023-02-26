namespace ElevateMeLater;

public class Building
{
    public int MinFloor { get; }
    public int MaxFloor { get; }
    public Dictionary<int, Floor> Floors { get; } = new();

    public Elevator Elevator { get; } = new(4500);

    public Building(int min, int max)
    {
        MinFloor = min;
        MaxFloor = max;
        SetFloors();
    }

    private void SetFloors()
    {
        for (var i = MinFloor; i <= MaxFloor; i++)
        {
            Floors.Add(i, new Floor());
            Floors[i].UpRequests = new List<Rider>();
            
            Floors[i].DownRequests = new List<Rider>();
        }
    }
}