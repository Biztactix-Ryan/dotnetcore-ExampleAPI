---
assignee: claude
created: '2026-03-19'
depends_on: []
id: US-EX-24-5
points: 2
status: done
story_id: US-EX-24
tags: []
title: Enable JWT issuer, audience, and expiration validation
updated: '2026-03-20'
---

In JWTInstaller, set ValidateIssuer = true, ValidateAudience = true, and RequireExpirationTime = true in TokenValidationParameters. Configure ValidIssuer and ValidAudience from appsettings.json. Update JWTService.GenerateJSONWebToken() to include matching issuer and audience in generated tokens. Files: Installers/JWTInstaller.cs, Services/JWTService.cs, appsettings.json