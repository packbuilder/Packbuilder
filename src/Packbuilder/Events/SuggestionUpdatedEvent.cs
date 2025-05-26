namespace Packbuilder.Events;

public record SuggestionUpdatedEvent(
    int ModpackId,
    int SuggestionId
);