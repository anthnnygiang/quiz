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

