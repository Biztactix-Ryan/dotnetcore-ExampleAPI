---
acceptance_criteria:
- LogHelper checks if context.Items["User"] is non-null before casting
- No NullReferenceException in finally block
- Original response is not swallowed by logging errors
created: '2026-03-19'
epic_id: EPIC-EX-5
id: US-EX-16
points: 5
priority: must
status: done
tags:
- bugfix
- critical
- middleware
title: Fix NullReferenceException in LogHelper for authenticated users
updated: '2026-03-20'
---

As a developer, I want the LogHelper middleware to safely handle cases where the User item is not attached to context so that logging doesn't crash in the finally block. Currently if JWTHelper fails to attach a user, the cast to LoggedinUser on line 34 throws NullReferenceException inside a finally block, swallowing the original response.