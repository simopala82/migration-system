using Microsoft.Extensions.Logging;

namespace Migration.Core.Services
{
    public interface IMessageProducer
    {
        Task SendMigrationRequest(int legacyUserId);
        Task SendForcedMigrationRequest(int legacyUserId, string adminUser);
    }

    public class MessageProducer : IMessageProducer
    {
        private readonly ILogger<MessageProducer> _logger;
        // Add RabbitMQ/Kafka/Azure SB client here

        public MessageProducer(ILogger<MessageProducer> logger)
        {
            _logger = logger;
        }
        
        public Task SendMigrationRequest(int legacyUserId)
        {
            _logger.LogInformation($"[Broker] Sending standard request for User ID: {legacyUserId}");
            // Actual logic: Use the broker client to send a message
            // to the 'migration-requests' queue
            return Task.CompletedTask;
        }

        // Simulates sending a forced/priority message
        public Task SendForcedMigrationRequest(int legacyUserId, string adminUser)
        {
            _logger.LogWarning($"[Broker] Sending FORCED/ADMIN request for User ID: {legacyUserId} by {adminUser}");
            // Actual logic: Send to the 'forced-migration-requests' queue or
            // set a priority header on the standard message.
            return Task.CompletedTask;
        }
    }
}
