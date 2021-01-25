using Amazon.Runtime;
using Amazon.SecurityToken;
using Amazon.SecurityToken.Model;
using Amazon.SellingPartnerAPIAA;
using RestSharp;
using System;

namespace Amazon.SellingPartnerAPI.Simple2
{
    class Program
    {
        static async System.Threading.Tasks.Task Main(string[] args)
        {
            // 相关值为AWS IAM增加User后下载的cvs
            var accessKey = "XXXXXXXXXXXXXXXXXXX";
            var secretKey = "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX";
            var credentials = new BasicAWSCredentials(accessKey, secretKey);
            var client = new AmazonSecurityTokenServiceClient(credentials);
            var assumeRoleRequest = new AssumeRoleRequest()
            {
                // AWS IAM Role ARN
                DurationSeconds = 3600,
                RoleArn = "arn:aws:iam::0000000000000:role/XXXXXXXX",
                RoleSessionName = DateTime.Now.Ticks.ToString()
            };
            AssumeRoleResponse assumeRoleResponse = await client.AssumeRoleAsync(assumeRoleRequest);

            RestClient restClient = new RestClient("https://sellingpartnerapi-na.amazon.com");
            IRestRequest restRequest = new RestRequest("/orders/v0/orders", Method.GET);
            restRequest.AddQueryParameter("CreatedAfter", "2020-12-01T00:00:00Z");
            restRequest.AddQueryParameter("marketplaceIds", "ATVPDKIKX0DER");
            var lwaAuthCreds = new LWAAuthorizationCredentials
            {
                // 相关值App客户端增加后就会有
                ClientId = "amzn1.application-XXX-client.XXXXXXXXXXXXXXXXXXXXXXX",
                ClientSecret = "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX",
                RefreshToken = "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX",
                Endpoint = new Uri("https://api.amazon.com/auth/o2/token")
            };
            restRequest = new LWAAuthorizationSigner(lwaAuthCreds).Sign(restRequest);
            var awsAuthCreds = new AWSAuthenticationCredentials
            {
                AccessKeyId = assumeRoleResponse.Credentials.AccessKeyId,
                SecretKey = assumeRoleResponse.Credentials.SecretAccessKey,
                Region = "us-east-1"
            };
            restRequest.AddHeader("X-Amz-Security-Token", assumeRoleResponse.Credentials.SessionToken);
            restRequest = new AWSSigV4Signer(awsAuthCreds)
                .Sign(restRequest, restClient.BaseUrl.Host);
            var resp = restClient.Execute(restRequest);
            Console.WriteLine(resp.StatusCode);
            Console.WriteLine(resp.Content);
        }
    }
}
