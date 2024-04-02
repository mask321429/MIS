using AutoMapper;
using MIS.Data;
using MIS.Data.Models;
using MIS.Services.Interfaces;
using MIS.Data.DTO;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Linq;

public class SpecialityService : ISpeciality
{
    private ApplicationDbContext _dbContext;
    private ApplicationDbContextForMIS _dbContextMIS;
    public SpecialityService(ApplicationDbContext dbContext, ApplicationDbContextForMIS dbContextMIS)
    {
        _dbContext = dbContext;
        _dbContextMIS = dbContextMIS;
    }

    public async Task<SpecialitiesResponseDTO> SpecialityGet(SpecialityGetDTO specialityGetDTO)
    {
        const int maxPageSize = 15038;
        if (specialityGetDTO.Page == null)
        {
            specialityGetDTO.Page = 1;
        }
        if (specialityGetDTO.Size == null)
        {
            specialityGetDTO.Size = 5;
        }

        if (specialityGetDTO.Page <= 0 || specialityGetDTO.Size <= 0)
        {
            throw new BadHttpRequestException("Page and Size should be greater than zero.");
        }

        var query = _dbContext.Specialiti.AsQueryable();

        if (!string.IsNullOrEmpty(specialityGetDTO.Name))
        {
            query = query.Where(s => s.Name.ToLower().Contains(specialityGetDTO.Name.ToLower()));
        }

        var totalCount = await query.CountAsync();

        if (specialityGetDTO.Size > maxPageSize || specialityGetDTO.Page > (totalCount + specialityGetDTO.Size - 1) / specialityGetDTO.Size)
        {
            throw new BadHttpRequestException("Invalid Page or Size value.");
        }

        var specialties = await query
          .Skip((int)((specialityGetDTO.Page - 1) * specialityGetDTO.Size))
        .Take((int)specialityGetDTO.Size)
        .ToListAsync();

        var specialtiesDTO = specialties.Select(s => new SpecialityDTO
        {
            Id = s.Id,
            Name = s.Name,
            CreateTime = s.CreateTime
        }).ToList();

        var pagination = new PaginationDTO
        {
            Size = specialityGetDTO.Size,
            Count = totalCount,
            Current = specialityGetDTO.Page
        };

        return new SpecialitiesResponseDTO
        {
            Specialties = specialtiesDTO,
            Pagination = pagination
        };
        
    }

    public async Task<SpecialitiesNameAndIdDTO> SpecialityGetNameAndId(SpecialityGetDTO specialityGetDTO)
    {
        const int maxPageSize = 15038;

        if (specialityGetDTO.Page <= 0 || specialityGetDTO.Size <= 0)
        {
            throw new BadHttpRequestException("Page and Size should be greater than zero.");
        }

        if (specialityGetDTO.Size > maxPageSize || specialityGetDTO.Page > int.MaxValue / maxPageSize)
        {
            throw new BadHttpRequestException("Invalid Page or Size value.");
        }

        var query = _dbContextMIS.Records.AsQueryable();

        if (!string.IsNullOrEmpty(specialityGetDTO.Name))
        {
            query = query.Where(r =>
                r.MKB_NAME.ToLower().Contains(specialityGetDTO.Name.ToLower()) ||
                r.MKB_CODE.ToLower().Contains(specialityGetDTO.Name.ToLower())
            );
        }

        var totalCount = await query.Where(r => r.ID != null).CountAsync();

        var totalPages = (int)Math.Ceiling((decimal)((double)totalCount / (specialityGetDTO.Size ?? 5)));

        if (specialityGetDTO.Page > totalPages)
        {
            throw new BadHttpRequestException("Page value exceeds total pages.");
        }

        var specialties = await query
            .Where(r => r.ID != null)
            .Skip((int)(((specialityGetDTO.Page ?? 1) - 1) * (specialityGetDTO.Size ?? 5)))
            .Take((int)(specialityGetDTO.Size ?? 5))
            .ToListAsync();
        var specialtiesDTO = specialties.Select(s => new SpecialityWithCodeDTO
        {
            Code = s.MKB_CODE,
            Id = s.ID != null ? s.ID : default,
            Name = s.MKB_NAME,
            CreateTime = DateTime.UtcNow
        }).ToList();



        var pagination = new PaginationDTO
        {
            Size = specialityGetDTO.Size,
            Count = totalCount,
            Current = specialityGetDTO.Page
        };

        return new SpecialitiesNameAndIdDTO
        {
            Specialties = specialtiesDTO,
            Pagination = pagination
        };
    }

    public async Task<List<SpecialityWithCodeDTO>> GetRoots()
    {
        var validCodes = new HashSet<string>();

        for (int i = 1; i <= 24; i++)
        {
            validCodes.Add(i.ToString("00"));
        }

        var entities = await _dbContextMIS.Records
            .Where(entity => validCodes.Contains(entity.REC_CODE) && entity.REC_CODE.Length <= 3)
            .ToListAsync();

        var result = entities
            .Select(s => new SpecialityWithCodeDTO
            {
                Id = s.ID,
                Code = s.MKB_CODE,
                Name = s.MKB_NAME,
                CreateTime = DateTime.UtcNow
            })
            .ToList();

        return result;
    }









}

// 22