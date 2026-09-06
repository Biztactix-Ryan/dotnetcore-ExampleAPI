---
acceptance_criteria:
- Get(id) returns NotFound or BadRequest when object doesn't exist
- No NullReferenceException when requesting non-existent ID
- Existing valid ID requests still work correctly
created: '2026-03-20'
epic_id: EPIC-EX-5
id: US-EX-27
points: 3
priority: must
status: done
tags:
- bugfix
- null-reference
title: Fix NullReferenceException in ExampleController.Get(int id)
updated: '2026-03-20'
---

As a developer, I want ExampleController.Get(id) to handle missing objects gracefully so that requests for non-existent IDs return a proper error instead of crashing. Currently on line 44 of Controllers/V1/ExampleController.cs, GetObjectByKey() can return null and the result is passed directly to AutoMapper without a null check. The Put() method on line 62 already handles this correctly with a null check returning BadRequest.