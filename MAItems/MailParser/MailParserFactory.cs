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
                // Features（備考）などからURLを抽出
                var urlMatch = Regex.Match(deal.Features ?? "", @"URL:\s*(https?://\S+)");
                if (urlMatch.Success)
                {
                    string targetUrl = urlMatch.Groups[1].Value;
                    // WebScraperを呼び出して足りない項目を上書き
                    await WebScraper.EnrichDealFromWebAsync(deal, targetUrl);
                }
            }

            return deals;
        }

    }
}