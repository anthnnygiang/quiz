# Learn

Quiz CLI with culture-aware answer matching.

## Usage

Questions in code are represented as:

```csharp
new Question("Question text", "Answer", new CultureInfo("ja-JP"))
```

For CSV-backed quizzes, keep the existing two-column format or add an optional culture column:

```csv
question,answer
```

```csv
question,answer,culture
What is the capital of France?,Paris,
Tiếng Việt: thủ đô của Việt Nam là gì?,Hà Nội,vi-VN
日本語: 日本の首都はどこですか？,とうきょう,ja-JP
```

Comparison behavior:

* default questions use invariant, case-insensitive comparison
* `vi-VN` keeps Vietnamese accents significant
* `ja-JP` ignores case, kana type, and full-width vs half-width differences

```bash
$ learn <file.csv>
```
* shows results at end of quiz
* instant feedback on each question
* list quiz files in tree structure

## Future
* retry wrong answers
* multiple answers per question
* ai?