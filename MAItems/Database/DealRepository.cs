using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Text;

namespace MAItems.Database
{
    public class DealRepository
    {
        private readonly DatabaseContext _context;

        public DealRepository(DatabaseContext context)
        {
            _context = context;
            MigrateNumericIfEmpty(); // 初回同期チェック
        }

        public List<Deal> GetAllDeals()
        {
            var list = new List<Deal>();
            using var conn = _context.GetConnection();
            using var cmd = new SqliteCommand("SELECT * FROM Deals ORDER BY LastUpdatedAt, Id;", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read()) list.Add(MapDeal(reader));
            return list;
        }

        public List<Deal> SearchDeals(string keyword)
        {
            var list = new List<Deal>();
            using var conn = _context.GetConnection();
            string sql = @"SELECT * FROM Deals WHERE Title LIKE @kw OR BusinessContent LIKE @kw OR Area LIKE @kw OR BrokerCompany LIKE @kw OR Status LIKE @kw ORDER BY LastUpdatedAt, Id;";
            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@kw", $"%{keyword}%");
            using var reader = cmd.ExecuteReader();
            while (reader.Read()) list.Add(MapDeal(reader));
            return list;
        }

        public List<Deal> GetDealsByFilter(string? fromDate, string? toDate, string keyword)
        {
            var list = new List<Deal>();
            using var conn = _context.GetConnection();
            var sb = new StringBuilder("SELECT * FROM Deals WHERE 1=1");
            if (!string.IsNullOrEmpty(fromDate)) sb.Append(" AND InputDate >= @FromDate");
            if (!string.IsNullOrEmpty(toDate)) sb.Append(" AND InputDate <= @ToDate");
            if (!string.IsNullOrEmpty(keyword)) sb.Append(" AND (Title LIKE @kw OR BusinessContent LIKE @kw OR Area LIKE @kw OR BrokerCompany LIKE @kw OR Status LIKE @kw)");
            sb.Append(" ORDER BY LastUpdatedAt, Id;");

            using var cmd = new SqliteCommand(sb.ToString(), conn);
            if (!string.IsNullOrEmpty(fromDate)) cmd.Parameters.AddWithValue("@FromDate", fromDate);
            if (!string.IsNullOrEmpty(toDate)) cmd.Parameters.AddWithValue("@ToDate", toDate);
            if (!string.IsNullOrEmpty(keyword)) cmd.Parameters.AddWithValue("@kw", $"%{keyword}%");

            using var reader = cmd.ExecuteReader();
            while (reader.Read()) list.Add(MapDeal(reader));
            return list;
        }

        public Deal? GetDealById(long id)
        {
            using var conn = _context.GetConnection();
            using var cmd = new SqliteCommand("SELECT * FROM Deals WHERE Id = @Id;", conn);
            cmd.Parameters.AddWithValue("@Id", id);
            using var reader = cmd.ExecuteReader();
            return reader.Read() ? MapDeal(reader) : null;
        }

        public void AddDeal(Deal d)
        {
            long newId;
            d.LastUpdatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            using (var conn = _context.GetConnection())
            {
                string sql = @"INSERT INTO Deals (InputDate, Route, BrokerCompany, Title, DealId, BusinessContent, Area, Revenue, OperatingProfit, EBITDA, NetAssets, TotalAssets, NetCashDebt, CashEquivalents, InterestBearingDebt, EmployeeCount, Features, AskingPrice, TransferType, TransferReason, TransferConditions, Status, AttachmentsSummary, IsProcessing, LastUpdatedAt) VALUES (@InputDate, @Route, @BrokerCompany, @Title, @DealId, @BusinessContent, @Area, @Revenue, @OperatingProfit, @EBITDA, @NetAssets, @TotalAssets, @NetCashDebt, @CashEquivalents, @InterestBearingDebt, @EmployeeCount, @Features, @AskingPrice, @TransferType, @TransferReason, @TransferConditions, @Status, @AttachmentsSummary, @IsProcessing, @LastUpdatedAt); SELECT last_insert_rowid();";
                using var cmd = new SqliteCommand(sql, conn);
                BindParameters(cmd, d);
                newId = (long)cmd.ExecuteScalar()!;
            }
            UpsertNumeric(newId);
        }

        public long AddEmptyDeal()
        {
            long newId;
            string now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            using (var conn = _context.GetConnection())
            {
                string sql = @"INSERT INTO Deals (InputDate, Route, BrokerCompany, Title, DealId, BusinessContent, Area, Revenue, OperatingProfit, EBITDA, NetAssets, TotalAssets, NetCashDebt, CashEquivalents, InterestBearingDebt, EmployeeCount, Features, AskingPrice, TransferType, TransferReason, TransferConditions, Status, AttachmentsSummary, IsProcessing, LastUpdatedAt) VALUES (@InputDate, '', '', '', '', '', '', '', '', '', '', '', '', '', '', '', '', '', '', '', '', '', '', 0, @LastUpdatedAt); SELECT last_insert_rowid();";
                using var cmd = new SqliteCommand(sql, conn);
                cmd.Parameters.AddWithValue("@InputDate", DateTime.Now.ToString("yyyy/M/d"));
                cmd.Parameters.AddWithValue("@LastUpdatedAt", now);
                newId = (long)cmd.ExecuteScalar()!;
            }
            UpsertNumeric(newId);
            return newId;
        }

        public void UpdateDeal(Deal d)
        {
            d.LastUpdatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            using (var conn = _context.GetConnection())
            {
                string sql = @"UPDATE Deals SET InputDate=@InputDate, Route=@Route, BrokerCompany=@BrokerCompany, Title=@Title, DealId=@DealId, BusinessContent=@BusinessContent, Area=@Area, Revenue=@Revenue, OperatingProfit=@OperatingProfit, EBITDA=@EBITDA, NetAssets=@NetAssets, TotalAssets=@TotalAssets, NetCashDebt=@NetCashDebt, CashEquivalents=@CashEquivalents, InterestBearingDebt=@InterestBearingDebt, EmployeeCount=@EmployeeCount, Features=@Features, AskingPrice=@AskingPrice, TransferType=@TransferType, TransferReason=@TransferReason, TransferConditions=@TransferConditions, Status=@Status, AttachmentsSummary=@AttachmentsSummary, IsProcessing=@IsProcessing, LastUpdatedAt=@LastUpdatedAt WHERE Id = @Id;";
                using var cmd = new SqliteCommand(sql, conn);
                BindParameters(cmd, d);
                cmd.Parameters.AddWithValue("@Id", d.Id);
                cmd.ExecuteNonQuery();
            }
            UpsertNumeric(d.Id);
        }

        public void DeleteDeal(long id)
        {
            using (var conn = _context.GetConnection())
            {
                using var cmd = new SqliteCommand("DELETE FROM Deals WHERE Id = @Id;", conn);
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.ExecuteNonQuery();
            }
            DeleteNumeric(id);
        }

        // --- Numeric (数値モード) 同期処理 ---
        private void UpsertNumeric(long id)
        {
            Deal? deal = GetDealById(id);
            if (deal == null) return;
            string now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            using var conn = _context.GetConnection();
            string sql = @"INSERT OR REPLACE INTO DealsNumeric (Id, InputDate, Route, BrokerCompany, Title, DealId, BusinessContent, Area, Revenue, OperatingProfit, EBITDA, NetAssets, TotalAssets, NetCashDebt, CashEquivalents, InterestBearingDebt, EmployeeCount, Features, AskingPrice, TransferType, TransferReason, TransferConditions, Status, ConvertedAt) VALUES (@Id, @InputDate, @Route, @BrokerCompany, @Title, @DealId, @BusinessContent, @Area, @Revenue, @OperatingProfit, @EBITDA, @NetAssets, @TotalAssets, @NetCashDebt, @CashEquivalents, @InterestBearingDebt, @EmployeeCount, @Features, @AskingPrice, @TransferType, @TransferReason, @TransferConditions, @Status, @ConvertedAt);";
            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Id", deal.Id); cmd.Parameters.AddWithValue("@InputDate", deal.InputDate); cmd.Parameters.AddWithValue("@Route", deal.Route); cmd.Parameters.AddWithValue("@BrokerCompany", deal.BrokerCompany); cmd.Parameters.AddWithValue("@Title", deal.Title); cmd.Parameters.AddWithValue("@DealId", deal.DealId); cmd.Parameters.AddWithValue("@BusinessContent", deal.BusinessContent); cmd.Parameters.AddWithValue("@Area", deal.Area); cmd.Parameters.AddWithValue("@Features", deal.Features); cmd.Parameters.AddWithValue("@TransferType", deal.TransferType); cmd.Parameters.AddWithValue("@TransferReason", deal.TransferReason); cmd.Parameters.AddWithValue("@TransferConditions", deal.TransferConditions); cmd.Parameters.AddWithValue("@Status", deal.Status); cmd.Parameters.AddWithValue("@ConvertedAt", now);
            DbHelperUtils.BindNullableReal(cmd, "@Revenue", deal.Revenue); DbHelperUtils.BindNullableReal(cmd, "@OperatingProfit", deal.OperatingProfit); DbHelperUtils.BindNullableReal(cmd, "@EBITDA", deal.EBITDA); DbHelperUtils.BindNullableReal(cmd, "@NetAssets", deal.NetAssets); DbHelperUtils.BindNullableReal(cmd, "@TotalAssets", deal.TotalAssets); DbHelperUtils.BindNullableReal(cmd, "@NetCashDebt", deal.NetCashDebt); DbHelperUtils.BindNullableReal(cmd, "@CashEquivalents", deal.CashEquivalents); DbHelperUtils.BindNullableReal(cmd, "@InterestBearingDebt", deal.InterestBearingDebt); DbHelperUtils.BindNullableReal(cmd, "@EmployeeCount", deal.EmployeeCount); DbHelperUtils.BindNullableReal(cmd, "@AskingPrice", deal.AskingPrice);
            cmd.ExecuteNonQuery();
        }

        private void DeleteNumeric(long id)
        {
            using var conn = _context.GetConnection();
            using var cmd = new SqliteCommand("DELETE FROM DealsNumeric WHERE Id = @Id;", conn);
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.ExecuteNonQuery();
        }

        public int RebuildNumericTable()
        {
            var deals = GetAllDeals();
            using var conn = _context.GetConnection();
            using (var del = new SqliteCommand("DELETE FROM DealsNumeric;", conn)) del.ExecuteNonQuery();
            foreach (var d in deals) UpsertNumeric(d.Id);
            return deals.Count;
        }

        private void MigrateNumericIfEmpty()
        {
            using var conn = _context.GetConnection();
            using var cntNum = new SqliteCommand("SELECT COUNT(1) FROM DealsNumeric;", conn);
            if ((long)cntNum.ExecuteScalar()! > 0) return;
            using var cntDeal = new SqliteCommand("SELECT COUNT(1) FROM Deals;", conn);
            if ((long)cntDeal.ExecuteScalar()! > 0) RebuildNumericTable();
        }

        public List<DealNumeric> GetAllDealNumerics()
        {
            var list = new List<DealNumeric>();
            using var conn = _context.GetConnection();
            using var cmd = new SqliteCommand("SELECT * FROM DealsNumeric ORDER BY LastUpdatedAt, Id;", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read()) list.Add(MapDealNumeric(reader));
            return list;
        }

        public List<DealNumeric> SearchDealNumerics(string keyword)
        {
            var list = new List<DealNumeric>();
            using var conn = _context.GetConnection();
            string sql = @"SELECT * FROM DealsNumeric WHERE Title LIKE @kw OR BusinessContent LIKE @kw OR Area LIKE @kw OR BrokerCompany LIKE @kw OR Status LIKE @kw ORDER BY LastUpdatedAt, Id;";
            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@kw", $"%{keyword}%");
            using var reader = cmd.ExecuteReader();
            while (reader.Read()) list.Add(MapDealNumeric(reader));
            return list;
        }

        // --- マッピングとバインド ---
        private static Deal MapDeal(SqliteDataReader r) => new Deal
        {
            Id = r.GetInt64(r.GetOrdinal("Id")),
            InputDate = DbHelperUtils.StrD(r, "InputDate"),
            Route = DbHelperUtils.StrD(r, "Route"),
            BrokerCompany = DbHelperUtils.StrD(r, "BrokerCompany"),
            Title = DbHelperUtils.StrD(r, "Title"),
            DealId = DbHelperUtils.StrD(r, "DealId"),
            BusinessContent = DbHelperUtils.StrD(r, "BusinessContent"),
            Area = DbHelperUtils.StrD(r, "Area"),
            Revenue = DbHelperUtils.StrD(r, "Revenue"),
            OperatingProfit = DbHelperUtils.StrD(r, "OperatingProfit"),
            EBITDA = DbHelperUtils.StrD(r, "EBITDA"),
            NetAssets = DbHelperUtils.StrD(r, "NetAssets"),
            TotalAssets = DbHelperUtils.StrD(r, "TotalAssets"),
            NetCashDebt = DbHelperUtils.StrD(r, "NetCashDebt"),
            CashEquivalents = DbHelperUtils.StrD(r, "CashEquivalents"),
            InterestBearingDebt = DbHelperUtils.StrD(r, "InterestBearingDebt"),
            EmployeeCount = DbHelperUtils.StrD(r, "EmployeeCount"),
            Features = DbHelperUtils.StrD(r, "Features"),
            AskingPrice = DbHelperUtils.StrD(r, "AskingPrice"),
            TransferType = DbHelperUtils.StrD(r, "TransferType"),
            TransferReason = DbHelperUtils.StrD(r, "TransferReason"),
            TransferConditions = DbHelperUtils.StrD(r, "TransferConditions"),
            Status = DbHelperUtils.StrD(r, "Status"),
            AttachmentsSummary = DbHelperUtils.HasColumn(r, "AttachmentsSummary") ? DbHelperUtils.StrD(r, "AttachmentsSummary") : "",
            IsProcessing = DbHelperUtils.HasColumn(r, "IsProcessing") && !r.IsDBNull(r.GetOrdinal("IsProcessing")) ? r.GetInt32(r.GetOrdinal("IsProcessing")) == 1 : false,
            LastUpdatedAt = DbHelperUtils.HasColumn(r, "LastUpdatedAt") ? DbHelperUtils.StrD(r, "LastUpdatedAt") : ""
        };

        private static DealNumeric MapDealNumeric(SqliteDataReader r) => new DealNumeric
        {
            Id = r.GetInt64(r.GetOrdinal("Id")),
            InputDate = DbHelperUtils.StrD(r, "InputDate"),
            Route = DbHelperUtils.StrD(r, "Route"),
            BrokerCompany = DbHelperUtils.StrD(r, "BrokerCompany"),
            Title = DbHelperUtils.StrD(r, "Title"),
            DealId = DbHelperUtils.StrD(r, "DealId"),
            BusinessContent = DbHelperUtils.StrD(r, "BusinessContent"),
            Area = DbHelperUtils.StrD(r, "Area"),
            Revenue = DbHelperUtils.RealN(r, "Revenue"),
            OperatingProfit = DbHelperUtils.RealN(r, "OperatingProfit"),
            EBITDA = DbHelperUtils.RealN(r, "EBITDA"),
            NetAssets = DbHelperUtils.RealN(r, "NetAssets"),
            TotalAssets = DbHelperUtils.RealN(r, "TotalAssets"),
            NetCashDebt = DbHelperUtils.RealN(r, "NetCashDebt"),
            CashEquivalents = DbHelperUtils.RealN(r, "CashEquivalents"),
            InterestBearingDebt = DbHelperUtils.RealN(r, "InterestBearingDebt"),
            EmployeeCount = DbHelperUtils.RealN(r, "EmployeeCount"),
            Features = DbHelperUtils.StrD(r, "Features"),
            AskingPrice = DbHelperUtils.RealN(r, "AskingPrice"),
            TransferType = DbHelperUtils.StrD(r, "TransferType"),
            TransferReason = DbHelperUtils.StrD(r, "TransferReason"),
            TransferConditions = DbHelperUtils.StrD(r, "TransferConditions"),
            Status = DbHelperUtils.StrD(r, "Status"),
            ConvertedAt = DbHelperUtils.StrD(r, "ConvertedAt")
        };

        private static void BindParameters(SqliteCommand cmd, Deal d)
        {
            cmd.Parameters.AddWithValue("@InputDate", d.InputDate); cmd.Parameters.AddWithValue("@Route", d.Route); cmd.Parameters.AddWithValue("@BrokerCompany", d.BrokerCompany); cmd.Parameters.AddWithValue("@Title", d.Title); cmd.Parameters.AddWithValue("@DealId", d.DealId); cmd.Parameters.AddWithValue("@BusinessContent", d.BusinessContent); cmd.Parameters.AddWithValue("@Area", d.Area); cmd.Parameters.AddWithValue("@Revenue", d.Revenue); cmd.Parameters.AddWithValue("@OperatingProfit", d.OperatingProfit); cmd.Parameters.AddWithValue("@EBITDA", d.EBITDA); cmd.Parameters.AddWithValue("@NetAssets", d.NetAssets); cmd.Parameters.AddWithValue("@TotalAssets", d.TotalAssets); cmd.Parameters.AddWithValue("@NetCashDebt", d.NetCashDebt); cmd.Parameters.AddWithValue("@CashEquivalents", d.CashEquivalents); cmd.Parameters.AddWithValue("@InterestBearingDebt", d.InterestBearingDebt); cmd.Parameters.AddWithValue("@EmployeeCount", d.EmployeeCount); cmd.Parameters.AddWithValue("@Features", d.Features); cmd.Parameters.AddWithValue("@AskingPrice", d.AskingPrice); cmd.Parameters.AddWithValue("@TransferType", d.TransferType); cmd.Parameters.AddWithValue("@TransferReason", d.TransferReason); cmd.Parameters.AddWithValue("@TransferConditions", d.TransferConditions); cmd.Parameters.AddWithValue("@Status", d.Status); cmd.Parameters.AddWithValue("@AttachmentsSummary", d.AttachmentsSummary ?? ""); cmd.Parameters.AddWithValue("@IsProcessing", d.IsProcessing ? 1 : 0); cmd.Parameters.AddWithValue("@LastUpdatedAt", d.LastUpdatedAt ?? "");
        }
    }
}