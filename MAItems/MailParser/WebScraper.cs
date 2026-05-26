using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MAItems.MailParser
{
    public static class WebScraper
    {
        // アプリケーション全体で1つのHttpClientを使い回す（リソース枯渇を防ぐベストプラクティス）
        private static readonly HttpClient _httpClient = new HttpClient();
        static WebScraper()
        {
            // Chromeブラウザを偽装するUser-Agent
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/124.0.0.0 Safari/537.36");

            // 人間らしいアクセスに見せるための追加ヘッダー
            _httpClient.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,*/*;q=0.8");
            _httpClient.DefaultRequestHeaders.Add("Accept-Language", "ja,en-US;q=0.9,en;q=0.8");
        }


        // ══════════════════════════════════════════════════════
        // URLからWebページを取得し、案件情報の足りない項目を埋める
        // ══════════════════════════════════════════════════════
        public static async Task EnrichDealFromWebAsync(ParsedDeal deal, string url)
        {
            if (string.IsNullOrEmpty(url)) return;

            try
            {
                // WebページのHTMLデータを取得
                string html = await _httpClient.GetStringAsync(url);

                // HTMLのテーブル等から正規表現でデータを抽出して追記する
                // ※M&Aキャピタルパートナーズのサイトの一般的な <th>項目名</th><td>値</td> 構造を想定

                if (string.IsNullOrEmpty(deal.OperatingProfit))
                    deal.OperatingProfit = ExtractHtmlValue(html, "営業利益");

                if (string.IsNullOrEmpty(deal.EBITDA))
                    deal.EBITDA = ExtractHtmlValue(html, "EBITDA");

                if (string.IsNullOrEmpty(deal.NetAssets))
                    deal.NetAssets = ExtractHtmlValue(html, "純資産");

                if (string.IsNullOrEmpty(deal.TotalAssets))
                    deal.TotalAssets = ExtractHtmlValue(html, "総資産");

                if (string.IsNullOrEmpty(deal.EmployeeCount))
                    deal.EmployeeCount = ExtractHtmlValue(html, "従業員数");

                if (string.IsNullOrEmpty(deal.TransferReason))
                    deal.TransferReason = ExtractHtmlValue(html, "譲渡理由");

                if (string.IsNullOrEmpty(deal.AskingPrice))
                    deal.AskingPrice = ExtractHtmlValue(html, "希望額");

            }
            catch (Exception ex)
            {
                // 通信エラーや404エラーが起きてもアプリを落とさず、備考にエラーをメモして続行
                deal.Features += $"\r\n[Web取得エラー: {ex.Message}]";
            }
        }

        // <th>項目名</th> に続く <td>値</td> の中身を抜き出すヘルパーメソッド
        private static string ExtractHtmlValue(string html, string itemName)
        {
            string pattern = $@"<dt[^>]*>\s*{itemName}\s*</dt>\s*<dd[^>]*>\s*(.*?)\s*</dd>";

            var match = Regex.Match(html, pattern, RegexOptions.Singleline | RegexOptions.IgnoreCase);

            if (match.Success)
            {
                // HTMLタグが混ざっていたら除去
                string value = Regex.Replace(match.Groups[1].Value, "<.*?>", string.Empty);
                // 途中の改行や連続するスペースを綺麗にする
                value = Regex.Replace(value, @"\s+", " ");
                return System.Net.WebUtility.HtmlDecode(value.Trim());
            }
            return "";
        }
    }
}