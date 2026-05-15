using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MAItems.Database
{
    public static class CsvParser
    {
        private static readonly Dictionary<string, string> HeaderMap = new()
        {
            ["入力日"] = "InputDate",
            ["経路"] = "Route",
            ["仲介会社"] = "BrokerCompany",
            ["タイトル"] = "Title",
            ["案件ID"] = "DealId",
            ["事業内容"] = "BusinessContent",
            ["エリア"] = "Area",
            ["売上高"] = "Revenue",
            ["営業利益"] = "OperatingProfit",
            ["EBITDA"] = "EBITDA",
            ["純資産額"] = "NetAssets",
            ["総資産額"] = "TotalAssets",
            ["NET Cash/Debt"] = "NetCashDebt",
            ["現金・現金同等物"] = "CashEquivalents",
            ["有利子負債等"] = "InterestBearingDebt",
            ["従業員数"] = "EmployeeCount",
            ["特徴"] = "Features",
            ["譲渡希望額"] = "AskingPrice",
            ["譲渡希望形態"] = "TransferType",
            ["譲渡希望理由"] = "TransferReason",
            ["希望譲渡条件"] = "TransferConditions",
            ["処理"] = "Status",
        };

        public static List<Deal> Parse(string filePath)
        {
            var deals = new List<Deal>();

            using var reader = new StreamReader(
                filePath, Encoding.UTF8,
                detectEncodingFromByteOrderMarks: true);

            string? headerLine = reader.ReadLine();
            if (headerLine == null) return deals;

            var headers = ParseLine(headerLine);
            var columnIndex = new Dictionary<string, int>();

            for (int i = 0; i < headers.Count; i++)
            {
                string h = headers[i].Trim();
                if (HeaderMap.TryGetValue(h, out string? propName))
                    columnIndex[propName] = i;
            }

            string? line;
            while ((line = reader.ReadLine()) != null)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;

                var cols = ParseLine(line);
                var deal = new Deal();

                string Get(string prop)
                    => columnIndex.TryGetValue(prop, out int idx)
                       && idx < cols.Count
                       ? cols[idx].Trim()
                       : string.Empty;

                deal.InputDate = Get("InputDate");
                deal.Route = Get("Route");
                deal.BrokerCompany = Get("BrokerCompany");
                deal.Title = Get("Title");
                deal.DealId = Get("DealId");
                deal.BusinessContent = Get("BusinessContent");
                deal.Area = Get("Area");
                deal.Revenue = Get("Revenue");
                deal.OperatingProfit = Get("OperatingProfit");
                deal.EBITDA = Get("EBITDA");
                deal.NetAssets = Get("NetAssets");
                deal.TotalAssets = Get("TotalAssets");
                deal.NetCashDebt = Get("NetCashDebt");
                deal.CashEquivalents = Get("CashEquivalents");
                deal.InterestBearingDebt = Get("InterestBearingDebt");
                deal.EmployeeCount = Get("EmployeeCount");
                deal.Features = Get("Features");
                deal.AskingPrice = Get("AskingPrice");
                deal.TransferType = Get("TransferType");
                deal.TransferReason = Get("TransferReason");
                deal.TransferConditions = Get("TransferConditions");
                deal.Status = Get("Status");

                deals.Add(deal);
            }

            return deals;
        }

        private static List<string> ParseLine(string line)
        {
            var fields = new List<string>();
            var current = new StringBuilder();
            bool inQuote = false;

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];

                if (inQuote)
                {
                    if (c == '"')
                    {
                        if (i + 1 < line.Length && line[i + 1] == '"')
                        {
                            current.Append('"');
                            i++;
                        }
                        else
                        {
                            inQuote = false;
                        }
                    }
                    else
                    {
                        current.Append(c);
                    }
                }
                else
                {
                    if (c == '"')
                        inQuote = true;
                    else if (c == ',')
                    {
                        fields.Add(current.ToString());
                        current.Clear();
                    }
                    else
                        current.Append(c);
                }
            }

            fields.Add(current.ToString());
            return fields;
        }
    }
}