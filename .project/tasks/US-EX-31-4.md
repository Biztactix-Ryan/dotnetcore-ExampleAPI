---
assignee: claude
created: '2026-03-20'
depends_on: []
id: US-EX-31-4
points: 1
status: done
story_id: US-EX-31
tags:
- bugfix
- code-quality
title: Fix weak regex patterns in ExampleObjectValidators to use + instead of *
updated: '2026-03-20'
---

Update regex patterns in ExampleObjectValidators to use + quantifier instead of * where empty strings should not be allowed. Review phone validation to explicitly handle empty strings. Files: Validators/ExampleObjectValidator.cs and related validators.