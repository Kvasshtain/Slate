using Microsoft.EntityFrameworkCore;
using slate.DbServices;

namespace slate.DrawingServices
{
    public class BlackboardStoreService : IBlackboardStoreService
    {
        private const string connection =
            "Host=localhost;Port=5432;Database=slate;Username=postgres;Password=Kvaskovu20031986";

        private static readonly DbContextOptionsBuilder<ApplicationContext> optionsBuilder = new();
        private static readonly DbContextOptions<ApplicationContext> options = optionsBuilder
            .UseNpgsql(connection)
            .Options;

        public IEnumerable<BlackboardObjectData> GetBlackboardObjects(int blackboardId) =>
            [
                .. new ApplicationContext(options).BlackboardObjectData.Where(item => item.BlackboardId == blackboardId)
            ];

        public async Task<BlackboardObjectData?> GetBlackboardObjectByIdAsync(int id)
        {
            await using var db = new ApplicationContext(options);

            var blackboardObjectData = db.BlackboardObjectData;

            return await blackboardObjectData.FindAsync(id);
        }

        public async Task<int?> GetObjectBlackboardIdAsync(int id)
        {
            var blackboardObject = await GetBlackboardObjectByIdAsync(id);

            return blackboardObject?.BlackboardId;
        }

        public async Task<bool> TryAddObjectAsync(BlackboardObjectData blackboardObjectData)
        {
            ArgumentNullException.ThrowIfNull(blackboardObjectData);

            await using var db = new ApplicationContext(options);

            await db.BlackboardObjectData.AddAsync(blackboardObjectData);

            int objCount = await db.SaveChangesAsync();

            return objCount == 1;
        }

        public async Task<bool> TryDeleteObjectByIdAsync(int deletedFromCanvasObjectId)
        {
            ArgumentNullException.ThrowIfNull(deletedFromCanvasObjectId);

            await using var db = new ApplicationContext(options);

            var blackboardObject =
                await db.BlackboardObjectData.FirstOrDefaultAsync(obj =>
                    obj.Id == deletedFromCanvasObjectId
                );

            if (blackboardObject is null)
                return false;

            db.BlackboardObjectData.Remove(blackboardObject);

            int objCount = await db.SaveChangesAsync();

            return objCount == 1;
        }

        public async Task<bool> DragObjectAsync(DragObjectData dragObjectData)
        {
            ArgumentNullException.ThrowIfNull(dragObjectData);

            await using var db = new ApplicationContext(options);

            var blackboardObject =
                await db.BlackboardObjectData.FirstOrDefaultAsync(obj =>
                    obj.Id == dragObjectData.Id
                );

            if (blackboardObject is null)
                return false;

            blackboardObject.Left = dragObjectData.Left;
            blackboardObject.Top = dragObjectData.Top;

            int objCount = await db.SaveChangesAsync();

            return objCount == 1;
        }

        public async Task<bool> ScaleObjectAsync(ScaleObjectData scaleObjectData)
        {
            ArgumentNullException.ThrowIfNull(scaleObjectData);

            await using var db = new ApplicationContext(options);

            var blackboardObject =
                await db.BlackboardObjectData.FirstOrDefaultAsync(obj =>
                    obj.Id == scaleObjectData.Id
                );

            if (blackboardObject is null)
                return false;

            blackboardObject.Left = scaleObjectData.Left;
            blackboardObject.Top = scaleObjectData.Top;
            blackboardObject.ScaleX = scaleObjectData.ScaleX;
            blackboardObject.ScaleY = scaleObjectData.ScaleY;

            int objCount = await db.SaveChangesAsync();

            return objCount == 1;
        }

        public async Task<bool> RotateObjectAsync(RotateObjectData rotateObjectData)
        {
            ArgumentNullException.ThrowIfNull(rotateObjectData);

            await using var db = new ApplicationContext(options);

            var blackboardObject =
                await db.BlackboardObjectData.FirstOrDefaultAsync(obj =>
                    obj.Id == rotateObjectData.Id
                );

            if (blackboardObject is null)
                return false;

            blackboardObject.Angle = rotateObjectData.Angle;

            int objCount = await db.SaveChangesAsync();
            await db.SaveChangesAsync();

            return objCount == 1;
        }
    }
}
