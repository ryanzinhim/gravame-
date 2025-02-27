using Common.Database;
using Common.Models.Gravame;
using gravameApi.src.Interface;

namespace gravameApi.src.Repository
{
    public class MunicipioTomRepository : RepositoryBase<MunicipioTom>, IMunicipioTomRepository
    {
        public MunicipioTomRepository(Context context) : base(context)
        {
        }
    }
}
