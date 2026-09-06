---
acceptance_criteria:
- Backend token password loaded from appsettings.json or secret manager
- No hardcoded secrets in source code
- Existing backend auth flow still works
created: '2026-03-19'
epic_id: EPIC-EX-5
id: US-EX-22
points: 5
priority: must
status: done
tags:
- security
title: Move hardcoded backend token to configuration
updated: '2026-03-20'
---

As a developer, I want the backend token password to come from configuration so that secrets are not hardcoded in source code. Currently JWTHelper line 63 has password: "WhyHaveAStaticToken?" hardcoded with a TODO comment.