using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using AF_Quintero_Common.Models;
using AF_Quintero_Common.Responses;
using AF_Quintero.Functions.Entities;

namespace AF_Quintero.Functions.Functions
{
    public class AFapi
    {
        [FunctionName(nameof(CreateTodo))]
        public static async Task<IActionResult> CreateTodo(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "registrationemployed")] HttpRequest req,
            [Table("registrationemployed", Connection = "AzureWebJobsStorage")] CloudTable registrationemployedTable,
            ILogger log)
        {
            log.LogInformation("Recieved a new todo.");



            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            AF registrationemployed = JsonConvert.DeserializeObject<AF>(requestBody);

            if (string.IsNullOrEmpty(registrationemployed?.employedId))
            {
                return new BadRequestObjectResult(new Responses
                {
                    IsSuccess = false,
                    Message = "The request must have a TaskDescription."

                });
            }

            AFEntity todoEntity = new AFEntity
            {
                createdtime = DateTime.UtcNow,
                ETag = "*",
                Isconsolidated = false,
                PartitionKey = "registrationemployed",
                RowKey = Guid.NewGuid().ToString(),
                employedId = registrationemployed.employedId
            };

            TableOperation addOperation = TableOperation.Insert(todoEntity);
            await registrationemployedTable.ExecuteAsync(addOperation);

            string message = "New todo stored in table";
            log.LogInformation(message);



            return new OkObjectResult(new Responses
            {
                IsSuccess = true,
                Message = message,
                Result = todoEntity
            });
        }



        [FunctionName(nameof(UpdateTodo))]
        public static async Task<IActionResult> UpdateTodo(
       [HttpTrigger(AuthorizationLevel.Anonymous, "PUT", Route = "registrationemployed/{id}")] HttpRequest req,
       [Table("registrationemployed", Connection = "AzureWebJobsStorage")] CloudTable registrationemployedTable,
       string id,
       ILogger log)
        {
            log.LogInformation($"Update for todo: {id},received.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            registrationemployed todo = JsonConvert.DeserializeObject<registrationemployed>(requestBody);
            //validate todo id
            TableOperation findOperation = TableOperation.Retrieve<AFEntity>("TODO", id);
            TableResult findResult = await registrationemployedTable.ExecuteAsync(findOperation);

            if (findResult.Result == null)
            {
                return new BadRequestObjectResult(new Responses
                {
                    IsSuccess = false,
                    Message = "Todo not found."

                });
            }

            //Update todo
            AFEntity todoEntity = (AFEntity)findResult.Result;
            todoEntity.Isconsolidated =todo.Isconsolidated;
            if (!string.IsNullOrEmpty(todo.employedId))
            {
                todoEntity.employedId = todo.employedId;
            }


            TableOperation replaceOperation = TableOperation.Replace(todoEntity);
            await registrationemployedTable.ExecuteAsync(replaceOperation);

            string message = $"Todo: {id} update in table.";
            log.LogInformation(message);



            return new OkObjectResult(new Responses
            {
                IsSuccess = true,
                Message = message,
                Result = todoEntity
            });
        }

        [FunctionName(nameof(GetallTodos))]
        public static async Task<IActionResult> GetallTodos(
           [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "registrationemployed")] HttpRequest req,
           [Table("registrationemployed", Connection = "AzureWebJobsStorage")] CloudTable registrationemployedTable,
           ILogger log)
        {
            log.LogInformation("Get all todos received.");

            TableQuery<AFEntity> query = new TableQuery<AFEntity>();
            TableQuerySegment<AFEntity> todos = await registrationemployedTable.ExecuteQuerySegmentedAsync(query, null);

            string message = "Retrieved all todos";
            log.LogInformation(message);



            return new OkObjectResult(new Responses
            {
                IsSuccess = true,
                Message = message,
                Result = todos
            });
        }



        [FunctionName(nameof(GetTodoById))]
        public static IActionResult GetTodoById(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "registrationemployed/{id}")] HttpRequest req,
            [Table("registrationemployed", "TODO", "{id}", Connection = "AzureWebJobsStorage")] AFEntity todoEntity,
            string id,
            ILogger log)
        {
            log.LogInformation($"Get todo by id:{id}, received.");
            if (todoEntity == null)
            {
                return new NotFoundObjectResult(new Responses
                {
                    IsSuccess = false,
                    Message = "Todo not found."
                });
            }

            // Send response
            string message = $"Todo: {todoEntity.RowKey}, retrieved.";
            log.LogInformation(message);

            return new OkObjectResult(new Responses
            {
                IsSuccess = true,
                Message = message,
                Result = todoEntity
            });
        }


        [FunctionName(nameof(DeleteTodo))]
        public static async Task<IActionResult> DeleteTodo(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "registrationemployed/{id}")] HttpRequest req,
            [Table("registrationemployed", "TODO", "{id}", Connection = "AzureWebJobsStorage")] AFEntity todoEntity,
             [Table("registrationemployed", Connection = "AzureWebJobsStorage")] CloudTable registrationemployedTable,
            string id,
            ILogger log)
        {
            log.LogInformation($"Delete todo by id:{id}, received.");
            if (todoEntity == null)
            {
                return new NotFoundObjectResult(new Responses
                {
                    IsSuccess = false,
                    Message = "Todo not found."
                });
            }

            await registrationemployedTable.ExecuteAsync(TableOperation.Delete(todoEntity));
            string message = $"Todo: {todoEntity.RowKey}, delete.";
            log.LogInformation(message);

            return new OkObjectResult(new Responses
            {
                IsSuccess = true,
                Message = message,
                Result = todoEntity
            });
        }
    }

}
