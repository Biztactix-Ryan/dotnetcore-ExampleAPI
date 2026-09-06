---
acceptance_criteria:
- CreateCustomer creates one record per call not 272
- Total seed data is 272 records (17*16) not 73984
- Startup time is reasonable
created: '2026-03-19'
epic_id: EPIC-EX-5
id: US-EX-17
points: 3
priority: should
status: done
tags:
- bugfix
- data
title: Fix massive data duplication in SeedDataHelper.CreateCustomer()
updated: '2026-03-20'
---

As a developer, I want the seed data helper to create a reasonable number of records so that startup isn't slow and the database isn't bloated. Currently Seed() calls CreateCustomer() 272 times and each call creates 272 objects internally (73,984 records total). The inner loop in CreateCustomer() should be removed — it should create just one record per call.