---
acceptance_criteria:
- AuthController methods return IActionResult or Task.FromResult instead of using
  async
- No compiler warnings about async methods lacking await
- API behavior unchanged
created: '2026-03-20'
epic_id: EPIC-EX-5
id: US-EX-30
points: 3
priority: could
status: done
tags:
- bugfix
- code-quality
title: Remove unnecessary async from AuthController methods
updated: '2026-03-20'
---

As a developer, I want AuthController methods to not use async when they contain no await statements so that unnecessary state machine overhead is avoided. Both V1 and V2 AuthController Post methods (lines 19, 25) are marked async Task but just return Ok() synchronously.