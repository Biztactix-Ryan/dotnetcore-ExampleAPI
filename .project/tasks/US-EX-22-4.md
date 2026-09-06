---
assignee: claude
created: '2026-03-19'
depends_on: []
id: US-EX-22-4
points: 2
status: done
story_id: US-EX-22
tags: []
title: Move hardcoded backend token password to appsettings.json
updated: '2026-03-20'
---

Replace the hardcoded password "WhyHaveAStaticToken?" on line 63 of JWTHelper with a value loaded from IConfiguration. Add a config section in appsettings.json (e.g., "BackendAuth:TokenPassword"). Inject IConfiguration into JWTHelper or pass the value through middleware options. Files: Middleware/JWTHelper.cs, appsettings.json