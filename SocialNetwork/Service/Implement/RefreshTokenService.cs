//using AutoMapper;
//using DocumentFormat.OpenXml.Spreadsheet;
//using Microsoft.IdentityModel.Tokens;
//using Service.Implement.ObjectMapping;
//using SocialNetwork.DTO;
//using SocialNetwork.Entity;
//using SocialNetwork.ExceptionModel;
//using SocialNetwork.Repository;
//using System.IdentityModel.Tokens.Jwt;
//using System.Security.Claims;
//using System.Text;

//namespace SocialNetwork.Service.Implement
//{
//    public class RefreshTokenService : IRefreshTokenService
//    {
//        private readonly IRefreshTokenRepository refreshTokenRepository;

//        private readonly IUserRepository userRepository;

//        private readonly IConfiguration configuration;

//        private readonly IMapper mapper = new MapperConfiguration(cfg =>
//        {
//            cfg.AddProfile(new MappingProfile());
//        }).CreateMapper();

//        public RefreshTokenService(IRefreshTokenRepository refreshTokenRepository,
//            IUserRepository userRepository,
//            IConfiguration configuration)
//        {
//            this.userRepository = userRepository;
//            this.refreshTokenRepository = refreshTokenRepository;
//            this.configuration = configuration;
//        }

//        public RefreshTokenDTO Create(RefreshTokenDTO dto)
//        {
//            throw new NotImplementedException();
//        }

//        public void Delete(string id)
//        {
//            throw new NotImplementedException();
//        }

//        private string GenerateAccessToken(User user)
//        {
//            var appSetting = configuration.GetSection("AppSettings");
//            var jwtTokenHandler = new JwtSecurityTokenHandler();
//            var secretKeyBytes = Encoding.UTF8.GetBytes(appSetting["SecretKey"]!);

//            var tokenDescription = new SecurityTokenDescriptor
//            {
//                Subject = new ClaimsIdentity(new[] {
//                    new Claim(ClaimTypes.Role, user.IsAdmin == true ? "Admin" : "Common"),
//                }),
//                Expires = DateTime.UtcNow.AddMinutes(15),
//                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey
//                    (secretKeyBytes), SecurityAlgorithms.HmacSha256Signature)
//            };

//            var token = jwtTokenHandler.CreateToken(tokenDescription);
//            var accessToken = jwtTokenHandler.WriteToken(token);
//            return accessToken;
//        }

//        private RefreshToken GenerateRefreshToken(User user)
//        {
//            var refreshTokenEntity = new RefreshToken
//            {
//                UserId = user.Id,
//                IsActivated = true,
//                ExpirationDate = DateTime.UtcNow.AddDays(7)
//            };

//            return refreshTokenEntity;
//        }

//        public AuthResponse GenerateLoginTokens(User user)
//        {
//            string accessToken = GenerateAccessToken(user);
//            RefreshToken refreshTokenEntity = GenerateRefreshToken(user);


//            refreshTokenRepository.Create(refreshTokenEntity);
//            refreshTokenRepository.Save();

//            var userDTO = new
//            {
//                id = user.Id,
//                email = user.Email,
//                password = "",
//                role = (bool)user.IsAdmin! ? "admin" : "common"
//            };

//            TokenDTO tokenDTO = new TokenDTO
//            {
//                AccessToken = accessToken,
//                RefreshToken = refreshTokenEntity.Id
//            };

//            return new AuthResponse
//            {
//                Token = tokenDTO,
//                User = userDTO
//            };
//        }

//        public List<RefreshTokenDTO> GetAll()
//        {
//            throw new NotImplementedException();
//        }

//        public RefreshTokenDTO GetByUserId(string id)
//        {
//            throw new NotImplementedException();
//        }

//        public bool IsActivate(RefreshTokenDTO dto)
//        {
//            throw new NotImplementedException();
//        }

//        public AuthResponse RefreshToken(string token)
//        {
//            // #4: Check refresh token is existed ?
//            var storedToken = refreshTokenRepository.FindById(token!);
//            if (storedToken == null)
//            {
//                throw new BadRequestException("Refresh token doesn't exist");
//            }

//            // #5: Check refresh token is activated ?
//            if (!(storedToken.IsActivated == true))
//            {
//                if (storedToken.FamilyId != null)
//                {
//                    DeleteDeactivatedToken(storedToken.FamilyId);
//                }
//                else
//                {
//                    DeleteDeactivatedToken(storedToken.Id);
//                }

//                throw new BadRequestException("Refresh token is not activated");
//            }

//            // Update token isActivate (true => false)
//            storedToken.IsActivated = false;
//            refreshTokenRepository.Update(storedToken);

//            // Generate new token
//            var user = userRepository.FindById(storedToken.UserId!);
//            string accessToken = GenerateAccessToken(user);
//            RefreshToken refreshToken = GenerateRefreshToken(user);


//            // Set FamilyId to refresh token
//            if (storedToken.FamilyId != null)
//            {
//                refreshToken.FamilyId = storedToken.FamilyId;
//            }
//            else
//            {
//                refreshToken.FamilyId = storedToken.Id;
//            }
//            refreshTokenRepository.Create(refreshToken);

//            refreshTokenRepository.Save();

//            var userDTO = new
//            {
//                id = user.Id,
//                email = user.Email,
//                password = "",
//                role = (bool)user.IsAdmin! ? "admin" : "common"
//            };

//            TokenDTO tokenDTO = new TokenDTO
//            {
//                AccessToken = accessToken,
//                RefreshToken = refreshToken.Id
//            };

//            return new AuthResponse
//            {
//                Token = tokenDTO,
//                User = userDTO
//            };
//        }

//        public void DeleteRefreshTokenExpired()
//        {
//            var refreshTokenList = refreshTokenRepository
//                .FindByCondition(x => x.ExpirationDate < DateTime.UtcNow.AddDays(-3));

//            foreach (var item in refreshTokenList)
//            {
//                refreshTokenRepository.Delete(item);
//            }

//            refreshTokenRepository.Save();
//        }

//        public void DeleteTokenWhenLogout(string refreshTokenId)
//        {
//            var refreshToken = refreshTokenRepository.FindById(refreshTokenId!);

//            if (refreshToken == null)
//            {
//                throw new BadRequestException("Refresh token doesn't exist");
//            }

//            if (refreshToken.FamilyId != null)
//            {
//                DeleteDeactivatedToken(refreshToken.FamilyId);
//            }
//            else
//            {
//                DeleteDeactivatedToken(refreshTokenId);
//            }
//        }

//        private void DeleteDeactivatedToken(string id)
//        {
//            List<RefreshToken> refreshTokenList = refreshTokenRepository.FindAll();
//            List<RefreshToken> toDeleteList = new List<RefreshToken>();
//            foreach (RefreshToken refreshToken in refreshTokenList)
//            {
//                if (refreshToken.Id == id || refreshToken.FamilyId == id)
//                {
//                    toDeleteList.Add(refreshToken);
//                }
//            }

//            foreach (RefreshToken refreshToken in toDeleteList)
//            {
//                refreshTokenRepository.Delete(refreshToken);
//            }

//            refreshTokenRepository.Save();
//        }
//    }
//}
