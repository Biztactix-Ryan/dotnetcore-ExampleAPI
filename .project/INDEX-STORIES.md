# Stories

| ID | Title | Status | Priority | Points | Tags | Epic | ACs | Tasks |
| -- | ----- | ------ | -------- | ------ | ---- | ---- | --- | ----- |
| [US-EX-1](stories/US-EX-1.md) | Add LogHelper Unit Tests | ✅ done | must | 5 | testing | [EPIC-EX-1](epics/EPIC-EX-1.md) | 3 | 4 |
| [US-EX-10](stories/US-EX-10.md) | Add V2 APIRoutes Contract Tests | ✅ done | must | 3 | testing | [EPIC-EX-4](epics/EPIC-EX-4.md) | 2 | 3 |
| [US-EX-11](stories/US-EX-11.md) | Fix dead null check in XPOInstaller.GetClasses() | ✅ done | should | 3 | bugfix, reflection | [EPIC-EX-5](epics/EPIC-EX-5.md) | 3 | 4 |
| [US-EX-12](stories/US-EX-12.md) | Fix NullReferenceException in JWTHelper.Invoke() after failed user attach | ✅ done | must | 5 | bugfix, critical, middleware | [EPIC-EX-5](epics/EPIC-EX-5.md) | 3 | 4 |
| [US-EX-13](stories/US-EX-13.md) | Fix Guid.Parse on nullable value in JWTHelper.attachUserToContext() | ✅ done | must | 5 | bugfix, critical, middleware | [EPIC-EX-5](epics/EPIC-EX-5.md) | 3 | 4 |
| [US-EX-14](stories/US-EX-14.md) | Add missing UserID claim to JWTService.GenerateJSONWebToken() | ✅ done | must | 3 | bugfix, critical, auth | [EPIC-EX-5](epics/EPIC-EX-5.md) | 3 | 4 |
| [US-EX-15](stories/US-EX-15.md) | Fix stack trace destruction in RabbitManager.Publish() | ✅ done | must | 3 | bugfix, critical | [EPIC-EX-5](epics/EPIC-EX-5.md) | 3 | 4 |
| [US-EX-16](stories/US-EX-16.md) | Fix NullReferenceException in LogHelper for authenticated users | ✅ done | must | 5 | bugfix, critical, middleware | [EPIC-EX-5](epics/EPIC-EX-5.md) | 3 | 4 |
| [US-EX-17](stories/US-EX-17.md) | Fix massive data duplication in SeedDataHelper.CreateCustomer() | ✅ done | should | 3 | bugfix, data | [EPIC-EX-5](epics/EPIC-EX-5.md) | 3 | 4 |
| [US-EX-18](stories/US-EX-18.md) | Fix middleware ordering in Startup.Configure() | ✅ done | should | 3 | bugfix, middleware | [EPIC-EX-5](epics/EPIC-EX-5.md) | 2 | 3 |
| [US-EX-19](stories/US-EX-19.md) | Fix EnableEndpointRouting conflict with UseEndpoints() | ✅ done | should | 5 | bugfix, configuration | [EPIC-EX-5](epics/EPIC-EX-5.md) | 3 | 4 |
| [US-EX-2](stories/US-EX-2.md) | Add SeedDataHelper Unit Tests | ✅ done | should | 5 | testing | [EPIC-EX-1](epics/EPIC-EX-1.md) | 3 | 4 |
| [US-EX-20](stories/US-EX-20.md) | Fix ineffective config binding in JWTInstaller | ✅ done | should | 5 | bugfix, configuration, auth | [EPIC-EX-5](epics/EPIC-EX-5.md) | 3 | 4 |
| [US-EX-21](stories/US-EX-21.md) | Fix NullReferenceExceptions in VerifyBackendToken() | ✅ done | must | 5 | bugfix, critical, middleware, auth | [EPIC-EX-5](epics/EPIC-EX-5.md) | 3 | 4 |
| [US-EX-22](stories/US-EX-22.md) | Move hardcoded backend token to configuration | ✅ done | must | 5 | security | [EPIC-EX-5](epics/EPIC-EX-5.md) | 3 | 4 |
| [US-EX-23](stories/US-EX-23.md) | Restrict CORS policy from allowing all origins | ✅ done | should | 5 | security | [EPIC-EX-5](epics/EPIC-EX-5.md) | 3 | 4 |
| [US-EX-24](stories/US-EX-24.md) | Enable JWT issuer and audience validation | ✅ done | should | 8 | security, auth | [EPIC-EX-5](epics/EPIC-EX-5.md) | 4 | 5 |
| [US-EX-25](stories/US-EX-25.md) | Add execution ordering to reflection-based installer discovery | ✅ done | should | 8 | bugfix, reflection | [EPIC-EX-5](epics/EPIC-EX-5.md) | 3 | 4 |
| [US-EX-26](stories/US-EX-26.md) | Handle ReflectionTypeLoadException in XPOInstaller.GetClasses() | ✅ done | should | 5 | bugfix, reflection | [EPIC-EX-5](epics/EPIC-EX-5.md) | 3 | 4 |
| [US-EX-27](stories/US-EX-27.md) | Fix NullReferenceException in ExampleController.Get(int id) | ✅ done | must | 3 | bugfix, null-reference | [EPIC-EX-5](epics/EPIC-EX-5.md) | 3 | 4 |
| [US-EX-28](stories/US-EX-28.md) | Fix potential NullReferenceException on RemoteIpAddress in JWTHelper and LogHelper | ✅ done | should | 3 | bugfix, null-reference | [EPIC-EX-5](epics/EPIC-EX-5.md) | 3 | 4 |
| [US-EX-29](stories/US-EX-29.md) | Fix resource leak in RabbitManager - IConnection not disposed | ✅ done | should | 5 | bugfix, resource-leak | [EPIC-EX-5](epics/EPIC-EX-5.md) | 3 | 4 |
| [US-EX-3](stories/US-EX-3.md) | Add XPODemoData Unit Tests | ✅ done | could | 3 | testing | [EPIC-EX-1](epics/EPIC-EX-1.md) | 2 | 3 |
| [US-EX-30](stories/US-EX-30.md) | Remove unnecessary async from AuthController methods | ✅ done | could | 3 | bugfix, code-quality | [EPIC-EX-5](epics/EPIC-EX-5.md) | 3 | 4 |
| [US-EX-31](stories/US-EX-31.md) | Fix weak regex patterns in ExampleObjectValidators | ✅ done | could | 3 | bugfix, validation | [EPIC-EX-5](epics/EPIC-EX-5.md) | 3 | 4 |
| [US-EX-4](stories/US-EX-4.md) | Add Auth Contract Tests | ✅ done | should | 8 | testing | [EPIC-EX-2](epics/EPIC-EX-2.md) | 4 | 5 |
| [US-EX-5](stories/US-EX-5.md) | Add Example Object Contract Tests | ✅ done | should | 5 | testing | [EPIC-EX-2](epics/EPIC-EX-2.md) | 3 | 4 |
| [US-EX-6](stories/US-EX-6.md) | Add ErrorModel and V2 APIRoutes Tests | ✅ done | should | 3 | testing | [EPIC-EX-2](epics/EPIC-EX-2.md) | 2 | 3 |
| [US-EX-7](stories/US-EX-7.md) | Add XPO ExampleObject Model Tests | ✅ done | could | 3 | testing | [EPIC-EX-2](epics/EPIC-EX-2.md) | 2 | 3 |
| [US-EX-8](stories/US-EX-8.md) | Add Installer Registration Tests | ✅ done | should | 13 | testing | [EPIC-EX-3](epics/EPIC-EX-3.md) | 6 | 7 |
| [US-EX-9](stories/US-EX-9.md) | Add V2 AuthController Tests | ✅ done | must | 8 | testing | [EPIC-EX-4](epics/EPIC-EX-4.md) | 3 | 4 |
