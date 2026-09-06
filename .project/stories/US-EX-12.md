---
acceptance_criteria:
- No NullReferenceException when user attachment fails
- Logging on line 29 checks if user was successfully attached before accessing properties
- Requests with malformed claims are handled gracefully
created: '2026-03-19'
epic_id: EPIC-EX-5
id: US-EX-12
points: 5
priority: must
status: done
tags:
- bugfix
- critical
- middleware
title: Fix NullReferenceException in JWTHelper.Invoke() after failed user attach
updated: '2026-03-20'
---

As a developer, I want the JWTHelper middleware to handle failed user attachment gracefully so that authenticated requests with malformed claims don't crash the pipeline. Currently attachUserToContext() swallows all exceptions silently, but the next line on line 29 casts context.Items["User"] to LoggedinUser and accesses .UserID — causing a NullReferenceException when attachment fails.