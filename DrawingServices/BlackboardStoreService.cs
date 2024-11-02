using slate.DbServices;
using Microsoft.EntityFrameworkCore;

namespace DrawingServices
{
    public class BlackboardStoreService : IBlackboardStoreService
    {
        private const string connection = "Host=localhost;Port=5432;Database=blackboardObjectsdb;Username=postgres;Password=Kvaskovu20031986";

        private static readonly DbContextOptionsBuilder<ApplicationContext> optionsBuilder = new DbContextOptionsBuilder<ApplicationContext>();
        private static readonly DbContextOptions<ApplicationContext> options = optionsBuilder.UseNpgsql(connection).Options;

        public IEnumerable<BlackboardObjectData> BlackboardObjects 
        {
            get
            {
                using var db = new ApplicationContext(options);
                return db.BlackboardObjectDatas.ToList();
            }
        }

        public async Task<bool> TryAddObject(BlackboardObjectData blackboardObjectData)
        {
            ArgumentNullException.ThrowIfNull(blackboardObjectData);

            using var db = new ApplicationContext(options);

            await db.BlackboardObjectDatas.AddAsync(blackboardObjectData);

            await db.SaveChangesAsync();

            return true;
        }

        public async Task<bool> TryDeleteObjectsByIds(string[] deletedFromCanvasObjectsIds)
        {
            ArgumentNullException.ThrowIfNull(deletedFromCanvasObjectsIds);

            using var db = new ApplicationContext(options);

            bool result = true;

            foreach (var id in deletedFromCanvasObjectsIds)
            {
                BlackboardObjectData? blackboardObject = await db.BlackboardObjectDatas.FirstOrDefaultAsync(obj => obj.Id == id);

                if (blackboardObject is null)
                {
                    result = false;
                    continue;
                }

                db.BlackboardObjectDatas.Remove(blackboardObject);
                await db.SaveChangesAsync();
            }

            return result;
        }

        public async void DragObject(DragObjectData dragObjectData)
        {
            ArgumentNullException.ThrowIfNull(dragObjectData);

            using var db = new ApplicationContext(options);

            BlackboardObjectData? blackboardObject = await db.BlackboardObjectDatas.FirstOrDefaultAsync(obj => obj.Id == dragObjectData.Id);

            if (blackboardObject is null)
            {
                return;
            }

            blackboardObject.Left = dragObjectData.Left;
            blackboardObject.Top = dragObjectData.Top;

            await db.SaveChangesAsync();
        }

        public async void ScaleObject(ScaleObjectData scaleObjectData)
        {
            ArgumentNullException.ThrowIfNull(scaleObjectData);

            using var db = new ApplicationContext(options);

            BlackboardObjectData? blackboardObject = await db.BlackboardObjectDatas.FirstOrDefaultAsync(obj => obj.Id == scaleObjectData.Id);

            if (blackboardObject is null)
            {
                return;
            }

            blackboardObject.Left = scaleObjectData.Left;
            blackboardObject.Top = scaleObjectData.Top;
            blackboardObject.ScaleX = scaleObjectData.ScaleX;
            blackboardObject.ScaleY = scaleObjectData.ScaleY;

            await db.SaveChangesAsync();
        }

        public async void RotateObject(RotateObjectData rotateObjectData)
        {
            ArgumentNullException.ThrowIfNull(rotateObjectData);

            using var db = new ApplicationContext(options);

            BlackboardObjectData? blackboardObject = await db.BlackboardObjectDatas.FirstOrDefaultAsync(obj => obj.Id == rotateObjectData.Id);

            if (blackboardObject is null)
            {
                return;
            }

            blackboardObject.Angle = rotateObjectData.Angle;

            await db.SaveChangesAsync();await db.SaveChangesAsync();
        }
    }
}
