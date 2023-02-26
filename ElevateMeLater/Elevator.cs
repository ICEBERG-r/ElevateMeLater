
namespace ElevateMeLater;

public class Elevator
{
    private int MaxWeight { get; }
    public Sensor Sensor { get; }
    public List<Rider> IdleRiders { get; }
    private Dictionary<int, List<Rider>> ExitingRiders { get; }
    
    public bool InUse { get; private set; }
    public static bool IsFirstValidInput { get; set; }
    private readonly Logger _log = new ();
    

    public Elevator(int maxWeight)
    {
        MaxWeight = maxWeight;
        Sensor = new Sensor();
        IdleRiders = new List<Rider>();
        ExitingRiders = new Dictionary<int, List<Rider>>();
    }
    public async Task OperateElevatorAsync(Building b)
    {
        SetFloorsForExitingRiders(b);
        InUse = true;
        Sensor.GoingUp = true;
        while (InUse)
        {
            while (Sensor.GoingUp)
            {
                var waitingForRiders = false;
                if (Sensor.CurrentFloor != b.MaxFloor)
                {
                    Sensor.NextFloor = Sensor.CurrentFloor + 1;
                    var upRequests = b.Floors[Sensor.CurrentFloor].UpRequests;
                    if (upRequests != null && upRequests.Any() && !Sensor.AtMaxWeight)
                    {
                        Sensor.InMotion = false;
                        LoadUpwardRiders(b);
                        if (Sensor.CurrentWeight > MaxWeight)
                        {
                            Sensor.AtMaxWeight = true;
                        }
                
                        if (!waitingForRiders)
                        {
                            _log.LogStoppedAtFloor(Sensor);
                            await Task.Delay(1000).ConfigureAwait(false);
                            waitingForRiders = true;
                        }

                    }
                }

                if (ExitingRiders[Sensor.CurrentFloor].Any())
                {
                    Sensor.InMotion = false;
                    UnloadRiders();

                    if (Sensor.CurrentWeight < MaxWeight)
                    {
                        Sensor.AtMaxWeight = false;
                    }
                
                    if (!waitingForRiders)
                    {
                        _log.LogStoppedAtFloor(Sensor);
                        await Task.Delay(1000).ConfigureAwait(false);
                        waitingForRiders = true;
                    }

                }
            
                if (!RequestsAtLowerFloors(b) && !RequestsAtHigherFloors(b))
                {
                    if (IdleRiders.Any())
                    {
                        Sensor.GoingUp = false;
                    }
                    else 
                    {
                        InUse = false;
                        IsFirstValidInput = true;
                        break;
                    }
                }

                if ((!RequestsAtHigherFloors(b) || Sensor.CurrentFloor == b.MaxFloor) &&
                Sensor.CurrentFloor != b.MinFloor)
                {
                    Sensor.GoingUp = false;
                    break;
                }

                if (!waitingForRiders)
                {
                    _log.LogSkippedFloor(Sensor);
                }
            
                Sensor.InMotion = true;
                await Task.Delay(3000).ConfigureAwait(false);
                Sensor.CurrentFloor++;
            }

            while (!Sensor.GoingUp)
            {
                var waitingForRiders = false;
                if (Sensor.CurrentFloor != b.MinFloor)
                {
                    Sensor.NextFloor = Sensor.CurrentFloor - 1;
                    var downRequests = b.Floors[Sensor.CurrentFloor].DownRequests;
                    if (downRequests != null && downRequests.Any() && !Sensor.AtMaxWeight)
                    {
                        Sensor.InMotion = false;
                        LoadDownwardRiders(b);

                        if (Sensor.CurrentWeight > MaxWeight)
                        {
                            Sensor.AtMaxWeight = true;
                        }
                        if (!waitingForRiders)
                        {
                            _log.LogStoppedAtFloor(Sensor);
                            await Task.Delay(1000).ConfigureAwait(false);
                            waitingForRiders = true;
                        }
                    }
                }

                if (ExitingRiders[Sensor.CurrentFloor].Any())
                {
                    Sensor.InMotion = false;
                    UnloadRiders();

                    if (Sensor.CurrentWeight < MaxWeight)
                    {
                        Sensor.AtMaxWeight = false;
                    }
                    if (!waitingForRiders)
                    {
                        _log.LogStoppedAtFloor(Sensor);
                        await Task.Delay(1000).ConfigureAwait(false);
                        waitingForRiders = true;
                    }

                }
                if (!RequestsAtLowerFloors(b) && !RequestsAtHigherFloors(b) && Sensor.CurrentFloor == b.MinFloor)
                {
                    InUse = false;
                    IsFirstValidInput = true;
                    break;
                }
            
                if (((!RequestsAtLowerFloors(b) && RequestsAtHigherFloors(b)) || Sensor.CurrentFloor == b.MinFloor) &&
                Sensor.CurrentFloor != b.MaxFloor && !Sensor.GoingUp)
                {
                    Sensor.GoingUp = true;
                    break;
                }

            

                if (!waitingForRiders)
                {
                    _log.LogSkippedFloor(Sensor);
                }
                Sensor.InMotion = true;
                await Task.Delay(3000).ConfigureAwait(false);
                Sensor.CurrentFloor--;

            }

        }
    }

    private bool RequestsAtLowerFloors(Building b)
    {
        var upRequests = false;
        var downRequests = false;
        var requests = false;
        if (Sensor.CurrentFloor != b.MaxFloor)
        {
            for (var i = Sensor.CurrentFloor; i >= b.MinFloor; i--)
            {
                var riders = b.Floors[i].UpRequests;
                if (riders != null && (riders.Any() || ExitingRiders[i].Any()))
                {
                    upRequests = true;
                }
            }
        }

        if (Sensor.CurrentFloor != b.MinFloor)
        {
            for (var i = Sensor.CurrentFloor; i > b.MinFloor; i--)
            {
                var riders = b.Floors[i].DownRequests;
                if (riders != null && riders.Any())
                {
                    downRequests = true;
                }
            }
        }

        if (upRequests || downRequests)
        {
            requests = true;
        }

        return requests;
    }

    private bool RequestsAtHigherFloors(Building b)
    {
        var upRequests = false;
        var downRequests = false;
        var requests = false;
        if (Sensor.CurrentFloor != b.MaxFloor)
        {
            for (var i = Sensor.CurrentFloor; i < b.MaxFloor; i++)
            {
                var riders = b.Floors[i].UpRequests;
                if (riders != null && (riders.Any() || ExitingRiders[i].Any()))
                {
                    upRequests = true;
                }
            }

        }

        if (Sensor.CurrentFloor != b.MinFloor)
        {
            for (var i = Sensor.CurrentFloor; i <= b.MaxFloor; i++)
            {
                var riders = b.Floors[i].DownRequests;
                if (riders != null && (riders.Any() || ExitingRiders[i].Any()))
                {
                    downRequests = true;
                }
            }
        }

        if (upRequests || downRequests)
        {
            requests = true;
        }

        return requests;
    }

    private void LoadUpwardRiders(Building b)
    {
        var ur = b.Floors[Sensor.CurrentFloor].UpRequests;
        if (ur != null)
            foreach (var rider in ur)
            {
                IdleRiders.Add(rider);
                Sensor.CurrentWeight += rider.Weight;
            }

        b.Floors[Sensor.CurrentFloor].UpRequests?.Clear();
    }

    private void LoadDownwardRiders(Building b)
    {
        var dr = b.Floors[Sensor.CurrentFloor].DownRequests;
        if (dr != null)
            foreach (var rider in dr)
            {
                IdleRiders.Add(rider);
                Sensor.CurrentWeight += rider.Weight;
            }

        b.Floors[Sensor.CurrentFloor].DownRequests?.Clear();
    }

    private void UnloadRiders()
    {
        foreach (var rider in ExitingRiders[Sensor.CurrentFloor])
        {
            Sensor.CurrentWeight -= rider.Weight;
        }
        ExitingRiders[Sensor.CurrentFloor].Clear();
    }

    public void AddRiderToExitQueue(int floor)
    {
        if (!IdleRiders.Any()) return;
        _log.LogExitRequest(floor);
        var rider = IdleRiders.First();
        ExitingRiders[floor].Add(rider);
        IdleRiders.Remove(rider);
    }

    public async Task AddRiderToExitQueueAsync(int floor)
    {
        if (IdleRiders.Any())
        {
            await Task.Delay(3001);
            _log.LogExitRequest(floor);
            var rider = IdleRiders.First();
            ExitingRiders[floor].Add(rider);
            IdleRiders.Remove(rider);
        }
    }

    public void MakeDecisionForIdleRiders(Building b)
    {
        if (!IdleRiders.Any()) return;
        foreach (var rider in IdleRiders.ToList())
        {
            ExitingRiders[b.MinFloor].Add(rider);
            IdleRiders.Remove(rider);
        }
    }

    private void SetFloorsForExitingRiders(Building b)
    {
        for (var i = b.MinFloor; i <= b.MaxFloor; i++)
        {
            ExitingRiders.Add(i, new List<Rider>());
        }
    }
}