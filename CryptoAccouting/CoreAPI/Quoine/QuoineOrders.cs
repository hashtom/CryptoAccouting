using System;
using System.Collections.Generic;

namespace CoinBalance.CoreAPI
{
    public class QuoineOrders
    {

        public List<Order> models { get; set; }
        public int current_page { get; set; }
        public int total_pages { get; set; }

        public class Order
        {
            public string id { get; set; }
            public string order_type { get; set; }
            public decimal quantity { get; set; }
            public decimal disc_quantity { get; set; }
            public decimal iceberg_total_quantity { get; set; }
            public string side { get; set; }
            public decimal filled_quantity { get; set; }
            public decimal price { get; set; }
            public DateTime created_at { get; set; }
            public DateTime updated_at { get; set; }
            public string status { get; set; }
            public int leverage_level { get; set; }
            public string source_exchange { get; set; }
            public int product_id { get; set; }
            public string product_code { get; set; }
            public string funding_currency { get; set; }
            public string currency_pair_code { get; set; }
            public decimal order_fee { get; set; }
            public List<Execution> executions { get; set; }
        }

        public class Execution
        {
            public int id { get; set; }
            public decimal quantity { get; set; }
            public decimal price { get; set; }
            public string taker_side { get; set; }
            public string my_side { get; set; }
            public DateTime created_at { get; set; }
        }
    }
}