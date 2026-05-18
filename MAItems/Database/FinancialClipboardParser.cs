using System;
using System.Collections.Generic;

namespace MAItems.Database
{
    /// <summary>
    /// 解析結果を格納するデータクラス
    /// </summary>
    public class ParsedFinancialTable
    {
        // Key: 列インデックス (0〜5), Value: 期のラベル (例: "24/3期")
        public Dictionary<int, string> Headers { get; set; } = new Dictionary<int, string>();

        // Key: DBのフィールド名 (例: "Revenue"), Value: 抽出された数値の配列 (最大6列分)
        public Dictionary<string, double?[]> Rows { get; set; } = new Dictionary<string, double?[]>();
    }

    /// <summary>
    /// クリップボードの財務データを解析するパーサークラス
    /// </summary>
    public static class FinancialClipboardParser
    {
        // 表題のゆらぎ吸収用辞書
        private static readonly Dictionary<string, string> LabelMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "売上高", "Revenue" },
            { "売上原価", "CostRate" },
            { "原価", "CostRate" },
            { "売上総利益", "GrossProfit" },
            { "粗利益", "GrossProfit" },
            { "販売費", "SGA" },
            { "一般管理費", "SGA" },
            { "販管費", "SGA" },
            { "営業利益", "OperatingProfit" },
            { "経常利益", "OrdinaryProfit" },
            { "当期純利益", "NetIncome" },
            { "純利益", "NetIncome" },
            { "EBITDA", "EBITDA" },
            { "減価償却", "Depreciation" },
            { "設備投資", "CapEx" },
            { "流動資産", "CurrentAssets" },
            { "現金", "CashEquivalents" },
            { "売掛金", "AccountsReceivable" },
            { "棚卸資産", "Inventory" },
            { "固定資産", "FixedAssets" },
            { "総資産", "TotalAssets" },
            { "流動負債", "CurrentLiabilities" },
            { "買掛金", "AccountsPayable" },
            { "短期借入金", "ShortTermDebt" },
            { "固定負債", "FixedLiabilities" },
            { "長期借入金", "LongTermDebt" },
            { "負債合計", "TotalLiabilities" },
            { "純資産", "NetAssets" },
            { "利益剰余金", "RetainedEarnings" }
        };

        /// <summary>
        /// タブ区切りのTSVテキストを解析し、構造化されたデータを返します
        /// </summary>
        public static ParsedFinancialTable ParseTsv(string text, int maxColumns = 6)
        {
            var result = new ParsedFinancialTable();
            if (string.IsNullOrWhiteSpace(text)) return result;

            var lines = text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            bool isFirstLine = true; // 1行目かどうかを判定するフラグ

            foreach (var line in lines)
            {
                var cols = line.Split('\t');
                if (cols.Length < 2) continue;

                string rowLabel = cols[0].Trim();

                // --- 1. ヘッダー行の判定（確実に1行目だけをヘッダーとして処理する） ---
                if (isFirstLine)
                {
                    for (int i = 1; i < cols.Length && (i - 1) < maxColumns; i++)
                    {
                        string headerStr = cols[i].Trim();
                        if (!string.IsNullOrEmpty(headerStr))
                        {
                            result.Headers[i - 1] = headerStr;
                        }
                    }
                    isFirstLine = false; // 1行目が終わったらフラグを折る
                    continue;
                }

                // --- 2. 除外キーワード（これらが含まれる行は安全のため無視する） ---
                // ※「調整項目」や細かい内訳が誤検知されるのを防ぎます
                if (rowLabel.Contains("調整項目") ||
                    rowLabel.Contains("有形") ||
                    rowLabel.Contains("無形"))
                {
                    continue;
                }

                // --- 3. データ行の判定（キーワードマッチング） ---
                string? targetTag = null;
                foreach (var kvp in LabelMap)
                {
                    if (rowLabel.Contains(kvp.Key, StringComparison.OrdinalIgnoreCase))
                    {
                        targetTag = kvp.Value;
                        break;
                    }
                }

                if (targetTag != null)
                {
                    // --- 4. 上書き防止チェック ---
                    // すでに値がセットされている項目（大項目）は、後から来た内訳項目で上書きしない！
                    // 例：「固定資産」のあとに「その他の固定資産」が来ても無視する
                    // 例：「EBITDA」のあとに「調整後EBITDA」が来ても無視する
                    if (!result.Rows.ContainsKey(targetTag))
                    {
                        var values = new double?[maxColumns];
                        for (int i = 1; i < cols.Length && (i - 1) < maxColumns; i++)
                        {
                            values[i - 1] = NumericConverter.Convert(cols[i]);
                        }
                        result.Rows[targetTag] = values;
                    }
                }
            }

            return result;
        }
    }
}