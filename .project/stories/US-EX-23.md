---
acceptance_criteria:
- CORS origins configured from appsettings.json
- Only approved origins are allowed
- AllowAnyOrigin removed from production configuration
created: '2026-03-19'
epic_id: EPIC-EX-5
id: US-EX-23
points: 5
priority: should
status: done
tags:
- security
title: Restrict CORS policy from allowing all origins
updated: '2026-03-20'
---

As a developer, I want CORS configured with appropriate origin restrictions so that the API is not open to cross-origin requests from any domain. Both the NNCors and default policies use AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod() which effectively disables CORS protection.