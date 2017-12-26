using System;
using System.Collections.Generic;

namespace CoinBalance.CoreAPI
{
    public class QuoineExecutions
    {

        public List<execution> models { get; set; }
        public int current_page { get; set; }
        public int total_pages { get; set; }

        public class execution
        {
            public int Id { get; set; }
            public decimal quantity { get; set; }
            public decimal price { get; set; }
            public string taker_side { get; set; }
            public string my_side { get; set; }
            public DateTime created_at { get; set; }
        }
    }
}
