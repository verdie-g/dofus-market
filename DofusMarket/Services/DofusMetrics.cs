using System;
using InfluxDB.Client;
using InfluxDB.Client.Writes;

namespace DofusMarket.Services
{
    internal class DofusMetrics : IDisposable
    {
        private readonly InfluxDBClient _influxDb;
        private readonly WriteApi _writeApi;
        private readonly int _serverId;

        public DofusMetrics(InfluxDBClient influxDb, int serverId)
        {
            _influxDb = influxDb;
            _serverId = serverId;
            _writeApi = influxDb.GetWriteApi();
        }

        public void EmitItemPrice(int itemId, int categoryId, int setSize, int price)
        {
            var point = PointData
                .Measurement("item_price")
                .Tag("server_id", _serverId.ToString())
                .Tag("item_id", itemId.ToString())
                .Tag("category_id", categoryId.ToString())
                .Tag("set_size", setSize.ToString())
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
