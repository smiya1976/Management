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
            return (mailBody.Contains("M&A Capital Partners") || mailBody.Contains("M&A案件情報のお知らせ"))
                && mailBody.Contains("新着案件情報")
                && mailBody.Contains("案件番号：");
        }

        public override List<ParsedDeal> Parse(string mailBody)
        {
            var deals = new List<ParsedDeal>();
            string inputDate = DateTime.Today.ToString("yyyy/MM/dd");

            using (var reader = new StringReader(mailBody))
            {
                string? line;
                string lastNonEmptyLine = "";
                ParsedDeal? currentDeal = null;

                while ((line = reader.ReadLine()) != null)
                {
                    line = line.Trim();
                    if (string.IsNullOrEmpty(line)) continue;

                    if (line.StartsWith("案件番号："))
                    {
                        currentDeal = new ParsedDeal
                        {
                            BrokerCompany = Identifier,
                            Route = "メール",
                            InputDate = inputDate,
                            Title = lastNonEmptyLine,
                            Features = lastNonEmptyLine,
                            DealId = line.Substring("案件番号：".Length).Trim()
                        };
                    }
                    else if (currentDeal != null && line.StartsWith("詳細業種："))
                    {
                        currentDeal.BusinessContent = line.Substring("詳細業種：".Length).Trim();
                    }
                    else if (currentDeal != null && line.StartsWith("所在地："))
                    {
                        currentDeal.Area = line.Substring("所在地：".Length).Trim();
                    }
                    else if (currentDeal != null && line.StartsWith("概算売上："))
                    {
                        currentDeal.Revenue = line.Substring("概算売上：".Length).Trim();
                    }
                    else if (currentDeal != null && line.StartsWith("URL："))
                    {
                        currentDeal.Features += "\r\nURL: " + line.Substring("URL：".Length).Trim();
                        deals.Add(currentDeal);
                        currentDeal = null;
                    }
                    else
                    {
                        lastNonEmptyLine = line;
                    }
                }
            }

            return deals;
        }
    }
}