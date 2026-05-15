using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MAItems.Database
{
    public class DatabaseHelper
    {
        private readonly string _connectionString;

        public DatabaseHelper(string dbFileName = "app_data.db")
        {
            string dbPath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory, dbFileName);
            _connectionString = $"Data Source={dbPath}";
            InitializeDatabase();
        }

        // ══════════════════════════════════════════════════════
        // 初期化
        // ══════════════════════════════════════════════════════

        private void InitializeDatabase()
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();

            string sqlDeals = @"
                CREATE TABLE IF NOT EXISTS Deals (
                    Id                  INTEGER PRIMARY KEY AUTOINCREMENT,
                    InputDate           TEXT,
                    Route               TEXT,
                    BrokerCompany       TEXT,
                    Title               TEXT,
                    DealId              TEXT,
                    BusinessContent     TEXT,
                    Area                TEXT,
                    Revenue             TEXT,
                    OperatingProfit     TEXT,
                    EBITDA              TEXT,
                    NetAssets           TEXT,
                    TotalAssets         TEXT,
                    NetCashDebt         TEXT,
                    CashEquivalents     TEXT,
                    InterestBearingDebt TEXT,
                    EmployeeCount       TEXT,
                    Features            TEXT,
                    AskingPrice         TEXT,
                    TransferType        TEXT,
                    TransferReason      TEXT,
                    TransferConditions  TEXT,
                    Status              TEXT
                );";

            using (var cmd = new SqliteCommand(sqlDeals, conn))
                cmd.ExecuteNonQuery();

            string sqlNumeric = @"
                CREATE TABLE IF NOT EXISTS DealsNumeric (
                    Id                  INTEGER PRIMARY KEY,
                    InputDate           TEXT,
                    Route               TEXT,
                    BrokerCompany       TEXT,
                    Title               TEXT,
                    DealId              TEXT,
                    BusinessContent     TEXT,
                    Area                TEXT,
                    Revenue             REAL,
                    OperatingProfit     REAL,
                    EBITDA              REAL,
                    NetAssets           REAL,
                    TotalAssets         REAL,
                    NetCashDebt         REAL,
                    CashEquivalents     REAL,
                    InterestBearingDebt REAL,
                    EmployeeCount       REAL,
                    Features            TEXT,
                    AskingPrice         REAL,
                    TransferType        TEXT,
                    TransferReason      TEXT,
                    TransferConditions  TEXT,
                    Status              TEXT,
                    ConvertedAt         TEXT
                );";

            using (var cmd = new SqliteCommand(sqlNumeric, conn))
                cmd.ExecuteNonQuery();

            // ✅ 初回マイグレーション
            // DealsNumeric が空 かつ Deals にデータがある場合のみ全件構築
            MigrateNumericIfEmpty(conn);
        }

        // ══════════════════════════════════════════════════════
        // Deals CRUD
        // ══════════════════════════════════════════════════════

        // ─── 初回マイグレーション ──────────────────────────────
        /// <summary>
        /// DealsNumeric が空で Deals にデータがある場合のみ
        /// 全件変換して DealsNumeric を構築する。
        /// 既存DBをこのアプリで初めて開いた場合の自動補完用。
        /// </summary>
        private void MigrateNumericIfEmpty(SqliteConnection conn)
        {
            // DealsNumeric のレコード数を確認
            using var cntNumeric = new SqliteCommand(
                "SELECT COUNT(1) FROM DealsNumeric;", conn);
            long numericCount = (long)cntNumeric.ExecuteScalar()!;

            // すでにデータがあれば何もしない
            if (numericCount > 0) return;

            // Deals のレコード数を確認
            using var cntDeals = new SqliteCommand(
                "SELECT COUNT(1) FROM Deals;", conn);
            long dealsCount = (long)cntDeals.ExecuteScalar()!;

            // Deals も空なら何もしない
            if (dealsCount == 0) return;

            // Deals にデータがあり DealsNumeric が空 → 全件構築
            RebuildNumericTable();
        }


        public List<Deal> GetAllDeals()
        {
            var list = new List<Deal>();

            using var conn = new SqliteConnection(_connectionString);
            conn.Open();

            using var cmd = new SqliteCommand(
                "SELECT * FROM Deals ORDER BY Id;", conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
                list.Add(MapDeal(reader));

            return list;
        }

        public List<Deal> SearchDeals(string keyword)
        {
            var list = new List<Deal>();

            using var conn = new SqliteConnection(_connectionString);
            conn.Open();

            string sql = @"
                SELECT * FROM Deals
                WHERE  Title           LIKE @kw
                OR     BusinessContent LIKE @kw
                OR     Area            LIKE @kw
                OR     BrokerCompany   LIKE @kw
                OR     Status          LIKE @kw
                ORDER BY Id;";

            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@kw", $"%{keyword}%");
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
                list.Add(MapDeal(reader));

            return list;
        }

        private Deal? GetDealById(long id)
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();

            using var cmd = new SqliteCommand(
                "SELECT * FROM Deals WHERE Id = @Id;", conn);
            cmd.Parameters.AddWithValue("@Id", id);

            using var reader = cmd.ExecuteReader();
            return reader.Read() ? MapDeal(reader) : null;
        }

        public void AddDeal(Deal d)
        {
            long newId;

            using (var conn = new SqliteConnection(_connectionString))
            {
                conn.Open();

                string sql = @"
                    INSERT INTO Deals (
                        InputDate, Route, BrokerCompany, Title, DealId,
                        BusinessContent, Area, Revenue, OperatingProfit, EBITDA,
                        NetAssets, TotalAssets, NetCashDebt, CashEquivalents,
                        InterestBearingDebt, EmployeeCount, Features,
                        AskingPrice, TransferType, TransferReason,
                        TransferConditions, Status
                    ) VALUES (
                        @InputDate, @Route, @BrokerCompany, @Title, @DealId,
                        @BusinessContent, @Area, @Revenue, @OperatingProfit, @EBITDA,
                        @NetAssets, @TotalAssets, @NetCashDebt, @CashEquivalents,
                        @InterestBearingDebt, @EmployeeCount, @Features,
                        @AskingPrice, @TransferType, @TransferReason,
                        @TransferConditions, @Status
                    );
                    SELECT last_insert_rowid();";

                using var cmd = new SqliteCommand(sql, conn);
                BindParameters(cmd, d);
                newId = (long)cmd.ExecuteScalar()!;
            }

            UpsertNumeric(newId);
        }

        public long AddEmptyDeal()
        {
            long newId;

            using (var conn = new SqliteConnection(_connectionString))
            {
                conn.Open();

                string sql = @"
                    INSERT INTO Deals (
                        InputDate, Route, BrokerCompany, Title, DealId,
                        BusinessContent, Area, Revenue, OperatingProfit, EBITDA,
                        NetAssets, TotalAssets, NetCashDebt, CashEquivalents,
                        InterestBearingDebt, EmployeeCount, Features,
                        AskingPrice, TransferType, TransferReason,
                        TransferConditions, Status
                    ) VALUES (
                        @InputDate, '', '', '', '',
                        '', '', '', '', '',
                        '', '', '', '',
                        '', '', '',
                        '', '', '',
                        '', ''
                    );
                    SELECT last_insert_rowid();";

                using var cmd = new SqliteCommand(sql, conn);
                cmd.Parameters.AddWithValue(
                    "@InputDate",
                    DateTime.Now.ToString("yyyy/M/d"));
                newId = (long)cmd.ExecuteScalar()!;
            }

            UpsertNumeric(newId);
            return newId;
        }

        public void UpdateDeal(Deal d)
        {
            using (var conn = new SqliteConnection(_connectionString))
            {
                conn.Open();

                string sql = @"
                    UPDATE Deals SET
                        InputDate           = @InputDate,
                        Route               = @Route,
                        BrokerCompany       = @BrokerCompany,
                        Title               = @Title,
                        DealId              = @DealId,
                        BusinessContent     = @BusinessContent,
                        Area                = @Area,
                        Revenue             = @Revenue,
                        OperatingProfit     = @OperatingProfit,
                        EBITDA              = @EBITDA,
                        NetAssets           = @NetAssets,
                        TotalAssets         = @TotalAssets,
                        NetCashDebt         = @NetCashDebt,
                        CashEquivalents     = @CashEquivalents,
                        InterestBearingDebt = @InterestBearingDebt,
                        EmployeeCount       = @EmployeeCount,
                        Features            = @Features,
                        AskingPrice         = @AskingPrice,
                        TransferType        = @TransferType,
                        TransferReason      = @TransferReason,
                        TransferConditions  = @TransferConditions,
                        Status              = @Status
                    WHERE Id = @Id;";

                using var cmd = new SqliteCommand(sql, conn);
                BindParameters(cmd, d);
                cmd.Parameters.AddWithValue("@Id", d.Id);
                cmd.ExecuteNonQuery();
            }

            UpsertNumeric(d.Id);
        }

        public void DeleteDeal(long id)
        {
            using (var conn = new SqliteConnection(_connectionString))
            {
                conn.Open();

                using var cmd = new SqliteCommand(
                    "DELETE FROM Deals WHERE Id = @Id;", conn);
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.ExecuteNonQuery();
            }

            DeleteNumeric(id);
        }

        public (int added, int skipped) ImportFromCsv(string filePath)
        {
            var rows = CsvParser.Parse(filePath);
            int added = 0, skipped = 0;

            using (var conn = new SqliteConnection(_connectionString))
            {
                conn.Open();

                foreach (var row in rows)
                {
                    // ✅ 仲介会社と案件IDの両方が空欄の場合のみチェックをスキップ
                    bool hasBroker = !string.IsNullOrWhiteSpace(row.BrokerCompany);
                    bool hasDealId = !string.IsNullOrWhiteSpace(row.DealId);

                    if (hasBroker || hasDealId)
                    {
                        // 仲介会社 + 案件ID の組み合わせで重複チェック
                        using var chk = new SqliteCommand(@"
                    SELECT COUNT(1) FROM Deals
                    WHERE BrokerCompany = @BrokerCompany
                    AND   DealId        = @DealId;",
                            conn);
                        chk.Parameters.AddWithValue("@BrokerCompany",
                            row.BrokerCompany);
                        chk.Parameters.AddWithValue("@DealId",
                            row.DealId);

                        long count = (long)chk.ExecuteScalar()!;
                        if (count > 0) { skipped++; continue; }
                    }

                    string sql = @"
                INSERT INTO Deals (
                    InputDate, Route, BrokerCompany, Title, DealId,
                    BusinessContent, Area, Revenue, OperatingProfit, EBITDA,
                    NetAssets, TotalAssets, NetCashDebt, CashEquivalents,
                    InterestBearingDebt, EmployeeCount, Features,
                    AskingPrice, TransferType, TransferReason,
                    TransferConditions, Status
                ) VALUES (
                    @InputDate, @Route, @BrokerCompany, @Title, @DealId,
                    @BusinessContent, @Area, @Revenue, @OperatingProfit, @EBITDA,
                    @NetAssets, @TotalAssets, @NetCashDebt, @CashEquivalents,
                    @InterestBearingDebt, @EmployeeCount, @Features,
                    @AskingPrice, @TransferType, @TransferReason,
                    @TransferConditions, @Status
                );";

                    using var cmd = new SqliteCommand(sql, conn);
                    BindParameters(cmd, row);
                    cmd.ExecuteNonQuery();
                    added++;
                }
            }

            if (added > 0)
                RebuildNumericTable();

            return (added, skipped);
        }

        public void ExportToCsv(string filePath, List<Deal> deals)
        {
            using var writer = new StreamWriter(
                filePath, false, new UTF8Encoding(true));

            writer.WriteLine(
                "入力日,経路,仲介会社,タイトル,案件ID,事業内容,エリア," +
                "売上高,営業利益,EBITDA,純資産額,総資産額,NET Cash/Debt," +
                "現金・現金同等物,有利子負債等,従業員数,特徴," +
                "譲渡希望額,譲渡希望形態,譲渡希望理由,希望譲渡条件,処理");

            foreach (var d in deals)
            {
                writer.WriteLine(string.Join(",", new[]
                {
                    Escape(d.InputDate),
                    Escape(d.Route),
                    Escape(d.BrokerCompany),
                    Escape(d.Title),
                    Escape(d.DealId),
                    Escape(d.BusinessContent),
                    Escape(d.Area),
                    Escape(d.Revenue),
                    Escape(d.OperatingProfit),
                    Escape(d.EBITDA),
                    Escape(d.NetAssets),
                    Escape(d.TotalAssets),
                    Escape(d.NetCashDebt),
                    Escape(d.CashEquivalents),
                    Escape(d.InterestBearingDebt),
                    Escape(d.EmployeeCount),
                    Escape(d.Features),
                    Escape(d.AskingPrice),
                    Escape(d.TransferType),
                    Escape(d.TransferReason),
                    Escape(d.TransferConditions),
                    Escape(d.Status),
                }));
            }
        }

        // ══════════════════════════════════════════════════════
        // DealsNumeric 同期
        // ══════════════════════════════════════════════════════

        private void UpsertNumeric(long id)
        {
            Deal? deal = GetDealById(id);
            if (deal == null) return;

            string now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            using var conn = new SqliteConnection(_connectionString);
            conn.Open();

            string sql = @"
                INSERT OR REPLACE INTO DealsNumeric (
                    Id, InputDate, Route, BrokerCompany, Title, DealId,
                    BusinessContent, Area,
                    Revenue, OperatingProfit, EBITDA,
                    NetAssets, TotalAssets, NetCashDebt,
                    CashEquivalents, InterestBearingDebt, EmployeeCount,
                    Features, AskingPrice, TransferType,
                    TransferReason, TransferConditions, Status,
                    ConvertedAt
                ) VALUES (
                    @Id, @InputDate, @Route, @BrokerCompany, @Title, @DealId,
                    @BusinessContent, @Area,
                    @Revenue, @OperatingProfit, @EBITDA,
                    @NetAssets, @TotalAssets, @NetCashDebt,
                    @CashEquivalents, @InterestBearingDebt, @EmployeeCount,
                    @Features, @AskingPrice, @TransferType,
                    @TransferReason, @TransferConditions, @Status,
                    @ConvertedAt
                );";

            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Id", deal.Id);
            cmd.Parameters.AddWithValue("@InputDate", deal.InputDate);
            cmd.Parameters.AddWithValue("@Route", deal.Route);
            cmd.Parameters.AddWithValue("@BrokerCompany", deal.BrokerCompany);
            cmd.Parameters.AddWithValue("@Title", deal.Title);
            cmd.Parameters.AddWithValue("@DealId", deal.DealId);
            cmd.Parameters.AddWithValue("@BusinessContent", deal.BusinessContent);
            cmd.Parameters.AddWithValue("@Area", deal.Area);
            cmd.Parameters.AddWithValue("@Features", deal.Features);
            cmd.Parameters.AddWithValue("@TransferType", deal.TransferType);
            cmd.Parameters.AddWithValue("@TransferReason", deal.TransferReason);
            cmd.Parameters.AddWithValue("@TransferConditions", deal.TransferConditions);
            cmd.Parameters.AddWithValue("@Status", deal.Status);
            cmd.Parameters.AddWithValue("@ConvertedAt", now);

            BindNullableReal(cmd, "@Revenue", deal.Revenue);
            BindNullableReal(cmd, "@OperatingProfit", deal.OperatingProfit);
            BindNullableReal(cmd, "@EBITDA", deal.EBITDA);
            BindNullableReal(cmd, "@NetAssets", deal.NetAssets);
            BindNullableReal(cmd, "@TotalAssets", deal.TotalAssets);
            BindNullableReal(cmd, "@NetCashDebt", deal.NetCashDebt);
            BindNullableReal(cmd, "@CashEquivalents", deal.CashEquivalents);
            BindNullableReal(cmd, "@InterestBearingDebt", deal.InterestBearingDebt);
            BindNullableReal(cmd, "@EmployeeCount", deal.EmployeeCount);
            BindNullableReal(cmd, "@AskingPrice", deal.AskingPrice);

            cmd.ExecuteNonQuery();
        }

        private void DeleteNumeric(long id)
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();

            using var cmd = new SqliteCommand(
                "DELETE FROM DealsNumeric WHERE Id = @Id;", conn);
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.ExecuteNonQuery();
        }

        public int RebuildNumericTable()
        {
            var deals = GetAllDeals();
            string now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            using var conn = new SqliteConnection(_connectionString);
            conn.Open();

            using (var del = new SqliteCommand(
                "DELETE FROM DealsNumeric;", conn))
                del.ExecuteNonQuery();

            string sql = @"
                INSERT INTO DealsNumeric (
                    Id, InputDate, Route, BrokerCompany, Title, DealId,
                    BusinessContent, Area,
                    Revenue, OperatingProfit, EBITDA,
                    NetAssets, TotalAssets, NetCashDebt,
                    CashEquivalents, InterestBearingDebt, EmployeeCount,
                    Features, AskingPrice, TransferType,
                    TransferReason, TransferConditions, Status,
                    ConvertedAt
                ) VALUES (
                    @Id, @InputDate, @Route, @BrokerCompany, @Title, @DealId,
                    @BusinessContent, @Area,
                    @Revenue, @OperatingProfit, @EBITDA,
                    @NetAssets, @TotalAssets, @NetCashDebt,
                    @CashEquivalents, @InterestBearingDebt, @EmployeeCount,
                    @Features, @AskingPrice, @TransferType,
                    @TransferReason, @TransferConditions, @Status,
                    @ConvertedAt
                );";

            foreach (var d in deals)
            {
                using var cmd = new SqliteCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Id", d.Id);
                cmd.Parameters.AddWithValue("@InputDate", d.InputDate);
                cmd.Parameters.AddWithValue("@Route", d.Route);
                cmd.Parameters.AddWithValue("@BrokerCompany", d.BrokerCompany);
                cmd.Parameters.AddWithValue("@Title", d.Title);
                cmd.Parameters.AddWithValue("@DealId", d.DealId);
                cmd.Parameters.AddWithValue("@BusinessContent", d.BusinessContent);
                cmd.Parameters.AddWithValue("@Area", d.Area);
                cmd.Parameters.AddWithValue("@Features", d.Features);
                cmd.Parameters.AddWithValue("@TransferType", d.TransferType);
                cmd.Parameters.AddWithValue("@TransferReason", d.TransferReason);
                cmd.Parameters.AddWithValue("@TransferConditions", d.TransferConditions);
                cmd.Parameters.AddWithValue("@Status", d.Status);
                cmd.Parameters.AddWithValue("@ConvertedAt", now);

                BindNullableReal(cmd, "@Revenue", d.Revenue);
                BindNullableReal(cmd, "@OperatingProfit", d.OperatingProfit);
                BindNullableReal(cmd, "@EBITDA", d.EBITDA);
                BindNullableReal(cmd, "@NetAssets", d.NetAssets);
                BindNullableReal(cmd, "@TotalAssets", d.TotalAssets);
                BindNullableReal(cmd, "@NetCashDebt", d.NetCashDebt);
                BindNullableReal(cmd, "@CashEquivalents", d.CashEquivalents);
                BindNullableReal(cmd, "@InterestBearingDebt", d.InterestBearingDebt);
                BindNullableReal(cmd, "@EmployeeCount", d.EmployeeCount);
                BindNullableReal(cmd, "@AskingPrice", d.AskingPrice);

                cmd.ExecuteNonQuery();
            }

            return deals.Count;
        }

        // ══════════════════════════════════════════════════════
        // DealsNumeric 取得
        // ══════════════════════════════════════════════════════

        public List<DealNumeric> GetAllDealNumerics()
        {
            var list = new List<DealNumeric>();

            using var conn = new SqliteConnection(_connectionString);
            conn.Open();

            using var cmd = new SqliteCommand(
                "SELECT * FROM DealsNumeric ORDER BY Id;", conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
                list.Add(MapDealNumeric(reader));

            return list;
        }

        public List<DealNumeric> SearchDealNumerics(string keyword)
        {
            var list = new List<DealNumeric>();

            using var conn = new SqliteConnection(_connectionString);
            conn.Open();

            string sql = @"
                SELECT * FROM DealsNumeric
                WHERE  Title           LIKE @kw
                OR     BusinessContent LIKE @kw
                OR     Area            LIKE @kw
                OR     BrokerCompany   LIKE @kw
                OR     Status          LIKE @kw
                ORDER BY Id;";

            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@kw", $"%{keyword}%");
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
                list.Add(MapDealNumeric(reader));

            return list;
        }

        // ══════════════════════════════════════════════════════
        // ヘルパー
        // ══════════════════════════════════════════════════════

        private static Deal MapDeal(SqliteDataReader r) => new Deal
        {
            Id = r.GetInt64(r.GetOrdinal("Id")),
            InputDate = StrD(r, "InputDate"),
            Route = StrD(r, "Route"),
            BrokerCompany = StrD(r, "BrokerCompany"),
            Title = StrD(r, "Title"),
            DealId = StrD(r, "DealId"),
            BusinessContent = StrD(r, "BusinessContent"),
            Area = StrD(r, "Area"),
            Revenue = StrD(r, "Revenue"),
            OperatingProfit = StrD(r, "OperatingProfit"),
            EBITDA = StrD(r, "EBITDA"),
            NetAssets = StrD(r, "NetAssets"),
            TotalAssets = StrD(r, "TotalAssets"),
            NetCashDebt = StrD(r, "NetCashDebt"),
            CashEquivalents = StrD(r, "CashEquivalents"),
            InterestBearingDebt = StrD(r, "InterestBearingDebt"),
            EmployeeCount = StrD(r, "EmployeeCount"),
            Features = StrD(r, "Features"),
            AskingPrice = StrD(r, "AskingPrice"),
            TransferType = StrD(r, "TransferType"),
            TransferReason = StrD(r, "TransferReason"),
            TransferConditions = StrD(r, "TransferConditions"),
            Status = StrD(r, "Status"),
        };

        private static DealNumeric MapDealNumeric(SqliteDataReader r) => new DealNumeric
        {
            Id = r.GetInt64(r.GetOrdinal("Id")),
            InputDate = StrD(r, "InputDate"),
            Route = StrD(r, "Route"),
            BrokerCompany = StrD(r, "BrokerCompany"),
            Title = StrD(r, "Title"),
            DealId = StrD(r, "DealId"),
            BusinessContent = StrD(r, "BusinessContent"),
            Area = StrD(r, "Area"),
            Revenue = RealN(r, "Revenue"),
            OperatingProfit = RealN(r, "OperatingProfit"),
            EBITDA = RealN(r, "EBITDA"),
            NetAssets = RealN(r, "NetAssets"),
            TotalAssets = RealN(r, "TotalAssets"),
            NetCashDebt = RealN(r, "NetCashDebt"),
            CashEquivalents = RealN(r, "CashEquivalents"),
            InterestBearingDebt = RealN(r, "InterestBearingDebt"),
            EmployeeCount = RealN(r, "EmployeeCount"),
            Features = StrD(r, "Features"),
            AskingPrice = RealN(r, "AskingPrice"),
            TransferType = StrD(r, "TransferType"),
            TransferReason = StrD(r, "TransferReason"),
            TransferConditions = StrD(r, "TransferConditions"),
            Status = StrD(r, "Status"),
            ConvertedAt = StrD(r, "ConvertedAt"),
        };

        private static void BindParameters(SqliteCommand cmd, Deal d)
        {
            cmd.Parameters.AddWithValue("@InputDate", d.InputDate);
            cmd.Parameters.AddWithValue("@Route", d.Route);
            cmd.Parameters.AddWithValue("@BrokerCompany", d.BrokerCompany);
            cmd.Parameters.AddWithValue("@Title", d.Title);
            cmd.Parameters.AddWithValue("@DealId", d.DealId);
            cmd.Parameters.AddWithValue("@BusinessContent", d.BusinessContent);
            cmd.Parameters.AddWithValue("@Area", d.Area);
            cmd.Parameters.AddWithValue("@Revenue", d.Revenue);
            cmd.Parameters.AddWithValue("@OperatingProfit", d.OperatingProfit);
            cmd.Parameters.AddWithValue("@EBITDA", d.EBITDA);
            cmd.Parameters.AddWithValue("@NetAssets", d.NetAssets);
            cmd.Parameters.AddWithValue("@TotalAssets", d.TotalAssets);
            cmd.Parameters.AddWithValue("@NetCashDebt", d.NetCashDebt);
            cmd.Parameters.AddWithValue("@CashEquivalents", d.CashEquivalents);
            cmd.Parameters.AddWithValue("@InterestBearingDebt", d.InterestBearingDebt);
            cmd.Parameters.AddWithValue("@EmployeeCount", d.EmployeeCount);
            cmd.Parameters.AddWithValue("@Features", d.Features);
            cmd.Parameters.AddWithValue("@AskingPrice", d.AskingPrice);
            cmd.Parameters.AddWithValue("@TransferType", d.TransferType);
            cmd.Parameters.AddWithValue("@TransferReason", d.TransferReason);
            cmd.Parameters.AddWithValue("@TransferConditions", d.TransferConditions);
            cmd.Parameters.AddWithValue("@Status", d.Status);
        }

        private static void BindNullableReal(
            SqliteCommand cmd, string paramName, string rawValue)
        {
            double? converted = NumericConverter.Convert(rawValue);
            if (converted.HasValue)
                cmd.Parameters.AddWithValue(paramName, converted.Value);
            else
                cmd.Parameters.AddWithValue(paramName, DBNull.Value);
        }

        private static string StrD(SqliteDataReader r, string col)
            => r.IsDBNull(r.GetOrdinal(col))
               ? string.Empty
               : r.GetString(r.GetOrdinal(col));

        private static double? RealN(SqliteDataReader r, string col)
            => r.IsDBNull(r.GetOrdinal(col))
               ? null
               : r.GetDouble(r.GetOrdinal(col));

        private static string Escape(string value)
        {
            if (value.Contains(',') || value.Contains('"') ||
                value.Contains('\n') || value.Contains('\r'))
                return $"\"{value.Replace("\"", "\"\"")}\"";
            return value;
        }
    }
}