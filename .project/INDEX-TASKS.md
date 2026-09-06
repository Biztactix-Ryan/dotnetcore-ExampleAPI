# Tasks

| ID | Title | Status | Points | Tags | Assignee | Depends On | Story |
| -- | ----- | ------ | ------ | ---- | -------- | ---------- | ----- |
| [US-EX-1-1](tasks/US-EX-1-1.md) | Test: All public methods tested | ✅ done | 1 |  | claude | US-EX-1-4 | [US-EX-1](stories/US-EX-1.md) |
| [US-EX-1-2](tasks/US-EX-1-2.md) | Test: Edge cases covered | ✅ done | 1 |  | claude | US-EX-1-4 | [US-EX-1](stories/US-EX-1.md) |
| [US-EX-1-3](tasks/US-EX-1-3.md) | Test: Tests pass in CI | ✅ done | 1 |  | claude | US-EX-1-4 | [US-EX-1](stories/US-EX-1.md) |
| [US-EX-1-4](tasks/US-EX-1-4.md) | Implement LogHelper unit tests | ✅ done | 3 |  | claude | — | [US-EX-1](stories/US-EX-1.md) |
| [US-EX-10-1](tasks/US-EX-10-1.md) | Test: All V2 route constants validated | ✅ done | 1 |  | — | US-EX-10-3 | [US-EX-10](stories/US-EX-10.md) |
| [US-EX-10-2](tasks/US-EX-10-2.md) | Test: Routes match controller attributes | ✅ done | 1 |  | — | US-EX-10-3 | [US-EX-10](stories/US-EX-10.md) |
| [US-EX-10-3](tasks/US-EX-10-3.md) | Implement V2 APIRoutes contract validation tests | ✅ done | 1 |  | claude | — | [US-EX-10](stories/US-EX-10.md) |
| [US-EX-11-1](tasks/US-EX-11-1.md) | Test: Dead null check removed from GetClasses() | ✅ done | 1 |  | claude | US-EX-11-4 | [US-EX-11](stories/US-EX-11.md) |
| [US-EX-11-2](tasks/US-EX-11-2.md) | Test: Method always returns a valid Type[] (empty array if no types found) | ✅ done | 1 |  | claude | US-EX-11-4 | [US-EX-11](stories/US-EX-11.md) |
| [US-EX-11-3](tasks/US-EX-11-3.md) | Test: No null return possible | ✅ done | 1 |  | claude | US-EX-11-4 | [US-EX-11](stories/US-EX-11.md) |
| [US-EX-11-4](tasks/US-EX-11-4.md) | Remove dead null check from XPOInstaller.GetClasses() | ✅ done | 1 |  | claude | — | [US-EX-11](stories/US-EX-11.md) |
| [US-EX-12-1](tasks/US-EX-12-1.md) | Test: No NullReferenceException when user attachment fails | ✅ done | 1 |  | — | US-EX-12-4 | [US-EX-12](stories/US-EX-12.md) |
| [US-EX-12-2](tasks/US-EX-12-2.md) | Test: Logging on line 29 checks if user was successfully attached before accessing properties | ✅ done | 1 |  | — | US-EX-12-4 | [US-EX-12](stories/US-EX-12.md) |
| [US-EX-12-3](tasks/US-EX-12-3.md) | Test: Requests with malformed claims are handled gracefully | ✅ done | 1 |  | — | US-EX-12-4 | [US-EX-12](stories/US-EX-12.md) |
| [US-EX-12-4](tasks/US-EX-12-4.md) | Add null check before accessing User in JWTHelper.Invoke() | ✅ done | 2 |  | claude | — | [US-EX-12](stories/US-EX-12.md) |
| [US-EX-13-1](tasks/US-EX-13-1.md) | Test: UserID claim is safely parsed with TryParse or null check | ✅ done | 1 |  | claude | US-EX-13-4 | [US-EX-13](stories/US-EX-13.md) |
| [US-EX-13-2](tasks/US-EX-13-2.md) | Test: Missing UserID claim does not throw | ✅ done | 1 |  | claude | US-EX-13-4 | [US-EX-13](stories/US-EX-13.md) |
| [US-EX-13-3](tasks/US-EX-13-3.md) | Test: User attachment fails gracefully if UserID is missing or invalid | ✅ done | 1 |  | claude | US-EX-13-4 | [US-EX-13](stories/US-EX-13.md) |
| [US-EX-13-4](tasks/US-EX-13-4.md) | Replace Guid.Parse with Guid.TryParse in JWTHelper.attachUserToContext() | ✅ done | 2 |  | claude | — | [US-EX-13](stories/US-EX-13.md) |
| [US-EX-14-1](tasks/US-EX-14-1.md) | Test: UserID claim added to generated JWT tokens as new Claim("UserID" user.UserID.ToString()) | ✅ done | 1 |  | claude | US-EX-14-4 | [US-EX-14](stories/US-EX-14.md) |
| [US-EX-14-2](tasks/US-EX-14-2.md) | Test: JWTHelper middleware can successfully extract UserID from tokens | ✅ done | 1 |  | claude | US-EX-14-4 | [US-EX-14](stories/US-EX-14.md) |
| [US-EX-14-3](tasks/US-EX-14-3.md) | Test: End-to-end auth flow works without NullReferenceException | ✅ done | 1 |  | claude | US-EX-14-4 | [US-EX-14](stories/US-EX-14.md) |
| [US-EX-14-4](tasks/US-EX-14-4.md) | Add UserID claim to JWTService.GenerateJSONWebToken() | ✅ done | 1 |  | claude | — | [US-EX-14](stories/US-EX-14.md) |
| [US-EX-15-1](tasks/US-EX-15-1.md) | Test: Stack trace preserved on RabbitMQ publish errors | ✅ done | 1 |  | claude | US-EX-15-4 | [US-EX-15](stories/US-EX-15.md) |
| [US-EX-15-2](tasks/US-EX-15-2.md) | Test: Either use throw; or remove unnecessary try/catch | ✅ done | 1 |  | claude | US-EX-15-4 | [US-EX-15](stories/US-EX-15.md) |
| [US-EX-15-3](tasks/US-EX-15-3.md) | Test: finally block still returns channel to pool | ✅ done | 1 |  | claude | US-EX-15-4 | [US-EX-15](stories/US-EX-15.md) |
| [US-EX-15-4](tasks/US-EX-15-4.md) | Fix throw ex to throw in RabbitManager.Publish() | ✅ done | 1 |  | claude | — | [US-EX-15](stories/US-EX-15.md) |
| [US-EX-16-1](tasks/US-EX-16-1.md) | Test: LogHelper checks if context.Items["User"] is non-null before casting | ✅ done | 1 |  | claude | US-EX-16-4 | [US-EX-16](stories/US-EX-16.md) |
| [US-EX-16-2](tasks/US-EX-16-2.md) | Test: No NullReferenceException in finally block | ✅ done | 1 |  | claude | US-EX-16-4 | [US-EX-16](stories/US-EX-16.md) |
| [US-EX-16-3](tasks/US-EX-16-3.md) | Test: Original response is not swallowed by logging errors | ✅ done | 2 |  | claude | US-EX-16-4 | [US-EX-16](stories/US-EX-16.md) |
| [US-EX-16-4](tasks/US-EX-16-4.md) | Add null check for User context item in LogHelper finally block | ✅ done | 2 |  | claude | — | [US-EX-16](stories/US-EX-16.md) |
| [US-EX-17-1](tasks/US-EX-17-1.md) | Test: CreateCustomer creates one record per call not 272 | ✅ done | 1 |  | claude | US-EX-17-4 | [US-EX-17](stories/US-EX-17.md) |
| [US-EX-17-2](tasks/US-EX-17-2.md) | Test: Total seed data is 272 records (17*16) not 73984 | ✅ done | 1 |  | claude | US-EX-17-4 | [US-EX-17](stories/US-EX-17.md) |
| [US-EX-17-3](tasks/US-EX-17-3.md) | Test: Startup time is reasonable | ✅ done | 1 |  | claude | US-EX-17-4 | [US-EX-17](stories/US-EX-17.md) |
| [US-EX-17-4](tasks/US-EX-17-4.md) | Remove inner loop duplication in SeedDataHelper.CreateCustomer() | ✅ done | 1 |  | claude | — | [US-EX-17](stories/US-EX-17.md) |
| [US-EX-18-1](tasks/US-EX-18-1.md) | Test: UseHttpsRedirection() called before UseRouting() and UseCors() | ✅ done | 1 |  | claude | US-EX-18-3 | [US-EX-18](stories/US-EX-18.md) |
| [US-EX-18-2](tasks/US-EX-18-2.md) | Test: Non-HTTPS requests are redirected before any routing occurs | ✅ done | 1 |  | claude | US-EX-18-3 | [US-EX-18](stories/US-EX-18.md) |
| [US-EX-18-3](tasks/US-EX-18-3.md) | Reorder middleware pipeline in Startup.Configure() | ✅ done | 1 |  | claude | — | [US-EX-18](stories/US-EX-18.md) |
| [US-EX-19-1](tasks/US-EX-19-1.md) | Test: Routing configuration is consistent between installer and Startup | ✅ done | 1 |  | claude | US-EX-19-4 | [US-EX-19](stories/US-EX-19.md) |
| [US-EX-19-2](tasks/US-EX-19-2.md) | Test: No contradictory endpoint routing settings | ✅ done | 1 |  | claude | US-EX-19-4 | [US-EX-19](stories/US-EX-19.md) |
| [US-EX-19-3](tasks/US-EX-19-3.md) | Test: All routes resolve correctly | ✅ done | 2 |  | claude | US-EX-19-4 | [US-EX-19](stories/US-EX-19.md) |
| [US-EX-19-4](tasks/US-EX-19-4.md) | Resolve EnableEndpointRouting conflict between installer and Startup | ✅ done | 2 |  | claude | — | [US-EX-19](stories/US-EX-19.md) |
| [US-EX-2-1](tasks/US-EX-2-1.md) | Test: All public methods tested | ✅ done | 1 |  | claude | US-EX-2-4 | [US-EX-2](stories/US-EX-2.md) |
| [US-EX-2-2](tasks/US-EX-2-2.md) | Test: Mock data dependencies | ✅ done | 1 |  | claude | US-EX-2-4 | [US-EX-2](stories/US-EX-2.md) |
| [US-EX-2-3](tasks/US-EX-2-3.md) | Test: Tests pass in CI | ✅ done | 1 |  | claude | US-EX-2-4 | [US-EX-2](stories/US-EX-2.md) |
| [US-EX-2-4](tasks/US-EX-2-4.md) | Implement SeedDataHelper unit tests | ✅ done | 3 |  | claude | — | [US-EX-2](stories/US-EX-2.md) |
| [US-EX-20-1](tasks/US-EX-20-1.md) | Test: Config binding uses correct section name or is removed if unnecessary | ✅ done | 1 |  | claude | US-EX-20-4 | [US-EX-20](stories/US-EX-20.md) |
| [US-EX-20-2](tasks/US-EX-20-2.md) | Test: JWTService registered via DI for testability | ✅ done | 1 |  | claude | US-EX-20-4 | [US-EX-20](stories/US-EX-20.md) |
| [US-EX-20-3](tasks/US-EX-20-3.md) | Test: JWT token generation works correctly with configured settings | ✅ done | 2 |  | claude | US-EX-20-4 | [US-EX-20](stories/US-EX-20.md) |
| [US-EX-20-4](tasks/US-EX-20-4.md) | Fix config binding section name and register JWTService via DI | ✅ done | 2 |  | claude | — | [US-EX-20](stories/US-EX-20.md) |
| [US-EX-21-1](tasks/US-EX-21-1.md) | Test: All claim lookups use null-conditional operator or null checks | ✅ done | 1 |  | claude | US-EX-21-4 | [US-EX-21](stories/US-EX-21.md) |
| [US-EX-21-2](tasks/US-EX-21-2.md) | Test: Missing Token/Check/AppName claims don't crash the middleware | ✅ done | 2 |  | claude | US-EX-21-4 | [US-EX-21](stories/US-EX-21.md) |
| [US-EX-21-3](tasks/US-EX-21-3.md) | Test: Backend auth gracefully falls back to BackendAuth=false on missing claims | ✅ done | 1 |  | claude | US-EX-21-4 | [US-EX-21](stories/US-EX-21.md) |
| [US-EX-21-4](tasks/US-EX-21-4.md) | Add null-conditional operators to claim lookups in VerifyBackendToken() | ✅ done | 2 |  | claude | — | [US-EX-21](stories/US-EX-21.md) |
| [US-EX-22-1](tasks/US-EX-22-1.md) | Test: Backend token password loaded from appsettings.json or secret manager | ✅ done | 1 |  | claude | US-EX-22-4 | [US-EX-22](stories/US-EX-22.md) |
| [US-EX-22-2](tasks/US-EX-22-2.md) | Test: No hardcoded secrets in source code | ✅ done | 1 |  | claude | US-EX-22-4 | [US-EX-22](stories/US-EX-22.md) |
| [US-EX-22-3](tasks/US-EX-22-3.md) | Test: Existing backend auth flow still works | ✅ done | 1 |  | claude | US-EX-22-4 | [US-EX-22](stories/US-EX-22.md) |
| [US-EX-22-4](tasks/US-EX-22-4.md) | Move hardcoded backend token password to appsettings.json | ✅ done | 2 |  | claude | — | [US-EX-22](stories/US-EX-22.md) |
| [US-EX-23-1](tasks/US-EX-23-1.md) | Test: CORS origins configured from appsettings.json | ✅ done | 1 |  | claude | US-EX-23-4 | [US-EX-23](stories/US-EX-23.md) |
| [US-EX-23-2](tasks/US-EX-23-2.md) | Test: Only approved origins are allowed | ✅ done | 1 |  | claude | US-EX-23-4 | [US-EX-23](stories/US-EX-23.md) |
| [US-EX-23-3](tasks/US-EX-23-3.md) | Test: AllowAnyOrigin removed from production configuration | ✅ done | 1 |  | claude | US-EX-23-4 | [US-EX-23](stories/US-EX-23.md) |
| [US-EX-23-4](tasks/US-EX-23-4.md) | Replace AllowAnyOrigin with configured origin list in CORS policies | ✅ done | 2 |  | claude | — | [US-EX-23](stories/US-EX-23.md) |
| [US-EX-24-1](tasks/US-EX-24-1.md) | Test: ValidateIssuer and ValidateAudience set to true | ✅ done | 1 |  | claude | US-EX-24-5 | [US-EX-24](stories/US-EX-24.md) |
| [US-EX-24-2](tasks/US-EX-24-2.md) | Test: RequireExpirationTime set to true | ✅ done | 1 |  | claude | US-EX-24-5 | [US-EX-24](stories/US-EX-24.md) |
| [US-EX-24-3](tasks/US-EX-24-3.md) | Test: ValidIssuer and ValidAudience configured from settings | ✅ done | 1 |  | claude | US-EX-24-5 | [US-EX-24](stories/US-EX-24.md) |
| [US-EX-24-4](tasks/US-EX-24-4.md) | Test: Tokens from other systems are rejected | ✅ done | 2 |  | claude | US-EX-24-5 | [US-EX-24](stories/US-EX-24.md) |
| [US-EX-24-5](tasks/US-EX-24-5.md) | Enable JWT issuer, audience, and expiration validation | ✅ done | 2 |  | claude | — | [US-EX-24](stories/US-EX-24.md) |
| [US-EX-25-1](tasks/US-EX-25-1.md) | Test: Installers execute in a deterministic order | ✅ done | 2 |  | claude | US-EX-25-4 | [US-EX-25](stories/US-EX-25.md) |
| [US-EX-25-2](tasks/US-EX-25-2.md) | Test: Order can be specified per installer | ✅ done | 1 |  | claude | US-EX-25-4 | [US-EX-25](stories/US-EX-25.md) |
| [US-EX-25-3](tasks/US-EX-25-3.md) | Test: Existing installers assigned appropriate order values | ✅ done | 2 |  | claude | US-EX-25-4 | [US-EX-25](stories/US-EX-25.md) |
| [US-EX-25-4](tasks/US-EX-25-4.md) | Add Order property to IInstaller interface and implement ordering | ✅ done | 3 |  | claude | — | [US-EX-25](stories/US-EX-25.md) |
| [US-EX-26-1](tasks/US-EX-26-1.md) | Test: GetClasses() handles ReflectionTypeLoadException | ✅ done | 1 |  | claude | US-EX-26-4 | [US-EX-26](stories/US-EX-26.md) |
| [US-EX-26-2](tasks/US-EX-26-2.md) | Test: Unloadable types are skipped not crashed on | ✅ done | 1 |  | claude | US-EX-26-4 | [US-EX-26](stories/US-EX-26.md) |
| [US-EX-26-3](tasks/US-EX-26-3.md) | Test: Valid types still discovered and registered correctly | ✅ done | 1 |  | claude | US-EX-26-4 | [US-EX-26](stories/US-EX-26.md) |
| [US-EX-26-4](tasks/US-EX-26-4.md) | Handle ReflectionTypeLoadException in XPOInstaller.GetClasses() | ✅ done | 2 |  | claude | — | [US-EX-26](stories/US-EX-26.md) |
| [US-EX-27-1](tasks/US-EX-27-1.md) | Test: Get(id) returns NotFound or BadRequest when object doesn't exist | ✅ done | 1 |  | claude | US-EX-27-4 | [US-EX-27](stories/US-EX-27.md) |
| [US-EX-27-2](tasks/US-EX-27-2.md) | Test: No NullReferenceException when requesting non-existent ID | ✅ done | 1 |  | claude | US-EX-27-4 | [US-EX-27](stories/US-EX-27.md) |
| [US-EX-27-3](tasks/US-EX-27-3.md) | Test: Existing valid ID requests still work correctly | ✅ done | 1 |  | claude | US-EX-27-4 | [US-EX-27](stories/US-EX-27.md) |
| [US-EX-27-4](tasks/US-EX-27-4.md) | Add null check after GetObjectByKey() in ExampleController.Get(id) | ✅ done | 1 | bugfix, null-reference | claude | — | [US-EX-27](stories/US-EX-27.md) |
| [US-EX-28-1](tasks/US-EX-28-1.md) | Test: RemoteIpAddress accessed with null-conditional operator or null check | ✅ done | 1 |  | claude | US-EX-28-4 | [US-EX-28](stories/US-EX-28.md) |
| [US-EX-28-2](tasks/US-EX-28-2.md) | Test: Fallback value used when IP is null (e.g. 'unknown') | ✅ done | 1 |  | claude | US-EX-28-4 | [US-EX-28](stories/US-EX-28.md) |
| [US-EX-28-3](tasks/US-EX-28-3.md) | Test: No NullReferenceException when RemoteIpAddress is null | ✅ done | 1 |  | claude | US-EX-28-4 | [US-EX-28](stories/US-EX-28.md) |
| [US-EX-28-4](tasks/US-EX-28-4.md) | Add null-conditional operator to RemoteIpAddress in JWTHelper and LogHelper | ✅ done | 1 | bugfix, null-reference | claude | — | [US-EX-28](stories/US-EX-28.md) |
| [US-EX-29-1](tasks/US-EX-29-1.md) | Test: RabbitManager implements IDisposable | ✅ done | 1 |  | claude | US-EX-29-4 | [US-EX-29](stories/US-EX-29.md) |
| [US-EX-29-2](tasks/US-EX-29-2.md) | Test: Connection is disposed on shutdown | ✅ done | 1 |  | claude | US-EX-29-4 | [US-EX-29](stories/US-EX-29.md) |
| [US-EX-29-3](tasks/US-EX-29-3.md) | Test: Channel pool is cleaned up properly | ✅ done | 2 |  | claude | US-EX-29-4 | [US-EX-29](stories/US-EX-29.md) |
| [US-EX-29-4](tasks/US-EX-29-4.md) | Implement IDisposable on RabbitManager to dispose connection and channel pool | ✅ done | 2 | bugfix, resource-leak | claude | — | [US-EX-29](stories/US-EX-29.md) |
| [US-EX-3-1](tasks/US-EX-3-1.md) | Test: Demo data creation methods tested | ✅ done | 1 |  | claude | US-EX-3-3 | [US-EX-3](stories/US-EX-3.md) |
| [US-EX-3-2](tasks/US-EX-3-2.md) | Test: Tests pass in CI | ✅ done | 1 |  | — | US-EX-3-3 | [US-EX-3](stories/US-EX-3.md) |
| [US-EX-3-3](tasks/US-EX-3-3.md) | Implement XPODemoData unit tests | ✅ done | 2 |  | claude | — | [US-EX-3](stories/US-EX-3.md) |
| [US-EX-30-1](tasks/US-EX-30-1.md) | Test: AuthController methods return IActionResult or Task.FromResult instead of using async | ✅ done | 1 |  | claude | US-EX-30-4 | [US-EX-30](stories/US-EX-30.md) |
| [US-EX-30-2](tasks/US-EX-30-2.md) | Test: No compiler warnings about async methods lacking await | ✅ done | 1 |  | claude | US-EX-30-4 | [US-EX-30](stories/US-EX-30.md) |
| [US-EX-30-3](tasks/US-EX-30-3.md) | Test: API behavior unchanged | ✅ done | 1 |  | claude | US-EX-30-4 | [US-EX-30](stories/US-EX-30.md) |
| [US-EX-30-4](tasks/US-EX-30-4.md) | Remove async keyword from AuthController methods that lack await | ✅ done | 1 | bugfix, code-quality | claude | — | [US-EX-30](stories/US-EX-30.md) |
| [US-EX-31-1](tasks/US-EX-31-1.md) | Test: Regex patterns use + instead of * where appropriate | ✅ done | 1 |  | claude | US-EX-31-4 | [US-EX-31](stories/US-EX-31.md) |
| [US-EX-31-2](tasks/US-EX-31-2.md) | Test: Phone validation handles empty strings intentionally (either reject or explicitly allow) | ✅ done | 1 |  | claude | US-EX-31-4 | [US-EX-31](stories/US-EX-31.md) |
| [US-EX-31-3](tasks/US-EX-31-3.md) | Test: Existing valid inputs still pass validation | ✅ done | 1 |  | claude | US-EX-31-4 | [US-EX-31](stories/US-EX-31.md) |
| [US-EX-31-4](tasks/US-EX-31-4.md) | Fix weak regex patterns in ExampleObjectValidators to use + instead of * | ✅ done | 1 | bugfix, code-quality | claude | — | [US-EX-31](stories/US-EX-31.md) |
| [US-EX-4-1](tasks/US-EX-4-1.md) | Test: V1 APIKeyAuthenticateRequest tested | ✅ done | 1 |  | claude | US-EX-4-5 | [US-EX-4](stories/US-EX-4.md) |
| [US-EX-4-2](tasks/US-EX-4-2.md) | Test: V1 UserAuthenticateRequest tested | ✅ done | 1 |  | claude | US-EX-4-5 | [US-EX-4](stories/US-EX-4.md) |
| [US-EX-4-3](tasks/US-EX-4-3.md) | Test: V2 APIKeyAuthenticateRequest tested | ✅ done | 1 |  | claude | US-EX-4-5 | [US-EX-4](stories/US-EX-4.md) |
| [US-EX-4-4](tasks/US-EX-4-4.md) | Test: V2 UserAuthenticateRequest tested | ✅ done | 1 |  | claude | US-EX-4-5 | [US-EX-4](stories/US-EX-4.md) |
| [US-EX-4-5](tasks/US-EX-4-5.md) | Implement V1 and V2 auth request contract tests | ✅ done | 3 |  | claude | — | [US-EX-4](stories/US-EX-4.md) |
| [US-EX-5-1](tasks/US-EX-5-1.md) | Test: ExampleObjectCreate tested | ✅ done | 1 |  | — | US-EX-5-4 | [US-EX-5](stories/US-EX-5.md) |
| [US-EX-5-2](tasks/US-EX-5-2.md) | Test: ExampleObjectResponse tested | ✅ done | 1 |  | — | US-EX-5-4 | [US-EX-5](stories/US-EX-5.md) |
| [US-EX-5-3](tasks/US-EX-5-3.md) | Test: ExampleObjectUpdate tested | ✅ done | 1 |  | — | US-EX-5-4 | [US-EX-5](stories/US-EX-5.md) |
| [US-EX-5-4](tasks/US-EX-5-4.md) | Implement ExampleObject contract tests | ✅ done | 2 |  | claude | — | [US-EX-5](stories/US-EX-5.md) |
| [US-EX-6-1](tasks/US-EX-6-1.md) | Test: ErrorModel tested | ✅ done | 1 |  | — | US-EX-6-3 | [US-EX-6](stories/US-EX-6.md) |
| [US-EX-6-2](tasks/US-EX-6-2.md) | Test: V2 APIRoutes tested | ✅ done | 1 |  | — | US-EX-6-3 | [US-EX-6](stories/US-EX-6.md) |
| [US-EX-6-3](tasks/US-EX-6-3.md) | Implement ErrorModel and V2 APIRoutes tests | ✅ done | 2 |  | claude | — | [US-EX-6](stories/US-EX-6.md) |
| [US-EX-7-1](tasks/US-EX-7-1.md) | Test: Model properties tested | ✅ done | 1 |  | claude | US-EX-7-3 | [US-EX-7](stories/US-EX-7.md) |
| [US-EX-7-2](tasks/US-EX-7-2.md) | Test: Business logic methods tested | ✅ done | 1 |  | claude | US-EX-7-3 | [US-EX-7](stories/US-EX-7.md) |
| [US-EX-7-3](tasks/US-EX-7-3.md) | Implement ExampleObject XPO model tests | ✅ done | 2 |  | claude | — | [US-EX-7](stories/US-EX-7.md) |
| [US-EX-8-1](tasks/US-EX-8-1.md) | Test: aspnetCoreInstaller tested | ✅ done | 1 |  | — | US-EX-8-7 | [US-EX-8](stories/US-EX-8.md) |
| [US-EX-8-2](tasks/US-EX-8-2.md) | Test: AutomapperInstaller tested | ✅ done | 1 |  | — | US-EX-8-7 | [US-EX-8](stories/US-EX-8.md) |
| [US-EX-8-3](tasks/US-EX-8-3.md) | Test: JWTInstaller tested | ✅ done | 1 |  | — | US-EX-8-7 | [US-EX-8](stories/US-EX-8.md) |
| [US-EX-8-4](tasks/US-EX-8-4.md) | Test: RabbitMQ installer tested | ✅ done | 1 |  | — | US-EX-8-7 | [US-EX-8](stories/US-EX-8.md) |
| [US-EX-8-5](tasks/US-EX-8-5.md) | Test: SwaggerInstaller tested | ✅ done | 1 |  | — | US-EX-8-7 | [US-EX-8](stories/US-EX-8.md) |
| [US-EX-8-6](tasks/US-EX-8-6.md) | Test: XPOInstaller tested | ✅ done | 1 |  | — | US-EX-8-7 | [US-EX-8](stories/US-EX-8.md) |
| [US-EX-8-7](tasks/US-EX-8-7.md) | Implement installer registration tests for all 6 installers | ✅ done | 5 |  | claude | — | [US-EX-8](stories/US-EX-8.md) |
| [US-EX-9-1](tasks/US-EX-9-1.md) | Test: All V2 auth endpoints tested | ✅ done | 1 |  | claude | US-EX-9-4 | [US-EX-9](stories/US-EX-9.md) |
| [US-EX-9-2](tasks/US-EX-9-2.md) | Test: Auth token validation tested | ✅ done | 2 |  | claude | US-EX-9-4 | [US-EX-9](stories/US-EX-9.md) |
| [US-EX-9-3](tasks/US-EX-9-3.md) | Test: Error responses tested | ✅ done | 2 |  | claude | US-EX-9-4 | [US-EX-9](stories/US-EX-9.md) |
| [US-EX-9-4](tasks/US-EX-9-4.md) | Implement V2 AuthController unit tests | ✅ done | 3 |  | claude | — | [US-EX-9](stories/US-EX-9.md) |
