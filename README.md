# Quiz

Quiz CLI with culture-aware answer matching.

## Usage

Questions in code are represented as:

```csharp
new Question("Question text", "Answer", new CultureInfo("ja-JP"))
```

use JSON
```json
[
  {
    "question": "Cái gì có thể bay nhưng không có cánh?",
    "answer": "Con chim",
    "culture": "vi-VN"
  },
  {
    "question": "What has keys but can't open locks?",
    "answer": "A piano",
    "culture": "en-US"
  },
  {
    "question": "カタカナで「コンピュータ」と書いてください。",
    "answer": "コンピュータ",
    "culture": "ja-JP"
  }
]
```


Comparison behavior:

* default questions use invariant, case-insensitive comparison
* `vi-VN` keeps Vietnamese accents significant
* `ja-JP` ignores case, kana type, and full-width vs half-width differences

```bash
$ quiz <file.tsv>
```
* shows results at end of quiz
* instant feedback on each question
* list quiz files in tree structure

## Future
* retry wrong answers
* multiple answers per question
* ai?