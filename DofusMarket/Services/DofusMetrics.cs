using System;
using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
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

        public void EmitItemPrice(int itemId, int categoryId, int setSize, int price)
        {
            var point = PointData
                .Measurement("item_price")
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
