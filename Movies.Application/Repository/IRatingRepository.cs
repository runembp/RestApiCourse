﻿using Movies.Application.Models;

namespace Movies.Application.Repository;

public interface IRatingRepository
{
    Task<bool> RateMovieAsync(Guid movieId, Guid userId, int rating, CancellationToken token = default);
    Task<float?> GetRatingAsync(Guid movieId, CancellationToken token = default);
    Task<(float? rating, int? UserRating)> GetRatingAsync(Guid movieId, Guid userId, CancellationToken token = default);
    Task<bool> DeleteRatingAsync(Guid movieId, Guid userId, CancellationToken token = default);
    Task<IEnumerable<MovieRating>> GetRatingsForUserASync(Guid userId, CancellationToken token = default);
}