using System;
using System.Text;

namespace MAItems.MailParser.Parsers
{
    /// <summary>
    /// M&A総合研究所のメール本文パーサー。
    /// 識別キーワード：「M&A総合研究所」
    /// </summary>
    public class MASoukenParser : MailParserBase
    {
        private const string Identifier = "M&A総合研究所";

        public override bool CanParse(string mailBody)
            => mailBody.Contains(Identifier, StringComparison.OrdinalIgnoreCase);

        public override ParsedDeal Parse(string mailBody)
        {
            // 全角→半角に正規化
            string body = NormalizeBody(mailBody);

            // 行ごとに分割
            var lines = body.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

            var result = new ParsedDeal
            {
                BrokerCompany = "M&A総合研究所",
                Route = "メール",
                InputDate = ExtractSentDate(body),
            };

            // 1. 基準行 "案件概要" を探す
            int refIndex = -1;
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Contains("案件概要"))
                {
                    refIndex = i;
                    break;
                }
            }

            // 基準行が見つからなければそのまま返す
            if (refIndex == -1) return result;

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

            // --- タイトルの取得 (案件概要の3行前) ---
            if (refIndex >= 3)
            {
                result.Title = lines[refIndex - 3].Trim();
            }

            bool featuresStarted = false;
            var featuresSb = new StringBuilder();

            // 基準行の3行前 ～ 終了行までループして抽出
            for (int k = Math.Max(0, refIndex - 3); k <= endIndex; k++)
            {
                // 全角スペースを半角に置換してからTrim
                string currentLine = lines[k].Replace("　", " ").Trim();

                // --- 特徴の複数行取得処理 ---
                if (featuresStarted)
                {
                    if (!string.IsNullOrEmpty(currentLine))
                    {
                        // VBAでは "／" で繋いでいましたが、C#の複数行テキストボックスで
                        // 見やすく表示されるよう、改行(\r\n)で繋ぐようにしています。
                        featuresSb.AppendLine(currentLine);
                    }
                }

                // --- 各項目の抽出 ---
                if (currentLine.Contains("■案件ID")) result.DealId = ExtractValue(currentLine, "■案件ID");
                else if (currentLine.Contains("■業種")) result.BusinessContent = ExtractValue(currentLine, "■業種");
                else if (currentLine.Contains("■エリア")) result.Area = ExtractValue(currentLine, "■エリア");
                else if (currentLine.Contains("■売上高")) result.Revenue = ExtractValue(currentLine, "■売上高");
                else if (currentLine.Contains("■営業利益")) result.OperatingProfit = ExtractValue(currentLine, "■営業利益");
                else if (currentLine.Contains("■EBITDA")) result.EBITDA = ExtractValue(currentLine, "■EBITDA");
                else if (currentLine.Contains("■純資産額")) result.NetAssets = ExtractValue(currentLine, "■純資産額");
                else if (currentLine.Contains("■総資産額")) result.TotalAssets = ExtractValue(currentLine, "■総資産額");
                else if (currentLine.Contains("■現金・現金同等物")) result.CashEquivalents = ExtractValue(currentLine, "■現金・現金同等物");
                else if (currentLine.Contains("■実質手元資金")) result.NetCashDebt = ExtractValue(currentLine, "■実質手元資金");
                else if (currentLine.Contains("■有利子負債等")) result.InterestBearingDebt = ExtractValue(currentLine, "■有利子負債等");
                else if (currentLine.Contains("■ネット有利子負債"))
                {
                    string? val = ExtractValue(currentLine, "■ネット有利子負債");
                    if (!string.IsNullOrEmpty(val))
                    {
                        // すでにマイナス記号が付いていなければ「▲」を付与
                        if (!val.StartsWith("▲") && !val.StartsWith("-") && !val.StartsWith("△"))
                            result.NetCashDebt = "▲" + val;
                        else
                            result.NetCashDebt = val;
                    }
                }
                else if (currentLine.Contains("■従業員数")) result.EmployeeCount = ExtractValue(currentLine, "■従業員数");

                // 特徴の開始判定 (文字列「特徴」が含まれる行)
                else if (currentLine.Contains("特徴") && !featuresStarted)
                {
                    featuresStarted = true;
                    int idx = currentLine.IndexOf("特徴");
                    string tempFeature = currentLine.Substring(idx + 2).Trim();

                    // コロン(:や：)があれば除去
                    if (tempFeature.StartsWith("：") || tempFeature.StartsWith(":"))
                    {
                        tempFeature = tempFeature.Substring(1).Trim();
                    }

                    if (!string.IsNullOrEmpty(tempFeature))
                    {
                        featuresSb.AppendLine(tempFeature);
                    }
                }

                else if (currentLine.Contains("■譲渡希望額")) result.AskingPrice = ExtractValue(currentLine, "■譲渡希望額");
                else if (currentLine.Contains("■譲渡希望形態")) result.TransferType = ExtractValue(currentLine, "■譲渡希望形態");
                else if (currentLine.Contains("■譲渡希望理由")) result.TransferReason = ExtractValue(currentLine, "■譲渡希望理由");
                else if (currentLine.Contains("■希望譲渡条件")) result.TransferConditions = ExtractValue(currentLine, "■希望譲渡条件");
            }

            if (featuresSb.Length > 0)
            {
                result.Features = featuresSb.ToString().TrimEnd();
            }

            return result;
        }

        // --- 補助関数 (VBAの ExtractValueFromLine 相当) ---
        private static string? ExtractValue(string line, string key)
        {
            int idx = line.IndexOf(key, StringComparison.OrdinalIgnoreCase);
            if (idx >= 0)
            {
                string val = line.Substring(idx + key.Length).Trim();

                // 念のためコロンが含まれている場合は除去
                if (val.StartsWith("：") || val.StartsWith(":"))
                {
                    val = val.Substring(1).Trim();
                }
                return string.IsNullOrEmpty(val) ? null : val;
            }
            return null;
        }
    }
}