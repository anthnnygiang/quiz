using System.Globalization;
using System.Text;
using Spectre.Console;

namespace Quiz;

public enum AnswerMatch
{
    Correct,
    Close,
    Incorrect,
}

public class Card(string question, string answer, string culture)
{
    public string Question { get; } = question;
    public string Answer { get; } = answer;
    public string Culture { get; } = culture;
    private CultureInfo CultureInfo { get; } = GetCulture(culture);
    private static readonly Dictionary<string, CultureInfo> CultureDictionary = new(StringComparer.OrdinalIgnoreCase);

    private static CultureInfo GetCulture(string culture)
    {
        if (string.IsNullOrWhiteSpace(culture))
        {
            return CultureInfo.InvariantCulture;
        }

        if (CultureDictionary.TryGetValue(culture, out var cachedCulture))
        {
            return cachedCulture;
        }

        var cultureInfo = CultureInfo.GetCultureInfo(culture);
        CultureDictionary.TryAdd(cultureInfo.Name, cultureInfo);
        return cultureInfo;
    }

    public AnswerMatch CheckAnswer(string response)
    {
        if (IsCorrectAnswer(response, Answer))
        {
            return AnswerMatch.Correct;
        }

        if (IsCloseAnswer(response, Answer))
        {
            return AnswerMatch.Close;
        }

        return AnswerMatch.Incorrect;
    }

    private bool IsCorrectAnswer(string response, string answer)
    {
        var normalizedResponse = NormalizeForCorrectComparison(response);
        var normalizedAnswer = NormalizeForCorrectComparison(answer);
        return CultureInfo.CompareInfo.Compare(normalizedResponse, normalizedAnswer, GetCompareOptions(CultureInfo)) == 0;
    }

    private bool IsCloseAnswer(string response, string answer)
    {
        var normalizedResponse = NormalizeForCloseComparison(response);
        var normalizedAnswer = NormalizeForCloseComparison(answer);
        return CultureInfo.CompareInfo.Compare(normalizedResponse, normalizedAnswer, GetCompareOptions(CultureInfo)) == 0;
    }

    private static string NormalizeForCorrectComparison(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        var normalized = value.Normalize(NormalizationForm.FormC);
        // (char[]?)null splits on all whitespace characters
        return string.Join(' ', normalized.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries));
    }

    private static string NormalizeForCloseComparison(string value)
    {
        var normalized = NormalizeForCorrectComparison(value);

        if (normalized.Length == 0)
        {
            return string.Empty;
        }

        var builder = new StringBuilder(normalized.Length);

        foreach (var c in normalized)
        {
            if (char.IsWhiteSpace(c) || IsIgnoredCloseMatchCharacter(c))
            {
                continue;
            }

            builder.Append(c);
        }

        return builder.ToString();
    }

    // Characters that are ignored for close match comparison (e.g. punctuation, symbols)
    private static bool IsIgnoredCloseMatchCharacter(char c)
    {
        return char.GetUnicodeCategory(c) switch
        {
            UnicodeCategory.ConnectorPunctuation or
                UnicodeCategory.DashPunctuation or
                UnicodeCategory.OpenPunctuation or
                UnicodeCategory.ClosePunctuation or
                UnicodeCategory.InitialQuotePunctuation or
                UnicodeCategory.FinalQuotePunctuation or
                UnicodeCategory.OtherPunctuation or
                UnicodeCategory.MathSymbol or
                UnicodeCategory.CurrencySymbol or
                UnicodeCategory.ModifierSymbol or
                UnicodeCategory.OtherSymbol => true,
            _ => false,
        };
    }

    private static CompareOptions GetCompareOptions(CultureInfo cultureInfo)
    {
        return cultureInfo.Name switch
        {
            // Japanese: hiragana/katakana and full/half-width are interchangeable
            "ja-JP" => CompareOptions.IgnoreKanaType | CompareOptions.IgnoreCase | CompareOptions.IgnoreWidth,

            // Chinese and Korean: full-width and half-width are interchangeable
            "zh-CN" or "zh-TW" or "zh-HK" or "ko-KR" => CompareOptions.IgnoreCase | CompareOptions.IgnoreWidth,

            // Default: case-insensitive invariant fallback; specify CultureInfo for
            // language-specific answers such as vi-VN, tr-TR, el-GR, ja-JP, zh-*, and ko-KR
            _ => CompareOptions.IgnoreCase,
        };
    }
}