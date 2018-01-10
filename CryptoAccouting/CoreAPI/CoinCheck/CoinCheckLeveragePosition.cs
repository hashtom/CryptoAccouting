using System;
using System.Collections.Generic;

namespace CoinBalance.CoreAPI
{
    public class CoinCheckLeveragePosition
    {
        public bool success { get; set; }
        public Pagination pagination { get; set; }
        public List<position> data { get; set; }

        public class Pagination
        {
            public int limit { get; set; }
            public string order { get; set; }
            public object starting_after { get; set; }
            public object ending_before { get; set; }
        }

        public class Order
        {
            public string id { get; set; }
            public string side { get; set; }
            public decimal rate { get; set; }
            public decimal amount { get; set; }
            public decimal pending_amount { get; set; }
            public string status { get; set; }
            public DateTime created_at { get; set; }
        }

        public class position
        {
            public string id { get; set; }
            public string pair { get; set; }
            public string status { get; set; }
            public DateTime created_at { get; set; }
            public DateTime closed_at { get; set; }
            public decimal open_rate { get; set; }
            public decimal closed_rate { get; set; }
            public decimal amount { get; set; }
            public decimal all_amount { get; set; }
            public string side { get; set; }
            public decimal pl { get; set; }
            public Order new_order { get; set; }
            public List<Order> close_orders { get; set; }
        }
    }
}