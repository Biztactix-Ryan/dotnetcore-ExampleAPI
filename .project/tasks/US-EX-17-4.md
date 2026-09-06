---
assignee: claude
created: '2026-03-19'
depends_on: []
id: US-EX-17-4
points: 1
status: done
story_id: US-EX-17
tags: []
title: Remove inner loop duplication in SeedDataHelper.CreateCustomer()
updated: '2026-03-20'
---

In SeedDataHelper.CreateCustomer(), remove the inner loop that creates 272 objects per call. The method should create just one customer record per invocation. The outer Seed() method already calls CreateCustomer() 272 times (17*16), so the inner loop causes 73,984 records instead of 272. Files: Helpers/SeedDataHelper.cs