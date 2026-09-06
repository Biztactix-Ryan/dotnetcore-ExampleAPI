---
acceptance_criteria:
- Config binding uses correct section name or is removed if unnecessary
- JWTService registered via DI for testability
- JWT token generation works correctly with configured settings
created: '2026-03-19'
epic_id: EPIC-EX-5
id: US-EX-20
points: 5
priority: should
status: done
tags:
- bugfix
- configuration
- auth
title: Fix ineffective config binding in JWTInstaller
updated: '2026-03-20'
---

As a developer, I want the JWTService to be properly configured so that config binding actually works. Currently configuration.Bind(nameof(jwtService), jwtService) binds to section "jwtService" (lowercase j from variable name) which likely doesn't exist in appsettings.json. Also JWTService is manually constructed instead of using DI, making it hard to test.