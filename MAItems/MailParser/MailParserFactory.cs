using System.Collections.Generic;

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
            new Parsers.MACapitalParser(),
            new Parsers.MASoukenParser(),
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
    }
}