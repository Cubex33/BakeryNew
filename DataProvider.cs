using EF.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace SP2
{
    public static class DataProvider
    {
        public static void init()
        {
            dbContext = new BakeryDbContext();
        }

        public static BakeryDbContext dbContext { get; private set; }
    }
}
