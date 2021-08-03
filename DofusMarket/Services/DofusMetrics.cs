using System;
using InfluxDB.Client;
using InfluxDB.Client.Writes;

namespace DofusMarket.Services
{
    internal class DofusMetrics : IDisposable
    {
        private readonly InfluxDBClient _influxDb;
        private readonly WriteApi _writeApi;

        public DofusMetrics(InfluxDBClient influxDb)
        {
            _influxDb = influxDb;
            _writeApi = influxDb.GetWriteApi();
        }

        public void EmitItemPrice(string serverName, string itemName, string itemTypeName, int stackSize, int price)
        {
            var point = PointData
                .Measurement("item_price")
                .Tag("server_name_fr", serverName)
                .Tag("item_name_fr", itemName)
                .Tag("item_type_name_fr", itemTypeName)
                .Tag("stack_size", stackSize.ToString())
                .Field("value", price);

            _writeApi.WritePoint(point);
        }

        public void Dispose()
        {
            _writeApi.Dispose();
            _influxDb.Dispose();
        }
    }
}
