﻿using Dapper;
using Movies.Application.Database;

namespace Movies.Application.Repository;

public class ratingRepositoryBO : IRatingRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public ratingRepositoryBO(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<float?> GetRatingAsync(Guid movieId, CancellationToken token = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        return await connection.QuerySingleOrDefaultAsync<float?>(new CommandDefinition("""
                                                                                        select round(avg(r.rating), 1) from ratings r
                                                                                        where r.movieid = @movieId
                                                                                        """, new { movieId }, cancellationToken: token));
    }

    public async Task<(float? rating, int? UserRating)> GetRatingAsync(Guid movieId, Guid userId, CancellationToken token = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);

        return await connection.QuerySingleOrDefaultAsync<(float?, int?)>(new CommandDefinition("""
                                                                                                select round(avg(rating), 1), 
                                                                                                (select rating from ratings
                                                                                                where movieid = @movieId
                                                                                                and userid = @userId
                                                                                                limit 1)
                                                                                                from ratings
                                                                                                where movieid = @movieId
                                                                                                """, new { movieId, userId }, cancellationToken: token));
    }
}