using System.Text.RegularExpressions;
using Navtrack.Library.DI;
using Navtrack.Listener.Helpers;
using Navtrack.Listener.Models;
using Navtrack.Listener.Server;

namespace Navtrack.Listener.Protocols.Haicom
{
    [Service(typeof(ICustomMessageHandler<HaicomProtocol>))]
    public class HaicomMessageHandler : BaseMessageHandler<HaicomProtocol>
    {
        public override Location Parse(MessageInput input)
        {
            Match locationMatch =
                new Regex("GPRS(\\d{15})," + // imei
                          "(.*?)," + // version
                          "(\\d{2})(\\d{2})(\\d{2})," + // yy mm dd
                          "(\\d{2})(\\d{2})(\\d{2})," + // hh mm ss
                          "(\\d)" + // flags
                          "(\\d{2})(\\d{5})" + // latitude
                          "(\\d{3})(\\d{5})," + // longitude
                          "(\\d+)," + // speed
                          "(\\d+)," + // heading
                          "(\\d+)," + // status
                          "(.*?)," + // gprs count value
                          "(.*?)," + // gps power saving counting value
                          "(.*?)," + // switch status
                          "(.*?)" + // voltage
                          "#V(\\d+)") // battery
                    .Match(input.DataMessage.String);

            if (locationMatch.Groups.Count == 22)
            {
                int flags = locationMatch.Groups[9].Get<int>();

                Location location = new Location
                {
                    Device = new Device
                    {
                        IMEI = locationMatch.Groups[1].Value
                    },
                    DateTime = DateTimeUtil.New(locationMatch.Groups[3].Value, locationMatch.Groups[4].Value,
                        locationMatch.Groups[5].Value, locationMatch.Groups[6].Value, locationMatch.Groups[7].Value,
                        locationMatch.Groups[8].Value),
                    Latitude = GetCoordinate(locationMatch, 10, 11, flags, 2),
                    Longitude = GetCoordinate(locationMatch, 12, 13, flags, 1),
                    Speed = locationMatch.Groups[14].Get<decimal?>() / 10,
                    Heading = locationMatch.Groups[15].Get<decimal?>() / 10
                };
           
                return location;
            }

            return null;
        }

        private static decimal GetCoordinate(Match locationMatch, int degreesIndex, int minutesIndex, int flags, int i2)
        {
            decimal coordinate = locationMatch.Groups[degreesIndex].Get<decimal>() +
                                 locationMatch.Groups[minutesIndex].Get<decimal>() / 60000;

            return !BitUtl.IsTrue(flags, i2) ? -coordinate : coordinate;
        }
    }
}