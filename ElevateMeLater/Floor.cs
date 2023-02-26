namespace ElevateMeLater;

public class Floor
{
    public List<Rider>? UpRequests;
    public List<Rider>? DownRequests;

    public void AddPotentialRider(string dir)
    {
        switch (dir)
        {
            case "U":
            {
                var rider = new Rider();
                UpRequests?.Add(rider);
                break;
            }
            case "D":
            {
                var rider = new Rider();
                DownRequests?.Add(rider);
                break;
            }
        }
    }

    public async Task AddPotentialRiderAsync(string dir)
    {
        switch (dir)
        {
            case "U":
            {
                var rider = new Rider();
                await Task.Delay(3001);
                UpRequests?.Add(rider);
                break;
            }
            case "D":
            {
                var rider = new Rider();
                await Task.Delay(3001);
                DownRequests?.Add(rider);
                break;
            }
        }
    }
}