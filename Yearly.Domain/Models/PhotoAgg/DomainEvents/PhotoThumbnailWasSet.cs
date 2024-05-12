using Yearly.Domain.Models.PhotoAgg.ValueObjects;

namespace Yearly.Domain.Models.PhotoAgg.DomainEvents;

public sealed record PhotoThumbnailWasSet(PhotoId PhotoId) : IDomainEvent;