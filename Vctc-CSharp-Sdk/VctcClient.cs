using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cobra.Utils;
using Newtonsoft.Json.Linq;
using RestSharp;
using Vctc_CSharp_Sdk.Utils;

namespace VctcNet.Sdk
{
    /**
     * Class VctcClient  for http request
     * @package Vastchain\VctcPhpSdk
     */
    public class VctcClient
    {
        private string apiPrefix = "https://v1.api.tc.vastchain.ltd";
        private string appId;
        private string appSecret;
        private readonly RestClient _client;

        public VctcClient(string appId, string appSecret, string apiPrefix)
        {
            if (string.Empty == appId || string.Empty == appSecret)
            {
                throw new Exception("invalid appId or/and appSecret");
            }

            this.appId = appId;
            this.apiPrefix = apiPrefix;
            this.appSecret = appSecret;
            this._client = new RestClient(apiPrefix);
        }


        /**
         *  Calculate the signature of a request.
         * @param method string Method of requesting http
         * @param path string  Path of requesting http
         * @param query array Query of requesting http
         * @param body array Body of requesting http
         * @return mixed
         * @throws VctcException
         */
        public (SortedList<string, string> fullQueries, string signature, string timestamp ) getSignature(string method, string path,
            SortedList<string, string> query, string body)
        {
            if (string.Empty == path)
            {
                throw new Exception("invalid path");
            }

            if (method != "GET" && method != "POST" && method != "DELETE" && method != "PUT")
            {
                throw new Exception("invalid method, only GET, POST, DELETE and PUT is supported");
            }

            if (query==null)
            {
                query=new SortedList<string, string>();
            }

            string textForSigning = method + " " + path + "\n";

            query["_appid"] = this.appId;
            query["_t"] = (DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0)).ToString();


            string queryStr = HtmlUtils.BuildQueryString(query);

            textForSigning += queryStr;

            if (body != "")
            {
                textForSigning += "\n" + body;
            }

            query["_s"] = HashUtils.HMAC_SHA256(textForSigning, this.appSecret);

            return (fullQueries: query, signature: query["_s"], timestamp: query["_t"]);
        }

        /**
         *  Request http api
         * @param method string Method of requesting http
         * @param path string  Path of requesting http
         * @param query array Query of requesting http
         * @param body array Body of requesting http
         * @return mixed
         * @throws VctcException
         */
        public async Task<string> callAPI(Method method, string path, SortedList<string, string> query, string body)

        {
            // if (is_array(body)) {
            //     this.fliterParams(body);
            // }
            // if (is_array(query)) {
            //     this.fliterParams(query);
            // }

            var signatures = this.getSignature(method.ToString(), path, query, body);


            var req = BuildRequest(path+"?"+HtmlUtils.BuildQueryString( signatures.fullQueries), method, body);
            var cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(3));

            var res = await _client.ExecuteTaskAsync(req, cts.Token);
            if (res.Content != "")
            {


                var ret = JObject.Parse(res.Content);
                if (ret.TryGetValue("error", out var err))
                {
                    var ex = new VctcException((string) ret["msg"], (string) ret["code"]);
                    ex.setRaw(res.Content, (string) ret["code"]);
                    throw ex;
                }
            }

            return res.Content;
        }

        public async Task<string> get(string path, SortedList<string, string> query)
        {
            return  await this.callAPI(Method.GET, path, query, "");
            
        }

        public async Task<string> post(string path, SortedList<string, string> query, string body)
        {
            return await this.callAPI(Method.POST, path, query, body);
        }

        public async Task<string> put(string path, SortedList<string, string> query, string body)
        {
            return await this.callAPI(Method.PUT, path, query, body);
        }

        public async Task<string> delete(string path, SortedList<string, string> query)
        {
            return await this.callAPI(Method.DELETE, path, query, "");
        }


        private IRestRequest BuildRequest(string path, Method method, string body)
        {
            var req = new RestRequest(path, method);
            req.AddHeader("Content-Type", "application/json");
            req.AddHeader("User-Agent", "vctc-sdk/net. Version=0.0.1");
            if (method == Method.POST && body != "")
            {
                req.AddJsonBody(body);
            }

            return req;
        }
    }
}