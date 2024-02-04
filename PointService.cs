namespace DrawingServices
{

    public class PointService : IPointService
    {
        List<DrawingAction> points = new List<DrawingAction>();

        public List<DrawingAction> Points { get => points; }
    }

}