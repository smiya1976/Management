using System;
using System.Text.RegularExpressions;

namespace MAItems.Database
{
    public static class NumericConverter
    {
        public static double? Convert(string? raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return null;

            string s = raw.Trim();

            // ── 1. マイナス記号の正規化 ───────────────────
            bool isNegative = s.StartsWith("▲") || s.StartsWith("△")
                           || s.StartsWith("-");
            s = s.Replace("▲", "").Replace("△", "").Replace("-", "");

            // ── 2. ％・パーセントフラグ ───────────────────
            bool isPercent = s.Contains('%') || s.Contains('％')
                          || s.Contains("パーセント")
                          || s.Contains("percent",
                                StringComparison.OrdinalIgnoreCase);

            // ── 3. ●割の処理 ─────────────────────────────
            // 算用数字＋割
            var wariMatch = Regex.Match(s, @"(\d+(?:\.\d+)?)\s*割");
            if (wariMatch.Success)
            {
                double wv = double.Parse(wariMatch.Groups[1].Value) * 0.1;
                return isNegative ? -wv : wv;
            }

            // 漢数字＋割
            var kanjiWari = Regex.Match(s, @"([一二三四五六七八九十]+)\s*割");
            if (kanjiWari.Success)
            {
                double? kv = KanjiToDouble(kanjiWari.Groups[1].Value);
                if (kv != null)
                {
                    double wv = kv.Value * 0.1;
                    return isNegative ? -wv : wv;
                }
            }

            // ── 4. 漢数字混じりの数値を解析 ──────────────
            double? value = ParseMixed(s);
            if (value == null) return null;

            // ── 5. ％適用 ─────────────────────────────────
            if (isPercent) value *= 0.01;

            return isNegative ? -value : value;
        }

        // ─── 漢数字混じり文字列を数値へ ──────────────────
        private static double? ParseMixed(string s)
        {
            // 桁区切りカンマ・空白を除去
            s = s.Replace(",", "").Replace("，", "")
                 .Replace(" ", "").Replace("　", "");

            // 数字・小数点・漢数字桁以外を除去
            s = Regex.Replace(s,
                @"[^\d.〇一二三四五六七八九十百千万億兆]", "");

            if (string.IsNullOrEmpty(s)) return null;

            // 漢字を含まない場合はそのまま数値解析
            if (!Regex.IsMatch(s, @"[〇一二三四五六七八九十百千万億兆]"))
                return double.TryParse(s, out double d) ? d : null;

            return ParseWithUnits(s);
        }

        // ─── 兆・億・万 ブロックに分解して合算 ───────────
        private static double? ParseWithUnits(string s)
        {
            double total = 0;

            total += ExtractUnit(ref s, "兆", 1_000_000_000_000.0);
            total += ExtractUnit(ref s, "億", 100_000_000.0);
            total += ExtractUnit(ref s, "万", 10_000.0);

            if (!string.IsNullOrEmpty(s))
            {
                double? rem = ParseBelow10000(s);
                if (rem != null) total += rem.Value;
            }

            return total == 0 ? null : total;
        }

        private static double ExtractUnit(
            ref string s, string unit, double multiplier)
        {
            int idx = s.IndexOf(unit, StringComparison.Ordinal);
            if (idx < 0) return 0;

            string before = s[..idx];
            s = s[(idx + unit.Length)..];

            if (string.IsNullOrEmpty(before)) return multiplier;
            if (double.TryParse(before, out double d)) return d * multiplier;

            double? k = ParseBelow10000(before);
            return k.HasValue ? k.Value * multiplier : 0;
        }

        // ─── 万未満の漢数字を数値へ ───────────────────────
        private static double? ParseBelow10000(string s)
        {
            if (string.IsNullOrEmpty(s)) return null;

            if (double.TryParse(s, out double plain)) return plain;

            double? k = KanjiToDouble(s);
            if (k != null) return k;

            double result = 0;
            result += ExtractSubUnit(ref s, "千", 1000);
            result += ExtractSubUnit(ref s, "百", 100);
            result += ExtractSubUnit(ref s, "十", 10);

            if (!string.IsNullOrEmpty(s))
            {
                if (double.TryParse(s, out double rem)) result += rem;
                else result += KanjiDigit(s) ?? 0;
            }

            return result == 0 ? null : result;
        }

        private static double ExtractSubUnit(
            ref string s, string unit, double multiplier)
        {
            int idx = s.IndexOf(unit, StringComparison.Ordinal);
            if (idx < 0) return 0;

            string before = s[..idx];
            s = s[(idx + unit.Length)..];

            if (string.IsNullOrEmpty(before)) return multiplier;
            if (double.TryParse(before, out double d)) return d * multiplier;
            return (KanjiDigit(before) ?? 1) * multiplier;
        }

        // ─── 純粋な漢数字文字列を double に変換 ──────────
        private static double? KanjiToDouble(string s)
        {
            if (string.IsNullOrEmpty(s)) return null;

            // 全て一桁漢数字（〇〜九）の場合
            if (Regex.IsMatch(s, @"^[〇一二三四五六七八九]+$"))
            {
                double val = 0;
                foreach (char c in s)
                {
                    int? d = KanjiDigit(c.ToString());
                    if (d == null) return null;
                    val = val * 10 + d.Value;
                }
                return val;
            }

            // 十・百・千を含む場合
            double result = 0;
            string temp = s;
            result += ExtractSubUnit(ref temp, "千", 1000);
            result += ExtractSubUnit(ref temp, "百", 100);
            result += ExtractSubUnit(ref temp, "十", 10);

            if (!string.IsNullOrEmpty(temp))
                result += KanjiDigit(temp) ?? 0;

            return result == 0 && !s.Contains('〇') ? null : result;
        }

        private static int? KanjiDigit(string s)
        {
            if (string.IsNullOrEmpty(s)) return null;
            return s[^1] switch
            {
                '〇' or '零' => 0,
                '一' => 1,
                '二' => 2,
                '三' => 3,
                '四' => 4,
                '五' => 5,
                '六' => 6,
                '七' => 7,
                '八' => 8,
                '九' => 9,
                _ => null,
            };
        }
    }
}