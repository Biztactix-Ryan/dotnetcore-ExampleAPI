---
assignee: claude
created: '2026-03-19'
depends_on: []
id: US-EX-25-4
points: 3
status: done
story_id: US-EX-25
tags: []
title: Add Order property to IInstaller interface and implement ordering
updated: '2026-03-20'
---

Add an Order property (int, default 0) to the IInstaller interface. Implement it on all existing installers with appropriate values (e.g., aspnetCoreInstaller = 0, XPOInstaller = 10, JWTInstaller = 20, etc.). Update InstallerExtensions to sort installers by Order before calling InstallServices(). Files: Installers/IInstaller.cs, Installers/InstallerExtensions.cs, all installer implementations