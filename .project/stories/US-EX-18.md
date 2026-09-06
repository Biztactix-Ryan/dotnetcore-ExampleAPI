---
acceptance_criteria:
- UseHttpsRedirection() called before UseRouting() and UseCors()
- Non-HTTPS requests are redirected before any routing occurs
created: '2026-03-19'
epic_id: EPIC-EX-5
id: US-EX-18
points: 3
priority: should
status: done
tags:
- bugfix
- middleware
title: Fix middleware ordering in Startup.Configure()
updated: '2026-03-20'
---

As a developer, I want the middleware pipeline ordered correctly so that HTTPS redirection happens before routing and CORS processing. Currently UseHttpsRedirection() is called after UseRouting() and UseCors(), meaning non-HTTPS requests get partially processed before being redirected.