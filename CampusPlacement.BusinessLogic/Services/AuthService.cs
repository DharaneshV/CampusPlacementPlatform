using CampusPlacement.BusinessLogic.DTOs.Auth;
using CampusPlacement.BusinessLogic.Interfaces;
using CampusPlacement.Domain.Entities;
using CampusPlacement.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using CampusPlacement.DataAccess.Context;

namespace CampusPlacement.BusinessLogic.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _configuration;
    private readonly ApplicationDbContext _dbContext;

    public AuthService(UserManager<ApplicationUser> userManager, IConfiguration configuration, ApplicationDbContext dbContext)
    {
        _userManager = userManager;
        _configuration = configuration;
        _dbContext = dbContext;
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto request)
    {
        var user = await _dbContext.Users
            .Include(u => u.PlacementOfficer)
            .Include(u => u.CompanyHR)
            .Include(u => u.Student)
            .FirstOrDefaultAsync(u => u.Email == request.Email);

        if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
            throw new UnauthorizedAccessException("Invalid email or password.");

        if (user.AccountStatus != AccountStatus.Approved)
            throw new UnauthorizedAccessException("Account is not active.");

        var roles = await _userManager.GetRolesAsync(user);
        var userRole = roles.FirstOrDefault() ?? string.Empty;

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email!),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.Role, userRole)
        };

        int? profileId = null;
        int? collegeId = null;
        int? companyId = null;

        // Apply Business Rules for login access & claims mapping
        if (userRole == Domain.Constants.RoleConstants.PlacementOfficer && user.PlacementOfficer != null)
        {
            profileId = user.PlacementOfficer.PlacementOfficerId;
            collegeId = user.PlacementOfficer.CollegeId;
            claims.Add(new Claim("ProfileId", profileId.ToString()!));
            claims.Add(new Claim("CollegeId", collegeId.ToString()!));
        }
        else if (userRole == Domain.Constants.RoleConstants.CompanyHR && user.CompanyHR != null)
        {
            profileId = user.CompanyHR.CompanyHRId;
            companyId = user.CompanyHR.CompanyId;
            claims.Add(new Claim("ProfileId", profileId.ToString()!));
            claims.Add(new Claim("CompanyId", companyId.ToString()!));
        }
        else if (userRole == Domain.Constants.RoleConstants.Student && user.Student != null)
        {
            profileId = user.Student.StudentId;
            collegeId = user.Student.CollegeId;
            claims.Add(new Claim("ProfileId", profileId.ToString()!));
            claims.Add(new Claim("CollegeId", collegeId.ToString()!));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiry = DateTime.UtcNow.AddMinutes(double.Parse(_configuration["JwtSettings:ExpiryInMinutes"]!));

        var token = new JwtSecurityToken(
            issuer: _configuration["JwtSettings:Issuer"],
            audience: _configuration["JwtSettings:Audience"],
            claims: claims,
            expires: expiry,
            signingCredentials: creds
        );

        return new AuthResponseDto
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            Role = userRole,
            FullName = user.FullName,
            ProfileId = profileId,
            CollegeId = collegeId,
            CompanyId = companyId
        };
    }
}