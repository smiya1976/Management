using Microsoft.Data.Sqlite;
using System;
using System.IO;

namespace MAItems.Database
{
    public class DatabaseContext
    {
        public string ConnectionString { get; }
        public static string DbFilePath => Path.Combine(Application.StartupPath, "app_data.db");

        public DatabaseContext(string dbFileName = "app_data.db")
        {
            string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, dbFileName);
            ConnectionString = $"Data Source={dbPath}";
            InitializeDatabase();
        }

        public SqliteConnection GetConnection()
        {
            var conn = new SqliteConnection(ConnectionString);
            conn.Open();
            return conn;
        }

        private void InitializeDatabase()
        {
            using var conn = GetConnection();

            string sqlDeals = @"
                CREATE TABLE IF NOT EXISTS Deals (
                    Id                  INTEGER PRIMARY KEY AUTOINCREMENT,
                    InputDate           TEXT, Route TEXT, BrokerCompany TEXT, Title TEXT, DealId TEXT,
                    BusinessContent     TEXT, Area TEXT, Revenue TEXT, OperatingProfit TEXT, EBITDA TEXT,
                    NetAssets           TEXT, TotalAssets TEXT, NetCashDebt TEXT, CashEquivalents TEXT,
                    InterestBearingDebt TEXT, EmployeeCount TEXT, Features TEXT, AskingPrice TEXT,
                    TransferType        TEXT, TransferReason TEXT, TransferConditions TEXT, Status TEXT,
                    AttachmentsSummary  TEXT, IsProcessing INTEGER DEFAULT 0, LastUpdatedAt TEXT
                );";
            using (var cmd = new SqliteCommand(sqlDeals, conn)) cmd.ExecuteNonQuery();

            // 既存DB用ALTER文 (エラー無視)
            SafeAlterTable(conn, "Deals", "AttachmentsSummary", "TEXT");
            SafeAlterTable(conn, "Deals", "IsProcessing", "INTEGER DEFAULT 0");
            SafeAlterTable(conn, "Deals", "LastUpdatedAt", "TEXT");

            string sqlNumeric = @"
                CREATE TABLE IF NOT EXISTS DealsNumeric (
                    Id                  INTEGER PRIMARY KEY,
                    InputDate           TEXT, Route TEXT, BrokerCompany TEXT, Title TEXT, DealId TEXT,
                    BusinessContent     TEXT, Area TEXT, Revenue REAL, OperatingProfit REAL, EBITDA REAL,
                    NetAssets           REAL, TotalAssets REAL, NetCashDebt REAL, CashEquivalents REAL,
                    InterestBearingDebt REAL, EmployeeCount REAL, Features TEXT, AskingPrice REAL,
                    TransferType        TEXT, TransferReason TEXT, TransferConditions TEXT, Status TEXT,
                    ConvertedAt         TEXT
                );";
            using (var cmd = new SqliteCommand(sqlNumeric, conn)) cmd.ExecuteNonQuery();

            InitializeExtendedTables(conn);
        }

        private void InitializeExtendedTables(SqliteConnection conn)
        {
            string sqlCompany = @"CREATE TABLE IF NOT EXISTS CompanyProfiles ( Id INTEGER PRIMARY KEY AUTOINCREMENT, DealId INTEGER NOT NULL UNIQUE, CompanyName TEXT, CompanyNameSub TEXT, HeadOfficeAddress TEXT, FactoryAddress TEXT, OtherOffice TEXT, Founded TEXT, Founded2 TEXT, Capital TEXT, RepresentativeName TEXT, RepresentativeProfile TEXT, ShareholderInfo TEXT, BusinessDetail TEXT, Revenue TEXT, Employees TEXT, MainClients TEXT, MainSuppliers TEXT, Certifications TEXT, GroupCompanies TEXT, TransferReason TEXT, Remarks TEXT );";
            using (var cmd = new SqliteCommand(sqlCompany, conn)) cmd.ExecuteNonQuery();

            string sqlFinancial = @"CREATE TABLE IF NOT EXISTS FinancialHighlights ( Id INTEGER PRIMARY KEY AUTOINCREMENT, DealId INTEGER NOT NULL, PeriodType TEXT NOT NULL, PeriodOrder INTEGER NOT NULL, PeriodLabel TEXT, Revenue REAL, CostRate REAL, GrossProfit REAL, GrossProfitRate REAL, SGA REAL, OperatingProfit REAL, OperatingProfitRate REAL, OrdinaryProfit REAL, NetIncome REAL, EBITDA REAL, Depreciation REAL, CapEx REAL, CurrentAssets REAL, CashEquivalents REAL, AccountsReceivable REAL, Inventory REAL, OtherCurrentAssets REAL, FixedAssets REAL, TotalAssets REAL, CurrentLiabilities REAL, AccountsPayable REAL, ShortTermDebt REAL, OtherCurrentLiabilities REAL, FixedLiabilities REAL, LongTermDebt REAL, OtherFixedLiabilities REAL, TotalLiabilities REAL, NetAssets REAL, RetainedEarnings REAL, UNIQUE(DealId, PeriodType, PeriodOrder) );";
            using (var cmd = new SqliteCommand(sqlFinancial, conn)) cmd.ExecuteNonQuery();

            string sqlValuation = @"CREATE TABLE IF NOT EXISTS ValuationData ( Id INTEGER PRIMARY KEY AUTOINCREMENT, DealId INTEGER NOT NULL UNIQUE, NetAssetValue REAL, NetAssetNote TEXT, EBITDABase REAL, EBITDABaseYear TEXT, EBITDAMultiple REAL, EBITDANetCashDebt REAL, EBITDANote TEXT, DCFDiscountRate REAL, DCFTerminalGrowth REAL, DCFEV REAL, DCFNetCashDebt REAL, DCFNote TEXT, NOI REAL, CapRate REAL, DirectNetCashDebt REAL, DirectNote TEXT, EBITDAEquityValue REAL, DCFEquityValue REAL, DirectEquityValue REAL, ValuationNote TEXT );";
            using (var cmd = new SqliteCommand(sqlValuation, conn)) cmd.ExecuteNonQuery();

            // ── 追加: 新しい ValuationData 列のマイグレーション ──
            string[] newValCols = {
                "CashAndDeposits", "MarketableSecurities", "InsuranceReserves", "OtherAssets", "WorkingCapitalMonths",
                "ShortTermDebt", "LongTermDebt", "LeaseObligations", "OtherLiabilities",
                "OpProfit_NA", "TaxRate_NA", "GoodwillYears", "OpProfit_Direct", "TaxRate_Direct" // ←追加
            };
            foreach (var col in newValCols) SafeAlterTable(conn, "ValuationData", col, "REAL");

            // ── 新規テーブル1: 純資産法の修正項目 ──
            string sqlNetAssetAdj = @"CREATE TABLE IF NOT EXISTS NetAssetAdjustments ( Id INTEGER PRIMARY KEY AUTOINCREMENT, DealId INTEGER NOT NULL, AdjustType INTEGER NOT NULL, ItemName TEXT, Amount REAL, Remarks TEXT );";
            using (var cmd = new SqliteCommand(sqlNetAssetAdj, conn)) cmd.ExecuteNonQuery();

            // ── 新規テーブル2: DCF法の事業計画 ──
            string sqlDcf = @"CREATE TABLE IF NOT EXISTS DcfProjections ( Id INTEGER PRIMARY KEY AUTOINCREMENT, DealId INTEGER NOT NULL, YearIndex INTEGER NOT NULL, Revenue REAL, OpProfit REAL, TaxRate REAL, DiscountRate REAL, TerminalGrowth REAL, UNIQUE(DealId, YearIndex) );";
            using (var cmd = new SqliteCommand(sqlDcf, conn)) cmd.ExecuteNonQuery();

            string sqlAttachments = @"CREATE TABLE IF NOT EXISTS Attachments ( Id INTEGER PRIMARY KEY AUTOINCREMENT, DealId INTEGER NOT NULL, FileName TEXT NOT NULL, FilePath TEXT NOT NULL, Description TEXT, UploadedAt TEXT );";
            using (var cmd = new SqliteCommand(sqlAttachments, conn)) cmd.ExecuteNonQuery();
        }

        private void SafeAlterTable(SqliteConnection conn, string table, string column, string definition)
        {
            try
            {
                using var cmd = new SqliteCommand($"ALTER TABLE {table} ADD COLUMN {column} {definition};", conn);
                cmd.ExecuteNonQuery();
            }
            catch { } // 既に列が存在する場合はエラーを無視する
        }
    }
}