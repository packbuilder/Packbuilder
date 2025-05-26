using System.Text.Json.Serialization;
using Packbuilder.Models;

namespace Packbuilder.Dto.ServiceDtos
{
    public class SuggestionVerificationDto
    {
        [JsonPropertyName("incompatibleMods")]
        public required List<string>? IncompatibleMods { get; set; }
        [JsonPropertyName("conflictingModifications")]
        public required List<Modification> ConflictingModifications { get; set; }
    }
}