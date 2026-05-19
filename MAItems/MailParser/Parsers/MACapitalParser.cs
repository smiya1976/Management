using System;
using System.Collections.Generic;

namespace MAItems.MailParser.Parsers
{
    /// <summary>
    /// M&Aキャピタルパートナーズ株式会社のメール本文パーサー。
    /// 識別キーワード：「M&Aキャピタルパートナーズ」
    /// </summary>
    public class MACapitalParser : MailParserBase
    {
        private const string Identifier = "M&Aキャピタルパートナーズ";

        public override bool CanParse(string mailBody)
            => mailBody.Contains(Identifier,
                System.StringComparison.OrdinalIgnoreCase);

        public override List<ParsedDeal> Parse(string mailBody)
        {
            // 全角→半角に正規化
            string body = NormalizeBody(mailBody);

            var result = new ParsedDeal
            {
                BrokerCompany = "M&Aキャピタルパートナーズ",
                Route = "メール",
                InputDate = ExtractSentDate(body),
            };

            // ── 案件番号 ──────────────────────────────────
            result.DealId = ExtractLine(body,
                @"案件番号\s*[：:]\s*(.+)");

            // ── 所在地（エリア） ──────────────────────────
            result.Area = ExtractLine(body,
                @"所在地\s*[：:]\s*(.+)");

            // ── 業種（事業内容） ──────────────────────────
            result.BusinessContent =
                ExtractLineBeforeKeyword(body, "所在地");

            // ── タイトル ──────────────────────────────────
            result.Title = ExtractTitle(body);

            // ── 財務項目（PL） ────────────────────────────
            result.Revenue = ExtractLine(body,
                @"(?:調整後|調整済|調整)?\s*売上高\s*[：:]\s*(.+)");
            result.OperatingProfit = ExtractLine(body,
                @"(?:調整後|調整済|調整)?\s*営業利益\s*[：:]\s*(.+)");
            result.EBITDA = ExtractLine(body,
                @"(?:調整後|調整済|調整)?\s*EBITDA\s*[：:]\s*(.+)");

            // ── 財務項目（BS） ────────────────────────────
            result.CashEquivalents = ExtractLine(body,
                @"現金(?:及び現金同等物|・現金同等物|同等物|及び同等物)?\s*[：:]\s*(.+)");

            // ✅ NetCash/NetDebt を符号付きで抽出
            result.NetCashDebt = ExtractNetCashDebt(body);

            result.NetAssets = ExtractLine(body,
                @"(?:調整後|調整済|調整)?\s*純資産\s*[：:]\s*(.+)");
            result.TotalAssets = ExtractLine(body,
                @"(?:調整後|調整済|調整)?\s*総資産\s*[：:]\s*(.+)");
            result.InterestBearingDebt = ExtractLine(body,
                @"(?:調整後|調整済|調整)?\s*有利子負債\s*[：:]\s*(.+)");

            // ── 従業員数 ──────────────────────────────────
            result.EmployeeCount = ExtractLine(body,
                @"従業員\s*(?:数|数等)?\s*[：:]\s*(.+)");

            // ── 譲渡希望額 ────────────────────────────────
            result.AskingPrice = ExtractLine(body,
                @"(?:譲渡対価|譲渡希望額|希望譲渡額|売却希望額)\s*[：:]\s*(.+)");

            // ── 譲渡形態 ──────────────────────────────────
            result.TransferType = ExtractLine(body,
                @"(?:譲渡形態|希望譲渡形態|売却形態)\s*[：:]\s*(.+)");

            // ── ブロック抽出 ──────────────────────────────
            result.Features = ExtractBlock(body, "【事業概要】");
            result.TransferReason = ExtractBlock(body, "【譲渡背景】");
            result.TransferConditions = ExtractBlock(body, "【希望条件】");

            return new List<ParsedDeal> { result };
        }

        // ─── Net Cash / Net Debt 抽出 ─────────────────────
        /// <summary>
        /// NetCash系キーワード → そのまま（プラス）
        /// NetDebt系キーワード → ▲を付与（マイナス）
        /// すでに▲や-が付いている場合は二重付与しない
        /// </summary>
        private static string? ExtractNetCashDebt(string body)
        {
            // ── NetCash系（プラス） ───────────────────────
            var netCashMatch = ExtractLine(body,
                @"(?:調整後|調整済|調整)?\s*" +
                @"(?:ネットキャッシュ|NET\s*Cash|実質手持ち資金|実質現預金|実質無借金)" +
                @"\s*[：:]\s*(.+)");

            if (netCashMatch != null)
                return netCashMatch;

            // ── NetDebt系（マイナス） ─────────────────────
            var netDebtMatch = ExtractLine(body,
                @"(?:調整後|調整済|調整)?\s*" +
                @"(?:ネットデット|NET\s*Debt|ネット有利子負債|実質有利子負債|純有利子負債)" +
                @"\s*[：:]\s*(.+)");

            if (netDebtMatch != null)
            {
                string trimmed = netDebtMatch.TrimStart();

                // すでにマイナス記号がある場合はそのまま返す
                if (trimmed.StartsWith("▲") ||
                    trimmed.StartsWith("△") ||
                    trimmed.StartsWith("-"))
                    return netDebtMatch;

                // マイナス記号を付与
                return "▲" + netDebtMatch;
            }

            return null;
        }

        // ── タイトル抽出 ──────────────────────────────────
        private static string? ExtractTitle(string body)
        {
            var m = System.Text.RegularExpressions.Regex.Match(body,
                @"件名[：:].+?】\s*(.+)",
                System.Text.RegularExpressions.RegexOptions.Multiline);

            if (m.Success)
                return m.Groups[1].Value.Trim();

            return null;
        }
    }
}