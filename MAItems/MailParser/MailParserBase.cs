using System;
using System.Text;
using System.Text.RegularExpressions;


namespace MAItems.MailParser
{
    /// <summary>
    /// メール本文パーサーの基底クラス。
    /// 全パーサー共通の以下の機能を提供する。
    ///   ・全角→半角 正規化
    ///   ・1行抽出（正規表現）
    ///   ・ブロック抽出
    ///   ・送信日時抽出
    /// 各仲介会社のパーサーはこのクラスを継承して作成する。
    /// </summary>
    public abstract class MailParserBase : IMailParser
    {
        // ── 抽象メソッド（各パーサーで必ず実装） ──────────
        public abstract bool CanParse(string mailBody);
        public abstract List<ParsedDeal> Parse(string mailBody);

        // ══════════════════════════════════════════════════════
        // 全角→半角 正規化
        // ══════════════════════════════════════════════════════

        /// <summary>
        /// メール本文の全角文字を半角に一括正規化する。
        /// ひらがな・カタカナ・漢字・和文記号はそのまま保持。
        /// 変換対象：全角英数字・全角記号・全角スペース
        /// </summary>
        protected static string NormalizeBody(string body)
        {
            if (string.IsNullOrEmpty(body)) return body;

            var sb = new StringBuilder(body.Length);
            foreach (char c in body)
                sb.Append(ToHalfWidth(c));

            return sb.ToString();
        }

        /// <summary>
        /// 1文字を全角→半角に変換する。
        /// 変換対象外の文字はそのまま返す。
        /// </summary>
        private static char ToHalfWidth(char c)
        {
            // 全角英数字・記号（U+FF01～U+FF5E）→ 半角（U+0021～U+007E）
            if (c >= '\uFF01' && c <= '\uFF5E')
                return (char)(c - 0xFEE0);

            // 全角スペース → 半角スペース
            if (c == '\u3000')
                return ' ';

            return c;
        }

        // ══════════════════════════════════════════════════════
        // 共通抽出メソッド
        // ══════════════════════════════════════════════════════

        /// <summary>
        /// 正規表現パターンで本文から1行分の値を抽出する。
        /// 末尾の制御文字・全角スペース・改行を自動除去。
        /// マッチしない場合は null を返す。
        /// </summary>
        protected static string? ExtractLine(string body, string pattern)
        {
            var m = Regex.Match(body, pattern,
                RegexOptions.Multiline | RegexOptions.IgnoreCase);

            if (!m.Success) return null;

            string value = m.Groups[1].Value
                .Trim()
                .TrimEnd('\r', '\n', '\t', '\u3000', ' ');

            return string.IsNullOrEmpty(value) ? null : value;
        }

        /// <summary>
        /// 「【見出し】」から次の「【」または本文末尾までを
        /// ブロックとして抽出し、整形して返す。
        /// マッチしない場合は null を返す。
        /// </summary>
        protected static string? ExtractBlock(string body, string heading)
        {
            int start = body.IndexOf(heading, StringComparison.Ordinal);
            if (start < 0) return null;

            int contentStart = body.IndexOf('\n', start);
            if (contentStart < 0) return null;
            contentStart++;

            int nextHeading = body.IndexOf("【", contentStart,
                StringComparison.Ordinal);

            string block = nextHeading > 0
                ? body[contentStart..nextHeading]
                : body[contentStart..];

            var lines = block.Split(
                new[] { "\r\n", "\r", "\n" },
                StringSplitOptions.RemoveEmptyEntries);

            var sb = new StringBuilder();
            foreach (var line in lines)
            {
                string trimmed = line.Trim();
                if (!string.IsNullOrEmpty(trimmed))
                    sb.AppendLine(trimmed);
            }

            string result = sb.ToString().Trim();
            return string.IsNullOrEmpty(result) ? null : result;
        }

        /// <summary>
        /// 「送信日時：2026年5月12日火曜日 13:11」形式から
        /// 日付を抽出して "yyyy/M/d" 形式で返す。
        /// 抽出失敗時は今日の日付を返す。
        /// </summary>
        protected static string ExtractSentDate(string body)
        {
            var m = Regex.Match(body,
                @"送信日時[：:\t\s]+(\d{4})年\s*(\d{1,2})月\s*(\d{1,2})日",
                RegexOptions.Multiline);

            if (!m.Success)
                return DateTime.Now.ToString("yyyy/M/d");

            try
            {
                int year = int.Parse(m.Groups[1].Value);
                int month = int.Parse(m.Groups[2].Value);
                int day = int.Parse(m.Groups[3].Value);
                return new DateTime(year, month, day).ToString("yyyy/M/d");
            }
            catch
            {
                return DateTime.Now.ToString("yyyy/M/d");
            }
        }

        /// <summary>
        /// 指定した見出し行（例：「所在地：」）の
        /// 直前にある非空行を返す。
        /// セクション冒頭の業種名など見出し直前の値の取得に使用。
        /// </summary>
        protected static string? ExtractLineBeforeKeyword(
            string body, string keyword)
        {
            var lines = body.Split(
                new[] { "\r\n", "\r", "\n" },
                StringSplitOptions.None);

            for (int i = 0; i < lines.Length; i++)
            {
                if (!lines[i].TrimStart().StartsWith(keyword)) continue;

                for (int j = i - 1; j >= 0; j--)
                {
                    string prev = lines[j].Trim();
                    if (!string.IsNullOrEmpty(prev))
                        return prev;
                }
            }
            return null;
        }
    }
}