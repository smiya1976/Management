using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MAItems.MailParser.Parsers
{
    public class MACapitalParser : MailParserBase
    {
        public override bool CanParse(string mailBody)
        {
            // M&Aキャピタルからのメールか判定（全角・半角の揺れに対応）
            bool isMACapital = mailBody.Contains("M&Aキャピタルパートナーズ") || mailBody.Contains("Ｍ＆Ａキャピタルパートナーズ");

            // 💡 【重要】複数案件のメルマガには必ず「詳細業種：」という言葉が入るため、
            // それが含まれていないものを「単一案件のメール」として確実に見分ける
            bool isMultiDealMail = mailBody.Contains("詳細業種：");

            return isMACapital && !isMultiDealMail;
        }

        public override List<ParsedDeal> Parse(string mailBody)
        {
            string body = NormalizeBody(mailBody);

            var result = new ParsedDeal
            {
                BrokerCompany = "M&Aキャピタルパートナーズ",
                Route = "メール",
                InputDate = ExtractSentDate(body),
            };

            // ── 基本情報 ──────────────────────────────────
            result.DealId = ExtractLine(body, @"案件番号\s*[：:]\s*(.+)");
            result.Area = ExtractLine(body, @"所在地\s*[：:]\s*(.+)");

            // 今回のフォーマットは「所在地：」の前の行が事業内容になっている
            result.BusinessContent = ExtractLineBeforeKeyword(body, "所在地");
            result.Title = result.BusinessContent; // 単一案件の場合、これをタイトルにもセット

            // ── 財務項目（PL）※箇条書きの「・」等に対応 ────────
            result.Revenue = ExtractLine(body, @"(?:・|■|▼|\-)?\s*(?:調整後|調整済|調整)?\s*売上高\s*[：:]\s*(.+)");
            result.OperatingProfit = ExtractLine(body, @"(?:・|■|▼|\-)?\s*(?:調整後|調整済|調整)?\s*営業利益\s*[：:]\s*(.+)");
            result.EBITDA = ExtractLine(body, @"(?:・|■|▼|\-)?\s*(?:調整後|調整済|調整)?\s*EBITDA\s*[：:]\s*(.+)");

            // ── 財務項目（BS）※「時価純資産」等に対応 ────────
            result.CashEquivalents = ExtractLine(body, @"(?:・|■|▼|\-)?\s*現金(?:及び現金同等物|・現金同等物|同等物|及び同等物)?\s*[：:]\s*(.+)");
            result.NetCashDebt = ExtractNetCashDebt(body);
            result.NetAssets = ExtractLine(body, @"(?:・|■|▼|\-)?\s*(?:調整後|調整済|調整|時価)?\s*純資産\s*[：:]\s*(.+)");
            result.TotalAssets = ExtractLine(body, @"(?:・|■|▼|\-)?\s*(?:調整後|調整済|調整)?\s*総資産\s*[：:]\s*(.+)");
            result.InterestBearingDebt = ExtractLine(body, @"(?:・|■|▼|\-)?\s*(?:調整後|調整済|調整)?\s*有利子負債\s*[：:]\s*(.+)");

            // ── その他 ──────────────────────────────────
            result.EmployeeCount = ExtractLine(body, @"(?:・|■|▼|\-)?\s*従業員(?:数|数等)?\s*[：:]\s*(.+)");
            result.AskingPrice = ExtractLine(body, @"(?:・|■|▼|\-)?\s*(?:譲渡対価|譲渡希望額|希望譲渡額|売却希望額)\s*[：:]\s*(.+)");
            result.TransferType = ExtractLine(body, @"(?:・|■|▼|\-)?\s*(?:譲渡形態|希望譲渡形態|売却形態)\s*[：:]\s*(.+)");

            // ── ブロック抽出 ──────────────────────────────
            result.Features = ExtractBlock(body, "【事業概要】");
            result.TransferReason = ExtractBlock(body, "【譲渡背景】");
            result.TransferConditions = ExtractBlock(body, "【希望条件】");

            // ── URL抽出（Featuresに結合させることでWebスクレイピングに引き継ぐ） ──
            var urlMatch = Regex.Match(body, @"URL\s*[：:]\s*(https?://\S+)");
            if (urlMatch.Success)
            {
                string url = urlMatch.Groups[1].Value;
                result.Features = (string.IsNullOrWhiteSpace(result.Features) ? "" : result.Features + "\r\n\r\n") + "URL: " + url;
            }

            return new List<ParsedDeal> { result };
        }

        private static string? ExtractNetCashDebt(string body)
        {
            var netCashMatch = ExtractLine(body, @"(?:・|■|▼|\-)?\s*(?:調整後|調整済|調整)?\s*(?:ネットキャッシュ|NET\s*Cash|実質手持ち資金|実質現預金|実質無借金)\s*[：:]\s*(.+)");
            if (netCashMatch != null) return netCashMatch;

            var netDebtMatch = ExtractLine(body, @"(?:・|■|▼|\-)?\s*(?:調整後|調整済|調整)?\s*(?:ネットデット|NET\s*Debt|ネット有利子負債|実質有利子負債|純有利子負債)\s*[：:]\s*(.+)");
            if (netDebtMatch != null)
            {
                string trimmed = netDebtMatch.TrimStart();
                if (trimmed.StartsWith("▲") || trimmed.StartsWith("△") || trimmed.StartsWith("-")) return netDebtMatch;
                return "▲" + netDebtMatch;
            }
            return null;
        }
    }
}