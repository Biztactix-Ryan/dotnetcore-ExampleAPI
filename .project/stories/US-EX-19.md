---
acceptance_criteria:
- Routing configuration is consistent between installer and Startup
- No contradictory endpoint routing settings
- All routes resolve correctly
created: '2026-03-19'
epic_id: EPIC-EX-5
id: US-EX-19
points: 5
priority: should
status: done
tags:
- bugfix
- configuration
title: Fix EnableEndpointRouting conflict with UseEndpoints()
updated: '2026-03-20'
---

As a developer, I want consistent routing configuration so that MVC routing works correctly. aspnetCoreInstaller sets EnableEndpointRouting = false but Startup.Configure() uses app.UseEndpoints() which requires endpoint routing. Either enable endpoint routing or switch to app.UseMvc().