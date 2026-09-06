---
assignee: claude
created: '2026-03-20'
depends_on: []
id: US-EX-28-4
points: 1
status: done
story_id: US-EX-28
tags:
- bugfix
- null-reference
title: Add null-conditional operator to RemoteIpAddress in JWTHelper and LogHelper
updated: '2026-03-20'
---

Add null-conditional operator (?.) to RemoteIpAddress access in JWTHelper.cs (lines 29, 34) and LogHelper.cs (lines 35, 44). Use a fallback value of "unknown" when RemoteIpAddress is null. This prevents NullReferenceException when requests come through certain proxies or localhost configurations.