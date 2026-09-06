---
acceptance_criteria:
- UserID claim is safely parsed with TryParse or null check
- Missing UserID claim does not throw
- User attachment fails gracefully if UserID is missing or invalid
created: '2026-03-19'
epic_id: EPIC-EX-5
id: US-EX-13
points: 5
priority: must
status: done
tags:
- bugfix
- critical
- middleware
title: Fix Guid.Parse on nullable value in JWTHelper.attachUserToContext()
updated: '2026-03-20'
---

As a developer, I want the attachUserToContext method to safely parse the UserID claim so that null claim values don't cause ArgumentNullException. FindFirst("UserID")?.Value can return null, and Guid.Parse(null) throws. Use Guid.TryParse or null-check before parsing.