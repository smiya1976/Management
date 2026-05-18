using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MAItems.Database
{
    public class DataSyncService
    {
        private readonly DatabaseContext _context;
        private readonly DealRepository _dealRepo;

        public DataSyncService(DatabaseContext context, DealRepository dealRepo)
        {
            _context = context;
            _dealRepo = dealRepo;
        }

        /// <summary>
        /// 既存のCSVインポート処理
        /// </summary>
        public (int added, int skipped) ImportFromCsv(string filePath)
        {
            var rows = CsvParser.Parse(filePath);
            int added = 0, skipped = 0;
            string now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            using (var conn = _context.GetConnection())
            {
                foreach (var row in rows)
                {
                    row.LastUpdatedAt = now;
                    row.IsProcessing = false;

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
                            TransferConditions, Status, AttachmentsSummary,
                            IsProcessing, LastUpdatedAt
                        ) VALUES (
                            @InputDate, @Route, @BrokerCompany, @Title, @DealId,
                            @BusinessContent, @Area, @Revenue, @OperatingProfit, @EBITDA,
                            @NetAssets, @TotalAssets, @NetCashDebt, @CashEquivalents,
                            @InterestBearingDebt, @EmployeeCount, @Features,
                            @AskingPrice, @TransferType, @TransferReason,
                            @TransferConditions, @Status, @AttachmentsSummary,
                            @IsProcessing, @LastUpdatedAt
                        );";
                    using var cmd = new SqliteCommand(sql, conn);

                    // DealRepository内のBindParametersはアクセス修飾子を考慮し、ここでは直接パラメータを展開するか、
                    // もしくは拡張メソッド等で共通化しますが、今回は独立して記述します
                    BindDealParams(cmd, row);
                    cmd.ExecuteNonQuery();
                    added++;
                }
            }
            if (added > 0) _dealRepo.RebuildNumericTable();
            return (added, skipped);
        }

        public void ExportDealsToCsv(string filePath, List<Deal> deals)
        {
            using var writer = new StreamWriter(filePath, false, new UTF8Encoding(true));
            writer.WriteLine("入力日,経路,仲介会社,タイトル,案件ID,事業内容,エリア,売上高,営業利益,EBITDA,純資産額,総資産額,NET Cash/Debt,現金・現金同等物,有利子負債等,従業員数,特徴,譲渡希望額,譲渡希望形態,譲渡希望理由,希望譲渡条件,処理,全体概況");

            foreach (var d in deals)
            {
                writer.WriteLine(string.Join(",", new[]
                {
                    DbHelperUtils.EscapeCsv(d.InputDate), DbHelperUtils.EscapeCsv(d.Route), DbHelperUtils.EscapeCsv(d.BrokerCompany), DbHelperUtils.EscapeCsv(d.Title), DbHelperUtils.EscapeCsv(d.DealId),
                    DbHelperUtils.EscapeCsv(d.BusinessContent), DbHelperUtils.EscapeCsv(d.Area), DbHelperUtils.EscapeCsv(d.Revenue), DbHelperUtils.EscapeCsv(d.OperatingProfit), DbHelperUtils.EscapeCsv(d.EBITDA),
                    DbHelperUtils.EscapeCsv(d.NetAssets), DbHelperUtils.EscapeCsv(d.TotalAssets), DbHelperUtils.EscapeCsv(d.NetCashDebt), DbHelperUtils.EscapeCsv(d.CashEquivalents),
                    DbHelperUtils.EscapeCsv(d.InterestBearingDebt), DbHelperUtils.EscapeCsv(d.EmployeeCount), DbHelperUtils.EscapeCsv(d.Features), DbHelperUtils.EscapeCsv(d.AskingPrice),
                    DbHelperUtils.EscapeCsv(d.TransferType), DbHelperUtils.EscapeCsv(d.TransferReason), DbHelperUtils.EscapeCsv(d.TransferConditions), DbHelperUtils.EscapeCsv(d.Status),
                    DbHelperUtils.EscapeCsv(d.AttachmentsSummary)
                }));
            }
        }

        public void ExportProfilesToCsv(string filePath, List<long> dealIds)
        {
            if (dealIds.Count == 0) return;
            using var conn = _context.GetConnection();
            string ids = string.Join(",", dealIds);
            using var cmd = new SqliteCommand($"SELECT * FROM CompanyProfiles WHERE DealId IN ({ids}) ORDER BY DealId;", conn);
            using var reader = cmd.ExecuteReader();
            using var writer = new StreamWriter(filePath, false, new UTF8Encoding(true));

            writer.WriteLine("案件内部ID(DealId),会社名,別会社名,本社住所,工場住所,その他事務所,設立,関連会社設立,資本金,代表者名,代表者略歴,株主構成,事業内容詳細,売上高,従業員数,主要取引先,主要仕入先,認証・許認可,グループ会社,譲渡理由,備考");
            while (reader.Read())
            {
                writer.WriteLine(string.Join(",", new[] {
                    reader.GetInt64(reader.GetOrdinal("DealId")).ToString(),
                    DbHelperUtils.EscapeCsv(DbHelperUtils.StrD(reader, "CompanyName")), DbHelperUtils.EscapeCsv(DbHelperUtils.StrD(reader, "CompanyNameSub")), DbHelperUtils.EscapeCsv(DbHelperUtils.StrD(reader, "HeadOfficeAddress")), DbHelperUtils.EscapeCsv(DbHelperUtils.StrD(reader, "FactoryAddress")), DbHelperUtils.EscapeCsv(DbHelperUtils.StrD(reader, "OtherOffice")),
                    DbHelperUtils.EscapeCsv(DbHelperUtils.StrD(reader, "Founded")), DbHelperUtils.EscapeCsv(DbHelperUtils.StrD(reader, "Founded2")), DbHelperUtils.EscapeCsv(DbHelperUtils.StrD(reader, "Capital")), DbHelperUtils.EscapeCsv(DbHelperUtils.StrD(reader, "RepresentativeName")), DbHelperUtils.EscapeCsv(DbHelperUtils.StrD(reader, "RepresentativeProfile")),
                    DbHelperUtils.EscapeCsv(DbHelperUtils.StrD(reader, "ShareholderInfo")), DbHelperUtils.EscapeCsv(DbHelperUtils.StrD(reader, "BusinessDetail")), DbHelperUtils.EscapeCsv(DbHelperUtils.StrD(reader, "Revenue")), DbHelperUtils.EscapeCsv(DbHelperUtils.StrD(reader, "Employees")), DbHelperUtils.EscapeCsv(DbHelperUtils.StrD(reader, "MainClients")),
                    DbHelperUtils.EscapeCsv(DbHelperUtils.StrD(reader, "MainSuppliers")), DbHelperUtils.EscapeCsv(DbHelperUtils.StrD(reader, "Certifications")), DbHelperUtils.EscapeCsv(DbHelperUtils.StrD(reader, "GroupCompanies")), DbHelperUtils.EscapeCsv(DbHelperUtils.StrD(reader, "TransferReason")), DbHelperUtils.EscapeCsv(DbHelperUtils.StrD(reader, "Remarks"))
                }));
            }
        }

        public void ExportFinancialsToCsv(string filePath, List<long> dealIds)
        {
            if (dealIds.Count == 0) return;
            using var conn = _context.GetConnection();
            string ids = string.Join(",", dealIds);
            using var cmd = new SqliteCommand($"SELECT * FROM FinancialHighlights WHERE DealId IN ({ids}) ORDER BY DealId, PeriodType DESC, PeriodOrder ASC;", conn);
            using var reader = cmd.ExecuteReader();
            using var writer = new StreamWriter(filePath, false, new UTF8Encoding(true));

            writer.WriteLine("案件内部ID(DealId),期区分(actual/forecast),順序(1-3),期ラベル,売上高,原価率,粗利益,粗利率,販管費,営業利益,営業利益率,経常利益,当期純利益,EBITDA,減価償却費,設備投資額,流動資産,現金預金,売掛金,棚卸資産,その他流動,固定資産,総資産,流動負債,買掛金,短期借入金,その他流動負債,固定負債,長期借入金,その他固定負債,負債合計,純資産合計,利益剰余金");
            while (reader.Read())
            {
                writer.WriteLine(string.Join(",", new[] {
                    reader.GetInt64(reader.GetOrdinal("DealId")).ToString(),
                    DbHelperUtils.EscapeCsv(DbHelperUtils.StrD(reader, "PeriodType")), reader.GetInt32(reader.GetOrdinal("PeriodOrder")).ToString(), DbHelperUtils.EscapeCsv(DbHelperUtils.StrD(reader, "PeriodLabel")),
                    DbHelperUtils.RealN(reader, "Revenue").ToString() ?? "", DbHelperUtils.RealN(reader, "CostRate").ToString() ?? "", DbHelperUtils.RealN(reader, "GrossProfit").ToString() ?? "", DbHelperUtils.RealN(reader, "GrossProfitRate").ToString() ?? "",
                    DbHelperUtils.RealN(reader, "SGA").ToString() ?? "", DbHelperUtils.RealN(reader, "OperatingProfit").ToString() ?? "", DbHelperUtils.RealN(reader, "OperatingProfitRate").ToString() ?? "", DbHelperUtils.RealN(reader, "OrdinaryProfit").ToString() ?? "",
                    DbHelperUtils.RealN(reader, "NetIncome").ToString() ?? "", DbHelperUtils.RealN(reader, "EBITDA").ToString() ?? "", DbHelperUtils.RealN(reader, "Depreciation").ToString() ?? "", DbHelperUtils.RealN(reader, "CapEx").ToString() ?? "",
                    DbHelperUtils.RealN(reader, "CurrentAssets").ToString() ?? "", DbHelperUtils.RealN(reader, "CashEquivalents").ToString() ?? "", DbHelperUtils.RealN(reader, "AccountsReceivable").ToString() ?? "", DbHelperUtils.RealN(reader, "Inventory").ToString() ?? "",
                    DbHelperUtils.RealN(reader, "OtherCurrentAssets").ToString() ?? "", DbHelperUtils.RealN(reader, "FixedAssets").ToString() ?? "", DbHelperUtils.RealN(reader, "TotalAssets").ToString() ?? "", DbHelperUtils.RealN(reader, "CurrentLiabilities").ToString() ?? "",
                    DbHelperUtils.RealN(reader, "AccountsPayable").ToString() ?? "", DbHelperUtils.RealN(reader, "ShortTermDebt").ToString() ?? "", DbHelperUtils.RealN(reader, "OtherCurrentLiabilities").ToString() ?? "", DbHelperUtils.RealN(reader, "FixedLiabilities").ToString() ?? "",
                    DbHelperUtils.RealN(reader, "LongTermDebt").ToString() ?? "", DbHelperUtils.RealN(reader, "OtherFixedLiabilities").ToString() ?? "", DbHelperUtils.RealN(reader, "TotalLiabilities").ToString() ?? "", DbHelperUtils.RealN(reader, "NetAssets").ToString() ?? "", DbHelperUtils.RealN(reader, "RetainedEarnings").ToString() ?? ""
                }));
            }
        }

        public void ExportValuationsToCsv(string filePath, List<long> dealIds)
        {
            if (dealIds.Count == 0) return;
            using var conn = _context.GetConnection();
            string ids = string.Join(",", dealIds);
            using var cmd = new SqliteCommand($"SELECT * FROM ValuationData WHERE DealId IN ({ids}) ORDER BY DealId;", conn);
            using var reader = cmd.ExecuteReader();
            using var writer = new StreamWriter(filePath, false, new UTF8Encoding(true));

            writer.WriteLine("案件内部ID(DealId),修正純資産額,純資産法備考,EBITDA基準値,EBITDA基準年度,マルチプル倍率,ネットキャッシュ(EBITDA用),EBITDA法備考,割引率,永続成長率,EV(DCF),ネットキャッシュ(DCF用),DCF法備考,NOI,キャップレート,ネットキャッシュ(直接還元用),直接還元備考,EBITDA算定株式価値,DCF算定株式価値,直接還元算定株式価値,総合備考");
            while (reader.Read())
            {
                writer.WriteLine(string.Join(",", new[] {
                    reader.GetInt64(reader.GetOrdinal("DealId")).ToString(),
                    DbHelperUtils.RealN(reader, "NetAssetValue").ToString() ?? "", DbHelperUtils.EscapeCsv(DbHelperUtils.StrD(reader, "NetAssetNote")),
                    DbHelperUtils.RealN(reader, "EBITDABase").ToString() ?? "", DbHelperUtils.EscapeCsv(DbHelperUtils.StrD(reader, "EBITDABaseYear")), DbHelperUtils.RealN(reader, "EBITDAMultiple").ToString() ?? "", DbHelperUtils.RealN(reader, "EBITDANetCashDebt").ToString() ?? "", DbHelperUtils.EscapeCsv(DbHelperUtils.StrD(reader, "EBITDANote")),
                    DbHelperUtils.RealN(reader, "DCFDiscountRate").ToString() ?? "", DbHelperUtils.RealN(reader, "DCFTerminalGrowth").ToString() ?? "", DbHelperUtils.RealN(reader, "DCFEV").ToString() ?? "", DbHelperUtils.RealN(reader, "DCFNetCashDebt").ToString() ?? "", DbHelperUtils.EscapeCsv(DbHelperUtils.StrD(reader, "DCFNote")),
                    DbHelperUtils.RealN(reader, "NOI").ToString() ?? "", DbHelperUtils.RealN(reader, "CapRate").ToString() ?? "", DbHelperUtils.RealN(reader, "DirectNetCashDebt").ToString() ?? "", DbHelperUtils.EscapeCsv(DbHelperUtils.StrD(reader, "DirectNote")),
                    DbHelperUtils.RealN(reader, "EBITDAEquityValue").ToString() ?? "", DbHelperUtils.RealN(reader, "DCFEquityValue").ToString() ?? "", DbHelperUtils.RealN(reader, "DirectEquityValue").ToString() ?? "", DbHelperUtils.EscapeCsv(DbHelperUtils.StrD(reader, "ValuationNote"))
                }));
            }
        }

        public void ExportAttachmentsToCsv(string filePath, List<long> dealIds)
        {
            if (dealIds.Count == 0) return;
            using var conn = _context.GetConnection();
            string ids = string.Join(",", dealIds);
            using var cmd = new SqliteCommand($"SELECT * FROM Attachments WHERE DealId IN ({ids}) ORDER BY DealId;", conn);
            using var reader = cmd.ExecuteReader();
            using var writer = new StreamWriter(filePath, false, new UTF8Encoding(true));

            writer.WriteLine("案件内部ID(DealId),ファイル名,アプリ内保管パス,ファイル備考,登録日時");
            while (reader.Read())
            {
                writer.WriteLine(string.Join(",", new[] {
                    reader.GetInt64(reader.GetOrdinal("DealId")).ToString(),
                    DbHelperUtils.EscapeCsv(DbHelperUtils.StrD(reader, "FileName")), DbHelperUtils.EscapeCsv(DbHelperUtils.StrD(reader, "FilePath")), DbHelperUtils.EscapeCsv(DbHelperUtils.StrD(reader, "Description")), DbHelperUtils.EscapeCsv(DbHelperUtils.StrD(reader, "UploadedAt"))
                }));
            }
        }

        public void CreateBackupZip(string zipFilePath)
        {
            string tempDir = Path.Combine(Path.GetTempPath(), "MAItemsBackup_" + Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDir);

            SqliteConnection.ClearAllPools(); // ロック解除
            string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "app_data.db");
            if (File.Exists(dbPath)) File.Copy(dbPath, Path.Combine(tempDir, "app_data.db"), true);

            string attachDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Attachments");
            if (Directory.Exists(attachDir))
            {
                string destAttachDir = Path.Combine(tempDir, "Attachments");
                Directory.CreateDirectory(destAttachDir);
                CopyDirectory(attachDir, destAttachDir);
            }

            if (File.Exists(zipFilePath)) File.Delete(zipFilePath);
            System.IO.Compression.ZipFile.CreateFromDirectory(tempDir, zipFilePath);
            Directory.Delete(tempDir, true);
        }

        public void RestoreFromZip(string zipFilePath)
        {
            string tempDir = Path.Combine(Path.GetTempPath(), "MAItemsRestore_" + Guid.NewGuid().ToString());
            System.IO.Compression.ZipFile.ExtractToDirectory(zipFilePath, tempDir);

            SqliteConnection.ClearAllPools(); // ロック解除

            string tempDb = Path.Combine(tempDir, "app_data.db");
            if (File.Exists(tempDb))
            {
                string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "app_data.db");
                File.Copy(tempDb, dbPath, true);
            }

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
            foreach (FileInfo file in dir.GetFiles()) file.CopyTo(Path.Combine(destinationDir, file.Name), true);
            foreach (DirectoryInfo subDir in dir.GetDirectories()) CopyDirectory(subDir.FullName, Path.Combine(destinationDir, subDir.Name));
        }

        private static void BindDealParams(SqliteCommand cmd, Deal d)
        {
            cmd.Parameters.AddWithValue("@InputDate", d.InputDate); cmd.Parameters.AddWithValue("@Route", d.Route); cmd.Parameters.AddWithValue("@BrokerCompany", d.BrokerCompany); cmd.Parameters.AddWithValue("@Title", d.Title); cmd.Parameters.AddWithValue("@DealId", d.DealId); cmd.Parameters.AddWithValue("@BusinessContent", d.BusinessContent); cmd.Parameters.AddWithValue("@Area", d.Area); cmd.Parameters.AddWithValue("@Revenue", d.Revenue); cmd.Parameters.AddWithValue("@OperatingProfit", d.OperatingProfit); cmd.Parameters.AddWithValue("@EBITDA", d.EBITDA); cmd.Parameters.AddWithValue("@NetAssets", d.NetAssets); cmd.Parameters.AddWithValue("@TotalAssets", d.TotalAssets); cmd.Parameters.AddWithValue("@NetCashDebt", d.NetCashDebt); cmd.Parameters.AddWithValue("@CashEquivalents", d.CashEquivalents); cmd.Parameters.AddWithValue("@InterestBearingDebt", d.InterestBearingDebt); cmd.Parameters.AddWithValue("@EmployeeCount", d.EmployeeCount); cmd.Parameters.AddWithValue("@Features", d.Features); cmd.Parameters.AddWithValue("@AskingPrice", d.AskingPrice); cmd.Parameters.AddWithValue("@TransferType", d.TransferType); cmd.Parameters.AddWithValue("@TransferReason", d.TransferReason); cmd.Parameters.AddWithValue("@TransferConditions", d.TransferConditions); cmd.Parameters.AddWithValue("@Status", d.Status); cmd.Parameters.AddWithValue("@AttachmentsSummary", d.AttachmentsSummary ?? ""); cmd.Parameters.AddWithValue("@IsProcessing", d.IsProcessing ? 1 : 0); cmd.Parameters.AddWithValue("@LastUpdatedAt", d.LastUpdatedAt ?? "");
        }
    }
}