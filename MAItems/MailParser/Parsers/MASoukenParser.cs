using System;
using System.Collections.Generic;
using System.Text;

namespace MAItems.MailParser.Parsers
{
    public class MASoukenParser : MailParserBase
    {
        private const string Identifier = "M&A総合研究所";

        public override bool CanParse(string mailBody)
            => mailBody.Contains(Identifier, StringComparison.OrdinalIgnoreCase);

        // 戻り値を List<ParsedDeal> に変更
        public override List<ParsedDeal> Parse(string mailBody)
        {
            var results = new List<ParsedDeal>(); // 複数案件を格納するリスト
            string body = NormalizeBody(mailBody);
            var lines = body.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            string sentDate = ExtractSentDate(body);

            for (int i = 0; i < lines.Length; i++)
            {
                // 1. 基準行 "案件概要" を探す
                if (lines[i].Contains("案件概要"))
                {
                    int refIndex = i;
                    if (refIndex < 3) continue;

                    // 2. レコードの終了行を探す
                    int endIndex = lines.Length - 1;
                    for (int j = refIndex + 1; j < lines.Length; j++)
                    {
                        if (lines[j].Contains("詳細情報はこちら") || lines[j].Contains("詳細はこちら") || lines[j].Contains("案件概要"))
                        {
                            endIndex = j - 1;
                            break;
                        }
                    }

                    var deal = new ParsedDeal
                    {
                        BrokerCompany = "M&A総合研究所",
                        Route = "メール",
                        InputDate = sentDate,
                        Title = lines[refIndex - 3].Trim()
                    };

                    bool featuresStarted = false;
                    var featuresSb = new StringBuilder();

                    // 基準行の3行前 ～ 終了行までループして抽出
                    for (int k = Math.Max(0, refIndex - 3); k <= endIndex; k++)
                    {
                        string currentLine = lines[k].Replace("　", " ").Trim();

                        // 特徴の複数行取得
                        if (featuresStarted)
                        {
                            if (currentLine.Contains("詳細情報はこちら") || currentLine.Contains("詳細はこちら"))
                            {
                                featuresStarted = false;
                            }
                            else if (!string.IsNullOrEmpty(currentLine) && !currentLine.StartsWith("■"))
                            {
                                featuresSb.AppendLine(currentLine);
                            }
                        }

                        // 各項目の抽出
                        if (currentLine.Contains("■案件ID")) deal.DealId = ExtractValue(currentLine, "■案件ID");
                        else if (currentLine.Contains("■業種")) deal.BusinessContent = ExtractValue(currentLine, "■業種");
                        else if (currentLine.Contains("■エリア")) deal.Area = ExtractValue(currentLine, "■エリア");
                        else if (currentLine.Contains("■売上高")) deal.Revenue = ExtractValue(currentLine, "■売上高");
                        else if (currentLine.Contains("■営業利益")) deal.OperatingProfit = ExtractValue(currentLine, "■営業利益");
                        else if (currentLine.Contains("■EBITDA")) deal.EBITDA = ExtractValue(currentLine, "■EBITDA");
                        else if (currentLine.Contains("■純資産額")) deal.NetAssets = ExtractValue(currentLine, "■純資産額");
                        else if (currentLine.Contains("■総資産額")) deal.TotalAssets = ExtractValue(currentLine, "■総資産額");
                        else if (currentLine.Contains("■現金・現金同等物")) deal.CashEquivalents = ExtractValue(currentLine, "■現金・現金同等物");
                        else if (currentLine.Contains("■実質手元資金")) deal.NetCashDebt = ExtractValue(currentLine, "■実質手元資金");
                        else if (currentLine.Contains("■有利子負債等")) deal.InterestBearingDebt = ExtractValue(currentLine, "■有利子負債等");
                        else if (currentLine.Contains("■ネット有利子負債"))
                        {
                            string? val = ExtractValue(currentLine, "■ネット有利子負債");
                            if (!string.IsNullOrEmpty(val))
                            {
                                if (!val.StartsWith("▲") && !val.StartsWith("-") && !val.StartsWith("△")) deal.NetCashDebt = "▲" + val;
                                else deal.NetCashDebt = val;
                            }
                        }
                        else if (currentLine.Contains("■従業員数")) deal.EmployeeCount = ExtractValue(currentLine, "■従業員数");

                        // 特徴の開始判定
                        else if (currentLine.Contains("特徴") && !featuresStarted)
                        {
                            featuresStarted = true;
                            string tempFeature = currentLine.Substring(currentLine.IndexOf("特徴") + 2).Trim();
                            if (tempFeature.StartsWith("：") || tempFeature.StartsWith(":")) tempFeature = tempFeature.Substring(1).Trim();
                            if (!string.IsNullOrEmpty(tempFeature)) featuresSb.AppendLine(tempFeature);
                        }

                        else if (currentLine.Contains("■譲渡希望額")) deal.AskingPrice = ExtractValue(currentLine, "■譲渡希望額");
                        else if (currentLine.Contains("■譲渡希望形態")) deal.TransferType = ExtractValue(currentLine, "■譲渡希望形態");
                        else if (currentLine.Contains("■譲渡希望理由")) deal.TransferReason = ExtractValue(currentLine, "■譲渡希望理由");
                        else if (currentLine.Contains("■希望譲渡条件")) deal.TransferConditions = ExtractValue(currentLine, "■希望譲渡条件");
                    }

                    if (featuresSb.Length > 0) deal.Features = featuresSb.ToString().TrimEnd();

                    // 完成した1件の案件をリストに追加
                    results.Add(deal);

                    // 処理済みの行までループをスキップ（次の案件を探すため）
                    i = endIndex;
                }
            }

            return results;
        }

        private static string? ExtractValue(string line, string key)
        {
            int idx = line.IndexOf(key, StringComparison.OrdinalIgnoreCase);
            if (idx >= 0)
            {
                string val = line.Substring(idx + key.Length).Trim();
                if (val.StartsWith("：") || val.StartsWith(":")) val = val.Substring(1).Trim();
                return string.IsNullOrEmpty(val) ? null : val;
            }
            return null;
        }
    }
}