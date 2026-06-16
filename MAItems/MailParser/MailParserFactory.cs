using MAItems.MailParser.Parsers;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MAItems.MailParser
{
    /// <summary>
    /// メール本文を受け取り、対応するパーサーを自動選択して返す。
    /// 新しい仲介会社のパーサーを追加する場合は
    /// _parsers リストに1行追加するだけでよい。
    /// </summary>
    public static class MailParserFactory
    {
        // ── 登録済みパーサー一覧（優先順位順） ────────────
        private static readonly List<IMailParser> _parsers =
            new List<IMailParser>
        {
            new Parsers.OndeckParser(),
            new Parsers.MACapitalMultiParser(),
            new Parsers.MACapitalParser(),
            new Parsers.MASoukenParser(),
            new Parsers.IntegroupParser(),

            // 将来追加例：
            // new Parsers.MitsubishiUFJParser(),
            // new Parsers.MAResearchParser(),
            // new Parsers.MAGeneralParser(),
        };

        /// <summary>
        /// 本文に対応するパーサーを返す。
        /// 対応するパーサーが見つからない場合は null を返す。
        /// </summary>
        public static IMailParser? GetParser(string mailBody)
        {
            foreach (var parser in _parsers)
            {
                if (parser.CanParse(mailBody))
                    return parser;
            }
            return null;
        }

        /// <summary>
        /// 登録済みパーサーの一覧を返す（設定画面等のUI表示用）
        /// </summary>
        public static IReadOnlyList<IMailParser> GetAllParsers()
            => _parsers.AsReadOnly();
    

        public static async Task<List<ParsedDeal>> ParseAndEnrichAsync(string mailBody)
        {
            var parser = GetParser(mailBody);

            // 対応するパーサーが無い場合は空リストを返す
            if (parser == null) return new List<ParsedDeal>();

            // 1. まずは通常通りテキストからデータを解析
            var deals = parser.Parse(mailBody);

            // 2. 解析結果にURLがあれば、裏側でWebデータを取りに行く
            foreach (var deal in deals)
            {
                var urlMatch = Regex.Match(deal.Features ?? "", @"URL:\s*(https?://\S+)");
                if (urlMatch.Success)
                {
                    string targetUrl = urlMatch.Groups[1].Value;

                    // 💡 【追加1】メール本文から読み取れた全データを「丸ごと」バックアップ
                    var emailBackup = CloneDeal(deal);

                    // WebScraper実行（ここで deal がWebの情報で容赦なく上書きされる）
                    await WebScraper.EnrichDealFromWebAsync(deal, targetUrl);

                    // 💡 【追加2】スクレイピング後、バックアップ側に文字が存在した項目「すべて」を強制復元
                    RestoreEmailPriority(deal, emailBackup);
                }
            }

            return deals;
        }

        // ══════════════════════════════════════════════════════
        // ── 追加: バックアップ作成用ヘルパーメソッド ──
        // ══════════════════════════════════════════════════════
        private static ParsedDeal CloneDeal(ParsedDeal source)
        {
            return new ParsedDeal
            {
                InputDate = source.InputDate,
                Route = source.Route,
                BrokerCompany = source.BrokerCompany,
                Title = source.Title,
                DealId = source.DealId,
                BusinessContent = source.BusinessContent,
                Area = source.Area,
                Revenue = source.Revenue,
                OperatingProfit = source.OperatingProfit,
                EBITDA = source.EBITDA,
                NetAssets = source.NetAssets,
                TotalAssets = source.TotalAssets,
                NetCashDebt = source.NetCashDebt,
                CashEquivalents = source.CashEquivalents,
                InterestBearingDebt = source.InterestBearingDebt,
                EmployeeCount = source.EmployeeCount,
                Features = source.Features,
                AskingPrice = source.AskingPrice,
                TransferType = source.TransferType,
                TransferReason = source.TransferReason,
                TransferConditions = source.TransferConditions,
                Status = source.Status
            };
        }

        // ══════════════════════════════════════════════════════
        // ── 追加: メール優先での強制復元メソッド ──
        // ══════════════════════════════════════════════════════
        private static void RestoreEmailPriority(ParsedDeal target, ParsedDeal backup)
        {
            // バックアップ(メール)に文字が入っていれば、ターゲット(Web上書き後)に上書きで戻す
            if (!string.IsNullOrWhiteSpace(backup.BusinessContent)) target.BusinessContent = backup.BusinessContent;
            if (!string.IsNullOrWhiteSpace(backup.Area)) target.Area = backup.Area;
            if (!string.IsNullOrWhiteSpace(backup.Revenue)) target.Revenue = backup.Revenue;
            if (!string.IsNullOrWhiteSpace(backup.OperatingProfit)) target.OperatingProfit = backup.OperatingProfit;
            if (!string.IsNullOrWhiteSpace(backup.EBITDA)) target.EBITDA = backup.EBITDA;
            if (!string.IsNullOrWhiteSpace(backup.NetAssets)) target.NetAssets = backup.NetAssets;
            if (!string.IsNullOrWhiteSpace(backup.TotalAssets)) target.TotalAssets = backup.TotalAssets;
            if (!string.IsNullOrWhiteSpace(backup.NetCashDebt)) target.NetCashDebt = backup.NetCashDebt;
            if (!string.IsNullOrWhiteSpace(backup.CashEquivalents)) target.CashEquivalents = backup.CashEquivalents;
            if (!string.IsNullOrWhiteSpace(backup.InterestBearingDebt)) target.InterestBearingDebt = backup.InterestBearingDebt;
            if (!string.IsNullOrWhiteSpace(backup.EmployeeCount)) target.EmployeeCount = backup.EmployeeCount;
            if (!string.IsNullOrWhiteSpace(backup.AskingPrice)) target.AskingPrice = backup.AskingPrice;
            if (!string.IsNullOrWhiteSpace(backup.TransferType)) target.TransferType = backup.TransferType;
            if (!string.IsNullOrWhiteSpace(backup.TransferReason)) target.TransferReason = backup.TransferReason;
            if (!string.IsNullOrWhiteSpace(backup.TransferConditions)) target.TransferConditions = backup.TransferConditions;

            // 💡 特徴(Features)だけの特別処理：メールの文章とWebの文章を両方生かす
            if (!string.IsNullOrWhiteSpace(backup.Features))
            {
                // Web側で新しく特徴が取得されていて、かつメールの内容と違う場合は「結合」する
                if (!string.IsNullOrWhiteSpace(target.Features) && target.Features != backup.Features)
                {
                    target.Features = backup.Features + "\r\n\r\n【Web補足情報】\r\n" + target.Features;
                }
                else
                {
                    target.Features = backup.Features;
                }
            }
        }

    }
}