﻿using Dapper;
using Movies.Application.Database;
using Movies.Application.Models;

namespace Movies.Application.Repository;

public class RatingRepository : IRatingRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public RatingRepository(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<bool> RateMovieAsync(Guid movieId, Guid userId, int rating, CancellationToken token = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);

        var result = await connection.ExecuteAsync(new CommandDefinition("""
                                                                         insert into ratings (userid, movieid, rating)
                                                                         values (@userId, @movieId, @rating)
                                                                         on conflict (userid, movieid) do update
                                                                           set rating = @rating
                                                                         """, new { userId, movieId, rating }, cancellationToken: token));

        return result > 0;
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

    public async Task<bool> DeleteRatingAsync(Guid movieId, Guid userId, CancellationToken token = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);

        var result = await connection.ExecuteAsync(new CommandDefinition("""
                                                                         delete from ratings 
                                                                         where movieid = @movieId 
                                                                         and userid = @userId
                                                                         """, new { movieId, userId }, cancellationToken: token));

        return result > 0;
    }

    public async Task<IEnumerable<MovieRating>> GetRatingsForUserASync(Guid userId, CancellationToken token = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);

        return await connection.QueryAsync<MovieRating>(new CommandDefinition("""
                                                                              select r.rating, r.movieid, m.slug 
                                                                              from ratings r
                                                                              inner join movies m on r.movieid = m.id
                                                                              where r.userid = @userId
                                                                              """, new { userId }, cancellationToken: token));
    }
}