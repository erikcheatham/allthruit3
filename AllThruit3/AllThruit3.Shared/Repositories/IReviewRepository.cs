using AllThruit3.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AllThruit3.Shared.Repositories;

public interface IReviewRepository
{
    Task<List<ReviewDTO>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<ReviewDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<ReviewDTO>> GetByVibeAsync(string vibe, CancellationToken cancellationToken = default);
    Task<ReviewDTO> CreateAsync(ReviewDTO dto, CancellationToken cancellationToken = default);
    Task<ReviewDTO> UpdateAsync(ReviewDTO dto, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}