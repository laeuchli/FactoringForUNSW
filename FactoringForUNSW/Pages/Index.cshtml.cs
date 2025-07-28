using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Numerics;
using Microsoft.Azure.Cosmos;

namespace FactoringForUNSW.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        [BindProperty]
        public string InputNumberString { get; set; }

        public BigInteger InputNumber { get; set; }

        public (BigInteger, BigInteger)? Factors { get; set; }

        public bool HasResult { get; set; }

        private CosmosClient _client;
        Database _databaseFactors;
        Microsoft.Azure.Cosmos.Container _containerFactors;

        bool _dbInit;

        public IndexModel(ILogger<IndexModel> logger, CosmosClient client)
        {
            _client = client;
            _logger = logger;
            _dbInit = false;
        }

        public class FactoringResult
        {

            public string id { get; set; }
            public string Factor1 { get; set; }
            public string Factor2 { get; set; }
            public DateTime Timestamp { get; set; }
        }

        public async Task<bool> CreateDataBase()
        {
            if (_dbInit == false)
            {
                _databaseFactors = await _client.CreateDatabaseIfNotExistsAsync(id: "Factors");
                _containerFactors = await _databaseFactors.CreateContainerIfNotExistsAsync(id: "Input", partitionKeyPath: "/id");


                _dbInit = true;
            }
            return true;
        }

        public async Task SaveFactoringResultAsync(FactoringResult result)
        {
            await _containerFactors.CreateItemAsync(result, new PartitionKey(result.id));
        }

        public async Task OnPostAsync()
        {

            bool database = await CreateDataBase();


            if (BigInteger.TryParse(InputNumberString, out var number) && number > 1)
            {
                InputNumber = number;
                Factors = FindFactors(number);

                var result = new FactoringResult
                {
                    id = InputNumber.ToString(),
                    Factor1 = Factors?.Item1.ToString(),
                    Factor2 = Factors?.Item2.ToString(),
                    Timestamp = DateTime.UtcNow
                };
                try
                {
                    await SaveFactoringResultAsync(result);
                }
                catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Conflict)
                {


                }


            }
            HasResult = true;

        }

        public static BigInteger Sqrt(BigInteger value)
        {
            if (value < 0) throw new ArgumentException("Negative argument.");
            if (value == 0) return 0;

            BigInteger n = value / 2;
            BigInteger last = 0;

            while (n != last)
            {
                last = n;
                n = (n + value / n) / 2;
            }

            return n;
        }

        private (BigInteger, BigInteger)? FindFactors(BigInteger number)
        {
            BigInteger limit = Sqrt(number) + 1;

            for (BigInteger i = 2; i <= limit; i++)
            {
                if (number % i == 0)
                {
                    return (i, number / i);
                }
            }

            return null; // no non-trivial factors
        }
    }
}