using Amazon;
using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using Amazon.S3;
using Amazon.S3.Model;
using AriguinhaLambdaTest.Exchanges;
using System.Linq;
using System.Text.Json;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace AriguinhaLambdaTest;

public class Function
{
    private static readonly string bucketName = "ariguinha-integraton-sqs-lambda-test";
    private static readonly RegionEndpoint bucketRegion = RegionEndpoint.SAEast1;
    private static readonly IAmazonS3 s3Client = new AmazonS3Client(bucketRegion);
    private static readonly List<GenericExchange> exchanges = new List<GenericExchange>() {
        new Okx(),
        new GateIo(),
    };

    public async Task FunctionHandler(SQSEvent sqsEvent, ILambdaContext context)
    {
        foreach (var record in sqsEvent.Records)
        {
            context.Logger.LogLine($"Recebida mensagem SQS: {record.Body}");

            try
            {
                var queueMessage = JsonSerializer.Deserialize<QueueMessage>(record.Body);
                if (queueMessage != null)
                {
                    foreach (var exchange in exchanges)
                    {
                        var filePath = await exchange.DownloadTrades(queueMessage.Symbol ?? "BTCUSDT-PERP", queueMessage.Date);
                        if (File.Exists(filePath))
                        {
                            var fileName = Path.GetFileName(filePath);
                            await UploadFileToS3Async(filePath, Path.Combine(exchange.Name, fileName), context);
                            File.Delete(filePath);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                context.Logger.LogLine($"Erro ao processar a mensagem: {ex.Message}");
            }
        }
    }

    private static async Task UploadFileToS3Async(string filePath, string fileName, ILambdaContext context)
    {
        try
        {
            var putRequest = new PutObjectRequest
            {
                BucketName = bucketName,
                Key = fileName,
                FilePath = filePath,
                ContentType = "application/octet-stream"
            };

            var response = await s3Client.PutObjectAsync(putRequest);
            context.Logger.LogLine($"Arquivo {fileName} enviado para S3 com sucesso.");
        }
        catch (Exception e)
        {
            context.Logger.LogLine($"Erro ao enviar o arquivo para S3: {e.Message}");
        }
    }
}