---
acceptance_criteria:
- UserID claim added to generated JWT tokens as new Claim("UserID" user.UserID.ToString())
- JWTHelper middleware can successfully extract UserID from tokens
- End-to-end auth flow works without NullReferenceException
created: '2026-03-19'
epic_id: EPIC-EX-5
id: US-EX-14
points: 3
priority: must
status: done
tags:
- bugfix
- critical
- auth
title: Add missing UserID claim to JWTService.GenerateJSONWebToken()
updated: '2026-03-20'
---

As a developer, I want the JWT token to include a UserID claim so that the JWTHelper middleware can extract it and attach the user to the context. Currently GenerateJSONWebToken() only adds Username, Email, and Jti claims but the middleware expects a UserID claim — making authentication completely broken at runtime.