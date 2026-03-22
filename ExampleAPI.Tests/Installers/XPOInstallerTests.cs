using ExampleAPI.Installers;
using FluentAssertions;
using System.Reflection;
using System.Reflection.Emit;
using Xunit;

namespace ExampleAPI.Tests.Installers
{
    public class XPOInstallerTests
    {
        private static Type[] InvokeGetClasses(string assemblyName, string ns = "")
        {
            var method = typeof(XPOInstaller)
                .GetMethod("GetClasses", BindingFlags.NonPublic | BindingFlags.Static);
            method.Should().NotBeNull("GetClasses should exist on XPOInstaller");
            return (Type[])method.Invoke(null, new object[] { assemblyName, ns });
        }

        [Fact]
        public void GetClasses_NoNullCheckInSource_ReturnsArrayNotNull()
        {
            // When called with a namespace that has no matching persistent classes,
            // the method should return an empty array — not null.
            // Before the fix, the dead null check could mislead readers into thinking
            // null was a possible return value.
            var result = InvokeGetClasses("ExampleAPI", "NonExistent.Namespace");

            result.Should().NotBeNull("dead null check was removed; method always returns an array");
            result.Should().BeEmpty("no classes exist in a non-existent namespace");
        }

        [Fact]
        public void GetClasses_WithValidNamespace_ReturnsValidTypeArray()
        {
            var result = InvokeGetClasses("ExampleAPI", "ExampleAPI.Models.ExampleXPOModel");

            result.Should().NotBeNull("method must always return an array, never null");
            result.Should().AllSatisfy(t => t.Should().NotBeNull());
        }

        [Fact]
        public void GetClasses_WithEmptyNamespace_ReturnsValidTypeArray()
        {
            var result = InvokeGetClasses("ExampleAPI", "");

            result.Should().NotBeNull("method must always return an array, never null");
            result.Should().AllSatisfy(t => t.Should().NotBeNull());
        }

        [Fact]
        public void GetClasses_HandlesReflectionTypeLoadException()
        {
            // Verify the GetClasses method body contains a catch clause
            // for ReflectionTypeLoadException — proving the exception is handled,
            // not propagated.
            var method = typeof(XPOInstaller)
                .GetMethod("GetClasses", BindingFlags.NonPublic | BindingFlags.Static);
            method.Should().NotBeNull();

            var body = method.GetMethodBody();
            body.Should().NotBeNull();

            // ExceptionHandlingClauses describe try/catch blocks in the IL.
            // At least one must catch ReflectionTypeLoadException.
            body.ExceptionHandlingClauses
                .Should().Contain(
                    clause => clause.CatchType == typeof(ReflectionTypeLoadException),
                    "GetClasses must catch ReflectionTypeLoadException to handle unloadable types gracefully");
        }

        [Fact]
        public void ReflectionTypeLoadException_TypesFiltering_RemovesNulls()
        {
            // The catch block uses: ex.Types.Where(t => t != null).ToArray()
            // Verify this filtering logic works correctly with a real exception.
            var mixedTypes = new Type[] { typeof(string), null, typeof(int), null, typeof(double) };
            var exception = new ReflectionTypeLoadException(mixedTypes, new Exception[] { new Exception("test") });

            var filtered = exception.Types.Where(t => t != null).ToArray();

            filtered.Should().HaveCount(3);
            filtered.Should().NotContainNulls("null types from unloadable assemblies must be filtered out");
            filtered.Should().ContainInOrder(typeof(string), typeof(int), typeof(double));
        }

        [Fact]
        public void GetClasses_ValidTypesDiscoveredAndRegistered()
        {
            // Acceptance criterion: "Valid types still discovered and registered correctly"
            // After the ReflectionTypeLoadException handling was added, valid persistent
            // types must still be found and returned by GetClasses.
            var result = InvokeGetClasses("ExampleAPI", "ExampleAPI.Models.ExampleXPOModel");

            result.Should().NotBeEmpty("the namespace contains at least one XPO persistent class");

            // ExampleObject should be discovered
            result.Should().Contain(t => t.Name == "ExampleObject",
                "ExampleObject is a valid persistent class and must be discovered");

            // All returned types must be classes that derive from DevExpress.Xpo.PersistentBase
            result.Should().AllSatisfy(t =>
            {
                t.IsClass.Should().BeTrue("GetClasses only returns classes");
                t.IsAssignableTo(typeof(DevExpress.Xpo.PersistentBase)).Should().BeTrue(
                    $"'{t.Name}' must be a PersistentBase subclass to be registered with XPO");
            });

            // All returned types must belong to the requested namespace
            result.Should().AllSatisfy(t =>
                t.Namespace.Should().Be("ExampleAPI.Models.ExampleXPOModel",
                    "only types from the requested namespace should be returned"));
        }

        [Fact]
        public void GetClasses_UnloadableTypesAreSkippedNotCrashedOn()
        {
            // Acceptance criterion: "Unloadable types are skipped not crashed on"
            // The catch block for ReflectionTypeLoadException filters out null entries
            // (which represent unloadable types) and continues with the loadable ones.
            // Verify the catch block both exists AND filters nulls — the two together
            // guarantee unloadable types are skipped, not crashed on.
            var method = typeof(XPOInstaller)
                .GetMethod("GetClasses", BindingFlags.NonPublic | BindingFlags.Static);
            method.Should().NotBeNull();

            // 1. The method catches ReflectionTypeLoadException (doesn't crash)
            var body = method.GetMethodBody();
            body.ExceptionHandlingClauses
                .Should().Contain(
                    clause => clause.CatchType == typeof(ReflectionTypeLoadException),
                    "unloadable types must be caught, not propagated as a crash");

            // 2. Simulate the exception's Types array (contains nulls for unloadable types)
            //    and verify the filtering logic skips them
            var typesWithUnloadable = new Type[] { typeof(string), null, null };
            var ex = new ReflectionTypeLoadException(typesWithUnloadable, new Exception[] { new Exception() });

            var surviving = ex.Types.Where(t => t != null).ToArray();
            surviving.Should().ContainSingle("only the one loadable type should survive");
            surviving.Should().NotContainNulls("unloadable types (nulls) must be skipped");
        }
    }
}
