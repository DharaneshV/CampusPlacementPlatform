using CampusPlacement.BusinessLogic.Common;
using CampusPlacement.BusinessLogic.DTOs.Colleges;
using CampusPlacement.BusinessLogic.Interfaces;
using CampusPlacement.DataAccess.Context;
using CampusPlacement.Domain.Entities;
using CampusPlacement.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace CampusPlacement.BusinessLogic.Services;

public class CollegeService : ICollegeService
{
    private readonly ApplicationDbContext _context;

    public CollegeService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<CollegeResponseDto> RegisterCollegeAsync(RegisterCollegeDto dto)
    {
        if (await _context.Colleges.AnyAsync(c => c.CollegeCode == dto.CollegeCode))
            throw new ConflictException("A college with this code already exists.");

        if (await _context.Colleges.AnyAsync(c => c.OfficialEmail == dto.OfficialEmail))
            throw new ConflictException("A college with this official email is already registered.");

        var college = new College
        {
            CollegeName = dto.CollegeName,
            CollegeCode = dto.CollegeCode,
            UniversityName = dto.UniversityName,
            OfficialEmail = dto.OfficialEmail,
            PhoneNumber = dto.PhoneNumber,
            Address = dto.Address,
            Website = dto.Website,
            Status = OrganizationStatus.Pending
        };

        _context.Colleges.Add(college);
        await _context.SaveChangesAsync();

        return MapToDto(college);
    }

    public async Task<IEnumerable<CollegeResponseDto>> GetAllCollegesAsync(OrganizationStatus? status = null)
    {
        var query = _context.Colleges.AsNoTracking();

        if (status.HasValue)
            query = query.Where(c => c.Status == status.Value);

        var colleges = await query.ToListAsync();
        return colleges.Select(MapToDto);
    }

    public async Task<CollegeResponseDto> GetCollegeByIdAsync(int id)
    {
        var college = await _context.Colleges.AsNoTracking()
            .FirstOrDefaultAsync(c => c.CollegeId == id);

        if (college == null)
            throw new NotFoundException($"College with ID {id} not found.");

        return MapToDto(college);
    }

    public async Task UpdateCollegeStatusAsync(int id, OrganizationStatus status)
    {
        var college = await _context.Colleges.FindAsync(id);

        if (college == null)
            throw new NotFoundException($"College with ID {id} not found.");

        college.Status = status;
        await _context.SaveChangesAsync();
    }

    private static CollegeResponseDto MapToDto(College college)
    {
        return new CollegeResponseDto
        {
            CollegeId = college.CollegeId,
            CollegeName = college.CollegeName,
            CollegeCode = college.CollegeCode,
            UniversityName = college.UniversityName,
            OfficialEmail = college.OfficialEmail,
            PhoneNumber = college.PhoneNumber,
            Address = college.Address,
            Website = college.Website,
            Status = college.Status.ToString(),
            CreatedAt = college.CreatedAt
        };
    }
}