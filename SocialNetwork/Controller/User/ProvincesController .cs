using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Middlewares;
using SocialNetwork.Service;

namespace SocialNetwork.Controller.User
{
    [TypeFilter(typeof(AuthenticationFilter))]
    [Route("api/[controller]")]
    [ApiController]
    public class ProvincesController : ControllerBase
    {
        private readonly IProvincesService _provincesService;

        public ProvincesController(IProvincesService provincesService)
        {
            _provincesService = provincesService;
        }

        [HttpGet("getAllProvinces")]
        public IActionResult GetAllProvinces()
        {
            var provinces = _provincesService.GetAllProvinces();
            return Ok(provinces);
        }

        [HttpGet("getDistrictsByProvinceId/{provinceId}")]
        public IActionResult GetDistrictsByProvinceId(string provinceId)
        {
            var districts = _provincesService.GetDistrictsByProvinceId(provinceId);
            return Ok(districts);
        }

        [HttpGet("getWardsByDistrictId/{districtId}")]
        public IActionResult GetWardsByDistrictId(string districtId)
        {
            var wards = _provincesService.GetWardsByDistrictId(districtId);
            return Ok(wards);
        }
    }
}
