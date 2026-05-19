using System.Collections.Generic;

namespace MAItems.MailParser
{
    /// <summary>
    /// メール本文パーサーの共通インターフェース。
    /// 仲介会社ごとに実装クラスを作成する。
    /// </summary>
    public interface IMailParser
    {
        /// <summary>
        /// このパーサーが対象のメール本文かどうかを判定する。
        /// </summary>
        bool CanParse(string mailBody);

        /// <summary>
        /// メール本文を解析して ParsedDeal に変換する。
        /// </summary>
        List<ParsedDeal> Parse(string mailBody);
    }
}