//using System;
//using System.Collections.Generic;
//using System.Linq;
//using Newtonsoft.Json;
//using NLog;
//using RestSharp;
//using CryptoArb.CoreModel;

//namespace CryptoArb.Exchange.Quoine
//{
//    public class QuoineAdapter : BrokerAdapter
//    {
//        private const string ApiRoot = "https://api.quoine.com";
//        //private static readonly Logger Log = LogManager.GetCurrentClassLogger();
//        private readonly BrokerConfig _config;
//        private readonly IRestClient _restClient;

//        public QuoineAdapter(IRestClient restClient, IConfigStore configStore): base(true, true)
//        {
//            Broker = Broker.Quoine;
//            _config = configStore.Config.Brokers.First(b => b.Broker == Broker);
//            _restClient = restClient ?? throw new ArgumentNullException(nameof(restClient));
//            _restClient.BaseUrl = new Uri(ApiRoot);
//        }

//        //public Broker Broker => Broker.Quoine;

//        protected override void SendOrder(Order order)
//        {
//            //if (order.Broker != Broker)
//            //{
//            //    throw new InvalidOperationException();
//            //}

//            var param = new
//            {
//                order = new SendOrderParam(order)
//            };
//            var reply = Send(param);
//            order.BrokerOrderId = reply.Id;
//            order.Status = OrderStatus.New;
//            order.SentTime = DateTime.Now;
//            order.LastUpdated = DateTime.Now;
//        }

//        protected override void RefreshOrder(Order order)
//        {
//            var reply = GetOrderState(order.BrokerOrderId);
//            reply.SetOrder(order);
//        }

//        protected override void CancelOrder(Order order)
//        {
//            Cancel(order.BrokerOrderId);
//            order.LastUpdated = DateTime.Now;
//            order.Status = OrderStatus.Canceled;
//        }

//        public override decimal GetBtcPosition()
//        {
//            var path = "/trading_accounts";
//            var req = BuildRequest(path);
//            var accounts = RestUtil.ExecuteRequest<List<TradingAccounts>>(_restClient, req);
//            var account = accounts.Find(b => b.CurrencyPairCode == "BTCJPY");
//            return account.Position;
//        }

//        public override IList<Quote> FetchQuotes()
//        {
//            try
//            {
//                Log.Debug($"Getting depth from {_config.Broker}...");
//                var path = "/products/5/price_levels";
//                var req = RestUtil.CreateJsonRestRequest(path);
//                var response = _restClient.Execute<Depth>(req);
//                if (response.ErrorException != null)
//                {
//                    throw response.ErrorException;
//                }

//                Log.Debug($"Received depth from {_config.Broker}.");
//                var quotes = response.Data.ToQuotes();
//                return quotes ?? new List<Quote>();
//            }
//            catch (Exception ex)
//            {
//                Log.Error(ex.Message);
//                Log.Debug(ex);
//                return new List<Quote>();
//            }
//        }

//        private SendResult Send(object param)
//        {
//            var method = "POST";
//            var path = "/orders/";
//            var body = JsonConvert.SerializeObject(param);
//            var req = BuildRequest(path, method, body);
//            var reply = RestUtil.ExecuteRequest<SendResult>(_restClient, req);
//            return reply;
//        }

//        private OrderStateResult GetOrderState(string id)
//        {
//            var path = $"/orders/{id}";
//            var req = BuildRequest(path);
//            return RestUtil.ExecuteRequest<OrderStateResult>(_restClient, req);
//        }

//        private void Cancel(string orderId)
//        {
//            var method = "PUT";
//            var path = $"/orders/{orderId}/cancel";
//            var req = BuildRequest(path, method);
//            RestUtil.ExecuteRequest(_restClient, req);
//        }

//        private RestRequest BuildRequest(string path, string method = "GET", string body = "")
//        {
//            var nonce = Util.Nonce;
//            var payload = new Dictionary<string, object>
//            {
//                {"path", path},
//                {"nonce", nonce},
//                {"token_id", _config.Key}
//            };
//            var sign = Util.JwtHs256Encode(payload, _config.Secret);
//            var req = RestUtil.CreateJsonRestRequest(path);
//            req.Method = Util.ParseEnum<Method>(method);
//            if (body != "")
//            {
//                req.AddParameter("application/json", body, ParameterType.RequestBody);
//            }
//            req.AddHeader("X-Quoine-API-Version", "2");
//            req.AddHeader("X-Quoine-Auth", sign);
//            return req;
//        }
//    }
//}