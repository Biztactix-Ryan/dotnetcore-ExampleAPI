---
acceptance_criteria:
- ValidateIssuer and ValidateAudience set to true
- RequireExpirationTime set to true
- ValidIssuer and ValidAudience configured from settings
- Tokens from other systems are rejected
created: '2026-03-19'
epic_id: EPIC-EX-5
id: US-EX-24
points: 8
priority: should
status: done
tags:
- security
- auth
title: Enable JWT issuer and audience validation
updated: '2026-03-20'
---

As a developer, I want JWT validation to check issuer and audience so that tokens from other systems are rejected. Currently ValidateIssuer, ValidateAudience, and RequireExpirationTime are all set to false, allowing tokens from unrelated systems to be accepted.