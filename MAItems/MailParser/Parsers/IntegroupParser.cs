using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace MAItems.MailParser.Parsers
{
    public class IntegroupParser : MailParserBase
    {
        private const string Identifier = "インテグループ";

        // 前回のエラー修正に則り、override は付けずにプロパティを定義
        public string BrokerName => Identifier;

        public override bool CanParse(string mailBody)
        {
            return mailBody.Contains(Identifier) || mailBody.Contains("INTEGROUP") || mailBody.Contains("integroup");
        }

        public override List<ParsedDeal> Parse(string mailBody)
        {
            var deals = new List<ParsedDeal>();
            string inputDate = DateTime.Today.ToString("yyyy/MM/dd");

            // メールのヘッダー等にある日付（例：2026年05月26日号）を取得
            var dateMatch = Regex.Match(mailBody, @"(\d{4})年(\d{2})月(\d{2})日号");
            if (dateMatch.Success)
            {
                inputDate = $"{dateMatch.Groups[1].Value}/{dateMatch.Groups[2].Value}/{dateMatch.Groups[3].Value}";
            }

            using (var reader = new StringReader(mailBody))
            {
                string? line;
                ParsedDeal? currentDeal = null;

                while ((line = reader.ReadLine()) != null)
                {
                    line = line.Trim();
                    // HTMLメール由来の「見えない文字（ゼロ幅スペース等）」を削除して安全に処理する
                    line = Regex.Replace(line, @"[\u200B-\u200D\uFEFF]", "");

                    if (string.IsNullOrEmpty(line)) continue;

                    // ①〜⑳の丸数字から始まる行を「案件の開始（タイトル）」として判定
                    var titleMatch = Regex.Match(line, @"^[①-⑳]\s*(.+)");
                    if (titleMatch.Success)
                    {
                        // 前の案件が処理中ならリストに追加
                        if (currentDeal != null)
                        {
                            deals.Add(currentDeal);
                        }

                        currentDeal = new ParsedDeal
                        {
                            BrokerCompany = Identifier,
                            Route = "メール",
                            InputDate = inputDate,
                            Title = titleMatch.Groups[1].Value.Trim()
                        };
                    }
                    else if (currentDeal != null)
                    {
                        // 💡 修正: 行頭(^)の縛りを外し、Match機能を使って「項目名」と「値」を一発で確実に分離する
                        var matchIndustry = Regex.Match(line, @"業\s*種\s*[：:]\s*(.+)");
                        var matchArea = Regex.Match(line, @"エリア\s*[：:]\s*(.+)");
                        var matchRevenue = Regex.Match(line, @"売\s*上\s*[：:]\s*(.+)");
                        var matchPic = Regex.Match(line, @"担当者\s*[：:]\s*(.+)");

                        if (matchIndustry.Success)
                        {
                            currentDeal.BusinessContent = matchIndustry.Groups[1].Value.Trim();
                        }
                        else if (matchArea.Success)
                        {
                            currentDeal.Area = matchArea.Groups[1].Value.Trim();
                        }
                        else if (matchRevenue.Success)
                        {
                            currentDeal.Revenue = matchRevenue.Groups[1].Value.Trim();
                        }
                        else if (matchPic.Success)
                        {
                            string pic = matchPic.Groups[1].Value.Trim();
                            currentDeal.Features = string.IsNullOrEmpty(currentDeal.Features)
                                ? $"担当者: {pic}"
                                : currentDeal.Features + $"\r\n担当者: {pic}";
                        }
                        else if (line.Contains("━━━━━"))
                        {
                            // 区切り線を検知したら、その案件のブロックは終了とみなす
                            // (※ここも StartsWith ではなく Contains にして安全性を高めました)
                            deals.Add(currentDeal);
                            currentDeal = null;
                        }
                    }
                }

                // 最後の案件が区切り線なしで終わっていた場合の救済処理
                if (currentDeal != null)
                {
                    deals.Add(currentDeal);
                }
            }

            return deals;
        }
    }
}