---
acceptance_criteria:
- Stack trace preserved on RabbitMQ publish errors
- Either use throw; or remove unnecessary try/catch
- finally block still returns channel to pool
created: '2026-03-19'
epic_id: EPIC-EX-5
id: US-EX-15
points: 3
priority: must
status: done
tags:
- bugfix
- critical
title: Fix stack trace destruction in RabbitManager.Publish()
updated: '2026-03-20'
---

As a developer, I want RabbitMQ publish errors to preserve their original stack trace so that debugging is possible. Currently 'throw ex;' on line 41 resets the stack trace. Change to 'throw;' or remove the try/catch entirely since the finally block handles cleanup.