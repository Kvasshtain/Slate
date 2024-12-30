namespace DrawingServices
{
    public class PointStoreService : IPointStoreService
    {
        readonly List<DrawingAction> points = [];

        public List<DrawingAction> Points => points;
    }
}
