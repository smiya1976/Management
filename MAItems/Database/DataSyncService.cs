using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using MAItems.Database;

namespace MAItems
{
    public class DataSyncService
    {
        private readonly DatabaseContext _context;
        private readonly DealRepository _dealRepo;
        private readonly FinancialRepository _finRepo;
        private readonly AttachmentRepository _attachRepo;

        public DataSyncService(DatabaseContext context)
        {
            _context = context;
            _dealRepo = new DealRepository(context);
            _finRepo = new FinancialRepository(context);
            _attachRepo = new AttachmentRepository(context);
        }

        // ══════════════════════════════════════════════════════
        // 1. エクスポート処理 (全5テーブルをCSV出力)
        // ══════════════════════════════════════════════════════
        public int ExportData(string folderPath, string? fromDate, string? toDate, string keyword)
        {
            // 1. 条件に合致する案件を取得
            var deals = _dealRepo.GetDealsByFilter(fromDate, toDate, keyword);
            if (deals.Count == 0) return 0;

            // 2. 出力先のファイルパス設定
            string fDeals = Path.Combine(folderPath, "1_案件一覧.csv");
            string fProfiles = Path.Combine(folderPath, "2_会社基礎情報.csv");
            string fFinancials = Path.Combine(folderPath, "3_財務ハイライト.csv");
            string fValuations = Path.Combine(folderPath, "4_株式価値試算.csv");
            string fAttachments = Path.Combine(folderPath, "5_添付ファイル情報.csv");

            // BOM付きUTF-8で出力 (Excelでの文字化け防止)
            var encoding = new UTF8Encoding(true);

            // --- 1. 案件一覧 ---
            var sbDeals = new StringBuilder();
            sbDeals.AppendLine("入力日,経路,仲介会社,タイトル,案件ID,事業内容,エリア,売上高,営業利益,EBITDA,純資産額,総資産額,NET Cash/Debt,現金・現金同等物,有利子負債等,従業員数,特徴,譲渡希望額,譲渡希望形態,譲渡希望理由,希望譲渡条件,処理,全体概況");

            var profiles = new List<CompanyProfile>();
            var financials = new List<FinancialHighlight>();
            var valuations = new List<ValuationData>();
            var attachments = new List<Attachment>();

            foreach (var d in deals)
            {
                sbDeals.AppendLine($"{Esc(d.InputDate)},{Esc(d.Route)},{Esc(d.BrokerCompany)},{Esc(d.Title)},{Esc(d.DealId)},{Esc(d.BusinessContent)},{Esc(d.Area)},{Esc(d.Revenue)},{Esc(d.OperatingProfit)},{Esc(d.EBITDA)},{Esc(d.NetAssets)},{Esc(d.TotalAssets)},{Esc(d.NetCashDebt)},{Esc(d.CashEquivalents)},{Esc(d.InterestBearingDebt)},{Esc(d.EmployeeCount)},{Esc(d.Features)},{Esc(d.AskingPrice)},{Esc(d.TransferType)},{Esc(d.TransferReason)},{Esc(d.TransferConditions)},{Esc(d.Status)},{Esc(d.AttachmentsSummary)}");

                // 子データを収集
                var p = _finRepo.GetCompanyProfile(d.Id);
                if (p != null) profiles.Add(p);

                financials.AddRange(_finRepo.GetFinancialHighlights(d.Id));

                var v = _finRepo.GetValuationData(d.Id);
                if (v != null) valuations.Add(v);

                attachments.AddRange(_attachRepo.GetAttachments(d.Id));
            }
            File.WriteAllText(fDeals, sbDeals.ToString(), encoding);

            // --- 2. 会社基礎情報 ---
            var sbProfiles = new StringBuilder();
            sbProfiles.AppendLine("案件内部ID(DealId),会社名,別会社名,本社住所,工場住所,その他事務所,設立,関連会社設立,資本金,代表者名,代表者略歴,株主構成,事業内容詳細,売上高,従業員数,主要取引先,主要仕入先,認証・許認可,グループ会社,譲渡理由,備考");
            foreach (var p in profiles) sbProfiles.AppendLine($"{p.DealId},{Esc(p.CompanyName)},{Esc(p.CompanyNameSub)},{Esc(p.HeadOfficeAddress)},{Esc(p.FactoryAddress)},{Esc(p.OtherOffice)},{Esc(p.Founded)},{Esc(p.Founded2)},{Esc(p.Capital)},{Esc(p.RepresentativeName)},{Esc(p.RepresentativeProfile)},{Esc(p.ShareholderInfo)},{Esc(p.BusinessDetail)},{Esc(p.Revenue)},{Esc(p.Employees)},{Esc(p.MainClients)},{Esc(p.MainSuppliers)},{Esc(p.Certifications)},{Esc(p.GroupCompanies)},{Esc(p.TransferReason)},{Esc(p.Remarks)}");
            File.WriteAllText(fProfiles, sbProfiles.ToString(), encoding);

            // --- 3. 財務ハイライト ---
            var sbFin = new StringBuilder();
            sbFin.AppendLine("案件内部ID(DealId),期区分(actual/forecast),順序(1-3),期ラベル,売上高,原価率,粗利益,粗利率,販管費,営業利益,営業利益率,経常利益,当期純利益,EBITDA,減価償却費,設備投資額,流動資産,現金預金,売掛金,棚卸資産,その他流動,固定資産,総資産,流動負債,買掛金,短期借入金,その他流動負債,固定負債,長期借入金,その他固定負債,負債合計,純資産合計,利益剰余金");
            foreach (var f in financials) sbFin.AppendLine($"{f.DealId},{Esc(f.PeriodType)},{f.PeriodOrder},{Esc(f.PeriodLabel)},{f.Revenue},{f.CostRate},{f.GrossProfit},{f.GrossProfitRate},{f.SGA},{f.OperatingProfit},{f.OperatingProfitRate},{f.OrdinaryProfit},{f.NetIncome},{f.EBITDA},{f.Depreciation},{f.CapEx},{f.CurrentAssets},{f.CashEquivalents},{f.AccountsReceivable},{f.Inventory},{f.OtherCurrentAssets},{f.FixedAssets},{f.TotalAssets},{f.CurrentLiabilities},{f.AccountsPayable},{f.ShortTermDebt},{f.OtherCurrentLiabilities},{f.FixedLiabilities},{f.LongTermDebt},{f.OtherFixedLiabilities},{f.TotalLiabilities},{f.NetAssets},{f.RetainedEarnings}");
            File.WriteAllText(fFinancials, sbFin.ToString(), encoding);

            // --- 4. 株式価値試算 ---
            var sbVal = new StringBuilder();
            sbVal.AppendLine("案件内部ID(DealId),修正純資産額,純資産法備考,EBITDA基準値,EBITDA基準年度,マルチプル倍率,ネットキャッシュ(EBITDA用),EBITDA法備考,割引率,永続成長率,EV(DCF),ネットキャッシュ(DCF用),DCF法備考,NOI,キャップレート,ネットキャッシュ(直接還元用),直接還元備考,EBITDA算定株式価値,DCF算定株式価値,直接還元算定株式価値,総合備考");
            foreach (var v in valuations) sbVal.AppendLine($"{v.DealId},{v.NetAssetValue},{Esc(v.NetAssetNote)},{v.EBITDABase},{Esc(v.EBITDABaseYear)},{v.EBITDAMultiple},{v.EBITDANetCashDebt},{Esc(v.EBITDANote)},{v.DCFDiscountRate},{v.DCFTerminalGrowth},{v.DCFEV},{v.DCFNetCashDebt},{Esc(v.DCFNote)},{v.NOI},{v.CapRate},{v.DirectNetCashDebt},{Esc(v.DirectNote)},{v.EBITDAEquityValue},{v.DCFEquityValue},{v.DirectEquityValue},{Esc(v.ValuationNote)}");
            File.WriteAllText(fValuations, sbVal.ToString(), encoding);

            // --- 5. 添付ファイル情報 ---
            var sbAtt = new StringBuilder();
            sbAtt.AppendLine("案件内部ID(DealId),ファイル名,アプリ内保管パス,ファイル備考,登録日時");
            foreach (var a in attachments) sbAtt.AppendLine($"{a.DealId},{Esc(a.FileName)},{Esc(a.FilePath)},{Esc(a.Description)},{Esc(a.UploadedAt)}");
            File.WriteAllText(fAttachments, sbAtt.ToString(), encoding);

            return deals.Count;
        }

        // CSVエスケープ用ヘルパーメソッド
        private string Esc(string? value)
        {
            if (string.IsNullOrEmpty(value)) return "";
            if (value.Contains(",") || value.Contains("\"") || value.Contains("\n") || value.Contains("\r"))
            {
                return "\"" + value.Replace("\"", "\"\"") + "\"";
            }
            return value;
        }

        // ══════════════════════════════════════════════════════
        // 2. インポート処理 (Upsert)
        // ══════════════════════════════════════════════════════
        public int ImportDeals(string filePath)
        {
            var deals = CsvParser.Parse(filePath);
            int count = 0;
            foreach (var deal in deals)
            {
                // DealId(手入力の案件ID文字列)をキーにして既存を検索
                var existing = _dealRepo.SearchDeals(deal.DealId).FirstOrDefault(d => d.DealId == deal.DealId);

                if (existing != null)
                {
                    // 既存データがある場合はIDを引き継いで「更新」
                    deal.Id = existing.Id;
                    _dealRepo.UpdateDeal(deal); // ★エラーが出る場合は既存の更新メソッド名(例: Update等)に変更してください
                }
                else
                {
                    // 既存データがない場合は「新規追加」
                    _dealRepo.AddDeal(deal); // ★エラーが出る場合は既存の追加メソッド名(例: InsertDeal等)に変更してください
                }

                count++;
            }
            return count;
        }

        public int ImportCompanyProfiles(string filePath)
        {
            var profiles = CsvParser.ParseCompanyProfiles(filePath);
            int count = 0;
            foreach (var p in profiles)
            {
                if (_dealRepo.GetDealById(p.DealId) != null) { _finRepo.UpsertCompanyProfile(p); count++; }
            }
            return count;
        }

        public int ImportFinancials(string filePath)
        {
            var financials = CsvParser.ParseFinancialHighlights(filePath);
            int count = 0;
            foreach (var f in financials)
            {
                if (_dealRepo.GetDealById(f.DealId) != null) { _finRepo.UpsertFinancialHighlight(f); count++; }
            }
            return count;
        }

        public int ImportValuations(string filePath)
        {
            var valuations = CsvParser.ParseValuationDataList(filePath);
            int count = 0;
            foreach (var v in valuations)
            {
                if (_dealRepo.GetDealById(v.DealId) != null) { _finRepo.UpsertValuationData(v); count++; }
            }
            return count;
        }

        public int ImportAttachments(string filePath)
        {
            var attachments = CsvParser.ParseAttachments(filePath);
            int count = 0;
            foreach (var a in attachments)
            {
                if (_dealRepo.GetDealById(a.DealId) != null) { _attachRepo.SaveAttachment(a); count++; }
            }
            return count;
        }

        // ══════════════════════════════════════════════════════
        // 3. バックアップ・復元処理 (ZIP)
        // ══════════════════════════════════════════════════════
        public void CreateBackupZip(string zipFilePath)
        {
            string tempDir = Path.Combine(Path.GetTempPath(), "MABackup_" + Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDir);

            try
            {
                // データベースファイルをテンポラリにコピー
                string dbPath = DatabaseContext.DbFilePath;
                if (File.Exists(dbPath))
                {
                    File.Copy(dbPath, Path.Combine(tempDir, "app_data.db"), true);
                }

                // 既存のZIPがあれば削除
                if (File.Exists(zipFilePath)) File.Delete(zipFilePath);

                // テンポラリフォルダごとZIP圧縮
                ZipFile.CreateFromDirectory(tempDir, zipFilePath);
            }
            finally
            {
                // 一時フォルダのお掃除
                if (Directory.Exists(tempDir)) Directory.Delete(tempDir, true);
            }
        }

        public void RestoreFromZip(string zipFilePath)
        {
            string tempDir = Path.Combine(Path.GetTempPath(), "MARestore_" + Guid.NewGuid().ToString());
            if (Directory.Exists(tempDir)) Directory.Delete(tempDir, true);
            Directory.CreateDirectory(tempDir);

            try
            {
                // ZIPを展開
                ZipFile.ExtractToDirectory(zipFilePath, tempDir);
                string extractedDbPath = Path.Combine(tempDir, "app_data.db");

                if (!File.Exists(extractedDbPath))
                    throw new FileNotFoundException("バックアップファイルの中にデータベース(app_data.db)が見つかりません。");

                // コネクションをすべて確実に閉じてから上書き
                GC.Collect();
                GC.WaitForPendingFinalizers();

                string currentDbPath = DatabaseContext.DbFilePath;

                // 元のDBを念のためリネームしてバックアップ
                string fallbackPath = currentDbPath + ".bak";
                if (File.Exists(currentDbPath))
                {
                    if (File.Exists(fallbackPath)) File.Delete(fallbackPath);
                    File.Move(currentDbPath, fallbackPath);
                }

                // 復元したDBを配置
                File.Copy(extractedDbPath, currentDbPath);
            }
            finally
            {
                // 一時フォルダのお掃除
                if (Directory.Exists(tempDir)) Directory.Delete(tempDir, true);
            }
        }
    }
}