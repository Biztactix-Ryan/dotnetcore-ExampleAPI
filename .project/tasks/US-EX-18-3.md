---
assignee: claude
created: '2026-03-19'
depends_on: []
id: US-EX-18-3
points: 1
status: done
story_id: US-EX-18
tags: []
title: Reorder middleware pipeline in Startup.Configure()
updated: '2026-03-20'
---

In Startup.Configure(), move UseHttpsRedirection() to before UseRouting() and UseCors(). The correct order should be: UseHttpsRedirection() → UseRouting() → UseCors() → UseAuthentication() → UseAuthorization() → UseEndpoints(). Files: Startup.cs