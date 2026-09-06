---
acceptance_criteria:
- RabbitManager implements IDisposable
- Connection is disposed on shutdown
- Channel pool is cleaned up properly
created: '2026-03-20'
epic_id: EPIC-EX-5
id: US-EX-29
points: 5
priority: should
status: done
tags:
- bugfix
- resource-leak
title: Fix resource leak in RabbitManager - IConnection not disposed
updated: '2026-03-20'
---

As a developer, I want RabbitManager to properly dispose its RabbitMQ connection so that connections are cleaned up on application shutdown. The _connection field in Services/RabbitManager.cs (line 60) is never disposed. The class should implement IDisposable and properly close/dispose the connection.