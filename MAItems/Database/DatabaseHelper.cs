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
            string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, dbFileName);
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
                    Status              TEXT,
                    AttachmentsSummary  TEXT
                );";
            using (var cmd = new SqliteCommand(sqlDeals, conn))
                cmd.ExecuteNonQuery();

            // 既存のDBからのアップデート用（すでにDealsがあり、AttachmentsSummary列がない場合に追加）
            try
            {
                using var cmdAlt = new SqliteCommand("ALTER TABLE Deals ADD COLUMN AttachmentsSummary TEXT;", conn);
                cmdAlt.ExecuteNonQuery();
            }
            catch { /* すでにある場合はエラーになるので無視 */ }

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

            InitializeExtendedTables(conn);

            // 初回マイグレーション
            MigrateNumericIfEmpty(conn);
        }

        private void InitializeExtendedTables(SqliteConnection conn)
        {
            // CompanyProfiles テーブル
            string sqlCompany = @"
                CREATE TABLE IF NOT EXISTS CompanyProfiles (
                    Id                     INTEGER PRIMARY KEY AUTOINCREMENT,
                    DealId                 INTEGER NOT NULL UNIQUE,
                    CompanyName            TEXT,
                    CompanyNameSub         TEXT,
                    HeadOfficeAddress      TEXT,
                    FactoryAddress         TEXT,
                    OtherOffice            TEXT,
                    Founded                TEXT,
                    Founded2               TEXT,
                    Capital                TEXT,
                    RepresentativeName     TEXT,
                    RepresentativeProfile  TEXT,
                    ShareholderInfo        TEXT,
                    BusinessDetail         TEXT,
                    Revenue                TEXT,
                    Employees              TEXT,
                    MainClients            TEXT,
                    MainSuppliers          TEXT,
                    Certifications         TEXT,
                    GroupCompanies         TEXT,
                    TransferReason         TEXT,
                    Remarks                TEXT
                );";
            using (var cmd = new SqliteCommand(sqlCompany, conn))
                cmd.ExecuteNonQuery();

            // FinancialHighlights テーブル
            string sqlFinancial = @"
                CREATE TABLE IF NOT EXISTS FinancialHighlights (
                    Id                       INTEGER PRIMARY KEY AUTOINCREMENT,
                    DealId                   INTEGER NOT NULL,
                    PeriodType               TEXT NOT NULL,
                    PeriodOrder              INTEGER NOT NULL,
                    PeriodLabel              TEXT,
                    Revenue                  REAL,
                    CostRate                 REAL,
                    GrossProfit              REAL,
                    GrossProfitRate          REAL,
                    SGA                      REAL,
                    OperatingProfit          REAL,
                    OperatingProfitRate      REAL,
                    OrdinaryProfit           REAL,
                    NetIncome                REAL,
                    EBITDA                   REAL,
                    Depreciation             REAL,
                    CapEx                    REAL,
                    CurrentAssets            REAL,
                    CashEquivalents          REAL,
                    AccountsReceivable       REAL,
                    Inventory                REAL,
                    OtherCurrentAssets       REAL,
                    FixedAssets              REAL,
                    TotalAssets              REAL,
                    CurrentLiabilities       REAL,
                    AccountsPayable          REAL,
                    ShortTermDebt            REAL,
                    OtherCurrentLiabilities  REAL,
                    FixedLiabilities         REAL,
                    LongTermDebt             REAL,
                    OtherFixedLiabilities    REAL,
                    TotalLiabilities         REAL,
                    NetAssets                REAL,
                    RetainedEarnings         REAL,
                    UNIQUE(DealId, PeriodType, PeriodOrder)
                );";
            using (var cmd = new SqliteCommand(sqlFinancial, conn))
                cmd.ExecuteNonQuery();

            // ValuationData テーブル
            string sqlValuation = @"
                CREATE TABLE IF NOT EXISTS ValuationData (
                    Id                   INTEGER PRIMARY KEY AUTOINCREMENT,
                    DealId               INTEGER NOT NULL UNIQUE,
                    NetAssetValue        REAL,
                    NetAssetNote         TEXT,
                    EBITDABase           REAL,
                    EBITDABaseYear       TEXT,
                    EBITDAMultiple       REAL,
                    EBITDANetCashDebt    REAL,
                    EBITDANote           TEXT,
                    DCFDiscountRate      REAL,
                    DCFTerminalGrowth    REAL,
                    DCFEV                REAL,
                    DCFNetCashDebt       REAL,
                    DCFNote              TEXT,
                    NOI                  REAL,
                    CapRate              REAL,
                    DirectNetCashDebt    REAL,
                    DirectNote           TEXT,
                    EBITDAEquityValue    REAL,
                    DCFEquityValue       REAL,
                    DirectEquityValue    REAL,
                    ValuationNote        TEXT
                );";
            using (var cmd = new SqliteCommand(sqlValuation, conn))
                cmd.ExecuteNonQuery();

            // Attachments テーブル (添付ファイル用)
            string sqlAttachments = @"
                CREATE TABLE IF NOT EXISTS Attachments (
                    Id          INTEGER PRIMARY KEY AUTOINCREMENT,
                    DealId      INTEGER NOT NULL,
                    FileName    TEXT NOT NULL,
                    FilePath    TEXT NOT NULL,
                    Description TEXT,
                    UploadedAt  TEXT
                );";
            using (var cmd = new SqliteCommand(sqlAttachments, conn))
                cmd.ExecuteNonQuery();
        }

        // ─── 初回マイグレーション ──────────────────────────────
        private void MigrateNumericIfEmpty(SqliteConnection conn)
        {
            using var cntNumeric = new SqliteCommand("SELECT COUNT(1) FROM DealsNumeric;", conn);
            long numericCount = (long)cntNumeric.ExecuteScalar()!;
            if (numericCount > 0) return;

            using var cntDeals = new SqliteCommand("SELECT COUNT(1) FROM Deals;", conn);
            long dealsCount = (long)cntDeals.ExecuteScalar()!;
            if (dealsCount == 0) return;

            RebuildNumericTable();
        }

        // ══════════════════════════════════════════════════════
        // Attachments CRUD (添付資料)
        // ══════════════════════════════════════════════════════

        public List<Attachment> GetAttachments(long dealId)
        {
            var list = new List<Attachment>();
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();
            using var cmd = new SqliteCommand("SELECT * FROM Attachments WHERE DealId = @DealId ORDER BY Id;", conn);
            cmd.Parameters.AddWithValue("@DealId", dealId);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new Attachment
                {
                    Id = reader.GetInt64(0),
                    DealId = reader.GetInt64(1),
                    FileName = reader.GetString(2),
                    FilePath = reader.GetString(3),
                    Description = reader.IsDBNull(4) ? "" : reader.GetString(4),
                    UploadedAt = reader.GetString(5)
                });
            }
            return list;
        }

        public void SaveAttachment(Attachment a)
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();
            string sql = @"INSERT OR REPLACE INTO Attachments (Id, DealId, FileName, FilePath, Description, UploadedAt) 
                           VALUES (@Id, @DealId, @FileName, @FilePath, @Description, @UploadedAt);";
            using var cmd = new SqliteCommand(sql, conn);
            if (a.Id > 0) cmd.Parameters.AddWithValue("@Id", a.Id); else cmd.Parameters.AddWithValue("@Id", DBNull.Value);
            cmd.Parameters.AddWithValue("@DealId", a.DealId);
            cmd.Parameters.AddWithValue("@FileName", a.FileName);
            cmd.Parameters.AddWithValue("@FilePath", a.FilePath);
            cmd.Parameters.AddWithValue("@Description", a.Description ?? "");
            cmd.Parameters.AddWithValue("@UploadedAt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            cmd.ExecuteNonQuery();
        }

        public void DeleteAttachment(long id)
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();
            using var cmd = new SqliteCommand("DELETE FROM Attachments WHERE Id = @Id;", conn);
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.ExecuteNonQuery();
        }

        // ══════════════════════════════════════════════════════
        // Deals CRUD
        // ══════════════════════════════════════════════════════

        public List<Deal> GetAllDeals()
        {
            var list = new List<Deal>();
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();
            using var cmd = new SqliteCommand("SELECT * FROM Deals ORDER BY Id;", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read()) list.Add(MapDeal(reader));
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
            while (reader.Read()) list.Add(MapDeal(reader));
            return list;
        }

        private Deal? GetDealById(long id)
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();
            using var cmd = new SqliteCommand("SELECT * FROM Deals WHERE Id = @Id;", conn);
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
                        TransferConditions, Status, AttachmentsSummary
                    ) VALUES (
                        @InputDate, @Route, @BrokerCompany, @Title, @DealId,
                        @BusinessContent, @Area, @Revenue, @OperatingProfit, @EBITDA,
                        @NetAssets, @TotalAssets, @NetCashDebt, @CashEquivalents,
                        @InterestBearingDebt, @EmployeeCount, @Features,
                        @AskingPrice, @TransferType, @TransferReason,
                        @TransferConditions, @Status, @AttachmentsSummary
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
                        TransferConditions, Status, AttachmentsSummary
                    ) VALUES (
                        @InputDate, '', '', '', '',
                        '', '', '', '', '',
                        '', '', '', '',
                        '', '', '',
                        '', '', '',
                        '', '', ''
                    );
                    SELECT last_insert_rowid();";
                using var cmd = new SqliteCommand(sql, conn);
                cmd.Parameters.AddWithValue("@InputDate", DateTime.Now.ToString("yyyy/M/d"));
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
                        Status              = @Status,
                        AttachmentsSummary  = @AttachmentsSummary
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
                using var cmd = new SqliteCommand("DELETE FROM Deals WHERE Id = @Id;", conn);
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
                    bool hasBroker = !string.IsNullOrWhiteSpace(row.BrokerCompany);
                    bool hasDealId = !string.IsNullOrWhiteSpace(row.DealId);
                    if (hasBroker || hasDealId)
                    {
                        using var chk = new SqliteCommand(@"
                            SELECT COUNT(1) FROM Deals
                            WHERE BrokerCompany = @BrokerCompany
                            AND   DealId        = @DealId;", conn);
                        chk.Parameters.AddWithValue("@BrokerCompany", row.BrokerCompany);
                        chk.Parameters.AddWithValue("@DealId", row.DealId);
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
                            TransferConditions, Status, AttachmentsSummary
                        ) VALUES (
                            @InputDate, @Route, @BrokerCompany, @Title, @DealId,
                            @BusinessContent, @Area, @Revenue, @OperatingProfit, @EBITDA,
                            @NetAssets, @TotalAssets, @NetCashDebt, @CashEquivalents,
                            @InterestBearingDebt, @EmployeeCount, @Features,
                            @AskingPrice, @TransferType, @TransferReason,
                            @TransferConditions, @Status, @AttachmentsSummary
                        );";
                    using var cmd = new SqliteCommand(sql, conn);
                    BindParameters(cmd, row);
                    cmd.ExecuteNonQuery();
                    added++;
                }
            }
            if (added > 0) RebuildNumericTable();
            return (added, skipped);
        }

        public void ExportToCsv(string filePath, List<Deal> deals)
        {
            using var writer = new StreamWriter(filePath, false, new UTF8Encoding(true));
            writer.WriteLine(
                "入力日,経路,仲介会社,タイトル,案件ID,事業内容,エリア," +
                "売上高,営業利益,EBITDA,純資産額,総資産額,NET Cash/Debt," +
                "現金・現金同等物,有利子負債等,従業員数,特徴," +
                "譲渡希望額,譲渡希望形態,譲渡希望理由,希望譲渡条件,処理,全体概況");

            foreach (var d in deals)
            {
                writer.WriteLine(string.Join(",", new[]
                {
                    Escape(d.InputDate), Escape(d.Route), Escape(d.BrokerCompany), Escape(d.Title), Escape(d.DealId),
                    Escape(d.BusinessContent), Escape(d.Area), Escape(d.Revenue), Escape(d.OperatingProfit), Escape(d.EBITDA),
                    Escape(d.NetAssets), Escape(d.TotalAssets), Escape(d.NetCashDebt), Escape(d.CashEquivalents),
                    Escape(d.InterestBearingDebt), Escape(d.EmployeeCount), Escape(d.Features), Escape(d.AskingPrice),
                    Escape(d.TransferType), Escape(d.TransferReason), Escape(d.TransferConditions), Escape(d.Status),
                    Escape(d.AttachmentsSummary)
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
                    BusinessContent, Area, Revenue, OperatingProfit, EBITDA,
                    NetAssets, TotalAssets, NetCashDebt, CashEquivalents, InterestBearingDebt, EmployeeCount,
                    Features, AskingPrice, TransferType, TransferReason, TransferConditions, Status,
                    ConvertedAt
                ) VALUES (
                    @Id, @InputDate, @Route, @BrokerCompany, @Title, @DealId,
                    @BusinessContent, @Area, @Revenue, @OperatingProfit, @EBITDA,
                    @NetAssets, @TotalAssets, @NetCashDebt, @CashEquivalents, @InterestBearingDebt, @EmployeeCount,
                    @Features, @AskingPrice, @TransferType, @TransferReason, @TransferConditions, @Status,
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
            using var cmd = new SqliteCommand("DELETE FROM DealsNumeric WHERE Id = @Id;", conn);
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.ExecuteNonQuery();
        }

        public int RebuildNumericTable()
        {
            var deals = GetAllDeals();
            string now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            using var conn = new SqliteConnection(_connectionString);
            conn.Open();
            using (var del = new SqliteCommand("DELETE FROM DealsNumeric;", conn))
                del.ExecuteNonQuery();

            string sql = @"
                INSERT INTO DealsNumeric (
                    Id, InputDate, Route, BrokerCompany, Title, DealId,
                    BusinessContent, Area, Revenue, OperatingProfit, EBITDA,
                    NetAssets, TotalAssets, NetCashDebt, CashEquivalents, InterestBearingDebt, EmployeeCount,
                    Features, AskingPrice, TransferType, TransferReason, TransferConditions, Status,
                    ConvertedAt
                ) VALUES (
                    @Id, @InputDate, @Route, @BrokerCompany, @Title, @DealId,
                    @BusinessContent, @Area, @Revenue, @OperatingProfit, @EBITDA,
                    @NetAssets, @TotalAssets, @NetCashDebt, @CashEquivalents, @InterestBearingDebt, @EmployeeCount,
                    @Features, @AskingPrice, @TransferType, @TransferReason, @TransferConditions, @Status,
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

        public List<DealNumeric> GetAllDealNumerics()
        {
            var list = new List<DealNumeric>();
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();
            using var cmd = new SqliteCommand("SELECT * FROM DealsNumeric ORDER BY Id;", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read()) list.Add(MapDealNumeric(reader));
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
            while (reader.Read()) list.Add(MapDealNumeric(reader));
            return list;
        }

        // ══════════════════════════════════════════════════════
        // CompanyProfiles / FinancialHighlights / ValuationData
        // ══════════════════════════════════════════════════════

        public CompanyProfile? GetCompanyProfile(long dealId)
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();
            using var cmd = new SqliteCommand("SELECT * FROM CompanyProfiles WHERE DealId = @DealId;", conn);
            cmd.Parameters.AddWithValue("@DealId", dealId);
            using var reader = cmd.ExecuteReader();
            return reader.Read() ? MapCompanyProfile(reader) : null;
        }

        public void UpsertCompanyProfile(CompanyProfile p)
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();
            string sql = @"
                INSERT INTO CompanyProfiles (
                    DealId, CompanyName, CompanyNameSub, HeadOfficeAddress, FactoryAddress, OtherOffice,
                    Founded, Founded2, Capital, RepresentativeName, RepresentativeProfile, ShareholderInfo,
                    BusinessDetail, Revenue, Employees, MainClients, MainSuppliers, Certifications,
                    GroupCompanies, TransferReason, Remarks
                ) VALUES (
                    @DealId, @CompanyName, @CompanyNameSub, @HeadOfficeAddress, @FactoryAddress, @OtherOffice,
                    @Founded, @Founded2, @Capital, @RepresentativeName, @RepresentativeProfile, @ShareholderInfo,
                    @BusinessDetail, @Revenue, @Employees, @MainClients, @MainSuppliers, @Certifications,
                    @GroupCompanies, @TransferReason, @Remarks
                )
                ON CONFLICT(DealId) DO UPDATE SET
                    CompanyName           = excluded.CompanyName,
                    CompanyNameSub        = excluded.CompanyNameSub,
                    HeadOfficeAddress     = excluded.HeadOfficeAddress,
                    FactoryAddress        = excluded.FactoryAddress,
                    OtherOffice           = excluded.OtherOffice,
                    Founded               = excluded.Founded,
                    Founded2              = excluded.Founded2,
                    Capital               = excluded.Capital,
                    RepresentativeName    = excluded.RepresentativeName,
                    RepresentativeProfile = excluded.RepresentativeProfile,
                    ShareholderInfo       = excluded.ShareholderInfo,
                    BusinessDetail        = excluded.BusinessDetail,
                    Revenue               = excluded.Revenue,
                    Employees             = excluded.Employees,
                    MainClients           = excluded.MainClients,
                    MainSuppliers         = excluded.MainSuppliers,
                    Certifications        = excluded.Certifications,
                    GroupCompanies        = excluded.GroupCompanies,
                    TransferReason        = excluded.TransferReason,
                    Remarks               = excluded.Remarks;";

            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@DealId", p.DealId);
            cmd.Parameters.AddWithValue("@CompanyName", p.CompanyName);
            cmd.Parameters.AddWithValue("@CompanyNameSub", p.CompanyNameSub);
            cmd.Parameters.AddWithValue("@HeadOfficeAddress", p.HeadOfficeAddress);
            cmd.Parameters.AddWithValue("@FactoryAddress", p.FactoryAddress);
            cmd.Parameters.AddWithValue("@OtherOffice", p.OtherOffice);
            cmd.Parameters.AddWithValue("@Founded", p.Founded);
            cmd.Parameters.AddWithValue("@Founded2", p.Founded2);
            cmd.Parameters.AddWithValue("@Capital", p.Capital);
            cmd.Parameters.AddWithValue("@RepresentativeName", p.RepresentativeName);
            cmd.Parameters.AddWithValue("@RepresentativeProfile", p.RepresentativeProfile);
            cmd.Parameters.AddWithValue("@ShareholderInfo", p.ShareholderInfo);
            cmd.Parameters.AddWithValue("@BusinessDetail", p.BusinessDetail);
            cmd.Parameters.AddWithValue("@Revenue", p.Revenue);
            cmd.Parameters.AddWithValue("@Employees", p.Employees);
            cmd.Parameters.AddWithValue("@MainClients", p.MainClients);
            cmd.Parameters.AddWithValue("@MainSuppliers", p.MainSuppliers);
            cmd.Parameters.AddWithValue("@Certifications", p.Certifications);
            cmd.Parameters.AddWithValue("@GroupCompanies", p.GroupCompanies);
            cmd.Parameters.AddWithValue("@TransferReason", p.TransferReason);
            cmd.Parameters.AddWithValue("@Remarks", p.Remarks);
            cmd.ExecuteNonQuery();
        }

        public List<FinancialHighlight> GetFinancialHighlights(long dealId)
        {
            var list = new List<FinancialHighlight>();
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();
            using var cmd = new SqliteCommand("SELECT * FROM FinancialHighlights WHERE DealId = @DealId ORDER BY PeriodType DESC, PeriodOrder ASC;", conn);
            cmd.Parameters.AddWithValue("@DealId", dealId);
            using var reader = cmd.ExecuteReader();
            while (reader.Read()) list.Add(MapFinancialHighlight(reader));
            return list;
        }

        public void UpsertFinancialHighlight(FinancialHighlight f)
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();
            string sql = @"
                INSERT INTO FinancialHighlights (
                    DealId, PeriodType, PeriodOrder, PeriodLabel, Revenue, CostRate, GrossProfit, GrossProfitRate,
                    SGA, OperatingProfit, OperatingProfitRate, OrdinaryProfit, NetIncome, EBITDA, Depreciation, CapEx,
                    CurrentAssets, CashEquivalents, AccountsReceivable, Inventory, OtherCurrentAssets, FixedAssets, TotalAssets,
                    CurrentLiabilities, AccountsPayable, ShortTermDebt, OtherCurrentLiabilities, FixedLiabilities, LongTermDebt,
                    OtherFixedLiabilities, TotalLiabilities, NetAssets, RetainedEarnings
                ) VALUES (
                    @DealId, @PeriodType, @PeriodOrder, @PeriodLabel, @Revenue, @CostRate, @GrossProfit, @GrossProfitRate,
                    @SGA, @OperatingProfit, @OperatingProfitRate, @OrdinaryProfit, @NetIncome, @EBITDA, @Depreciation, @CapEx,
                    @CurrentAssets, @CashEquivalents, @AccountsReceivable, @Inventory, @OtherCurrentAssets, @FixedAssets, @TotalAssets,
                    @CurrentLiabilities, @AccountsPayable, @ShortTermDebt, @OtherCurrentLiabilities, @FixedLiabilities, @LongTermDebt,
                    @OtherFixedLiabilities, @TotalLiabilities, @NetAssets, @RetainedEarnings
                )
                ON CONFLICT(DealId, PeriodType, PeriodOrder) DO UPDATE SET
                    PeriodLabel             = excluded.PeriodLabel,
                    Revenue                 = excluded.Revenue,
                    CostRate                = excluded.CostRate,
                    GrossProfit             = excluded.GrossProfit,
                    GrossProfitRate         = excluded.GrossProfitRate,
                    SGA                     = excluded.SGA,
                    OperatingProfit         = excluded.OperatingProfit,
                    OperatingProfitRate     = excluded.OperatingProfitRate,
                    OrdinaryProfit          = excluded.OrdinaryProfit,
                    NetIncome               = excluded.NetIncome,
                    EBITDA                  = excluded.EBITDA,
                    Depreciation            = excluded.Depreciation,
                    CapEx                   = excluded.CapEx,
                    CurrentAssets           = excluded.CurrentAssets,
                    CashEquivalents         = excluded.CashEquivalents,
                    AccountsReceivable      = excluded.AccountsReceivable,
                    Inventory               = excluded.Inventory,
                    OtherCurrentAssets      = excluded.OtherCurrentAssets,
                    FixedAssets             = excluded.FixedAssets,
                    TotalAssets             = excluded.TotalAssets,
                    CurrentLiabilities      = excluded.CurrentLiabilities,
                    AccountsPayable         = excluded.AccountsPayable,
                    ShortTermDebt           = excluded.ShortTermDebt,
                    OtherCurrentLiabilities = excluded.OtherCurrentLiabilities,
                    FixedLiabilities        = excluded.FixedLiabilities,
                    LongTermDebt            = excluded.LongTermDebt,
                    OtherFixedLiabilities   = excluded.OtherFixedLiabilities,
                    TotalLiabilities        = excluded.TotalLiabilities,
                    NetAssets               = excluded.NetAssets,
                    RetainedEarnings        = excluded.RetainedEarnings;";

            using var cmd = new SqliteCommand(sql, conn);
            BindFinancialHighlight(cmd, f);
            cmd.ExecuteNonQuery();
        }

        public ValuationData? GetValuationData(long dealId)
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();
            using var cmd = new SqliteCommand("SELECT * FROM ValuationData WHERE DealId = @DealId;", conn);
            cmd.Parameters.AddWithValue("@DealId", dealId);
            using var reader = cmd.ExecuteReader();
            return reader.Read() ? MapValuationData(reader) : null;
        }

        public void UpsertValuationData(ValuationData v)
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();
            string sql = @"
                INSERT INTO ValuationData (
                    DealId, NetAssetValue, NetAssetNote, EBITDABase, EBITDABaseYear, EBITDAMultiple,
                    EBITDANetCashDebt, EBITDANote, DCFDiscountRate, DCFTerminalGrowth, DCFEV, DCFNetCashDebt, DCFNote,
                    NOI, CapRate, DirectNetCashDebt, DirectNote, EBITDAEquityValue, DCFEquityValue, DirectEquityValue, ValuationNote
                ) VALUES (
                    @DealId, @NetAssetValue, @NetAssetNote, @EBITDABase, @EBITDABaseYear, @EBITDAMultiple,
                    @EBITDANetCashDebt, @EBITDANote, @DCFDiscountRate, @DCFTerminalGrowth, @DCFEV, @DCFNetCashDebt, @DCFNote,
                    @NOI, @CapRate, @DirectNetCashDebt, @DirectNote, @EBITDAEquityValue, @DCFEquityValue, @DirectEquityValue, @ValuationNote
                )
                ON CONFLICT(DealId) DO UPDATE SET
                    NetAssetValue      = excluded.NetAssetValue,
                    NetAssetNote       = excluded.NetAssetNote,
                    EBITDABase         = excluded.EBITDABase,
                    EBITDABaseYear     = excluded.EBITDABaseYear,
                    EBITDAMultiple     = excluded.EBITDAMultiple,
                    EBITDANetCashDebt  = excluded.EBITDANetCashDebt,
                    EBITDANote         = excluded.EBITDANote,
                    DCFDiscountRate    = excluded.DCFDiscountRate,
                    DCFTerminalGrowth  = excluded.DCFTerminalGrowth,
                    DCFEV              = excluded.DCFEV,
                    DCFNetCashDebt     = excluded.DCFNetCashDebt,
                    DCFNote            = excluded.DCFNote,
                    NOI                = excluded.NOI,
                    CapRate            = excluded.CapRate,
                    DirectNetCashDebt  = excluded.DirectNetCashDebt,
                    DirectNote         = excluded.DirectNote,
                    EBITDAEquityValue  = excluded.EBITDAEquityValue,
                    DCFEquityValue     = excluded.DCFEquityValue,
                    DirectEquityValue  = excluded.DirectEquityValue,
                    ValuationNote      = excluded.ValuationNote;";

            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@DealId", v.DealId);
            cmd.Parameters.AddWithValue("@NetAssetNote", v.NetAssetNote);
            cmd.Parameters.AddWithValue("@EBITDABaseYear", v.EBITDABaseYear);
            cmd.Parameters.AddWithValue("@EBITDANote", v.EBITDANote);
            cmd.Parameters.AddWithValue("@DCFNote", v.DCFNote);
            cmd.Parameters.AddWithValue("@DirectNote", v.DirectNote);
            cmd.Parameters.AddWithValue("@ValuationNote", v.ValuationNote);
            BindReal(cmd, "@NetAssetValue", v.NetAssetValue);
            BindReal(cmd, "@EBITDABase", v.EBITDABase);
            BindReal(cmd, "@EBITDAMultiple", v.EBITDAMultiple);
            BindReal(cmd, "@EBITDANetCashDebt", v.EBITDANetCashDebt);
            BindReal(cmd, "@DCFDiscountRate", v.DCFDiscountRate);
            BindReal(cmd, "@DCFTerminalGrowth", v.DCFTerminalGrowth);
            BindReal(cmd, "@DCFEV", v.DCFEV);
            BindReal(cmd, "@DCFNetCashDebt", v.DCFNetCashDebt);
            BindReal(cmd, "@NOI", v.NOI);
            BindReal(cmd, "@CapRate", v.CapRate);
            BindReal(cmd, "@DirectNetCashDebt", v.DirectNetCashDebt);
            BindReal(cmd, "@EBITDAEquityValue", v.EBITDAEquityValue);
            BindReal(cmd, "@DCFEquityValue", v.DCFEquityValue);
            BindReal(cmd, "@DirectEquityValue", v.DirectEquityValue);
            cmd.ExecuteNonQuery();
        }

        // ══════════════════════════════════════════════════════
        // ヘルパーメソッド群
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
            AttachmentsSummary = HasColumn(r, "AttachmentsSummary") ? StrD(r, "AttachmentsSummary") : ""
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
            ConvertedAt = StrD(r, "ConvertedAt")
        };

        private static CompanyProfile MapCompanyProfile(SqliteDataReader r) => new CompanyProfile
        {
            Id = r.GetInt64(r.GetOrdinal("Id")),
            DealId = r.GetInt64(r.GetOrdinal("DealId")),
            CompanyName = StrD(r, "CompanyName"),
            CompanyNameSub = StrD(r, "CompanyNameSub"),
            HeadOfficeAddress = StrD(r, "HeadOfficeAddress"),
            FactoryAddress = StrD(r, "FactoryAddress"),
            OtherOffice = StrD(r, "OtherOffice"),
            Founded = StrD(r, "Founded"),
            Founded2 = StrD(r, "Founded2"),
            Capital = StrD(r, "Capital"),
            RepresentativeName = StrD(r, "RepresentativeName"),
            RepresentativeProfile = StrD(r, "RepresentativeProfile"),
            ShareholderInfo = StrD(r, "ShareholderInfo"),
            BusinessDetail = StrD(r, "BusinessDetail"),
            Revenue = StrD(r, "Revenue"),
            Employees = StrD(r, "Employees"),
            MainClients = StrD(r, "MainClients"),
            MainSuppliers = StrD(r, "MainSuppliers"),
            Certifications = StrD(r, "Certifications"),
            GroupCompanies = StrD(r, "GroupCompanies"),
            TransferReason = StrD(r, "TransferReason"),
            Remarks = StrD(r, "Remarks")
        };

        private static FinancialHighlight MapFinancialHighlight(SqliteDataReader r) => new FinancialHighlight
        {
            Id = r.GetInt64(r.GetOrdinal("Id")),
            DealId = r.GetInt64(r.GetOrdinal("DealId")),
            PeriodType = StrD(r, "PeriodType"),
            PeriodOrder = r.GetInt32(r.GetOrdinal("PeriodOrder")),
            PeriodLabel = StrD(r, "PeriodLabel"),
            Revenue = RealN(r, "Revenue"),
            CostRate = RealN(r, "CostRate"),
            GrossProfit = RealN(r, "GrossProfit"),
            GrossProfitRate = RealN(r, "GrossProfitRate"),
            SGA = RealN(r, "SGA"),
            OperatingProfit = RealN(r, "OperatingProfit"),
            OperatingProfitRate = RealN(r, "OperatingProfitRate"),
            OrdinaryProfit = RealN(r, "OrdinaryProfit"),
            NetIncome = RealN(r, "NetIncome"),
            EBITDA = RealN(r, "EBITDA"),
            Depreciation = RealN(r, "Depreciation"),
            CapEx = RealN(r, "CapEx"),
            CurrentAssets = RealN(r, "CurrentAssets"),
            CashEquivalents = RealN(r, "CashEquivalents"),
            AccountsReceivable = RealN(r, "AccountsReceivable"),
            Inventory = RealN(r, "Inventory"),
            OtherCurrentAssets = RealN(r, "OtherCurrentAssets"),
            FixedAssets = RealN(r, "FixedAssets"),
            TotalAssets = RealN(r, "TotalAssets"),
            CurrentLiabilities = RealN(r, "CurrentLiabilities"),
            AccountsPayable = RealN(r, "AccountsPayable"),
            ShortTermDebt = RealN(r, "ShortTermDebt"),
            OtherCurrentLiabilities = RealN(r, "OtherCurrentLiabilities"),
            FixedLiabilities = RealN(r, "FixedLiabilities"),
            LongTermDebt = RealN(r, "LongTermDebt"),
            OtherFixedLiabilities = RealN(r, "OtherFixedLiabilities"),
            TotalLiabilities = RealN(r, "TotalLiabilities"),
            NetAssets = RealN(r, "NetAssets"),
            RetainedEarnings = RealN(r, "RetainedEarnings")
        };

        private static ValuationData MapValuationData(SqliteDataReader r) => new ValuationData
        {
            Id = r.GetInt64(r.GetOrdinal("Id")),
            DealId = r.GetInt64(r.GetOrdinal("DealId")),
            NetAssetValue = RealN(r, "NetAssetValue"),
            NetAssetNote = StrD(r, "NetAssetNote"),
            EBITDABase = RealN(r, "EBITDABase"),
            EBITDABaseYear = StrD(r, "EBITDABaseYear"),
            EBITDAMultiple = RealN(r, "EBITDAMultiple"),
            EBITDANetCashDebt = RealN(r, "EBITDANetCashDebt"),
            EBITDANote = StrD(r, "EBITDANote"),
            DCFDiscountRate = RealN(r, "DCFDiscountRate"),
            DCFTerminalGrowth = RealN(r, "DCFTerminalGrowth"),
            DCFEV = RealN(r, "DCFEV"),
            DCFNetCashDebt = RealN(r, "DCFNetCashDebt"),
            DCFNote = StrD(r, "DCFNote"),
            NOI = RealN(r, "NOI"),
            CapRate = RealN(r, "CapRate"),
            DirectNetCashDebt = RealN(r, "DirectNetCashDebt"),
            DirectNote = StrD(r, "DirectNote"),
            EBITDAEquityValue = RealN(r, "EBITDAEquityValue"),
            DCFEquityValue = RealN(r, "DCFEquityValue"),
            DirectEquityValue = RealN(r, "DirectEquityValue"),
            ValuationNote = StrD(r, "ValuationNote")
        };

        private static void BindParameters(SqliteCommand cmd, Deal d)
        {
            cmd.Parameters.AddWithValue("@InputDate", d.InputDate); cmd.Parameters.AddWithValue("@Route", d.Route); cmd.Parameters.AddWithValue("@BrokerCompany", d.BrokerCompany); cmd.Parameters.AddWithValue("@Title", d.Title); cmd.Parameters.AddWithValue("@DealId", d.DealId); cmd.Parameters.AddWithValue("@BusinessContent", d.BusinessContent); cmd.Parameters.AddWithValue("@Area", d.Area); cmd.Parameters.AddWithValue("@Revenue", d.Revenue); cmd.Parameters.AddWithValue("@OperatingProfit", d.OperatingProfit); cmd.Parameters.AddWithValue("@EBITDA", d.EBITDA); cmd.Parameters.AddWithValue("@NetAssets", d.NetAssets); cmd.Parameters.AddWithValue("@TotalAssets", d.TotalAssets); cmd.Parameters.AddWithValue("@NetCashDebt", d.NetCashDebt); cmd.Parameters.AddWithValue("@CashEquivalents", d.CashEquivalents); cmd.Parameters.AddWithValue("@InterestBearingDebt", d.InterestBearingDebt); cmd.Parameters.AddWithValue("@EmployeeCount", d.EmployeeCount); cmd.Parameters.AddWithValue("@Features", d.Features); cmd.Parameters.AddWithValue("@AskingPrice", d.AskingPrice); cmd.Parameters.AddWithValue("@TransferType", d.TransferType); cmd.Parameters.AddWithValue("@TransferReason", d.TransferReason); cmd.Parameters.AddWithValue("@TransferConditions", d.TransferConditions); cmd.Parameters.AddWithValue("@Status", d.Status);
            cmd.Parameters.AddWithValue("@AttachmentsSummary", d.AttachmentsSummary ?? "");
        }

        private static void BindFinancialHighlight(SqliteCommand cmd, FinancialHighlight f)
        {
            cmd.Parameters.AddWithValue("@DealId", f.DealId); cmd.Parameters.AddWithValue("@PeriodType", f.PeriodType); cmd.Parameters.AddWithValue("@PeriodOrder", f.PeriodOrder); cmd.Parameters.AddWithValue("@PeriodLabel", f.PeriodLabel ?? "");
            BindReal(cmd, "@Revenue", f.Revenue); BindReal(cmd, "@CostRate", f.CostRate); BindReal(cmd, "@GrossProfit", f.GrossProfit); BindReal(cmd, "@GrossProfitRate", f.GrossProfitRate); BindReal(cmd, "@SGA", f.SGA); BindReal(cmd, "@OperatingProfit", f.OperatingProfit); BindReal(cmd, "@OperatingProfitRate", f.OperatingProfitRate); BindReal(cmd, "@OrdinaryProfit", f.OrdinaryProfit); BindReal(cmd, "@NetIncome", f.NetIncome); BindReal(cmd, "@EBITDA", f.EBITDA); BindReal(cmd, "@Depreciation", f.Depreciation); BindReal(cmd, "@CapEx", f.CapEx); BindReal(cmd, "@CurrentAssets", f.CurrentAssets); BindReal(cmd, "@CashEquivalents", f.CashEquivalents); BindReal(cmd, "@AccountsReceivable", f.AccountsReceivable); BindReal(cmd, "@Inventory", f.Inventory); BindReal(cmd, "@OtherCurrentAssets", f.OtherCurrentAssets); BindReal(cmd, "@FixedAssets", f.FixedAssets); BindReal(cmd, "@TotalAssets", f.TotalAssets); BindReal(cmd, "@CurrentLiabilities", f.CurrentLiabilities); BindReal(cmd, "@AccountsPayable", f.AccountsPayable); BindReal(cmd, "@ShortTermDebt", f.ShortTermDebt); BindReal(cmd, "@OtherCurrentLiabilities", f.OtherCurrentLiabilities); BindReal(cmd, "@FixedLiabilities", f.FixedLiabilities); BindReal(cmd, "@LongTermDebt", f.LongTermDebt); BindReal(cmd, "@OtherFixedLiabilities", f.OtherFixedLiabilities); BindReal(cmd, "@TotalLiabilities", f.TotalLiabilities); BindReal(cmd, "@NetAssets", f.NetAssets); BindReal(cmd, "@RetainedEarnings", f.RetainedEarnings);
        }

        private static void BindNullableReal(SqliteCommand cmd, string paramName, string rawValue)
        {
            double? converted = NumericConverter.Convert(rawValue);
            if (converted.HasValue) cmd.Parameters.AddWithValue(paramName, converted.Value); else cmd.Parameters.AddWithValue(paramName, DBNull.Value);
        }

        private static void BindReal(SqliteCommand cmd, string param, double? value)
        {
            if (value.HasValue) cmd.Parameters.AddWithValue(param, value.Value); else cmd.Parameters.AddWithValue(param, DBNull.Value);
        }

        private static string StrD(SqliteDataReader r, string col) => r.IsDBNull(r.GetOrdinal(col)) ? string.Empty : r.GetString(r.GetOrdinal(col));
        private static double? RealN(SqliteDataReader r, string col) => r.IsDBNull(r.GetOrdinal(col)) ? null : r.GetDouble(r.GetOrdinal(col));

        private static bool HasColumn(SqliteDataReader r, string col)
        {
            for (int i = 0; i < r.FieldCount; i++) if (r.GetName(i).Equals(col, StringComparison.OrdinalIgnoreCase)) return true;
            return false;
        }

        private static string Escape(string value)
        {
            if (value == null) return string.Empty;
            if (value.Contains(',') || value.Contains('"') || value.Contains('\n') || value.Contains('\r'))
                return $"\"{value.Replace("\"", "\"\"")}\"";
            return value;
        }
        // ══════════════════════════════════════════════════════
        // データ同期・バックアップ用 拡張メソッド群
        // ══════════════════════════════════════════════════════

        /// <summary>
        /// 日付範囲とキーワードによる、Dealsテーブルの高度なフィルタリング抽出
        /// </summary>
        public List<Deal> GetDealsByFilter(string? fromDate, string? toDate, string keyword)
        {
            var list = new List<Deal>();
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();

            var sb = new StringBuilder("SELECT * FROM Deals WHERE 1=1");
            if (!string.IsNullOrEmpty(fromDate)) sb.Append(" AND InputDate >= @FromDate");
            if (!string.IsNullOrEmpty(toDate)) sb.Append(" AND InputDate <= @ToDate");
            if (!string.IsNullOrEmpty(keyword))
            {
                sb.Append(" AND (Title LIKE @kw OR BusinessContent LIKE @kw OR Area LIKE @kw OR BrokerCompany LIKE @kw OR Status LIKE @kw)");
            }
            sb.Append(" ORDER BY Id;");

            using var cmd = new SqliteCommand(sb.ToString(), conn);
            if (!string.IsNullOrEmpty(fromDate)) cmd.Parameters.AddWithValue("@FromDate", fromDate);
            if (!string.IsNullOrEmpty(toDate)) cmd.Parameters.AddWithValue("@ToDate", toDate);
            if (!string.IsNullOrEmpty(keyword)) cmd.Parameters.AddWithValue("@kw", $"%{keyword}%");

            using var reader = cmd.ExecuteReader();
            while (reader.Read()) list.Add(MapDeal(reader));
            return list;
        }

        public void ExportDealsToCsv(string filePath, List<Deal> list)
        {
            ExportToCsv(filePath, list); // 既存のエクスポート処理を流用
        }

        public void ExportProfilesToCsv(string filePath, List<long> dealIds)
        {
            if (dealIds.Count == 0) return;
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();
            string ids = string.Join(",", dealIds);

            using var cmd = new SqliteCommand($"SELECT * FROM CompanyProfiles WHERE DealId IN ({ids}) ORDER BY DealId;", conn);
            using var reader = cmd.ExecuteReader();
            using var writer = new StreamWriter(filePath, false, new UTF8Encoding(true));

            writer.WriteLine("案件内部ID(DealId),会社名,別会社名,本社住所,工場住所,その他事務所,設立,関連会社設立,資本金,代表者名,代表者略歴,株主構成,事業内容詳細,売上高,従業員数,主要取引先,主要仕入先,認証・許認可,グループ会社,譲渡理由,備考");
            while (reader.Read())
            {
                writer.WriteLine(string.Join(",", new[] {
                    reader.GetInt64(reader.GetOrdinal("DealId")).ToString(),
                    Escape(StrD(reader, "CompanyName")), Escape(StrD(reader, "CompanyNameSub")), Escape(StrD(reader, "HeadOfficeAddress")), Escape(StrD(reader, "FactoryAddress")), Escape(StrD(reader, "OtherOffice")),
                    Escape(StrD(reader, "Founded")), Escape(StrD(reader, "Founded2")), Escape(StrD(reader, "Capital")), Escape(StrD(reader, "RepresentativeName")), Escape(StrD(reader, "RepresentativeProfile")),
                    Escape(StrD(reader, "ShareholderInfo")), Escape(StrD(reader, "BusinessDetail")), Escape(StrD(reader, "Revenue")), Escape(StrD(reader, "Employees")), Escape(StrD(reader, "MainClients")),
                    Escape(StrD(reader, "MainSuppliers")), Escape(StrD(reader, "Certifications")), Escape(StrD(reader, "GroupCompanies")), Escape(StrD(reader, "TransferReason")), Escape(StrD(reader, "Remarks"))
                }));
            }
        }

        public void ExportFinancialsToCsv(string filePath, List<long> dealIds)
        {
            if (dealIds.Count == 0) return;
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();
            string ids = string.Join(",", dealIds);

            using var cmd = new SqliteCommand($"SELECT * FROM FinancialHighlights WHERE DealId IN ({ids}) ORDER BY DealId, PeriodType DESC, PeriodOrder ASC;", conn);
            using var reader = cmd.ExecuteReader();
            using var writer = new StreamWriter(filePath, false, new UTF8Encoding(true));

            writer.WriteLine("案件内部ID(DealId),期区分(actual/forecast),順序(1-3),期ラベル,売上高,原価率,粗利益,粗利率,販管費,営業利益,営業利益率,経常利益,当期純利益,EBITDA,減価償却費,設備投資額,流動資産,現金預金,売掛金,棚卸資産,その他流動,固定資産,総資産,流動負債,買掛金,短期借入金,その他流動負債,固定負債,長期借入金,その他固定負債,負債合計,純資産合計,利益剰余金");
            while (reader.Read())
            {
                writer.WriteLine(string.Join(",", new[] {
                    reader.GetInt64(reader.GetOrdinal("DealId")).ToString(),
                    Escape(StrD(reader, "PeriodType")), reader.GetInt32(reader.GetOrdinal("PeriodOrder")).ToString(), Escape(StrD(reader, "PeriodLabel")),
                    RealN(reader, "Revenue").ToString() ?? "", RealN(reader, "CostRate").ToString() ?? "", RealN(reader, "GrossProfit").ToString() ?? "", RealN(reader, "GrossProfitRate").ToString() ?? "",
                    RealN(reader, "SGA").ToString() ?? "", RealN(reader, "OperatingProfit").ToString() ?? "", RealN(reader, "OperatingProfitRate").ToString() ?? "", RealN(reader, "OrdinaryProfit").ToString() ?? "",
                    RealN(reader, "NetIncome").ToString() ?? "", RealN(reader, "EBITDA").ToString() ?? "", RealN(reader, "Depreciation").ToString() ?? "", RealN(reader, "CapEx").ToString() ?? "",
                    RealN(reader, "CurrentAssets").ToString() ?? "", RealN(reader, "CashEquivalents").ToString() ?? "", RealN(reader, "AccountsReceivable").ToString() ?? "", RealN(reader, "Inventory").ToString() ?? "",
                    RealN(reader, "OtherCurrentAssets").ToString() ?? "", RealN(reader, "FixedAssets").ToString() ?? "", RealN(reader, "TotalAssets").ToString() ?? "", RealN(reader, "CurrentLiabilities").ToString() ?? "",
                    RealN(reader, "AccountsPayable").ToString() ?? "", RealN(reader, "ShortTermDebt").ToString() ?? "", RealN(reader, "OtherCurrentLiabilities").ToString() ?? "", RealN(reader, "FixedLiabilities").ToString() ?? "",
                    RealN(reader, "LongTermDebt").ToString() ?? "", RealN(reader, "OtherFixedLiabilities").ToString() ?? "", RealN(reader, "TotalLiabilities").ToString() ?? "", RealN(reader, "NetAssets").ToString() ?? "", RealN(reader, "RetainedEarnings").ToString() ?? ""
                }));
            }
        }

        public void ExportValuationsToCsv(string filePath, List<long> dealIds)
        {
            if (dealIds.Count == 0) return;
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();
            string ids = string.Join(",", dealIds);

            using var cmd = new SqliteCommand($"SELECT * FROM ValuationData WHERE DealId IN ({ids}) ORDER BY DealId;", conn);
            using var reader = cmd.ExecuteReader();
            using var writer = new StreamWriter(filePath, false, new UTF8Encoding(true));

            writer.WriteLine("案件内部ID(DealId),修正純資産額,純資産法備考,EBITDA基準値,EBITDA基準年度,マルチプル倍率,ネットキャッシュ(EBITDA用),EBITDA法備考,割引率,永続成長率,EV(DCF),ネットキャッシュ(DCF用),DCF法備考,NOI,キャップレート,ネットキャッシュ(直接還元用),直接還元備考,EBITDA算定株式価値,DCF算定株式価値,直接還元算定株式価値,総合備考");
            while (reader.Read())
            {
                writer.WriteLine(string.Join(",", new[] {
                    reader.GetInt64(reader.GetOrdinal("DealId")).ToString(),
                    RealN(reader, "NetAssetValue").ToString() ?? "", Escape(StrD(reader, "NetAssetNote")),
                    RealN(reader, "EBITDABase").ToString() ?? "", Escape(StrD(reader, "EBITDABaseYear")), RealN(reader, "EBITDAMultiple").ToString() ?? "", RealN(reader, "EBITDANetCashDebt").ToString() ?? "", Escape(StrD(reader, "EBITDANote")),
                    RealN(reader, "DCFDiscountRate").ToString() ?? "", RealN(reader, "DCFTerminalGrowth").ToString() ?? "", RealN(reader, "DCFEV").ToString() ?? "", RealN(reader, "DCFNetCashDebt").ToString() ?? "", Escape(StrD(reader, "DCFNote")),
                    RealN(reader, "NOI").ToString() ?? "", RealN(reader, "CapRate").ToString() ?? "", RealN(reader, "DirectNetCashDebt").ToString() ?? "", Escape(StrD(reader, "DirectNote")),
                    RealN(reader, "EBITDAEquityValue").ToString() ?? "", RealN(reader, "DCFEquityValue").ToString() ?? "", RealN(reader, "DirectEquityValue").ToString() ?? "", Escape(StrD(reader, "ValuationNote"))
                }));
            }
        }

        public void ExportAttachmentsToCsv(string filePath, List<long> dealIds)
        {
            if (dealIds.Count == 0) return;
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();
            string ids = string.Join(",", dealIds);

            using var cmd = new SqliteCommand($"SELECT * FROM Attachments WHERE DealId IN ({ids}) ORDER BY DealId;", conn);
            using var reader = cmd.ExecuteReader();
            using var writer = new StreamWriter(filePath, false, new UTF8Encoding(true));

            writer.WriteLine("案件内部ID(DealId),ファイル名,アプリ内保管パス,ファイル備考,登録日時");
            while (reader.Read())
            {
                writer.WriteLine(string.Join(",", new[] {
                    reader.GetInt64(reader.GetOrdinal("DealId")).ToString(),
                    Escape(StrD(reader, "FileName")), Escape(StrD(reader, "FilePath")), Escape(StrD(reader, "Description")), Escape(StrD(reader, "UploadedAt"))
                }));
            }
        }

        /// <summary>
        /// まるごとZIPバックアップの作成処理
        /// </summary>
        public void CreateBackupZip(string zipFilePath)
        {
            string tempDir = Path.Combine(Path.GetTempPath(), "MAItemsBackup_" + Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDir);

            // 接続プールを一時クリアしてDBファイルのロックを外してコピー
            SqliteConnection.ClearAllPools();
            string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "app_data.db");
            if (File.Exists(dbPath))
            {
                File.Copy(dbPath, Path.Combine(tempDir, "app_data.db"), true);
            }

            // Attachments フォルダ（添付資料の実体）のコピー
            string attachDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Attachments");
            if (Directory.Exists(attachDir))
            {
                string destAttachDir = Path.Combine(tempDir, "Attachments");
                Directory.CreateDirectory(destAttachDir);
                CopyDirectory(attachDir, destAttachDir);
            }

            // ZIPファイルへ圧縮
            if (File.Exists(zipFilePath)) File.Delete(zipFilePath);
            System.IO.Compression.ZipFile.CreateFromDirectory(tempDir, zipFilePath);

            // 作業用一時フォルダを削除
            Directory.Delete(tempDir, true);
        }

        /// <summary>
        /// ZIPバックアップからの環境復元 (リストア)
        /// </summary>
        public void RestoreFromZip(string zipFilePath)
        {
            string tempDir = Path.Combine(Path.GetTempPath(), "MAItemsRestore_" + Guid.NewGuid().ToString());
            System.IO.Compression.ZipFile.ExtractToDirectory(zipFilePath, tempDir);

            // ロック解除
            SqliteConnection.ClearAllPools();

            // DBファイルの差し替え
            string tempDb = Path.Combine(tempDir, "app_data.db");
            if (File.Exists(tempDb))
            {
                string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "app_data.db");
                File.Copy(tempDb, dbPath, true);
            }

            // Attachments フォルダの差し替え
            string tempAttach = Path.Combine(tempDir, "Attachments");
            if (Directory.Exists(tempAttach))
            {
                string attachDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Attachments");
                if (Directory.Exists(attachDir)) Directory.Delete(attachDir, true);
                Directory.CreateDirectory(attachDir);
                CopyDirectory(tempAttach, attachDir);
            }

            Directory.Delete(tempDir, true);
        }

        private static void CopyDirectory(string sourceDir, string destinationDir)
        {
            var dir = new DirectoryInfo(sourceDir);
            if (!dir.Exists) return;
            Directory.CreateDirectory(destinationDir);
            foreach (FileInfo file in dir.GetFiles())
            {
                file.CopyTo(Path.Combine(destinationDir, file.Name), true);
            }
            foreach (DirectoryInfo subDir in dir.GetDirectories())
            {
                CopyDirectory(subDir.FullName, Path.Combine(destinationDir, subDir.Name));
            }
        }




    }
}