---
acceptance_criteria:
- Installers execute in a deterministic order
- Order can be specified per installer
- Existing installers assigned appropriate order values
created: '2026-03-19'
epic_id: EPIC-EX-5
id: US-EX-25
points: 8
priority: should
status: done
tags:
- bugfix
- reflection
title: Add execution ordering to reflection-based installer discovery
updated: '2026-03-20'
---

As a developer, I want installer execution order to be deterministic so that dependent registrations don't fail intermittently. The reflection-based IInstaller discovery in InstallerExtensions provides no guaranteed execution order. If one installer depends on another's registration, the app may fail depending on runtime type enumeration order. Add an Order property or attribute to IInstaller.