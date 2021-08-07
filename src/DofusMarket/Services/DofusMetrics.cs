using Npgsql;

namespace DofusMarket.Services
{
    internal class DofusMetrics
    {
        private readonly string _connString;

        public DofusMetrics(string connString)
        {
            _connString = connString;
        }

        public void WriteItemPrice(int serverId, int itemId, int itemTypeId, int stackSize, int price)
        {
            using NpgsqlConnection conn = new(_connString);
            conn.Open();

            using NpgsqlCommand cmd = new(@"
INSERT INTO item_prices(time, server_id, item_id, item_type_id, stack_size, price)
VALUES (NOW(), @server_id, @item_id, @item_type_id, @stack_size, @price);",
                conn);
            cmd.Parameters.AddWithValue("server_id", serverId);
            cmd.Parameters.AddWithValue("item_id", itemId);
            cmd.Parameters.AddWithValue("item_type_id", itemTypeId);
            cmd.Parameters.AddWithValue("stack_size", stackSize);
            cmd.Parameters.AddWithValue("price", price);
            cmd.ExecuteNonQuery();
        }
    }
}
