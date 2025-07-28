using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Azure.Cosmos;
using static FactoringForUNSW.Pages.IndexModel;

namespace FactoringForUNSW.Pages
{
    public class ShowFactorsModel : PageModel
    {
        private CosmosClient _client;
        Database _databaseFactors;
        Microsoft.Azure.Cosmos.Container _containerFactors;
        bool _dbInit;
        public List<FactoringResult> FactorResults { get; set; }
        public ShowFactorsModel(CosmosClient cosmosClient)
        {
            _client = cosmosClient;
            _dbInit = false;

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
        public async Task OnGetAsync()
        {
            bool database = await CreateDataBase();
            var query = _containerFactors.GetItemQueryIterator<FactoringResult>(
            new QueryDefinition("SELECT * FROM c"));

            FactorResults = new List<FactoringResult>();

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                FactorResults.AddRange(response);
            }
        }
    }
}
