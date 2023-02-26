namespace ElevateMeLater;
using Serilog;

public class Logger
{
    public Logger()
    {
        Log.Logger = new LoggerConfiguration().WriteTo.File("log.txt").CreateLogger();
    }
    public void LogStoppedAtFloor(Sensor s)
    {
        Log.Information("Elevator has stopped on floor " + s.CurrentFloor + " at " +
                        "{now}", DateTime.Now);
    }

    public void LogSkippedFloor(Sensor s)
    {
        Log.Information("Elevator has skipped floor " + s.CurrentFloor + " at {now}",
            DateTime.Now);
    }

    public void LogBoardingRequest(string sel)
    {
        Log.Information("Boarding requested: " + sel + " at {now}", DateTime.Now);
    }

    public void LogExitRequest(int f)
    {
        Log.Information("Elevator exit requested: " + f + " at {now}", DateTime.Now);
    }
}