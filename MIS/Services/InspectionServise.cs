using MIS.Services.Interfaces;
using MIS.Data.Models;
using MIS.Data;
using Microsoft.EntityFrameworkCore;
using MIS.Data.DTO;
using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace MIS.Services
{
    public class InspectionServise : IInspectionServise
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ApplicationDbContextForMIS _dbContextMIS;

        public InspectionServise(ApplicationDbContext dbContext, ApplicationDbContextForMIS dbContextMIS)
        {
            _dbContext = dbContext;
            _dbContextMIS = dbContextMIS;
        }

        public async Task<FullInfoAboutInspectionDTO> GetFullInspectionById(Guid id)
        {
            var inspection = await _dbContext.Inspections
      .Include(i => i.Diagnoses)
      .Include(i => i.Consultations)
      .AsSplitQuery()
      .FirstOrDefaultAsync(i => i.Id == id);
            Console.WriteLine($"{inspection.Id}");


            if (inspection == null)
            {
                throw new KeyNotFoundException("Осмотр не найден.");
            }

            var patient = await _dbContext.patientModels.FirstOrDefaultAsync(p => p.Id == inspection.PatientId);
            if (patient == null)
            {
                throw new KeyNotFoundException("Пациент не найден.");
            }

            var doctor = await _dbContext.Users.FirstOrDefaultAsync(d => d.Id == inspection.DoctorId);
            if (doctor == null)
            {
                throw new KeyNotFoundException("Врач не найден.");
            }
            var consultation = _dbContext.ConsultationModels.FirstOrDefault(c => c.InspectionId == id);
            int countComment = 0;
            if (consultation != null)
            {
                countComment = _dbContext.InspectionCommentModel.Count(x => x.IdConsultation == consultation.Id);
            }
            var inspectionDto = new FullInfoAboutInspectionDTO
            {

                Id = inspection.Id,
                CreateTime = inspection.CreateTime,
                Date = inspection.Date,
                Anamnesis = inspection.Anamnesis,
                Complaints = inspection.Complaints,
                Treatment = inspection.Treatment,
                Conclusion = (Conclusion)inspection.Conclusion,
                NextVisitDate = inspection.NextVisitDate,
                DeathDate = inspection.DeathDate,
                BaseInspectionId = inspection.BaseInspectionId,
                PreviousInspectionId = inspection.PreviousInspectionId,
                Patient = new PatientDTOFull
                {
                    Name = patient.Name,
                    Birthday = patient.BirthDate,
                    Gender = patient.Gender,
                    Id = patient.Id,
                    CreateTime = patient.CreateTime
                },
                Doctor = new DoctorDTOFull
                {
                    Name = doctor.FullName,
                    Birthday = (DateTime)doctor.BirthDate,
                    Gender = doctor.Gender,
                    Email = doctor.Email,
                    Phone = doctor.PhoneNumber,
                    Id = doctor.Id,
                    CreateTime = doctor.createTime
                },

                Diagnoses = inspection.Diagnoses.Select(d => new DiagnosisDTOFull
                {
                    Id = d.Id,
                    CreateTime = d.CreateTime,
                    Code = d.Code,
                    Name = d.Name,
                    Description = d.Description,
                    Type = d.Type,
                    InspectionId = d.InspectionId

                }).ToList(),

                Consultations = (await Task.WhenAll(inspection.Consultations.Select(async c =>
               {
                   try
                   {
                       InspectionCommentModel comment = null;
                       AuthorDTO authorDto = null;
                       SpecialityDTO speciality = null;

                       if (c.CommentId.HasValue)
                       {
                           comment = await _dbContext.InspectionCommentModel.FirstOrDefaultAsync(cm => cm.Id == c.CommentId.Value);
                           if (comment != null)
                           {
                               Console.WriteLine($"Найден комментарий: {comment.Content}");


                               var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == comment.AuthorID);
                               if (user != null)
                               {
                                   authorDto = new AuthorDTO
                                   {
                                       Name = user.FullName,
                                       Birthday = (DateTime)user.BirthDate,
                                       Gender = user.Gender,
                                       Email = user.Email,
                                       Phone = user.PhoneNumber,
                                       Id = user.Id,
                                       CreateTime = user.createTime
                                   };
                                   var specialityCheck = _dbContext.Specialiti.FirstOrDefault(x => x.Id == user.SpecialityId);
                                   speciality = new SpecialityDTO
                                   {
                                       Id = specialityCheck.Id,
                                       CreateTime = specialityCheck.CreateTime,
                                       Name = specialityCheck.Name
                                   };
                                   Console.WriteLine($"Найден пользователь: {user.FullName}");
                               }

                           }
                       }

                       return new ConsultationDTOFull
                       {
                           Id = c.Id,
                           CreateTime = c.CreateTime,
                           InspectionId = c.InspectionId.Value,
                           SpecialityId = c.SpecialityId,
                           Speciality = speciality,
                           CommentId = c.CommentId,
                           CommentsNumber = countComment,
                           RootComment = comment != null ? new RootCommentDTO
                           {
                               Id = comment.Id,
                               Content = comment.Content,
                               Author = authorDto,
                               ModifyTime = comment.ModifyTime ?? default(DateTime),
                               CreateTime = comment.CreateTime,
                               ParentId = comment.Parent
                           } : null
                       };
                   }
                   catch (Exception ex)
                   {
                       Console.WriteLine($"Ошибка в обработке консультации: {ex.Message}");
                       throw;
                   }
               }))).ToList()
            };

            return inspectionDto;
        }


        public async Task UpdateInspection(Guid id, UpdateInspectionDTO updateInspectionDTO, Guid userId)
        {
            var inspection = await _dbContext.Inspections
                .Include(i => i.Diagnoses)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (inspection == null)
            {
                throw new KeyNotFoundException("Осмотр не найден.");
            }
            if (updateInspectionDTO.NextVisitDate != null && updateInspectionDTO.NextVisitDate < DateTime.UtcNow)
            {
                var exception = new Exception();
                exception.Data.Add(StatusCodes.Status400BadRequest.ToString(), "Date and time of the next visit can't be earlier than now");
                throw exception;
            }
            foreach (var i in updateInspectionDTO.Diagnoses)
            {
                var icd = _dbContextMIS.Records.FirstOrDefault(x => x.ID == i.IcdDiagnosisId);
                if (icd == null)
                {
                    var exception = new Exception();
                    exception.Data.Add(StatusCodes.Status404NotFound.ToString(), $"Данный диагноз: {i.IcdDiagnosisId} не найден.");
                    throw exception;
                }
            }

            if (updateInspectionDTO.Diagnoses != null && updateInspectionDTO.Diagnoses.Where(x => x.Type == DiagnosisType.Main).Count() != 1)
            {
                var exception = new Exception();
                exception.Data.Add(StatusCodes.Status400BadRequest.ToString(), "Осмотр обязательно должен иметь один диагноз с типом диагноза - Main");
                throw exception;
            }

            inspection.Anamnesis = updateInspectionDTO.Anamnesis;
            inspection.Complaints = updateInspectionDTO.Complaints;
            inspection.Treatment = updateInspectionDTO.Treatment;
            inspection.Conclusion = updateInspectionDTO.Conclusion;
            inspection.NextVisitDate = updateInspectionDTO.NextVisitDate;
            inspection.DeathDate = updateInspectionDTO.DeathDate;


            foreach (var diagUpdate in updateInspectionDTO.Diagnoses)
            {
                var diagnosis = inspection.Diagnoses.FirstOrDefault(d => d.InspectionId == inspection.Id);
                if (diagnosis != null)
                {
                    diagnosis.IdDiagnosis = diagUpdate.IcdDiagnosisId;
                    diagnosis.Description = diagUpdate.Description;
                    diagnosis.Type = diagUpdate.Type;
                }
                else
                {
                    var exception = new Exception();
                    exception.Data.Add(StatusCodes.Status404NotFound.ToString(), "Диагноз не найден");
                    throw exception;
                }

            }

            await _dbContext.SaveChangesAsync();
        }


        public async Task<List<ChainDTO>> GetInspectionChain(Guid inspectionId)
        {
            var rootInspection = await _dbContext.Inspections
                .FirstOrDefaultAsync(x => x.Id == inspectionId && x.PreviousInspectionId == Guid.Empty);

            if (rootInspection == null)
            {
                var ex = new Exception();
                ex.Data.Add(StatusCodes.Status404NotFound.ToString(), $"Inspection с Id: {inspectionId} не найдена.");
                throw ex;
            }


            if (rootInspection.hasChain == true && rootInspection.hasNested == true)
            {

            }
            else
            {
                var ex = new Exception();
                ex.Data.Add(StatusCodes.Status400BadRequest.ToString(), $"Inspection с Id: {inspectionId} не является root.");
                throw ex;
            }

            var notRoot = _dbContext.Inspections.FirstOrDefault(x => x.PreviousInspectionId == rootInspection.Id);
            var chain = new List<ChainDTO> { ConvertToChainDTO(notRoot) };



            Guid? nextId = rootInspection.Id;
            while (nextId != null)
            {
                var nextInspection = await _dbContext.Inspections
                    .FirstOrDefaultAsync(x => x.PreviousInspectionId == nextId);
                if (nextInspection != null)
                {
                    chain.Add(ConvertToChainDTO(nextInspection));
                    nextId = nextInspection.Id;
                }
                else
                {
                    nextId = null;
                }
            }

            return chain;
        }

        private ChainDTO ConvertToChainDTO(Inspection inspection)
        {

            var patient = _dbContext.patientModels.FirstOrDefault(x => x.Id == inspection.PatientId);

            var doctor = _dbContext.Users.FirstOrDefault(x => x.Id == inspection.DoctorId);


            var diagnosisAll = _dbContext.DiagnosisModel
                .Where(x => x.InspectionId == inspection.Id).ToList();

            var diagnosisList = diagnosisAll.Select(i => new DiagnosisForDTO
            {
                Id = i.Id,
                CreateTime = i.CreateTime,
                Code = i.Code,
                Name = i.Name,
                Description = i.Description,
                Type = i.Type
            }).ToList();


            return new ChainDTO
            {
                Id = inspection.Id,
                Date = inspection.Date,
                CreateTime = inspection.CreateTime,
                PreviousId = inspection.PreviousInspectionId,
                Conclusion = (Conclusion)inspection.Conclusion,
                DoctorId = inspection.DoctorId,
                Doctor = doctor != null ? doctor.FullName : "Неизвестный врач",
                PatientId = inspection.PatientId,
                Patient = patient != null ? patient.Name : "Неизвестный пациент",
                Diagnosis = diagnosisList,
                HasChain = inspection.hasChain,
                HasNested = inspection.hasNested
            };
        }


    }
}

