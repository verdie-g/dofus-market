﻿using System;
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
    name_fr TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS items (
    id      INT  NOT NULL PRIMARY KEY,
    name_fr TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS item_prices (
    time         TIMESTAMPTZ NOT NULL,
    server_id    INT         NOT NULL,
    item_id      INT         NOT NULL,
    item_type_id INT         NOT NULL,
    stack_size   INT         NOT NULL,
    price        INT         NOT NULL
);

SELECT create_hypertable('item_prices', 'time', if_not_exists => TRUE);
CREATE INDEX IF NOT EXISTS item_prices_server_item_stack_idx ON item_prices(server_id, item_id, stack_size);
");
        }

        private static async Task InsertServersAsync(string connString, DofusData dofusData, DofusTexts dofusTexts)
        {
            StringBuilder queryBuilder = new("INSERT INTO servers(id, name_fr) VALUES ");
            foreach (var server in dofusData.GetDataForType("Servers"))
            {
                string serverName = dofusTexts.GetText((int)server.Value["nameId"]!, DofusLanguages.French);
                queryBuilder.AppendFormat("({0}, '{1}'), ", server.Key, serverName.Replace("'", "''"));
            }

            queryBuilder.Length -= ", ".Length;
            queryBuilder.Append(" ON CONFLICT (id) DO UPDATE SET name_fr = EXCLUDED.name_fr;");

            await RunQueryAsync(connString, queryBuilder.ToString());
        }

        private static async Task InsertItemsAsync(string connString, DofusData dofusData, DofusTexts dofusTexts)
        {
            StringBuilder queryBuilder = new("INSERT INTO items(id, name_fr) VALUES ");
            foreach (var item in dofusData.GetDataForType("Items"))
            {
                int itemNameId = (int)item.Value["nameId"]!;
                if (!dofusTexts.TryGetText(itemNameId, DofusLanguages.French, out string? itemName))
                {
                    itemName = item.Key.ToString();
                }

                queryBuilder.AppendFormat("({0}, '{1}'), ", item.Key, itemName.Replace("'", "''"));
            }

            queryBuilder.Length -= ", ".Length;
            queryBuilder.Append(" ON CONFLICT (id) DO UPDATE SET name_fr = EXCLUDED.name_fr;");

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
