using Microsoft.EntityFrameworkCore;
using MIS.Data;
using MIS.Data.DTO;
using MIS.Data.Models;
using MIS.Services.Interfaces;

namespace MIS.Services
{
    public class ConsultationSevrice : IConsultationServise
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ApplicationDbContextForMIS _dbContextMKB;


        public ConsultationSevrice(ApplicationDbContext context, ApplicationDbContextForMIS dbContextMKB)
        {
            _dbContext = context;
            _dbContextMKB = dbContextMKB;
        }




        public async Task<InspectionsResponseDTO> GetConsultaltatio(Guid Id, GetPatientInspection getPatientInspection)
        {
            ValidatePatientAndParameters(Id, getPatientInspection);

            var inspectionsQuery = GetInspectionsQuery(Id, getPatientInspection);
            var inspectionIdsWithDiagnoses = await GetInspectionIdsWithDiagnoses(inspectionsQuery);
            var filteredRecords = new List<Guid>();
            if (getPatientInspection.icdRoots != null)
            {
                filteredRecords = await FilterRecords(inspectionIdsWithDiagnoses, getPatientInspection);
                if (filteredRecords.Count == 0)
                {
                    var inspection = new InspectionsResponseDTO
                    {
                        Inspections = new List<getPatientInspectionsDTO>(),
                        Pagination = new PaginationDTO
                        {
                            Size = 5,
                            Count = 0,
                            Current = 1
                        }
                    };
                    return inspection;
                }
            }
            var diagnosisModels = await GetDiagnosisModels(filteredRecords, Id, inspectionsQuery);
            foreach (var i in diagnosisModels)
            {
                Console.WriteLine($"{i.Id}");
            }
            inspectionsQuery = ApplyPagination(inspectionsQuery, getPatientInspection);
            var inspections = await GetInspections(inspectionsQuery, diagnosisModels, getPatientInspection);

            var inspectionsDTO = MapInspectionsToDTO(inspections, diagnosisModels);
            var totalCountFromDatabase = await inspectionsQuery.CountAsync();

            var inspectionsResponse = CreateInspectionsResponse(inspectionsDTO, totalCountFromDatabase, getPatientInspection);

            return inspectionsResponse;
        }

        private void ValidatePatientAndParameters(Guid Id, GetPatientInspection getPatientInspection)
        {
            var checkPatient = _dbContext.Users.FirstOrDefault(x => x.Id == Id);
            if (checkPatient == null)
            {
                var exeption = new Exception();
                exeption.Data.Add(StatusCodes.Status404NotFound.ToString(), $"Не найден врач с id {Id}");
                throw exeption;
            }

            if (getPatientInspection.page < 0 || getPatientInspection.size < 0)
            {
                var exeption = new Exception();
                exeption.Data.Add(StatusCodes.Status400BadRequest.ToString(), $"Страница или размер не могут быть отрицательными");
                throw exeption;
            }

            if (getPatientInspection.icdRoots != null)
            {
                var checkIcdRoot = _dbContextMKB.Records
                    .Count(x => getPatientInspection.icdRoots.Contains(x.ROOT));

                if (checkIcdRoot == 0)
                {
                    var exception = new Exception();
                    exception.Data.Add(StatusCodes.Status404NotFound.ToString(), "Данный ROOT не найден");
                    throw exception;
                }

            }

        }

        private IQueryable<Inspection> GetInspectionsQuery(Guid Id, GetPatientInspection getPatientInspection)
        {
            var doctorSpecialityId = _dbContext.Users.Where(u => u.Id == Id).Select(u => u.SpecialityId).FirstOrDefault();

            Console.WriteLine(doctorSpecialityId);
            IQueryable<Inspection> inspectionsQuery = _dbContext.Inspections
                .Where(i => i.Diagnoses.Any(d => d.IdDoctor == Id) &&
                            i.Consultations.Any(c => c.SpecialityId == doctorSpecialityId))
                .Include(i => i.Diagnoses)
                .Include(i => i.Consultations);

            if (getPatientInspection.grouped == true)
            {
                inspectionsQuery = inspectionsQuery.Where(i => i.PreviousInspectionId == Guid.Empty);
            }
            else
            {
                inspectionsQuery = inspectionsQuery.Where(i => i.PreviousInspectionId == Guid.Empty || i.PreviousInspectionId != Guid.Empty);
            }

            foreach (var i in inspectionsQuery)
            {
                Console.WriteLine($"{i.Id}");
            }
            return inspectionsQuery;
        }

        private async Task<List<Guid>> GetInspectionIdsWithDiagnoses(IQueryable<Inspection> inspectionsQuery)
        {
            var inspectionIdsWithDiagnoses = await _dbContext.DiagnosisModel
       .Where(diagnosis => inspectionsQuery
           .Any(i => i.Id == diagnosis.InspectionId))
       .Select(diagnosis => diagnosis.IdDiagnosis)
       .ToListAsync();
            foreach (var i in inspectionIdsWithDiagnoses)
            {
                Console.WriteLine($"{i}");
            }
            return inspectionIdsWithDiagnoses;
        }

        private async Task<List<Guid>> FilterRecords(List<Guid> inspectionIdsWithDiagnoses, GetPatientInspection getPatientInspection)
        {
            if (getPatientInspection.icdRoots != null && getPatientInspection.icdRoots.Any())
            {
                var filteredRecords = await _dbContextMKB.Records
                    .Where(record => inspectionIdsWithDiagnoses.Contains(record.ID) && getPatientInspection.icdRoots.Contains(record.ROOT))
                    .Select(record => record.ID)
                    .ToListAsync();

                foreach (var i in filteredRecords)
                {
                    Console.WriteLine($"filteredRecords: {i}");
                }

                return filteredRecords;
            }
            return inspectionIdsWithDiagnoses;
        }

        private async Task<List<DiagnosisModel>> GetDiagnosisModels(List<Guid> filteredRecords, Guid Id, IQueryable<Inspection> inspectionsQuery)
        {
            Console.WriteLine($"Id: {Id}");

            if (filteredRecords.Count == 0)
            {
                Console.WriteLine("Filtered Records: None (empty filter)");

                var inspectionIds = await inspectionsQuery.Select(i => i.Id).ToListAsync();
                var diagnosisModels = await _dbContext.DiagnosisModel
                    .Where(diagnosis => Id == diagnosis.IdDoctor && diagnosis.Type == DiagnosisType.Main && inspectionIds.Contains(diagnosis.InspectionId))
                    .ToListAsync();

                Console.WriteLine($"Count of diagnosis models: {diagnosisModels.Count}");
                return diagnosisModels;
            }

            Console.WriteLine("Filtered Records:");
            foreach (var record in filteredRecords)
            {
                Console.WriteLine(record);
            }

            var diagnosisModelsFiltered = await _dbContext.DiagnosisModel
                .Where(diagnosis => Id == diagnosis.IdDoctor && filteredRecords.Contains(diagnosis.IdDiagnosis) && diagnosis.Type == DiagnosisType.Main)
                .ToListAsync();

            Console.WriteLine($"Count of diagnosis models: {diagnosisModelsFiltered.Count}");
            return diagnosisModelsFiltered;
        }



        private IQueryable<Inspection> ApplyPagination(IQueryable<Inspection> inspectionsQuery, GetPatientInspection getPatientInspection)
        {
            if (getPatientInspection.page.HasValue && getPatientInspection.size.HasValue)
            {
                inspectionsQuery = inspectionsQuery.Skip((getPatientInspection.page.Value - 1) * getPatientInspection.size.Value)
                                               .Take(getPatientInspection.size.Value);
            }

            return inspectionsQuery;
        }

        private async Task<List<Inspection>> GetInspections(IQueryable<Inspection> inspectionsQuery, List<DiagnosisModel> diagnosisModels, GetPatientInspection getPatientInspection)
        {
            List<Inspection> inspections = new List<Inspection>();
            if (getPatientInspection.page == null)
            {
                getPatientInspection.page = 1;
            }
            if (getPatientInspection.size == null)
            {
                getPatientInspection.size = 5;
            }

            if (getPatientInspection.page.HasValue && getPatientInspection.size.HasValue)
            {
                var skipAmount = (getPatientInspection.page.Value - 1) * getPatientInspection.size.Value;
                foreach (var i in diagnosisModels.Skip(skipAmount).Take(getPatientInspection.size.Value))
                {
                    var inspection = await _dbContext.Inspections.FirstOrDefaultAsync(x => x.Id == i.InspectionId);
                    if (inspection != null)
                    {
                        inspections.Add(inspection);
                    }
                }
            }

            return inspections;
        }


        private List<getPatientInspectionsDTO> MapInspectionsToDTO(List<Inspection> inspections, List<DiagnosisModel> diagnosisModels)
        {
            var inspectionsDTO = inspections.Select
            (inspection => new getPatientInspectionsDTO
            {
                Id = inspection.Id,
                createTime = inspection.CreateTime,
                previousId = inspection.PreviousInspectionId ?? Guid.Empty,
                date = inspection.Date,
                conclusion = (Conclusion)inspection.Conclusion,
                doctorId = inspection.DoctorId,
                doctor = _dbContext.Users.FirstOrDefault(u => u.Id == inspection.DoctorId)?.FullName,
                patientId = inspection.PatientId,
                patient = _dbContext.patientModels.FirstOrDefault(p => p.Id == inspection.PatientId)?.Name,
                diagnosis = diagnosisModels.FirstOrDefault(d => d.InspectionId == inspection.Id),
                hasNasted = inspection.hasNested,
                hasChain = inspection.hasChain
            }).ToList();

            return inspectionsDTO;
        }


        private InspectionsResponseDTO CreateInspectionsResponse(List<getPatientInspectionsDTO> inspectionsDTO, int totalCountFromDatabase, GetPatientInspection getPatientInspection)

        {
            var inspectionsResponse = new InspectionsResponseDTO
            {
                Inspections = inspectionsDTO.Select(inspection => new getPatientInspectionsDTO
                {
                    Id = inspection.Id,
                    createTime = inspection.createTime,
                    previousId = inspection.previousId,
                    date = inspection.date,
                    conclusion = inspection.conclusion,
                    doctorId = inspection.doctorId,
                    doctor = inspection.doctor,
                    patientId = inspection.patientId,
                    patient = inspection.patient,
                    diagnosis = inspection.diagnosis != null ? new DiagnosisModel
                    {
                        Id = inspection.diagnosis.Id,
                        CreateTime = inspection.diagnosis.CreateTime,
                        Code = inspection.diagnosis.Code,
                        Name = inspection.diagnosis.Name,
                        Description = inspection.diagnosis.Description,
                        Type = inspection.diagnosis.Type,
                        IdDiagnosis = inspection.diagnosis.IdDiagnosis,
                        InspectionId = inspection.diagnosis.InspectionId,
                        IdPatient = inspection.patientId,
                        IdDoctor = inspection.doctorId
                    } : null,
                    hasChain = inspection.hasChain,
                    hasNasted = inspection.hasNasted
                }).ToList(),
                Pagination = new PaginationDTO
                {
                    Size = inspectionsDTO.Count,
                    Count = totalCountFromDatabase,
                    Current = getPatientInspection.page ?? 1
                }
            };

            return inspectionsResponse;
        }
        public async Task<ConsultationForGetIdDTO> GetConsultation(Guid consultationId)
        {
            var consultation = await _dbContext.ConsultationModels
                .FirstOrDefaultAsync(c => c.Id == consultationId);

            if (consultation == null)
            {
                throw new KeyNotFoundException($"Консультация с ID '{consultationId}' не найдена.");
            }

            var speciality = await _dbContext.Specialiti
                .FirstOrDefaultAsync(s => s.Id == consultation.SpecialityId);

            var comments = new List<CommentGetIdDTO>();
            if (consultation.CommentId.HasValue)
            {
                var commentEntities = await _dbContext.InspectionCommentModel
                    .Where(comment => comment.Id == consultation.CommentId.Value)
                    .ToListAsync();

                comments = commentEntities.Select(comment => new CommentGetIdDTO
                {
                    Id = comment.Id,
                    CreateTime = comment.CreateTime,
                    ModifiedDate = comment.ModifyTime,
                    Content = comment.Content,
                    AuthorId = comment.AuthorID,
                    Author = comment.Author,
                    ParentId = comment.Parent
                }).ToList();
            }

            return new ConsultationForGetIdDTO
            {
                Id = consultation.Id,
                CreateTime = consultation.CreateTime,
                InspectionId = consultation.InspectionId ?? Guid.Empty,
                Speciality = speciality != null ? new SpecialityDTO
                {
                    Id = speciality.Id,
                    Name = speciality.Name
                } : null,
                Comments = comments
            };
        }


        public async Task<Guid> AddComment(Guid id, NewCommentDTO newCommentDTO, Guid IdDoctor)
        {
            var checkOnParentId = new InspectionCommentModel();
            var checkConsultationId = _dbContext.ConsultationModels.FirstOrDefault(x => x.Id == id);
            var checkDoctor = _dbContext.Users.FirstOrDefault(x => x.Id == IdDoctor);

            if (checkDoctor == null)
            {
                var exception = new Exception();
                exception.Data.Add(StatusCodes.Status404NotFound.ToString(), $"Доктор с {IdDoctor} не найдена.");
                throw exception;
            }
            if (checkConsultationId == null)
            {

                var exception = new Exception();
                exception.Data.Add(StatusCodes.Status404NotFound.ToString(), $"Косультация с {id} не найдена.");
                throw exception;

            }
            else
            {
                checkOnParentId = _dbContext.InspectionCommentModel.FirstOrDefault(x => x.Id == newCommentDTO.ParentId && checkConsultationId.Id == x.IdConsultation);
                if (checkOnParentId == null)
                {
                    var ex = new Exception();
                    ex.Data.Add(StatusCodes.Status404NotFound.ToString(), $"Данный коментарый с id: {newCommentDTO.ParentId} не найден.");
                    throw ex;
                }
            }
            var searchInspection = _dbContext.Inspections.FirstOrDefault(x => x.Id == checkConsultationId.InspectionId);
            if (searchInspection != null && searchInspection.DoctorId != IdDoctor)
            {
                var exception = new Exception();
                exception.Data.Add(StatusCodes.Status400BadRequest.ToString(), $"Вы не являетесь создателем консультации.");
                throw exception;
            }
            if (checkDoctor.SpecialityId != checkConsultationId.SpecialityId)
            {
                var exception = new Exception();
                exception.Data.Add(StatusCodes.Status400BadRequest.ToString(), $"Вы не являетесь специалистом в данной области.");
                throw exception;
            }
            var user = _dbContext.Users.FirstOrDefault(x => x.Id == IdDoctor);
            var NewId = Guid.NewGuid();
            var newComment = new InspectionCommentModel()
            {
                Author = user.FullName,
                AuthorID = IdDoctor,
                Content = newCommentDTO.content,
                CreateTime = DateTime.UtcNow,
                Id = NewId,
                ModifyTime = null,
                Parent = newCommentDTO.ParentId,
                IdConsultation = checkConsultationId.Id
            };
            _dbContext.Add(newComment);
            _dbContext.SaveChanges();
            return NewId;
        }

        public async Task UpdateComment(Guid IdComment, CommentDTO newComment, Guid idDoctor)
        {
            var searchComment = _dbContext.InspectionCommentModel.FirstOrDefault(x => x.Id == IdComment);
            var searchDoctor = _dbContext.Users.FirstOrDefault(x => x.Id == idDoctor);

            if (searchComment == null)
            {
                var ex = new Exception();
                ex.Data.Add(StatusCodes.Status404NotFound.ToString(), $"Данный коментарый с id: {IdComment} не найден.");
                throw ex;
            }
            if (searchDoctor == null)
            {
                var ex = new Exception();
                ex.Data.Add(StatusCodes.Status404NotFound.ToString(), $"Данный доктор с id: {idDoctor} не найден.");
                throw ex;
            }
            if (searchComment != null && searchDoctor != null)
            {
                if (searchComment.AuthorID != idDoctor)
                {
                    var ex = new Exception();
                    ex.Data.Add(StatusCodes.Status404NotFound.ToString(), $"Вы не являетесь создателем комментария с id: {idDoctor}");
                    throw ex;
                }
            }
            searchComment.Content = newComment.content;
            searchComment.ModifyTime = DateTime.Now; 
            _dbContext.SaveChanges();
        }

    }
}
