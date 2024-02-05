namespace DrawingServices
{

    public class PointStoreService : IPointStoreService
    {
        List<DrawingAction> points = new List<DrawingAction>();

        public List<DrawingAction> Points { get => points; }
    }

}