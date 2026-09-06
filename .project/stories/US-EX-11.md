---
acceptance_criteria:
- Dead null check removed from GetClasses()
- Method always returns a valid Type[] (empty array if no types found)
- No null return possible
created: '2026-03-19'
epic_id: EPIC-EX-5
id: US-EX-11
points: 3
priority: should
status: done
tags:
- bugfix
- reflection
title: Fix dead null check in XPOInstaller.GetClasses()
updated: '2026-03-20'
---

As a developer, I want the GetClasses() method to have correct null handling so that the code is clear and won't return null unexpectedly. The types variable is never null (assigned from .ToList()) so the null check on line 44 is dead code and the return null on line 45 is unreachable. Remove the misleading null check and just return types.ToArray().