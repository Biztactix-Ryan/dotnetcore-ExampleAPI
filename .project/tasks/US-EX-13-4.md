---
assignee: claude
created: '2026-03-19'
depends_on: []
id: US-EX-13-4
points: 2
status: done
story_id: US-EX-13
tags: []
title: Replace Guid.Parse with Guid.TryParse in JWTHelper.attachUserToContext()
updated: '2026-03-20'
---

In JWTHelper.attachUserToContext(), replace Guid.Parse(FindFirst("UserID")?.Value) with a null check + Guid.TryParse pattern. If the UserID claim is missing or invalid, fail gracefully instead of throwing ArgumentNullException. Files: Middleware/JWTHelper.cs