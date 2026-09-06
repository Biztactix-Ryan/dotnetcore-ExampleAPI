---
acceptance_criteria:
- All claim lookups use null-conditional operator or null checks
- Missing Token/Check/AppName claims don't crash the middleware
- Backend auth gracefully falls back to BackendAuth=false on missing claims
created: '2026-03-19'
epic_id: EPIC-EX-5
id: US-EX-21
points: 5
priority: must
status: done
tags:
- bugfix
- critical
- middleware
- auth
title: Fix NullReferenceExceptions in VerifyBackendToken()
updated: '2026-03-20'
---

As a developer, I want the backend token verification to safely handle missing claims so that it doesn't throw NullReferenceException. The Token, Check, and AppName claims are accessed with .Value directly (no null-conditional) unlike the BACKEND claim which uses ?.Value. Missing any of these claims causes a crash.