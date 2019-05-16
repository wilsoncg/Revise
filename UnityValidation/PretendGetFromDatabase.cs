using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityValidation
{
    public class PretendGetFromDatabase : IGetFromDatabase
    {
        public IEnumerable<CustomerDetails> CustomerDetails(Customer customer)
        {
            return new List<CustomerDetails> {
                new UnityValidation.CustomerDetails { Username = "test" },
                new UnityValidation.CustomerDetails { Username = "real" }
            };
        }
    }
}
