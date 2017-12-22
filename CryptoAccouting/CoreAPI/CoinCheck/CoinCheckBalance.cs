namespace CoinBalance.CoreAPI
{
    public class CoinCheckBalance
    {
        public bool Success { get; set; }
        public decimal Jpy { get; set; }
        public decimal Btc { get; set; }
        public decimal JpyReserved { get; set; }
        public decimal BtcReserved { get; set; }
        public decimal JpyLendInUse { get; set; }
        public decimal BtcLendInUse { get; set; }
        public decimal JpyLent { get; set; }
        public decimal BtcLent { get; set; }
        public decimal JpyDebt { get; set; }
        public decimal BtcDebt { get; set; }

        public decimal Bch { get; set; }
        public decimal Eth { get; set; }
        public decimal Etc { get; set; }
        public decimal Lsk { get; set; }
        public decimal Xmr { get; set; }
        public decimal Rep { get; set; }
        public decimal Xrp { get; set; }
        public decimal Zec { get; set; }
        public decimal Xem { get; set; }
        public decimal Ltc { get; set; }
        public decimal Dash { get; set; }
    }
}