---
assignee: claude
created: '2026-03-19'
depends_on: []
id: US-EX-11-4
points: 1
status: done
story_id: US-EX-11
tags: []
title: Remove dead null check from XPOInstaller.GetClasses()
updated: '2026-03-20'
---

In XPOInstaller.GetClasses(), remove the dead null check on line 44 and the unreachable 'return null' on line 45. The types variable is assigned from .ToList() which never returns null. Simplify to just return types.ToArray(). Files: Installers/XPOInstaller.cs