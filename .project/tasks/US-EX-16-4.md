---
assignee: claude
created: '2026-03-19'
depends_on: []
id: US-EX-16-4
points: 2
status: done
story_id: US-EX-16
tags: []
title: Add null check for User context item in LogHelper finally block
updated: '2026-03-20'
---

In LogHelper, add a null check before casting context.Items["User"] to LoggedinUser on line 34 in the finally block. If the User item is null (JWTHelper failed to attach), skip user-specific logging rather than throwing NullReferenceException which would swallow the original response. Files: Middleware/LogHelper.cs