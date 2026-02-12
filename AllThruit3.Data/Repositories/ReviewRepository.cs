using AllThruit3.Data.Contexts;
using AllThruit3.Data.Entities;
using AllThruit3.Shared.DTOs;
using AllThruit3.Shared.Repositories;
using Dapper;
using Mapster;
using System.Data;

namespace AllThruit3.Data.Repositories;

public class ReviewRepository : IReviewRepository
{
    private readonly AllThruitDbContext _db;
    private readonly IDbConnection _connection;

    public ReviewRepository(AllThruitDbContext db, IDbConnection connection)
    {
        _db = db;
        _connection = connection;
    }

    public async Task<List<ReviewDTO>> GetAllAsync(CancellationToken ct = default)
    {
        return (await _connection.QueryAsync<ReviewDTO>(
            "SELECT Id, UserId, TmdbMovieId, Text, Rating, Vibe, PosterBlobId, CreatedOn FROM Reviews"
        )).ToList();
    }

    public async Task<ReviewDTO?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _connection.QuerySingleOrDefaultAsync<ReviewDTO>(
            "SELECT Id, UserId, TmdbMovieId, Text, Rating, Vibe, PosterBlobId, CreatedOn FROM Reviews WHERE Id = @Id",
            new { Id = id });
    }

    public async Task<List<ReviewDTO>> GetByVibeAsync(string vibe, CancellationToken ct = default)
    {
        return (await _connection.QueryAsync<ReviewDTO>(
            @"SELECT Id, UserId, TmdbMovieId, Text, Rating, Vibe, PosterBlobId, CreatedOn 
              FROM Reviews 
              WHERE LOWER(Vibe) = LOWER(@Vibe)
              ORDER BY CreatedOn DESC",
            new { Vibe = vibe }
        )).ToList();
    }

    public async Task<ReviewDTO> CreateAsync(ReviewDTO dto, CancellationToken ct = default)
    {
        var entity = dto.Adapt<Review>();
        _db.Reviews.Add(entity);
        await _db.SaveChangesAsync(ct);
        return entity.Adapt<ReviewDTO>();
    }

    public async Task<ReviewDTO> UpdateAsync(ReviewDTO dto, CancellationToken ct = default)
    {
        var entity = await _db.Reviews.FindAsync(dto.Id);
        if (entity == null)
            throw new KeyNotFoundException($"Review with Id {dto.Id} not found.");

        dto.Adapt(entity);
        _db.Reviews.Update(entity);
        await _db.SaveChangesAsync(ct);
        return entity.Adapt<ReviewDTO>();
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _db.Reviews.FindAsync(id);
        if (entity == null)
            throw new KeyNotFoundException($"Review with Id {id} not found.");

        _db.Reviews.Remove(entity);
        await _db.SaveChangesAsync(ct);
    }
}