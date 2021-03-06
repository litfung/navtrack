using Navtrack.DataAccess.Model;
using Navtrack.Library.DI;
using Navtrack.Library.Services;
using Location = Navtrack.Listener.Models.Location;

namespace Navtrack.Listener.Mappers
{
    [Service(typeof(IMapper<Location, AssetEntity, DataAccess.Model.LocationEntity>))]
    public class LocationMapper : IMapper<Location, AssetEntity, DataAccess.Model.LocationEntity>
    {
        public DataAccess.Model.LocationEntity Map(Location source1, AssetEntity source2, DataAccess.Model.LocationEntity destination)
        {
            destination.DeviceId = source2.DeviceId;
            destination.AssetId = source2.Id;
            destination.Latitude = source1.Latitude;
            destination.Longitude = source1.Longitude;
            destination.DateTime = source1.DateTime;
            destination.PositionStatus = source1.PositionStatus;
            destination.Speed = source1.Speed;
            destination.Heading = source1.Heading;
            destination.Altitude = source1.Altitude;
            destination.Satellites = source1.Satellites;
            destination.HDOP = source1.HDOP;
            destination.GsmSignal = source1.GsmSignal;
            destination.Odometer = source1.Odometer;
            destination.MobileCountryCode = source1.MobileCountryCode;
            destination.MobileNetworkCode = source1.MobileNetworkCode;
            destination.LocationAreaCode = source1.LocationAreaCode;
            destination.CellId = source1.CellId;

            return destination;
        }
    }
}