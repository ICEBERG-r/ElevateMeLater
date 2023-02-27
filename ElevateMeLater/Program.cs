
using ElevateMeLater;

Building building = new(1,12);
var log = new Logger();
Elevator.IsFirstValidInput = true;

string? selection;

Task? elevatorAction = null;

Console.WriteLine("Welcome to the Elevator Simulator!");

do
{
    Console.Write("Input: ");
    selection = Console.ReadLine()?.ToUpper();
    var direction = "K";
    var floor = 0;
    
    if (selection!.EndsWith('U') || selection.EndsWith('D'))
    {
        direction = selection.Last().ToString(); 
        
        selection = selection.Remove(selection.Length - 1); 
    }

    if (selection != "Q")
    {
        try
        {
            floor = int.Parse(selection); 
        }
        catch
        {
            continue; 
        }
    }
    

    if (building.Floors.ContainsKey(floor)) 
    {
        if (SelectionIsMinFloorDownOrMaxFloorUp(floor, direction))
        {
            continue;
        }
        if (direction is "U" or "D")
        {
            if (ElevatorInMotionAndNextFloorSelected(floor))
            {
                await Task.Run(() => building.Floors[floor].AddPotentialRiderAsync(direction).ConfigureAwait(false));
                log.BoardingRequest(floor + direction);
                
            }
            else
            {
                building.Floors[floor].AddPotentialRider(direction);
                log.BoardingRequest(floor + direction);
            }
            
            if (Elevator.IsFirstValidInput)
            {
                Elevator.IsFirstValidInput = false;
                elevatorAction = building.Elevator.OperateElevatorAsync(building);
            }
            
            
        }
        else
        {
            if (building.Elevator.IdleRiders.Any())
            {
                if (ElevatorInMotionAndNextFloorSelected(floor))
                {
                    await Task.Run(() => building.Elevator.AddRiderToExitQueueAsync(floor));
                }
                else
                {
                    building.Elevator.AddRiderToExitQueue(floor);
                }
            }

            if (Elevator.IsFirstValidInput)
            {
                Elevator.IsFirstValidInput = false;
                elevatorAction = building.Elevator.OperateElevatorAsync(building);
            }
        }
    }
    if (selection == "Q")
    {
        if (building.Elevator.InUse)
        {
            Console.WriteLine("All passengers will now be unloaded.");
            building.Elevator.MakeDecisionForIdleRiders(building);
            if (elevatorAction != null)
            {
                await elevatorAction;
            }
        }
        
        Console.WriteLine("Goodbye!");

        Environment.Exit(0);
    }
    

} while (selection != "Q");

bool SelectionIsMinFloorDownOrMaxFloorUp(int floor, string direction)
{
    return floor + direction == building.MinFloor + "D" || floor + direction == building.MaxFloor + "U";
}

bool ElevatorInMotionAndNextFloorSelected(int floor)
{
    return building.Elevator.Sensor.InMotion && floor == building.Elevator.Sensor.NextFloor;
}