using Amazon.CloudWatchLogs;
using Amazon.CloudWatchLogs.Model;

namespace EmpresariosConLiderazgo.Services
{
    public class CloudwatchLogs : ICloudwatchLogs
    {
        public async Task InsertLogs(string controller, string method, string message)
        {
            var logClient = new AmazonCloudWatchLogsClient();
            var logGroupName = "/aws/EmpresariosConLiderazgo";
            var logStreamName = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff");
            var existing = await logClient
                .DescribeLogGroupsAsync(new DescribeLogGroupsRequest()
                    { LogGroupNamePrefix = logGroupName });
            var logGroupExists = existing.LogGroups.Any(l => l.LogGroupName == logGroupName);
            if (!logGroupExists)
                await logClient.CreateLogGroupAsync(new CreateLogGroupRequest(logGroupName));
            await logClient.CreateLogStreamAsync(new CreateLogStreamRequest(logGroupName, logStreamName));
            await logClient.PutLogEventsAsync(new PutLogEventsRequest()
            {
                LogGroupName = logGroupName,
                LogStreamName = logStreamName,
                LogEvents = new List<InputLogEvent>()
                {
                    new()
                    {
                        Message = $"Controller : {controller}, Method: {method}, Message :{message}",
                        Timestamp = DateTime.UtcNow
                    }
                }
            });
        }
    }

    public interface ICloudwatchLogs
    {
        Task InsertLogs(string controller, string method, string message);
    }
}