---
assignee: claude
created: '2026-03-19'
depends_on: []
id: US-EX-8-7
points: 5
status: done
story_id: US-EX-8
tags: []
title: Implement installer registration tests for all 6 installers
updated: '2026-03-20'
---

Create test files for aspnetCoreInstaller, AutomapperInstaller, JWTInstaller, RabbitMQ installer, SwaggerInstaller, and XPOInstaller. For each installer, verify that InstallServices() registers the expected services in the DI container. Use a real ServiceCollection and verify registrations.