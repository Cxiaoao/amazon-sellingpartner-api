using Amazon.SellingPartnerAPIAA;
using RestSharp;
using System;

namespace Amazon.SellingPartnerAPI.Simple1
{
    class Program
    {
        static void Main(string[] args)
        {
            LWAAuthorizationCredentials lwaAuthorizationCredentials = new LWAAuthorizationCredentials
            {
                // 相关值App客户端增加后就会有
                ClientId = "amzn1.application-XXX-client.XXXXXXXXXXXXXXXXXXXXXXX",
                ClientSecret = "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX",
                RefreshToken = "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX",
                Endpoint = new Uri("https://api.amazon.com/auth/o2/token")
            };
            RestClient client = new RestClient("https://sellingpartnerapi-na.amazon.com");

            // 获取订单列表
            RestRequest request = new RestRequest("/orders/v0/orders", Method.GET);
            request.AddQueryParameter("MarketplaceIds", "A2EUQ1WTGCTBG2");
            request.AddQueryParameter("CreatedAfter", "2020-12-01T00:00:00Z");
            LWAAuthorizationSigner auth = new LWAAuthorizationSigner(lwaAuthorizationCredentials);
            auth.Sign(request);

            AWSAuthenticationCredentials aws = new AWSAuthenticationCredentials();

            // 相关值为AWS IAM增加User后下载的cvs
            aws.AccessKeyId = "XXXXXXXXXXXXXXXXXXX";
            aws.SecretKey = "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX";
            aws.Region = "us-east-1";
            AWSSigV4Signer signer = new AWSSigV4Signer(aws);
            signer.Sign(request, client.BaseUrl.Host);

            var resp = client.Execute(request);
            Console.WriteLine(resp.StatusCode);
            Console.WriteLine(resp.Content);
        }
    }
}
