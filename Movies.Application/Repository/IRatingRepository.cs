namespace Movies.Application.Repository;

public interface IRatingsRepository
{
    Task<float?> GetRatingAsync(Guid movieId, CancellationToken token = default);
    Task<(float? rating, int? UserRating)> GetRatingAsync(Guid movieId, Guid userId, CancellationToken token = default);
}