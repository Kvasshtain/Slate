using Microsoft.EntityFrameworkCore;
using slate.DbServices;

namespace DrawingServices
{
    public class BlackboardStoreService : IBlackboardStoreService
    {
        private const string connection =
            "Host=localhost;Port=5432;Database=slate;Username=postgres;Password=Kvaskovu20031986";

        private static readonly DbContextOptionsBuilder<ApplicationContext> optionsBuilder = new();
        private static readonly DbContextOptions<ApplicationContext> options = optionsBuilder
            .UseNpgsql(connection)
            .Options;

        public IEnumerable<BlackboardObjectData> BlackboardObjects =>
            [.. new ApplicationContext(options).BlackboardObjectDatas];

        public async Task<bool> TryAddObject(BlackboardObjectData blackboardObjectData)
        {
            ArgumentNullException.ThrowIfNull(blackboardObjectData);

            using var db = new ApplicationContext(options);

            await db.BlackboardObjectDatas.AddAsync(blackboardObjectData);

            int objCount = await db.SaveChangesAsync();

            if (objCount != 1)
                return false;

            return true;
        }

        public async Task<bool> TryDeleteObjectById(string deletedFromCanvasObjectId)
        {
            ArgumentNullException.ThrowIfNull(deletedFromCanvasObjectId);

            using var db = new ApplicationContext(options);

            BlackboardObjectData? blackboardObject =
                await db.BlackboardObjectDatas.FirstOrDefaultAsync(obj =>
                    obj.Id == deletedFromCanvasObjectId
                );

            if (blackboardObject is null)
                return false;

            db.BlackboardObjectDatas.Remove(blackboardObject);

            int objCount = await db.SaveChangesAsync();

            if (objCount != 1)
                return false;

            return true;
        }

        public async Task<bool> DragObject(DragObjectData dragObjectData)
        {
            ArgumentNullException.ThrowIfNull(dragObjectData);

            using var db = new ApplicationContext(options);

            BlackboardObjectData? blackboardObject =
                await db.BlackboardObjectDatas.FirstOrDefaultAsync(obj =>
                    obj.Id == dragObjectData.Id
                );

            if (blackboardObject is null)
                return false;

            blackboardObject.Left = dragObjectData.Left;
            blackboardObject.Top = dragObjectData.Top;

            int objCount = await db.SaveChangesAsync();

            if (objCount != 1)
                return false;

            return true;
        }

        public async Task<bool> ScaleObject(ScaleObjectData scaleObjectData)
        {
            ArgumentNullException.ThrowIfNull(scaleObjectData);

            using var db = new ApplicationContext(options);

            BlackboardObjectData? blackboardObject =
                await db.BlackboardObjectDatas.FirstOrDefaultAsync(obj =>
                    obj.Id == scaleObjectData.Id
                );

            if (blackboardObject is null)
                return false;

            blackboardObject.Left = scaleObjectData.Left;
            blackboardObject.Top = scaleObjectData.Top;
            blackboardObject.ScaleX = scaleObjectData.ScaleX;
            blackboardObject.ScaleY = scaleObjectData.ScaleY;

            int objCount = await db.SaveChangesAsync();

            if (objCount != 1)
                return false;

            return true;
        }

        public async Task<bool> RotateObject(RotateObjectData rotateObjectData)
        {
            ArgumentNullException.ThrowIfNull(rotateObjectData);

            using var db = new ApplicationContext(options);

            BlackboardObjectData? blackboardObject =
                await db.BlackboardObjectDatas.FirstOrDefaultAsync(obj =>
                    obj.Id == rotateObjectData.Id
                );

            if (blackboardObject is null)
                return false;

            blackboardObject.Angle = rotateObjectData.Angle;

            int objCount = await db.SaveChangesAsync();
            await db.SaveChangesAsync();

            if (objCount != 1)
                return false;

            return true;
        }
    }
}
