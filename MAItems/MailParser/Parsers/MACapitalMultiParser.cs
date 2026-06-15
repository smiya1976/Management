using System;
using System.Collections.Generic;
using System.IO;

namespace MAItems.MailParser.Parsers
{
    public class MACapitalMultiParser : MailParserBase
    {
        private const string Identifier = "M&Aキャピタルパートナーズ";

        public string BrokerName => Identifier;

        public override bool CanParse(string mailBody)
        {
            // 💡 修正1: 「新着案件情報」という見出しが変わっても対応できるように条件を緩和
            return (mailBody.Contains("M&A Capital Partners") || mailBody.Contains("M&A案件情報のお知らせ") || mailBody.Contains("M&Aキャピタルパートナーズ"))
                && mailBody.Contains("案件番号：");
        }

        public override List<ParsedDeal> Parse(string mailBody)
        {
            var deals = new List<ParsedDeal>();
            string inputDate = DateTime.Today.ToString("yyyy/MM/dd");

            using (var reader = new StringReader(mailBody))
            {
                string? line;
                string previousLine = "";
                ParsedDeal? currentDeal = null;

                while ((line = reader.ReadLine()) != null)
                {
                    line = line.Trim();
                    if (string.IsNullOrEmpty(line)) continue;

                    // 案件ブロックの開始
                    if (line.StartsWith("案件番号："))
                    {
                        currentDeal = new ParsedDeal
                        {
                            BrokerCompany = Identifier,
                            Route = "メール",
                            InputDate = inputDate,
                            // 💡 修正2: 「案件番号」の直前の行をタイトルとして確実に取得する
                            Title = previousLine,
                            DealId = line.Substring("案件番号：".Length).Trim()
                        };
                    }
                    else if (currentDeal != null)
                    {
                        if (line.StartsWith("詳細業種："))
                        {
                            currentDeal.BusinessContent = line.Substring("詳細業種：".Length).Trim();
                        }
                        else if (line.StartsWith("所在地："))
                        {
                            currentDeal.Area = line.Substring("所在地：".Length).Trim();
                        }
                        else if (line.StartsWith("概算売上："))
                        {
                            currentDeal.Revenue = line.Substring("概算売上：".Length).Trim();
                        }
                        else if (line.StartsWith("URL：") || line.StartsWith("URL:"))
                        {
                            // 💡 修正3: 全角・半角コロンの両方に対応
                            int colonIndex = line.IndexOf("：") != -1 ? "URL：".Length : "URL:".Length;
                            currentDeal.Features = "URL: " + line.Substring(colonIndex).Trim();

                            // 💡 URLの行が来たらその案件のブロックは終了とし、リストに追加して次へ
                            deals.Add(currentDeal);
                            currentDeal = null;
                        }
                    }

                    // 次のループ処理のために、現在の行を「直前の行」として記憶しておく
                    previousLine = line;
                }
            }

            return deals;
        }
    }
}