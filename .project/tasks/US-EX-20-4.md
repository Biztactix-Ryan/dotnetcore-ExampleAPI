---
assignee: claude
created: '2026-03-19'
depends_on: []
id: US-EX-20-4
points: 2
status: done
story_id: US-EX-20
tags: []
title: Fix config binding section name and register JWTService via DI
updated: '2026-03-20'
---

In JWTInstaller: (1) Fix configuration.Bind() to use the correct appsettings.json section name (likely "JWTService" not "jwtService" from nameof). (2) Register JWTService in the DI container as a singleton/scoped service instead of manually constructing it. Verify the appsettings.json section exists and matches. Files: Installers/JWTInstaller.cs, appsettings.json