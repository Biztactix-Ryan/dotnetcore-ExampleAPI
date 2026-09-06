---
assignee: claude
created: '2026-03-20'
depends_on: []
id: US-EX-27-4
points: 1
status: done
story_id: US-EX-27
tags:
- bugfix
- null-reference
title: Add null check after GetObjectByKey() in ExampleController.Get(id)
updated: '2026-03-20'
---

Add a null check after GetObjectByKey() call on line 44 of Controllers/V1/ExampleController.cs. If the object is null, return NotFound(). Follow the same pattern already used in the Put() method (line 62) which correctly checks for null and returns BadRequest.