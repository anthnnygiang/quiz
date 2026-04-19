using System.Globalization;
using Spectre.Console;

namespace Learn;

internal sealed record Query(string Question, string Answer, CultureInfo? Culture = null)
{
    public void Ask()
    {
        var response = AnsiConsole.Ask<string>($"{Question} ");
        var isCorrect = IsCorrectAnswer(response);
        var icon = isCorrect ? "[green]✔[/]" : "[red]✗[/]";

        AnsiConsole.Write("\e[1A\e[2K"); // Clear the previous line
        AnsiConsole.MarkupLine($"{Question.EscapeMarkup()} {response.EscapeMarkup()} {icon}");
    }

    private bool IsCorrectAnswer(string response)
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

            // Default: case-insensitive invariant fallback; specify CultureInfo for
            // language-specific answers such as vi-VN, tr-TR, el-GR, ja-JP, zh-*, and ko-KR
            _ => CompareOptions.IgnoreCase,
        };
    }
}