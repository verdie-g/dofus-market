using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Dofus.Internationalization;
using Npgsql;

namespace Dofus.DataExporter
{
    class Program
    {
        static async Task Main()
        {
            string dofusPath = Environment.GetEnvironmentVariable("DOFUS_PATH") ?? throw new Exception("DOFUS_PATH not set");
            string connString = Environment.GetEnvironmentVariable("CONNECTION_STRING") ?? throw new Exception("CONNECTION_STRING not set");
            Console.WriteLine(connString);

            DofusData dofusData = DofusData.New(Path.Combine(dofusPath, "data/common"));
            DofusTexts dofusTexts = DofusTexts.New(Path.Combine(dofusPath, "data/i18n"));
            await CreateSchemaAsync(connString);
            await InsertServersAsync(connString, dofusData, dofusTexts);
            await InsertItemsAsync(connString, dofusData, dofusTexts);
        }

        private static Task CreateSchemaAsync(string connString)
        {
            return RunQueryAsync(connString, @"
CREATE TABLE IF NOT EXISTS servers (
    id      INT  NOT NULL PRIMARY KEY,
    name_en TEXT NOT NULL,
    name_fr TEXT NOT NULL,
    name_es TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS items (
    id      INT  NOT NULL PRIMARY KEY,
    name_en TEXT NOT NULL,
    name_fr TEXT NOT NULL,
    name_es TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS item_prices (
    time         TIMESTAMPTZ NOT NULL,
    server_id    INT         NOT NULL,
    item_id      INT         NOT NULL,
    item_type_id INT         NOT NULL,
    stack_size   INT         NOT NULL,
    price        INT         NOT NULL
);

SELECT create_hypertable('item_prices', 'time', chunk_time_interval => INTERVAL '7 days', if_not_exists => TRUE);
CREATE INDEX IF NOT EXISTS item_prices_server_item_stack_idx ON item_prices(server_id, item_id, stack_size);
");
        }

        private static async Task InsertServersAsync(string connString, DofusData dofusData, DofusTexts dofusTexts)
        {
            StringBuilder queryBuilder = new("INSERT INTO servers(id, name_en, name_fr, name_es) VALUES ");
            foreach (var server in dofusData.GetDataForType("Servers"))
            {
                int serverNameId = (int)server.Value["nameId"]!;
                string serverNameEn = dofusTexts.GetText(serverNameId, DofusLanguages.English);
                string serverNameFr = dofusTexts.GetText(serverNameId, DofusLanguages.French);
                string serverNameEs = dofusTexts.GetText(serverNameId, DofusLanguages.Spanish);
                queryBuilder.AppendFormat("({0}, '{1}', '{2}', '{3}'), ", server.Key,
                    serverNameEn.Replace("'", "''"),
                    serverNameFr.Replace("'", "''"),
                    serverNameEs.Replace("'", "''"));
            }

            queryBuilder.Length -= ", ".Length;
            queryBuilder.Append(" ON CONFLICT (id) DO UPDATE SET name_en = EXCLUDED.name_en, name_fr = EXCLUDED.name_fr, name_es = EXCLUDED.name_es;");

            await RunQueryAsync(connString, queryBuilder.ToString());
        }

        private static async Task InsertItemsAsync(string connString, DofusData dofusData, DofusTexts dofusTexts)
        {
            StringBuilder queryBuilder = new("INSERT INTO items(id, name_en, name_fr, name_es) VALUES ");
            foreach (var item in dofusData.GetDataForType("Items"))
            {
                int itemNameId = (int)item.Value["nameId"]!;

                if (!dofusTexts.TryGetText(itemNameId, DofusLanguages.English, out string? itemNameEn))
                {
                    itemNameEn = item.Key.ToString();
                }

                if (!dofusTexts.TryGetText(itemNameId, DofusLanguages.French, out string? itemNameFr))
                {
                    itemNameFr = item.Key.ToString();
                }

                if (!dofusTexts.TryGetText(itemNameId, DofusLanguages.Spanish, out string? itemNameEs))
                {
                    itemNameEs = item.Key.ToString();
                }

                queryBuilder.AppendFormat("({0}, '{1}', '{2}', '{3}'), ", item.Key,
                    itemNameEn.Replace("'", "''"),
                    itemNameFr.Replace("'", "''"),
                    itemNameEs.Replace("'", "''"));
            }

            queryBuilder.Length -= ", ".Length;
            queryBuilder.Append(" ON CONFLICT (id) DO UPDATE SET name_en = EXCLUDED.name_en, name_fr = EXCLUDED.name_fr, name_es = EXCLUDED.name_es;");

            await RunQueryAsync(connString, queryBuilder.ToString());
        }

        private static async Task RunQueryAsync(string connString, string query)
        {
            await using NpgsqlConnection conn = new(connString);
            await conn.OpenAsync();

            await using NpgsqlCommand cmd = new(query, conn);
            await cmd.ExecuteNonQueryAsync();
        }
    }
}
