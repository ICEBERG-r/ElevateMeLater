namespace ElevateMeLater;

public class Rider
{
    public int Weight { get; }

    public Rider()
    {
        Weight = RandomWeightGenerator();
    }

    private static int RandomWeightGenerator()
    {
        Random r = new();
        var rInt = r.Next(125, 350);
        return rInt;
    }
}