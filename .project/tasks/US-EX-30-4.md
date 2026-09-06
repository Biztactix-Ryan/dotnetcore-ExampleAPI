---
assignee: claude
created: '2026-03-20'
depends_on: []
id: US-EX-30-4
points: 1
status: done
story_id: US-EX-30
tags:
- bugfix
- code-quality
title: Remove async keyword from AuthController methods that lack await
updated: '2026-03-20'
---

Remove the async keyword from V1 and V2 AuthController Post methods (lines 19, 25 in each) that contain no await statements. Change return type from Task<IActionResult> to IActionResult and return Ok() directly. Files: Controllers/V1/AuthController.cs, Controllers/V2/AuthController.cs.