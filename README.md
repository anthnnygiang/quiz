# Quiz

Quiz CLI with culture-aware answer matching.


## TODO:
find better colors for breakdown chart

## Usage

Quiz files are represented as:

JSON format
```json
[
  {
    "question": "What is the capital of France?",
    "answer": "Paris",
    "culture": "en-US"
  },
  {
    "question": "What is the largest planet in our solar system?",
    "answer": "Jupiter",
    "culture": "en-US"
  }
]


```
Common culture values:
1. en-US - English (United States)
2. en-GB - English (United Kingdom)
3. es-419 - Spanish (Latin America and Caribbean)
4. fr-FR - French (France)
5. it-IT - Italian (Italy)
6. de-DE - German (Germany)
7. ru-RU - Russian (Russia)
8. pt-BR - Portuguese (Brazil)
9. hi-IN - Hindi (India)
10. zh-Hans-CN - Chinese (Simplified, China)
11. ja-JP - Japanese (Japan)
12. ko-KR - Korean (South Korea)
13. vi-VN - Vietnamese (Vietnam)
14. id-ID - Indonesian (Indonesia)
15. el-GR - Greek (Greece)

Parse quiz file
Load into memory
--shuffle --tags easy geography
Start quiz
Show total questions number
Show current question number
Show results BreakdownChart

Comparison behavior:

* default questions use invariant, case-insensitive comparison
* `vi-VN` keeps Vietnamese accents significant
* `ja-JP` ignores case, kana type, and full-width vs half-width differences

```bash
$ quiz start <file.json>
```
* shows results at end of quiz (chart, animation)
* instant feedback on each question
* list quiz files in tree structure

## Uncertain Future
* retry wrong answers
* multiple answers per question
* llm?

