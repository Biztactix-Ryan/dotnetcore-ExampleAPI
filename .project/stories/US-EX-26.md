---
acceptance_criteria:
- GetClasses() handles ReflectionTypeLoadException
- Unloadable types are skipped not crashed on
- Valid types still discovered and registered correctly
created: '2026-03-19'
epic_id: EPIC-EX-5
id: US-EX-26
points: 5
priority: should
status: done
tags:
- bugfix
- reflection
title: Handle ReflectionTypeLoadException in XPOInstaller.GetClasses()
updated: '2026-03-20'
---

As a developer, I want the XPO type discovery to handle assembly loading failures gracefully so that a single unloadable type doesn't crash the entire application. Assembly.GetTypes() throws ReflectionTypeLoadException if any type can't be loaded. Should use GetExportedTypes() or catch the exception and filter nulls from e.Types.