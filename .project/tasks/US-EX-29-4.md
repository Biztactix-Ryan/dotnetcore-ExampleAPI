---
assignee: claude
created: '2026-03-20'
depends_on: []
id: US-EX-29-4
points: 2
status: done
story_id: US-EX-29
tags:
- bugfix
- resource-leak
title: Implement IDisposable on RabbitManager to dispose connection and channel pool
updated: '2026-03-20'
---

Implement IDisposable on RabbitManager in Services/RabbitManager.cs. The _connection field (line 60) is never disposed. Add a Dispose() method that closes and disposes the IConnection, and cleans up the channel pool. Ensure the DI container will call Dispose on shutdown.