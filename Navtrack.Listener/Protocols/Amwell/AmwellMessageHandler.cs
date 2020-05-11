using System;
using System.Linq;
using Navtrack.Library.DI;
using Navtrack.Listener.Helpers;
using Navtrack.Listener.Models;
using Navtrack.Listener.Server;

namespace Navtrack.Listener.Protocols.Amwell
{
    [Service(typeof(ICustomMessageHandler<AmwellProtocol>))]
    public class AmwellMessageHandler : BaseMessageHandler<AmwellProtocol>
    {
        public override Location Parse(MessageInput input)
        {
            LoginHandler(input);

            return GetLocation(input);
        }

        private Location GetLocation(MessageInput input)
        {
            input.DataMessage.ByteReader.Skip(9);

            Location location = new Location
            {
                DateTime = new DateTime(int.Parse(input.DataMessage.Hex[9]) + 2000,
                    int.Parse(input.DataMessage.Hex[10]),
                    int.Parse(input.DataMessage.Hex[11]),
                    int.Parse(input.DataMessage.Hex[12]),
                    int.Parse(input.DataMessage.Hex[13]),
                    int.Parse(input.DataMessage.Hex[14])),
                Latitude = GetCoordinate(input.DataMessage.Hex[15..19], input.DataMessage.Bytes[15]),
                Longitude = GetCoordinate(input.DataMessage.Hex[19..23], input.DataMessage.Bytes[19]),
                Speed = decimal.Parse(input.DataMessage.Hex[23..25].StringJoin()),
                Heading = decimal.Parse(input.DataMessage.Hex[25..27].StringJoin())
            };

            return location;
        }

        private static decimal GetCoordinate(string[] input, byte highestByte)
        {
            string coordinate = input.StringJoin();
            
            decimal converted = GpsUtil.ConvertDdmToDecimal(decimal.Parse(coordinate.Substring(0, 3)),
                decimal.Parse(coordinate.Substring(3, 5))/1000, CardinalPoint.North);

            return (highestByte & 0x80) != 0 ? -converted : converted;
        }

        private static void LoginHandler(MessageInput input)
        {
            if (input.DataMessage.Bytes[2] == 0xB1)
            {
                const string header = "2929";
                const string command = "21";
                const int length = 5;
                string receivedCommand = input.DataMessage.Hex[2];
                string receivedChecksum = input.DataMessage.Hex[^2];
                const string data = "06";
                const string end = "0D";

                string reply = $"{header}{command}{length:X4}{receivedChecksum}{receivedCommand}{data}";

                string checksum = GetChecksum(reply);
                string fullReply = $"{reply}{checksum}{end}";

                input.NetworkStream.Write(HexUtil.ConvertHexStringToByteArray(fullReply));
            }
        }

        private static string GetChecksum(string reply)
        {
            int checksum = HexUtil.ConvertHexStringToByteArray(reply)
                .Aggregate(0, (current, next) => current ^ next);

            return $"{checksum:X2}";
        }
    }
}