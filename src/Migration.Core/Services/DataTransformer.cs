using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Migration.DataAccess.Model;

namespace Migration.Core.Services
{
    public interface IDataTransformer
    {
        UserNew Transform(UserOld oldUser);
    }

    public class DataTransformer : IDataTransformer
    {
        public UserNew Transform(UserOld oldUser)
        {
            if (string.IsNullOrWhiteSpace(oldUser.LegacyEmailAddress))
                throw new ValidationException($"Invalid for ID {oldUser.LegacyUserId}: Missing email.");

            var normalizedEmail = oldUser.LegacyEmailAddress.ToLowerInvariant().Trim();
            
            if (!Regex.IsMatch(normalizedEmail, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                throw new ValidationException($"Invalid email after normalization: {normalizedEmail}.");

            var newUser = new UserNew
            {
                UserId = Guid.NewGuid(),
                FullName = $"{oldUser.FirstName.Trim()} {oldUser.LastName.Trim()}",
                StandardizedEmail = normalizedEmail,
                IsActive = oldUser.Status.Equals("ACTIVE", StringComparison.OrdinalIgnoreCase),
                PreferredDocumentType = MapDocumentType(oldUser.DocumentType)
            };

            return newUser;
        }

        private static string MapDocumentType(string? oldType)
        {
            // Example of complex mapping logic
            return oldType?.ToUpper() switch
            {
                "FACTURE" => "INVOICE",
                "ORDINE" => "ORDER",
                _ => "DEFAULT"
            };
        }
    }
}
