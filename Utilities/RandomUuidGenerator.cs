using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiUCI.Utilities
{
    public class RandomUuidGenerator
    {
        public static string GenerateRandomUuid()
        {
            Guid newUuid = Guid.NewGuid();
            
            return newUuid.ToString();
        }
    }
}