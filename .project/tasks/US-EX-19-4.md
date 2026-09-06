---
assignee: claude
created: '2026-03-19'
depends_on: []
id: US-EX-19-4
points: 2
status: done
story_id: US-EX-19
tags: []
title: Resolve EnableEndpointRouting conflict between installer and Startup
updated: '2026-03-20'
---

Fix the contradiction where aspnetCoreInstaller sets EnableEndpointRouting = false but Startup.Configure() uses app.UseEndpoints() (which requires endpoint routing). Either: (a) remove EnableEndpointRouting = false from the installer, or (b) replace UseEndpoints() with UseMvc() in Startup. Option (a) is preferred for .NET 6+. Files: Installers/aspnetCoreInstaller.cs, Startup.cs