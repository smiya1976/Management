using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MAItems.Database;

namespace MAItems
{
    public static class CsvParser
    {
        public static List<Deal> Parse(string filePath)
        {
            var deals = new List<Deal>();

            // ファイルの1行目（ヘッダー）を読んで、タブ区切りかカンマ区切りかを自動判定
            string firstLine = File.ReadLines(filePath, Encoding.UTF8).FirstOrDefault() ?? "";
            char separator = firstLine.Contains('\t') ? '\t' : ',';

            // セル内の改行やカンマに対応したカスタムパーサーで読み込み
            var lines = ReadCsvLines(filePath, separator);
            if (lines.Count <= 1) return deals;

            // ヘッダーの列名をリスト化（列の順序が変わっても「名前」でデータを取得できるようにする）
            var headers = lines[0];

            for (int i = 1; i < lines.Count; i++)
            {
                var values = lines[i];
                if (values.Count == 0 || (values.Count == 1 && string.IsNullOrWhiteSpace(values[0]))) continue;

                // 配列のインデックス（番号）ではなく、ヘッダーの項目名でマッピングする
                var deal = new Deal
                {
                    Id = long.TryParse(GetValue(headers, values, "案件内部ID(Id)"), out long id) ? id : 0,
                    InputDate = GetValue(headers, values, "入力日") ?? "",
                    Route = GetValue(headers, values, "経路") ?? "",
                    BrokerCompany = GetValue(headers, values, "仲介会社") ?? "",
                    Title = GetValue(headers, values, "タイトル") ?? "",
                    DealId = GetValue(headers, values, "案件ID") ?? "",
                    BusinessContent = GetValue(headers, values, "事業内容") ?? "",
                    Area = GetValue(headers, values, "エリア") ?? "",
                    Revenue = GetValue(headers, values, "売上高") ?? "",
                    OperatingProfit = GetValue(headers, values, "営業利益") ?? "",
                    EBITDA = GetValue(headers, values, "EBITDA") ?? "",
                    NetAssets = GetValue(headers, values, "純資産額") ?? "",
                    TotalAssets = GetValue(headers, values, "総資産額") ?? "",
                    NetCashDebt = GetValue(headers, values, "NET Cash/Debt") ?? "",
                    CashEquivalents = GetValue(headers, values, "現金・現金同等物") ?? "",
                    InterestBearingDebt = GetValue(headers, values, "有利子負債等") ?? "",
                    EmployeeCount = GetValue(headers, values, "従業員数") ?? "",
                    Features = GetValue(headers, values, "特徴") ?? "",
                    AskingPrice = GetValue(headers, values, "譲渡希望額") ?? "",
                    TransferType = GetValue(headers, values, "譲渡希望形態") ?? "",
                    TransferReason = GetValue(headers, values, "譲渡希望理由") ?? "",
                    TransferConditions = GetValue(headers, values, "希望譲渡条件") ?? "",

                    // アプリ独自の項目（VBAからの取り込みなど、列が存在しない場合は空文字になる）
                    Status = GetValue(headers, values, "処理") ?? "",
                    AttachmentsSummary = GetValue(headers, values, "全体概況") ?? ""
                };
                deals.Add(deal);
            }
            return deals;
        }

        /// <summary>
        /// ヘッダー名から対応する列の値を安全に取得する
        /// </summary>
        private static string? GetValue(List<string> headers, List<string> values, string columnName)
        {
            int index = headers.IndexOf(columnName);
            if (index >= 0 && index < values.Count)
            {
                return values[index];
            }
            return null;
        }

        /// <summary>
        /// 「""」で囲まれたセル内の改行や区切り文字を考慮してファイルを解析する
        /// </summary>
        private static List<List<string>> ReadCsvLines(string filePath, char separator)
        {
            var result = new List<List<string>>();
            // Excel等で開いたCSVに対応するため UTF8 (または必要に応じて Shift_JIS) で読み込み
            string content = File.ReadAllText(filePath, Encoding.UTF8);

            var currentLine = new List<string>();
            var currentCell = new StringBuilder();
            bool inQuotes = false;

            for (int i = 0; i < content.Length; i++)
            {
                char c = content[i];

                if (c == '\"')
                {
                    // エスケープされたダブルクォート（""）の処理
                    if (inQuotes && i + 1 < content.Length && content[i + 1] == '\"')
                    {
                        currentCell.Append('\"');
                        i++;
                    }
                    else
                    {
                        inQuotes = !inQuotes;
                    }
                }
                else if (c == separator && !inQuotes)
                {
                    // セルの区切り
                    currentLine.Add(currentCell.ToString().Trim());
                    currentCell.Clear();
                }
                else if ((c == '\r' || c == '\n') && !inQuotes)
                {
                    // 行の区切り
                    if (c == '\r' && i + 1 < content.Length && content[i + 1] == '\n') i++; // CRLFをスキップ

                    currentLine.Add(currentCell.ToString().Trim());
                    currentCell.Clear();
                    result.Add(currentLine);
                    currentLine = new List<string>();
                }
                else
                {
                    // 通常の文字
                    currentCell.Append(c);
                }
            }

            // 最後のセル・行を処理
            if (currentCell.Length > 0 || currentLine.Count > 0)
            {
                currentLine.Add(currentCell.ToString().Trim());
                result.Add(currentLine);
            }

            return result;
        }

        // ══════════════════════════════════════════════════════
        // 2_会社基礎情報.csv のパース
        // ══════════════════════════════════════════════════════
        public static List<CompanyProfile> ParseCompanyProfiles(string filePath)
        {
            var list = new List<CompanyProfile>();
            string firstLine = File.ReadLines(filePath, Encoding.UTF8).FirstOrDefault() ?? "";
            char separator = firstLine.Contains('\t') ? '\t' : ',';

            var lines = ReadCsvLines(filePath, separator);
            if (lines.Count <= 1) return list;

            var headers = lines[0];
            for (int i = 1; i < lines.Count; i++)
            {
                var values = lines[i];
                if (values.Count == 0 || string.IsNullOrWhiteSpace(values[0])) continue;

                if (long.TryParse(GetValue(headers, values, "案件内部ID(DealId)"), out long dealId))
                {
                    list.Add(new CompanyProfile
                    {
                        DealId = dealId,
                        CompanyName = GetValue(headers, values, "会社名") ?? "",
                        CompanyNameSub = GetValue(headers, values, "別会社名") ?? "",
                        HeadOfficeAddress = GetValue(headers, values, "本社住所") ?? "",
                        FactoryAddress = GetValue(headers, values, "工場住所") ?? "",
                        OtherOffice = GetValue(headers, values, "その他事務所") ?? "",
                        Founded = GetValue(headers, values, "設立") ?? "",
                        Founded2 = GetValue(headers, values, "関連会社設立") ?? "",
                        Capital = GetValue(headers, values, "資本金") ?? "",
                        RepresentativeName = GetValue(headers, values, "代表者名") ?? "",
                        RepresentativeProfile = GetValue(headers, values, "代表者略歴") ?? "",
                        ShareholderInfo = GetValue(headers, values, "株主構成") ?? "",
                        BusinessDetail = GetValue(headers, values, "事業内容詳細") ?? "",
                        Revenue = GetValue(headers, values, "売上高") ?? "",
                        Employees = GetValue(headers, values, "従業員数") ?? "",
                        MainClients = GetValue(headers, values, "主要取引先") ?? "",
                        MainSuppliers = GetValue(headers, values, "主要仕入先") ?? "",
                        Certifications = GetValue(headers, values, "認証・許認可") ?? "",
                        GroupCompanies = GetValue(headers, values, "グループ会社") ?? "",
                        TransferReason = GetValue(headers, values, "譲渡理由") ?? "",
                        Remarks = GetValue(headers, values, "備考") ?? ""
                    });
                }
            }
            return list;
        }

        // ══════════════════════════════════════════════════════
        // 3_財務ハイライト.csv のパース
        // ══════════════════════════════════════════════════════
        public static List<FinancialHighlight> ParseFinancialHighlights(string filePath)
        {
            var list = new List<FinancialHighlight>();
            string firstLine = File.ReadLines(filePath, Encoding.UTF8).FirstOrDefault() ?? "";
            char separator = firstLine.Contains('\t') ? '\t' : ',';

            var lines = ReadCsvLines(filePath, separator);
            if (lines.Count <= 1) return list;

            var headers = lines[0];
            for (int i = 1; i < lines.Count; i++)
            {
                var values = lines[i];
                if (values.Count == 0 || string.IsNullOrWhiteSpace(values[0])) continue;

                if (long.TryParse(GetValue(headers, values, "案件内部ID(DealId)"), out long dealId))
                {
                    list.Add(new FinancialHighlight
                    {
                        DealId = dealId,
                        PeriodType = GetValue(headers, values, "期区分(actual/forecast)") ?? "actual",
                        PeriodOrder = int.TryParse(GetValue(headers, values, "順序(1-3)"), out int ord) ? ord : 1,
                        PeriodLabel = GetValue(headers, values, "期ラベル") ?? "",
                        Revenue = ParseNullableDouble(GetValue(headers, values, "売上高")),
                        CostRate = ParseNullableDouble(GetValue(headers, values, "原価率")),
                        GrossProfit = ParseNullableDouble(GetValue(headers, values, "粗利益")),
                        GrossProfitRate = ParseNullableDouble(GetValue(headers, values, "粗利率")),
                        SGA = ParseNullableDouble(GetValue(headers, values, "販管費")),
                        OperatingProfit = ParseNullableDouble(GetValue(headers, values, "営業利益")),
                        OperatingProfitRate = ParseNullableDouble(GetValue(headers, values, "営業利益率")),
                        OrdinaryProfit = ParseNullableDouble(GetValue(headers, values, "経常利益")),
                        NetIncome = ParseNullableDouble(GetValue(headers, values, "当期純利益")),
                        EBITDA = ParseNullableDouble(GetValue(headers, values, "EBITDA")),
                        Depreciation = ParseNullableDouble(GetValue(headers, values, "減価償却費")),
                        CapEx = ParseNullableDouble(GetValue(headers, values, "設備投資額")),
                        CurrentAssets = ParseNullableDouble(GetValue(headers, values, "流動資産")),
                        CashEquivalents = ParseNullableDouble(GetValue(headers, values, "現金預金")),
                        AccountsReceivable = ParseNullableDouble(GetValue(headers, values, "売掛金")),
                        Inventory = ParseNullableDouble(GetValue(headers, values, "棚卸資産")),
                        OtherCurrentAssets = ParseNullableDouble(GetValue(headers, values, "その他流動")),
                        FixedAssets = ParseNullableDouble(GetValue(headers, values, "固定資産")),
                        TotalAssets = ParseNullableDouble(GetValue(headers, values, "総資産")),
                        CurrentLiabilities = ParseNullableDouble(GetValue(headers, values, "流動負債")),
                        AccountsPayable = ParseNullableDouble(GetValue(headers, values, "買掛金")),
                        ShortTermDebt = ParseNullableDouble(GetValue(headers, values, "短期借入金")),
                        OtherCurrentLiabilities = ParseNullableDouble(GetValue(headers, values, "その他流動負債")),
                        FixedLiabilities = ParseNullableDouble(GetValue(headers, values, "固定負債")),
                        LongTermDebt = ParseNullableDouble(GetValue(headers, values, "長期借入金")),
                        OtherFixedLiabilities = ParseNullableDouble(GetValue(headers, values, "その他固定負債")),
                        TotalLiabilities = ParseNullableDouble(GetValue(headers, values, "負債合計")),
                        NetAssets = ParseNullableDouble(GetValue(headers, values, "純資産合計")),
                        RetainedEarnings = ParseNullableDouble(GetValue(headers, values, "利益剰余金"))
                    });
                }
            }
            return list;
        }

        // ══════════════════════════════════════════════════════
        // 4_株式価値試算.csv のパース
        // ══════════════════════════════════════════════════════
        public static List<ValuationData> ParseValuationDataList(string filePath)
        {
            var list = new List<ValuationData>();
            string firstLine = File.ReadLines(filePath, Encoding.UTF8).FirstOrDefault() ?? "";
            char separator = firstLine.Contains('\t') ? '\t' : ',';

            var lines = ReadCsvLines(filePath, separator);
            if (lines.Count <= 1) return list;

            var headers = lines[0];
            for (int i = 1; i < lines.Count; i++)
            {
                var values = lines[i];
                if (values.Count == 0 || string.IsNullOrWhiteSpace(values[0])) continue;

                if (long.TryParse(GetValue(headers, values, "案件内部ID(DealId)"), out long dealId))
                {
                    list.Add(new ValuationData
                    {
                        DealId = dealId,
                        NetAssetValue = ParseNullableDouble(GetValue(headers, values, "修正純資産額")),
                        NetAssetNote = GetValue(headers, values, "純資産法備考") ?? "",
                        EBITDABase = ParseNullableDouble(GetValue(headers, values, "EBITDA基準値")),
                        EBITDABaseYear = GetValue(headers, values, "EBITDA基準年度") ?? "",
                        EBITDAMultiple = ParseNullableDouble(GetValue(headers, values, "マルチプル倍率")),
                        EBITDANetCashDebt = ParseNullableDouble(GetValue(headers, values, "ネットキャッシュ(EBITDA用)")),
                        EBITDANote = GetValue(headers, values, "EBITDA法備考") ?? "",
                        DCFDiscountRate = ParseNullableDouble(GetValue(headers, values, "割引率")),
                        DCFTerminalGrowth = ParseNullableDouble(GetValue(headers, values, "永続成長率")),
                        DCFEV = ParseNullableDouble(GetValue(headers, values, "EV(DCF)")),
                        DCFNetCashDebt = ParseNullableDouble(GetValue(headers, values, "ネットキャッシュ(DCF用)")),
                        DCFNote = GetValue(headers, values, "DCF法備考") ?? "",
                        NOI = ParseNullableDouble(GetValue(headers, values, "NOI")),
                        CapRate = ParseNullableDouble(GetValue(headers, values, "キャップレート")),
                        DirectNetCashDebt = ParseNullableDouble(GetValue(headers, values, "ネットキャッシュ(直接還元用)")),
                        DirectNote = GetValue(headers, values, "直接還元備考") ?? "",
                        EBITDAEquityValue = ParseNullableDouble(GetValue(headers, values, "EBITDA算定株式価値")),
                        DCFEquityValue = ParseNullableDouble(GetValue(headers, values, "DCF算定株式価値")),
                        DirectEquityValue = ParseNullableDouble(GetValue(headers, values, "直接還元算定株式価値")),
                        ValuationNote = GetValue(headers, values, "総合備考") ?? ""
                    });
                }
            }
            return list;
        }

        // ══════════════════════════════════════════════════════
        // 5_添付ファイル情報.csv のパース
        // ══════════════════════════════════════════════════════
        public static List<Attachment> ParseAttachments(string filePath)
        {
            var list = new List<Attachment>();
            string firstLine = File.ReadLines(filePath, Encoding.UTF8).FirstOrDefault() ?? "";
            char separator = firstLine.Contains('\t') ? '\t' : ',';

            var lines = ReadCsvLines(filePath, separator);
            if (lines.Count <= 1) return list;

            var headers = lines[0];
            for (int i = 1; i < lines.Count; i++)
            {
                var values = lines[i];
                if (values.Count == 0 || string.IsNullOrWhiteSpace(values[0])) continue;

                if (long.TryParse(GetValue(headers, values, "案件内部ID(DealId)"), out long dealId))
                {
                    list.Add(new Attachment
                    {
                        DealId = dealId,
                        FileName = GetValue(headers, values, "ファイル名") ?? "",
                        FilePath = GetValue(headers, values, "アプリ内保管パス") ?? "",
                        Description = GetValue(headers, values, "ファイル備考") ?? "",
                        UploadedAt = GetValue(headers, values, "登録日時") ?? ""
                    });
                }
            }
            return list;
        }

        private static double? ParseNullableDouble(string? val)
        {
            if (string.IsNullOrWhiteSpace(val)) return null;
            return double.TryParse(val, out double r) ? r : null;
        }


    }
}