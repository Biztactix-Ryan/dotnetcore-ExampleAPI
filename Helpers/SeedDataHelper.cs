using DevExpress.Xpo;
using ExampleAPI.Models.ExampleXPOModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExampleAPI.Helpers
{
    public class SeedDataHelper
    {
        private static readonly string[] firstNames = new string[] {
            "Peter", "Ryan", "Richard", "Tom", "Mark", "Steve",
            "Jimmy", "Jeffrey", "Andrew", "Dave", "Bert", "Mike",
            "Ray", "Paul", "Brad", "Carl", "Jerry" };
        private static readonly string[] lastNames = new string[] {
            "Dolan", "Fischer", "Hamlett", "Hamilton", "Lee",
            "Lewis", "McClain", "Miller", "Murrel", "Parkins",
            "Roller", "Shipman", "Bailey", "Barnes", "Lucas", "Campbell" };
      
        private static readonly Random Random = new Random(0);

        public static void Seed(UnitOfWork uow)
        {
            var names = new KeyValuePair<string, string>[firstNames.Length * lastNames.Length];
            for (int i = 0; i < firstNames.Length * lastNames.Length; i++)
            {
                int j = Random.Next(i + 1);
                names[i] = names[j];
                names[j] = new KeyValuePair<string, string>(firstNames[i / lastNames.Length], lastNames[i % lastNames.Length]);
            }
            foreach (var t in names)
            {
                CreateCustomer(uow, t.Key, t.Value);
            }
            uow.CommitChanges();
        }

        private static void CreateCustomer(UnitOfWork uow, string firstName, string lastName)
        {
            for (int i = 0; i < firstNames.Length * lastNames.Length; i++)
            {
                ExampleObject customer = new ExampleObject(uow);
                customer.FirstName = firstName;
                customer.LastName = lastName;
            }
            
            
        }
    }
}
