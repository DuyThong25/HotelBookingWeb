using HotelBookingWeb.Data.IRepository;
using HotelBookingWeb.Models;
using HotelBookingWeb.Models.ViewModel;
using Microsoft.DotNet.Scaffolding.Shared.CodeModifier.CodeChange;
using Newtonsoft.Json;
using RestSharp;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace HotelBookingWeb.Utility
{
    public class MomoSettings : IMomoSetting
    {
        private readonly IConfiguration _configuration;

        public MomoSettings(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string ComputeHmacSha256(string message, string secretKey)
        {
            var keyBytes = Encoding.UTF8.GetBytes(secretKey);
            var messageBytes = Encoding.UTF8.GetBytes(message);

            byte[] hashBytes;

            using (var hmac = new HMACSHA256(keyBytes))
            {
                hashBytes = hmac.ComputeHash(messageBytes);
            }

            var hashString = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

            return hashString;
        }

        public dynamic CreatePaymentAsync(ShoppingCartVM model)
        {
            string accessKey = _configuration["MomoAPI:AccessKey"];
            string partnerCode = _configuration.GetSection("MomoAPI:PartnerCode").Get<string>();
            string redirectUrl = _configuration.GetSection("MomoAPI:ReturnUrl").Get<string>();
            string requestType = _configuration.GetSection("MomoAPI:RequestType").Get<string>();
            string amount = model.OrderHeader.OrderTotal.ToString();
            string orderId = "MMHUFBOOKPAY-" + model.OrderHeader.Id.ToString();
            string requestId = $"MM{orderId}";
            string orderInfo = $"{model.OrderHeader.Name}";
            string ipnUrl = "https://momo.vn";
            string extraData = "";
            string notifyUrl = _configuration["MomoAPI:NotifyUrl"];
            string returnUrl = _configuration["MomoAPI:ReturnUrl"];
            string lang = "vi";
         
            //   var rawData =
            //$"partnerCode={partnerCode}&accessKey={accessKey}&requestId={requestId}&amount={amount}&orderId={orderId}&orderInfo={orderInfo}&returnUrl={returnUrl}&notifyUrl={notifyUrl}&extraData={extraData}";
            var rawData =
                $"accessKey={accessKey}&amount={amount}&extraData={extraData}&ipnUrl={ipnUrl}&orderId={orderId}&orderInfo={orderInfo}&partnerCode={partnerCode}&redirectUrl={redirectUrl}&requestId={requestId}&requestType={requestType}";
            
            var signature = ComputeHmacSha256(rawData, _configuration.GetSection("MomoAPI:SecretKey").Get<string>());
            var client = new RestClient(_configuration.GetSection("MomoAPI:MomoApiUrl").Get<string>());
            var request = new RestRequest()
            {
                Method = RestSharp.Method.Post
            };
            request.AddHeader("Content-Type", "application/json; charset=UTF-8");

            // Create an object representing the request data
            var requestData = new
            {
                partnerCode = partnerCode,
                requestId = requestId,
                amount = amount,
                orderId = orderId,
                orderInfo = orderInfo,
                redirectUrl = redirectUrl,
                ipnUrl = ipnUrl,
                requestType = requestType,
                extraData = extraData,
                lang = lang,
                signature = signature,
            };

            request.AddParameter("application/json", JsonConvert.SerializeObject(requestData), ParameterType.RequestBody);

            //var response = await client.ExecuteAsync(request);
            var content = client.ExecuteAsync(request).GetAwaiter().GetResult().Content;

            return JsonConvert.DeserializeObject<dynamic>(content);
        }
    }
}
