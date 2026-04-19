using System.Globalization;

namespace Learn;

internal sealed record QuestionAnswer(string Question, string Answer, CultureInfo? Culture = null)
{
    public bool IsCorrectAnswer(string response)
    {
        // response is already trimmed and normalized by AnsiConsole.Ask
        var compareInfo = (Culture ?? CultureInfo.InvariantCulture).CompareInfo;
        return compareInfo.Compare(response, Answer, GetCompareOptions()) == 0;
    }

    private CompareOptions GetCompareOptions()
    {
        return Culture?.Name switch
        {
            // Japanese: hiragana/katakana and full/half-width are interchangeable
            "ja-JP" => CompareOptions.IgnoreKanaType | CompareOptions.IgnoreCase | CompareOptions.IgnoreWidth,

            // Chinese and Korean: full-width and half-width are interchangeable
            "zh-CN" or "zh-TW" or "zh-HK" or "ko-KR" => CompareOptions.IgnoreCase | CompareOptions.IgnoreWidth,

            // Default: case-insensitive (covers accent-significant languages like
            // vi-VN, es-*, fr-FR, de-DE, pt-*, it-IT, ru-RU, ar-SA, and unknown cultures)
            _ => CompareOptions.IgnoreCase,
        };
    }
}