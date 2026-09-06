---
assignee: claude
created: '2026-03-19'
depends_on: []
id: US-EX-23-4
points: 2
status: done
story_id: US-EX-23
tags: []
title: Replace AllowAnyOrigin with configured origin list in CORS policies
updated: '2026-03-20'
---

In the CORS installer/Startup, replace AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod() with WithOrigins() loaded from appsettings.json (e.g., "Cors:AllowedOrigins" array). Apply to both NNCors and default policies. Add the config section to appsettings.json with sensible defaults for development. Files: Installers/ or Startup.cs, appsettings.json, appsettings.Development.json