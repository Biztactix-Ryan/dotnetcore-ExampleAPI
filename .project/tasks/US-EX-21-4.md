---
assignee: claude
created: '2026-03-19'
depends_on: []
id: US-EX-21-4
points: 2
status: done
story_id: US-EX-21
tags: []
title: Add null-conditional operators to claim lookups in VerifyBackendToken()
updated: '2026-03-20'
---

In JWTHelper.VerifyBackendToken(), add null-conditional operators (?.) to the Token, Check, and AppName claim lookups, matching the pattern already used for the BACKEND claim. If any claim is missing, set BackendAuth = false instead of throwing NullReferenceException. Files: Middleware/JWTHelper.cs