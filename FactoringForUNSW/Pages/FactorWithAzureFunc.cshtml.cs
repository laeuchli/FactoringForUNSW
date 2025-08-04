using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Azure.Cosmos;
using System.Numerics;

namespace FactoringForUNSW.Pages
{
    public class FactorWithAzureFuncModel : PageModel
    {
        [BindProperty]
        public string InputNumberString { get; set; }

        public BigInteger InputNumber { get; set; }
        public (BigInteger, BigInteger)? Factors { get; set; }
        public bool HasResult { get; set; }

        public string resultstring = " ";
        private CosmosClient _client;
        Database _databaseFactors;
        Microsoft.Azure.Cosmos.Container _containerFactors;
        bool _dbInit;
        private IConfiguration _configuration;
        public FactorWithAzureFuncModel(IConfiguration configuration, CosmosClient client)
        {

            _dbInit = false;

            _configuration = configuration;

        }
        public class FactoringResult
        {

            public string id { get; set; }
            public string Factor1 { get; set; }
            public string Factor2 { get; set; }
            public DateTime Timestamp { get; set; }
        }




        public async Task OnPostAsync()
        {
            HasResult = false;



            if (BigInteger.TryParse(InputNumberString, out var number) && number > 1)
            {
                using var client = new HttpClient();

                string url = $"http://localhost:7179/api/FindFactors?number={number}";

                var response = await client.GetAsync(url);
                resultstring = await response.Content.ReadAsStringAsync();


            }





        }



    }
}
