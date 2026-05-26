using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace MAItems.MailParser.Parsers
{
    public class OndeckParser : MailParserBase
    {
        private const string Identifier = "オンデック";

        public string BrokerName => Identifier;

        public override bool CanParse(string mailBody)
        {
            return (mailBody.Contains(Identifier) || mailBody.Contains("ONDECK"))
                && mailBody.Contains("【案件No.】");
        }

        public override List<ParsedDeal> Parse(string mailBody)
        {
            var deals = new List<ParsedDeal>();

            string inputDate = DateTime.Today.ToString("yyyy/MM/dd");
            var dateMatch = Regex.Match(mailBody, @"【(\d{1,2})月(\d{1,2})日\s*配信】");
            if (dateMatch.Success)
            {
                int month = int.Parse(dateMatch.Groups[1].Value);
                int day = int.Parse(dateMatch.Groups[2].Value);
                inputDate = new DateTime(DateTime.Today.Year, month, day).ToString("yyyy/MM/dd");
            }

            string[] blocks = Regex.Split(mailBody, @"(?=【案件No\.】)");

            foreach (var block in blocks)
            {
                if (!block.Contains("【業種】")) continue;

                var deal = new ParsedDeal
                {
                    BrokerCompany = Identifier,
                    Route = "メール",
                    InputDate = inputDate
                };

                var dict = new Dictionary<string, string>();
                string currentKey = "";
                var featuresBuilder = new StringBuilder();

                using (var reader = new StringReader(block))
                {
                    string? line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        line = line.Trim();
                        if (string.IsNullOrEmpty(line)) continue;

                        if (line.StartsWith("※この案件に関する") || line.StartsWith("各SNSでも") || line.StartsWith("発行元："))
                        {
                            currentKey = "";
                            continue;
                        }

                        var keyMatch = Regex.Match(line, @"^【(.+?)】\s*(.*)");
                        if (keyMatch.Success)
                        {
                            currentKey = keyMatch.Groups[1].Value.Trim();
                            string val = keyMatch.Groups[2].Value.Trim();
                            dict[currentKey] = val;

                            if (currentKey == "備考" && !string.IsNullOrEmpty(val))
                            {
                                featuresBuilder.AppendLine(val);
                            }
                        }
                        else if (line.StartsWith("■"))
                        {
                            currentKey = "FeaturesMarker";
                            featuresBuilder.AppendLine(line);
                        }
                        else if (line.StartsWith("*"))
                        {
                            featuresBuilder.AppendLine(line);
                        }
                        else if (!string.IsNullOrEmpty(currentKey))
                        {
                            if (currentKey == "FeaturesMarker" || currentKey == "備考")
                            {
                                featuresBuilder.AppendLine(line);
                            }
                            else
                            {
                                dict[currentKey] = (dict[currentKey] + " " + line).Trim();
                            }
                        }
                    }
                }

                deal.DealId = GetValue(dict, "案件No.");
                deal.BusinessContent = GetValue(dict, "業種");
                deal.Area = GetValue(dict, "所在地");
                deal.Revenue = GetValue(dict, "売上高");
                deal.OperatingProfit = GetValue(dict, "営業利益");
                deal.EBITDA = GetValue(dict, "EBITDA");
                deal.CashEquivalents = GetValue(dict, "実質手元資金");
                deal.TotalAssets = GetValue(dict, "簿価総資産");
                deal.NetAssets = GetValue(dict, "簿価純資産");
                deal.EmployeeCount = GetValue(dict, "従業員数");
                deal.TransferReason = GetValue(dict, "譲渡理由");
                deal.AskingPrice = GetValue(dict, "譲渡希望額");
                deal.TransferType = GetValue(dict, "譲渡形態");

                deal.Features = featuresBuilder.ToString().Trim();

                if (!string.IsNullOrEmpty(deal.BusinessContent))
                {
                    deal.Title = $"{deal.BusinessContent} ({deal.Area})";
                }

                deals.Add(deal);
            }

            return deals;
        }

        private string GetValue(Dictionary<string, string> dict, string key)
        {
            return dict.TryGetValue(key, out string? val) ? val : "";
        }
    }
}