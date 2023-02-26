namespace ElevateMeLater;
using Serilog;

public class Logger
{
    public Logger()
    {
        Log.Logger = new LoggerConfiguration().WriteTo.File("log.txt").CreateLogger();
    }
    public void StoppedAtFloor(Sensor sensor)
    {
        Log.Information("Elevator has stopped on floor " + sensor.CurrentFloor + " at " +
                        "{now}", DateTime.Now);
    }

    public void SkippedFloor(Sensor sensor)
    {
        Log.Information("Elevator has skipped floor " + sensor.CurrentFloor + " at {now}",
            DateTime.Now);
    }

    public void BoardingRequest(string floorAndDirection)
    {
        Log.Information("Boarding requested: " + floorAndDirection + " at {now}", DateTime.Now);
    }

    public void ExitRequest(int floor)
    {
        Log.Information("Elevator exit requested: " + floor + " at {now}", DateTime.Now);
    }
}