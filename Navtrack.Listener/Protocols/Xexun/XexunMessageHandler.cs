using System.Globalization;
using System.Text.RegularExpressions;
using Navtrack.Library.DI;
using Navtrack.Listener.Helpers;
using Navtrack.Listener.Models;
using Navtrack.Listener.Server;

namespace Navtrack.Listener.Protocols.Xexun
{
    [Service(typeof(ICustomMessageHandler<XexunProtocol>))]
    public class XexunMessageHandler : BaseMessageHandler<XexunProtocol>
    {
        public override Location Parse(MessageInput input)
         {
             // TODO: join patterns
             GroupCollection lgc =
                new Regex(
                        @"GPRMC,(\d{2})(\d{2})(\d{2}).(\d{3}),(A|V),(\d{4}.\d{4}),(N|S),(\d{5}.\d{4}),(E|W),(.*?),(.*?),(\d{2})(\d{2})(\d{2})(.*?\*..,)(\w)(.*?imei:)(\d{15})")
                    .Matches(input.DataMessage.String)[0].Groups;

            if (lgc.Count == 19)
            {
                Location location = new Location
                {
                    Device = new Device
                    {
                        IMEI = lgc[18].Value
                    },
                    DateTime = DateTimeUtil.New(lgc[12].Value, lgc[13].Value, lgc[14].Value, lgc[1].Value, lgc[2].Value,
                        lgc[3].Value, lgc[4].Value),
                    PositionStatus = lgc[16].Value == "F",
                    Latitude = GpsUtil.ConvertDmmLatToDecimal(lgc[6].Value, lgc[7].Value),
                    Longitude = GpsUtil.ConvertDmmLongToDecimal(lgc[8].Value, lgc[9].Value),
                    Speed = decimal.Parse(lgc[10].Value) * (decimal) 1.852,
                    Heading = decimal.Parse(lgc[11].Value),
                };

                GroupCollection extra =
                    new Regex(@"(.*?imei:)(\d{15}),(\d+),(.*?),(F:)(.*?)V,(1|0),(.*?),(.*?),(\d+),(\d+),(.*?),(\d+)")
                        .Matches(input.DataMessage.String)[0].Groups;

                if (extra.Count == 14)
                {
                    location.Satellites = short.Parse(extra[3].Value);
                    location.Altitude = decimal.Parse(extra[4].Value);
                    location.MobileCountryCode = int.Parse(extra[10].Value);
                    location.MobileNetworkCode = int.Parse(extra[11].Value, NumberStyles.HexNumber);
                    location.LocationAreaCode = int.Parse(extra[12].Value, NumberStyles.HexNumber);
                    location.CellId = int.Parse(extra[13].Value, NumberStyles.HexNumber);
                }

                return location;
            }

            return null;
        }
    }
}