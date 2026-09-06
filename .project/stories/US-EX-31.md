---
acceptance_criteria:
- Regex patterns use + instead of * where appropriate
- Phone validation handles empty strings intentionally (either reject or explicitly
  allow)
- Existing valid inputs still pass validation
created: '2026-03-20'
epic_id: EPIC-EX-5
id: US-EX-31
points: 3
priority: could
status: done
tags:
- bugfix
- validation
title: Fix weak regex patterns in ExampleObjectValidators
updated: '2026-03-20'
---

As a developer, I want validator regex patterns to use correct quantifiers so that validation is accurate. In Contracts/V1/Validators/ExampleObjectValidators.cs, the regex patterns use * (zero or more) instead of + (one or more). While NotEmpty() guards FirstName/LastName, the Phone field's regex ^[0-9]*$ has no NotEmpty guard, allowing empty strings to pass validation silently.