using Perfume_King.Shared.DTOs;

namespace Perfume_King.Shared.Services
{
    public interface IPerfumeService
    {
        public Task<ServiceResult<List<PerfumeDTO>>> GetFeaturedPerfumesAsync();
        public Task<ServiceResult<List<PerfumeDTO>>> GetPerfumesAsync();
        public Task<ServiceResult<PerfumeDTO>> GetPerfumeAsync(int id);

        public Task<ServiceResult> DeletePerfumeAsync(int perfumeId);
        public Task<ServiceResult> DeleteAllPerfumesAsync();
    }
}
