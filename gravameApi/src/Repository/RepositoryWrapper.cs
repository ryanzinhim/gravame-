using Common.Database;
using gravameApi.src.Interface;

namespace gravameApi.src.Repository
{
    public class RepositoryWrapper(Context context) : IRepositoryWrapper
    {
        private Context context = context;

        private readonly IMunicipioTomRepository municipiosTom;

        public IMunicipioTomRepository MunicipiosTom
        {
            get
            {
                if (municipiosTom == null)
                {
                    return new MunicipioTomRepository(context);
                }
                return municipiosTom;
            }
        }

        public async Task SaveAsync()
        {
            await context.SaveChangesAsync();
        }

    }
}
