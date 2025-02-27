using Microsoft.EntityFrameworkCore;

namespace gravameApi.src.Interface
{
    public interface IRepositoryWrapper
    {
        IMunicipioTomRepository MunicipiosTom { get; }

        Task SaveAsync();
    }
}
