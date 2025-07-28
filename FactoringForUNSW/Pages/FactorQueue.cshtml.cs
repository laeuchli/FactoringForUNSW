using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Azure.Cosmos;
using Azure.Storage.Queues;
using System.Numerics;

namespace FactoringForUNSW.Pages
{
    public class FactorQueueModel : PageModel
    {
        private CosmosClient _client;
        Database _databaseFactors;
        Microsoft.Azure.Cosmos.Container _containerFactors;
        bool _dbInit;
        private IConfiguration _configuration;


        [BindProperty]
        public string InputNumberString { get; set; }


        public BigInteger InputNumber { get; set; }
        
        public bool HasResult { get; set; }


        public FactorQueueModel(IConfiguration configuration, CosmosClient client)
        {
            _dbInit = false;
            _client = client;
            _configuration = configuration;
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
        public async Task OnPostAsync()
        {
            bool database = await CreateDataBase();
            if (BigInteger.TryParse(InputNumberString, out var number) && number > 1)
            {
                QueueClient queueClient = new QueueClient(_configuration["Queue:ConnectionString"], _configuration["Queue:Name"]);
                queueClient.CreateIfNotExists();
                if (queueClient.Exists())
                {
                    queueClient.SendMessage(InputNumberString);
                }
            }
        }
    }
}
