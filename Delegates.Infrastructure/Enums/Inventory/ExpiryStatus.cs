using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Infrastructure.Enums.Inventory
{
    public enum ExpiryStatus
    {
        Green = 1,  // بعيد
        Yellow = 2, // قرب ينتهي (أقل من 30 يوم)
        Red = 3     // منتهي
    }
}
