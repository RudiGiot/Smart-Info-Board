[System.Serializable]
public class Rootobject
{
    public Line[] lines;
}
[System.Serializable]
public class Line
{
    public int lineId;
    public Vehicleposition[] vehiclePositions;
}
[System.Serializable]
public class Vehicleposition
{
    public int directionId;
    public int distanceFromPoint;
    public int pointId;
}
