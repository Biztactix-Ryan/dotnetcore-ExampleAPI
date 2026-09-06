---
assignee: claude
created: '2026-03-19'
depends_on: []
id: US-EX-12-4
points: 2
status: done
story_id: US-EX-12
tags: []
title: Add null check before accessing User in JWTHelper.Invoke()
updated: '2026-03-20'
---

In JWTHelper.Invoke(), add a null check on context.Items["User"] before casting to LoggedinUser and accessing .UserID on line 29. If user attachment failed (attachUserToContext swallows exceptions), skip the user-dependent logging. Files: Middleware/JWTHelper.cs