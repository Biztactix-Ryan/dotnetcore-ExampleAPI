---
assignee: claude
created: '2026-03-19'
depends_on: []
id: US-EX-14-4
points: 1
status: done
story_id: US-EX-14
tags: []
title: Add UserID claim to JWTService.GenerateJSONWebToken()
updated: '2026-03-20'
---

In JWTService.GenerateJSONWebToken(), add a new Claim("UserID", user.UserID.ToString()) to the claims list alongside the existing Username, Email, and Jti claims. This is required for JWTHelper middleware to extract the UserID and attach the user to context. Files: Services/JWTService.cs