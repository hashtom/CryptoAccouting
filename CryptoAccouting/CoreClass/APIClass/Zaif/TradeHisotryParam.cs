namespace CoinBalance.CoreClass.APIClass
{
    public class TradeHisotryParam
    {
        public int from { get; set; }
        public int count { get; set; }
        public int from_id { get; set; }
        public int end_id { get; set; }
        public string order { get; set; }
        public long since { get; set; }
        public long end { get; set; }
        public string currency_pair { get; set; }
        public bool is_token { get; set; }
    }
}
