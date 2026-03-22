using ExampleAPI.Installers;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit;

namespace ExampleAPI.Tests.Installers
{
    public class InstallerOrderTests
    {
        private static List<IInstaller> GetAllInstallers()
        {
            return typeof(Startup).Assembly.ExportedTypes
                .Where(x => typeof(IInstaller).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
                .Select(Activator.CreateInstance)
                .Cast<IInstaller>()
                .ToList();
        }

        [Fact]
        public void AllInstallers_HaveDistinctOrderValues()
        {
            var installers = GetAllInstallers();

            var orderValues = installers.Select(i => i.Order).ToList();

            orderValues.Should().OnlyHaveUniqueItems(
                "each installer must have a unique Order value to guarantee deterministic execution");
        }

        [Fact]
        public void Installers_WhenSortedByOrder_ProduceDeterministicSequence()
        {
            var installers = GetAllInstallers();

            var sorted = installers.OrderBy(i => i.Order).ToList();

            sorted.Should().BeInAscendingOrder(i => i.Order,
                "installers must execute in ascending Order to ensure deterministic behavior");
        }

        [Fact]
        public void Installers_SortedOrder_IsConsistentAcrossMultipleDiscoveries()
        {
            // Run discovery multiple times and verify the sorted order is always the same
            var sequences = new List<List<string>>();

            for (int i = 0; i < 10; i++)
            {
                var installers = GetAllInstallers();
                var sorted = installers.OrderBy(inst => inst.Order)
                    .Select(inst => inst.GetType().Name)
                    .ToList();
                sequences.Add(sorted);
            }

            var first = sequences[0];
            foreach (var sequence in sequences.Skip(1))
            {
                sequence.Should().Equal(first,
                    "installer execution order must be identical across repeated discoveries");
            }
        }

        [Fact]
        public void AllInstallers_AreDiscoveredByReflection()
        {
            var installers = GetAllInstallers();

            installers.Should().NotBeEmpty("at least one IInstaller implementation should exist");
            installers.Count.Should().BeGreaterOrEqualTo(6,
                "all known installers should be discovered");
        }

        [Fact]
        public void IInstaller_Interface_DeclaresOrderProperty()
        {
            var orderProperty = typeof(IInstaller).GetProperty("Order");

            orderProperty.Should().NotBeNull("IInstaller must declare an Order property so each installer can specify its execution order");
            orderProperty!.PropertyType.Should().Be(typeof(int));
        }

        [Theory]
        [InlineData(typeof(aspnetCoreInstaller), 0)]
        [InlineData(typeof(XPOInstaller), 10)]
        [InlineData(typeof(JWTInstaller), 20)]
        [InlineData(typeof(AutomapperInstaller), 30)]
        [InlineData(typeof(SwaggerInstaller), 40)]
        [InlineData(typeof(ExampleAPI.Installers.RabbitMQ), 50)]
        public void Installer_SpecifiesExpectedOrder(Type installerType, int expectedOrder)
        {
            var installer = (IInstaller)Activator.CreateInstance(installerType)!;

            installer.Order.Should().Be(expectedOrder,
                $"{installerType.Name} should specify Order = {expectedOrder}");
        }

        [Fact]
        public void EachInstaller_ExplicitlyDefinesOrderProperty()
        {
            var installerTypes = typeof(Startup).Assembly.ExportedTypes
                .Where(x => typeof(IInstaller).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
                .ToList();

            foreach (var type in installerTypes)
            {
                var orderProperty = type.GetProperty("Order", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                orderProperty.Should().NotBeNull(
                    $"{type.Name} must explicitly define its own Order property rather than relying on the interface default");
            }
        }
    }
}
