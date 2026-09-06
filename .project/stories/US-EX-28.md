---
acceptance_criteria:
- RemoteIpAddress accessed with null-conditional operator or null check
- Fallback value used when IP is null (e.g. 'unknown')
- No NullReferenceException when RemoteIpAddress is null
created: '2026-03-20'
epic_id: EPIC-EX-5
id: US-EX-28
points: 3
priority: should
status: done
tags:
- bugfix
- null-reference
title: Fix potential NullReferenceException on RemoteIpAddress in JWTHelper and LogHelper
updated: '2026-03-20'
---

As a developer, I want the middleware to handle null RemoteIpAddress gracefully so that requests behind certain proxies or on localhost don't crash. context.Connection.RemoteIpAddress can be null in some configurations, and calling .ToString() on it in JWTHelper.cs (lines 29, 34) and LogHelper.cs (lines 35, 44) will throw NullReferenceException.