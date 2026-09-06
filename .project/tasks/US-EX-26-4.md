---
assignee: claude
created: '2026-03-19'
depends_on: []
id: US-EX-26-4
points: 2
status: done
story_id: US-EX-26
tags: []
title: Handle ReflectionTypeLoadException in XPOInstaller.GetClasses()
updated: '2026-03-20'
---

In XPOInstaller.GetClasses(), wrap Assembly.GetTypes() in a try/catch for ReflectionTypeLoadException. When caught, use exception.Types.Where(t => t != null) to get the loadable types. Alternatively, switch to Assembly.GetExportedTypes() which is more forgiving. Log a warning for any skipped types. Files: Installers/XPOInstaller.cs