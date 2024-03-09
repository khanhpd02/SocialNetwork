using SocialNetwork.Entity;

namespace SocialNetwork.Service
{
    public interface IProvincesService
    {
        IEnumerable<Province> GetAllProvinces();
        IEnumerable<District> GetDistrictsByProvinceId(string provinceId);
        IEnumerable<Ward> GetWardsByDistrictId(string districtId);
    }
}
