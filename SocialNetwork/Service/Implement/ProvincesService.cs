using SocialNetwork.Entity;

namespace SocialNetwork.Service.Implement
{
    public class ProvincesService : IProvincesService
    {
        private readonly SocialNetworkContext _context;

        public ProvincesService(SocialNetworkContext context)
        {
            _context = context;
        }

        public IEnumerable<Province> GetAllProvinces()
        {
            return _context.Provinces.ToList();
        }

        public IEnumerable<District> GetDistrictsByProvinceId(string provinceId)
        {
            return _context.Districts
                .Where(d => d.ProvinceCode == provinceId)
                .ToList();
        }

        public IEnumerable<Ward> GetWardsByDistrictId(string districtId)
        {
            return _context.Wards
                .Where(w => w.DistrictCode == districtId)
                .ToList();
        }
    }
}