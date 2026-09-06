---
assignee: claude
created: '2026-03-19'
depends_on: []
id: US-EX-15-4
points: 1
status: done
story_id: US-EX-15
tags: []
title: Fix throw ex to throw in RabbitManager.Publish()
updated: '2026-03-20'
---

In RabbitManager.Publish(), change 'throw ex;' on line 41 to 'throw;' to preserve the original stack trace. Alternatively, remove the try/catch entirely since the finally block handles channel cleanup regardless. Files: Services/RabbitManager.cs