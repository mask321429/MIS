using AutoMapper;
using MIS.Data;
using MIS.Data.Models;
using MIS.Services.Interfaces;
using MIS.Data.DTO;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Drawing;
using System.Linq;

namespace MIS.Services
{

    public class PatientService : IPatientService
    {

        private ApplicationDbContext _dbContext;
        private ApplicationDbContextForMIS _dbContextMKB;
        public PatientService(ApplicationDbContext dbContext, IMapper mapper, ApplicationDbContextForMIS dbContextMKB)
        {
            _dbContext = dbContext;
            _dbContextMKB = dbContextMKB;

        }
        public async Task<Guid> Register(PatientDTO userRegistrationDTO)
        {
            ValidateUserData(userRegistrationDTO);


            var newUser = new PatientModel
            {
                Id = Guid.NewGuid(),
                Name = userRegistrationDTO.Name,
                BirthDate = userRegistrationDTO.birthday,
                Gender = userRegistrationDTO.gender,
                CreateTime = DateTime.UtcNow
            };



            await _dbContext.patientModels.AddAsync(newUser);
            await _dbContext.SaveChangesAsync();

            return newUser.Id;
        }


        public async Task<Guid> CreateInspection(CreateInspectionDTO createInspectionDTO, Guid id, Guid IdDoctor)
        {
            var GuidIdForInspection = Guid.NewGuid();
            var diagnosesList = new List<DiagnosisModel>();
            var consultationsList = new List<ConsultationModel>();
            var CheckIdPreviusInspection = _dbContext.Inspections
     .Where(x => x.PreviousInspectionId == createInspectionDTO.CreateInspectionInfo.PreviousInspectionId && x.PreviousInspectionId != Guid.Empty)
     .Count();

            var CheckIdPreviusOnChainRoot = _dbContext.Inspections
    .Where(x => x.Id == createInspectionDTO.CreateInspectionInfo.PreviousInspectionId && x.PreviousInspectionId == Guid.Empty).FirstOrDefault();
            if (CheckIdPreviusOnChainRoot != null)
            {
                CheckIdPreviusOnChainRoot.hasChain = true;
            }

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Ошибка при сохранении CheckIdPreviusOnChainRoot в базу данных: " + ex.Message);
                throw;
            }
            var CheckIdPreviusInspection2 = _dbContext.Inspections
   .Where(x => x.Id == createInspectionDTO.CreateInspectionInfo.PreviousInspectionId).FirstOrDefault();
            if (CheckIdPreviusInspection2 != null)
            {
                CheckIdPreviusInspection2.hasNested = true;

            }
            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Ошибка при сохранении CheckIdPreviusInspection2 в базу данных: " + ex.Message);
                throw;
            }
            if (createInspectionDTO.CreateInspectionInfo.Date > DateTime.UtcNow)
            {
                var exception = new Exception();
                exception.Data.Add(StatusCodes.Status400BadRequest.ToString(), "Date and time can't be later than now ");
                throw exception;
            }

            if (createInspectionDTO.CreateInspectionInfo.NextVisitDate != null && createInspectionDTO.CreateInspectionInfo.NextVisitDate < DateTime.UtcNow)
            {
                var exception = new Exception();
                exception.Data.Add(StatusCodes.Status400BadRequest.ToString(), "Date and time of the next visit can't be earlier than now");
                throw exception;
            }
            var patientDeathCheck = _dbContext.Inspections
        .FirstOrDefault(x => x.Id == createInspectionDTO.CreateInspectionInfo.PreviousInspectionId && x.DeathDate != null && x.PatientId == id);

            if (patientDeathCheck != null)
            {
                var exception = new Exception();
                exception.Data.Add(StatusCodes.Status400BadRequest.ToString(), "Пациент умер");
                throw exception;
            }
            var baseId = _dbContext.Inspections.FirstOrDefault(x => x.Id == createInspectionDTO.CreateInspectionInfo.PreviousInspectionId && x.PatientId == id);
            if (baseId == null && createInspectionDTO.CreateInspectionInfo.PreviousInspectionId != Guid.Empty)
            {
                var exception = new Exception();
                exception.Data.Add(StatusCodes.Status404NotFound.ToString(), "Осмотр не найден");
                throw exception;
            }

            if (createInspectionDTO.CreateInspectionInfo.NextVisitDate != null && createInspectionDTO.CreateInspectionInfo.DeathDate != null)
            {
                var exception = new Exception();
                exception.Data.Add(StatusCodes.Status400BadRequest.ToString(), "Пациент умер, он не придет. Удали дату сделующего осмотра.");
                throw exception;
            }
            var checkTime = _dbContext.Inspections.Where(x => x.Id == createInspectionDTO.CreateInspectionInfo.PreviousInspectionId && x.PatientId == id && x.NextVisitDate > createInspectionDTO.CreateInspectionInfo.NextVisitDate).FirstOrDefault();
            if (checkTime != null)
            {
                var exception = new Exception();
                exception.Data.Add(StatusCodes.Status400BadRequest.ToString(), "Inspection date and time can't be earlier than date and time of previous inspection");
                throw exception;
            }

            if (createInspectionDTO.CreateInspectionInfo.Consultations != null && createInspectionDTO.CreateInspectionInfo.Consultations.Select(x => x.SpecialityId).Distinct().Count() != createInspectionDTO.CreateInspectionInfo.Consultations.Count)
            {
                var exception = new Exception();
                exception.Data.Add(StatusCodes.Status400BadRequest.ToString(), "Осмотр не может иметь несколько консультаций с одинаковой специальностью врача");
                throw exception;
            }
            if (createInspectionDTO.CreateInspectionInfo.Diagnoses != null && createInspectionDTO.CreateInspectionInfo.Diagnoses.Where(x => x.Type == DiagnosisType.Main).Count() != 1)
            {
                var exception = new Exception();
                exception.Data.Add(StatusCodes.Status400BadRequest.ToString(), "Осмотр обязательно должен иметь один диагноз с типом диагноза - Main");
                throw exception;
            }

            var checkOnConclusion = _dbContext.Inspections
     .FirstOrDefault(x => x.Id == createInspectionDTO.CreateInspectionInfo.PreviousInspectionId &&
                          x.PatientId == id &&
                          x.Conclusion == Conclusion.Recovery);
            if (checkOnConclusion != null)
            {
                var exception = new Exception();
                exception.Data.Add(StatusCodes.Status400BadRequest.ToString(), "Пациент уже здоров");
                throw exception;
            }
            var newInspection = new Inspection { };

            if (CheckIdPreviusInspection < 1)
            {
                newInspection = new Inspection
                {
                    Id = GuidIdForInspection,
                    CreateTime = DateTime.UtcNow,
                    Date = createInspectionDTO.CreateInspectionInfo.Date,
                    Anamnesis = createInspectionDTO.CreateInspectionInfo.Anamnesis,
                    Complaints = createInspectionDTO.CreateInspectionInfo.Complaints,
                    Treatment = createInspectionDTO.CreateInspectionInfo.Treatment,
                    Conclusion = createInspectionDTO.CreateInspectionInfo.Conclusion,
                    NextVisitDate = createInspectionDTO.CreateInspectionInfo.NextVisitDate,
                    DeathDate = createInspectionDTO.CreateInspectionInfo.DeathDate,
                    BaseInspectionId = createInspectionDTO.CreateInspectionInfo.PreviousInspectionId != Guid.Empty
                            ? baseId.BaseInspectionId
                            : GuidIdForInspection,
                    PreviousInspectionId = createInspectionDTO.CreateInspectionInfo.PreviousInspectionId,
                    Diagnoses = diagnosesList,
                    Consultations = consultationsList,
                    PatientId = id,
                    DoctorId = IdDoctor,
                    hasNested = false,
                    hasChain = false

                };
            }
            else
            {
                var exception = new Exception();
                exception.Data.Add(StatusCodes.Status400BadRequest.ToString(), "У этого осмотра уже есть ребенок");
                throw exception;
            }

            _dbContext.Inspections.Add(newInspection);

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Ошибка при сохранении в базу данных: " + ex.Message);
                throw;
            }

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Ошибка при сохранении в базу данных: " + ex.Message);
                throw;
            }

            var IdConsultation = Guid.NewGuid();
            foreach (var diagnosisDto in createInspectionDTO.CreateInspectionInfo.Diagnoses)
            {

                var diagnoseCode = await _dbContextMKB.Records.FirstOrDefaultAsync(x =>
                    x.ID == diagnosisDto.IcdDiagnosisId);

                if (diagnoseCode != null)
                {

                    var diagnosis = new DiagnosisModel
                    {
                        Id = Guid.NewGuid(),
                        CreateTime = DateTime.UtcNow,
                        Code = diagnoseCode.MKB_CODE,
                        Name = diagnoseCode.MKB_NAME,
                        Description = diagnosisDto.Description,
                        Type = diagnosisDto.Type,
                        InspectionId = GuidIdForInspection,
                        IdDiagnosis = diagnoseCode.ID,
                        IdPatient = id,
                        IdDoctor = IdDoctor
                    };
                    Console.WriteLine("Диагноз добавлен");
                    diagnosesList.Add(diagnosis);
                }
                else
                {
                    var exception = new Exception();
                    exception.Data.Add(StatusCodes.Status404NotFound.ToString(), "DiagnosisId not found");
                    throw exception;
                }

            }


            _dbContext.DiagnosisModel.AddRange(diagnosesList);




            var author = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == IdDoctor);


            var commentId = Guid.NewGuid();

            var commentsList = createInspectionDTO.CreateInspectionInfo.Consultations
                .Select(commentDto => new InspectionCommentModel
                {
                    Id = commentId,
                    CreateTime = DateTime.UtcNow,
                    Parent = null,
                    Content = commentDto.Comment.content,
                    Author = author?.FullName,
                    AuthorID = IdDoctor,
                    ModifyTime = null,
                    IdConsultation = IdConsultation,
                }).ToList();
            _dbContext.InspectionCommentModel.AddRange(commentsList);

            var chekPatient = _dbContext.patientModels.FirstOrDefault(x => x.Id == id);

            if (chekPatient == null)
            {
                var exception = new Exception();
                exception.Data.Add(StatusCodes.Status404NotFound.ToString(), "Пользователь ненайден");
                throw exception;
            }

            if (createInspectionDTO.CreateInspectionInfo.Consultations != null)
            {
                foreach (var consultationDto in createInspectionDTO.CreateInspectionInfo.Consultations)
                {
                    if (consultationDto.SpecialityId != null)
                    {
                        var checkspecialityId = _dbContext.Specialiti
                            .Where(x => x.Id == consultationDto.SpecialityId)
                            .FirstOrDefault();

                        if (checkspecialityId != null)
                        {
                            var consultation = new ConsultationModel
                            {
                                Id = IdConsultation,
                                CreateTime = DateTime.UtcNow,
                                InspectionId = GuidIdForInspection,
                                SpecialityId = author?.SpecialityId,
                                CommentId = commentId
                            };

                            consultationsList.Add(consultation);
                        }
                        else
                        {
                            var exception = new Exception();
                            exception.Data.Add(StatusCodes.Status404NotFound.ToString(), "specialityId not found");
                            throw exception;
                        }
                        if (consultationsList.Any())
                        {
                            _dbContext.ConsultationModels.AddRange(consultationsList);
                        }
                    }

                }
            }

            var inspectionPatient = new InspectionPatient
            {
                id = Guid.NewGuid(),
                idPatient = id,
                idInspection = GuidIdForInspection
            };
            _dbContext.InspectionPatient.AddRange(inspectionPatient);

            var inspectionDoctor = new InspectionDoctor
            {
                id = Guid.NewGuid(),
                idDoctor = IdDoctor,
                idInspection = GuidIdForInspection
            };
            _dbContext.InspectionDoctor.AddRange(inspectionDoctor);


            foreach (var diagnosis in diagnosesList)
            {
                var patientDiagnosis = new PatientDiagnosis
                {
                    id = Guid.NewGuid(),
                    idPatient = id,
                    idDiagnosis = diagnosis.Id
                };
                _dbContext.PatientDiagnoses.AddRange(patientDiagnosis);
            }


            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {

                Debug.WriteLine("Ошибка при сохранении в базу данных: " + ex.Message);
                throw;
            }
            return GuidIdForInspection;
        }


        public async Task<ReturnDTOGetPatient> GetPatient(GetPatientDTO parameters, Guid id)
        {
            IQueryable<PatientModel> query = _dbContext.patientModels.AsQueryable();


            if (!string.IsNullOrEmpty(parameters.Name))
            {
                query = query.Where(patient => patient.Name.ToLower().Contains(parameters.Name.ToLower()));
            }
            if (parameters.Conclusions != null)
            {
                var selectedConclusions = parameters.Conclusions;

                var patientsWithInspections = query
                    .Join(
                        _dbContext.Inspections,
                        patient => patient.Id,
                        inspection => inspection.PatientId,
                        (patient, inspection) => inspection
                    )
                    .ToList();


                var filteredPatientIds = patientsWithInspections
       .Where(insp => selectedConclusions.Contains((Conclusion)insp.Conclusion))
       .Select(insp => insp.PatientId)
       .Distinct()
       .ToList();

                query = query.Where(patient => filteredPatientIds.Contains(patient.Id));
            }

            /*  foreach (var i in selectedConclusions)
              {
                  var patientIds = patientsWithInspections.Where(x => x.Patient)
              }

          }*/
            /* var selectedConclusions = parameters.Conclusions.Value;

             var patientIds = _dbContext.Inspections
                 .Where(insp => (selectedConclusions & insp.Conclusion) == insp.Conclusion)
                 .Select(insp => insp.PatientId)
                 .Distinct();

             query = query.Where(patient => patientIds.Contains(patient.Id));*/




            if (parameters.ScheduledVisits == null)
            {
                parameters.ScheduledVisits = false;
            }

            if (parameters.ScheduledVisits == true)
            {
                var currentTime = DateTime.UtcNow;
                var scheduledVisits = _dbContext.Inspections
                    .Where(x => x.NextVisitDate > currentTime && x.hasNested == false)
                    .Select(x => x.PatientId)
                    .ToList();
                query = query.Where(patient => scheduledVisits.Contains(patient.Id));
            }

            if (parameters.OnlyMine == null)
            {
                parameters.ScheduledVisits = false;
            }
            if (parameters.OnlyMine == true)
            {
                var checkMy = _dbContext.Inspections
                    .Where(x => x.DoctorId == id)
                    .Select(x => x.PatientId)
                    .ToList();
                query = query.Where(patient => checkMy.Contains(patient.Id));
            }

            if (parameters.Sortings != null)
            {

                switch (parameters.Sortings)
                {
                    case Sorting.NameAsc:
                        query = query.OrderBy(pat => pat.Name);
                        break;
                    case Sorting.NameDesc:
                        query = query.OrderByDescending(pat => pat.Name);
                        break;
                    case Sorting.CreateAsc:
                        query = query.OrderBy(pat => pat.CreateTime);
                        break;
                    case Sorting.CreateDesc:
                        query = query.OrderByDescending(pat => pat.CreateTime);
                        break;
                    case Sorting.InspectionAsc:
                        query = query.OrderBy(patient =>
                            _dbContext.Inspections
                                .Where(insp => insp.PatientId == patient.Id)
                                .Max(insp => insp.Date));
                        break;
                    case Sorting.InspectionDesc:
                        query = query.OrderByDescending(patient =>
                            _dbContext.Inspections
                                .Where(insp => insp.PatientId == patient.Id)
                                .Min(insp => insp.Date));
                        break;
                    default:
                        query = query.OrderBy(patient => patient.CreateTime);
                        break;
                }
            }
            var totalCount = await query.CountAsync();

            if (parameters.Page == null)
            {
                parameters.Page = 1;
            }
            if (parameters.Size == null)
            {
                parameters.Size = 5;
            }

            if (parameters.Page <= 0 || parameters.Size <= 0)
            {
                var exception = new Exception();
                exception.Data.Add(StatusCodes.Status400BadRequest.ToString(), "Страница или размер страницы не может быть отрицательным");
                throw exception;
            }
            var specialties = await query
     .Skip((int)((parameters.Page - 1) * parameters.Size))
   .Take((int)parameters.Size)
   .ToListAsync();
            var result = new List<PatientModel>();

            foreach (var specialty in specialties)
            {
                var patientModel = new PatientModel
                {
                    Id = specialty.Id,
                    BirthDate = specialty.BirthDate,
                    CreateTime = specialty.CreateTime,
                    Gender = specialty.Gender,
                    Name = specialty.Name
                };



                result.Add(patientModel);
            }

            var pagination = new PaginationDTO
            {
                Size = parameters.Size,
                Count = totalCount,
                Current = parameters.Page
            };

            return new ReturnDTOGetPatient
            {
                patientModels = result,
                Pagination = pagination
            };
        }


        public async Task<GetCardPatientDTO> GetPatientCard(Guid id)
        {
            var patient = await _dbContext.patientModels.FirstOrDefaultAsync(x => x.Id == id);
            if (patient == null)
            {
                var exception = new Exception();
                exception.Data.Add(StatusCodes.Status404NotFound.ToString(), "Patient not found");
                throw exception;
            }
            else
            {
                var patientRes = new GetCardPatientDTO
                {
                    Name = patient.Name,
                    Id = patient.Id,
                    CreateTime = patient.CreateTime,
                    BirthDate = patient.BirthDate,
                    Gender = patient.Gender
                };
                return patientRes;
            }

        }

        public async Task<List<GetInspectionCardDTO>> GetPatientCardSearch(Guid id, string code)
        {
            var results = new List<GetInspectionCardDTO>();
            var diagnosisResults = new List<DiagnosisModel>();
            var patient = await _dbContext.patientModels.FirstOrDefaultAsync(x => x.Id == id);
            if (patient == null)
            {
                var exception = new Exception();
                exception.Data.Add(StatusCodes.Status404NotFound.ToString(), "patientId not found");
                throw exception;
            }
            else
            {
                var inspectionRoot = await _dbContext.Inspections
                    .Where(x => x.PreviousInspectionId == Guid.Empty && x.PatientId == id)
                    .ToListAsync();
                if (inspectionRoot != null)
                {
                    foreach (var inspection in inspectionRoot)
                    {
                        if (code != null)
                        {
                            diagnosisResults = await _dbContext.DiagnosisModel
     .Where(x =>
         (x.Code.ToLower().Contains(code.ToLower()) || x.Name.ToLower().Contains(code.ToLower()))
         && x.InspectionId == inspection.Id
         && x.Type == DiagnosisType.Main)
     .ToListAsync();
                        }
                        else
                        {
                            diagnosisResults = await _dbContext.DiagnosisModel
                              .Where(x => x.InspectionId == inspection.Id && x.Type == DiagnosisType.Main)
                              .ToListAsync();
                        }
                        foreach (var diagnosis in diagnosisResults)
                        {
                            var inspectionCardDTO = new GetInspectionCardDTO
                            {
                                id = inspection.Id,
                                createTime = inspection.CreateTime,
                                data = inspection.Date,
                                diagnosis = new DiagnosisModel
                                {
                                    Id = diagnosis.Id,
                                    CreateTime = diagnosis.CreateTime,
                                    Code = diagnosis.Code,
                                    Name = diagnosis.Name,
                                    Description = diagnosis.Description,
                                    Type = diagnosis.Type,
                                    InspectionId = diagnosis.InspectionId

                                }
                            };
                            results.Add(inspectionCardDTO);
                        }
                    }
                }
                else
                {
                    var exception = new Exception();
                    exception.Data.Add(StatusCodes.Status404NotFound.ToString(), "Болезни not found");
                    throw exception;
                }
            }

            return results;
        }


        /*public async Task<InspectionsResponseDTO> GetInspectionPatient(Guid Id, GetPatientInspection getPatientInspection)
        {
            // Проверка наличия пациента с указанным Id
            var checkPatient = _dbContext.patientModels.FirstOrDefault(x => x.Id == Id);
            if (checkPatient == null)
            {
                throw new Exception($"Not Found patient with id {Id}");
            }

            // Проверка на отрицательные значения страницы или размера
            if (getPatientInspection.page < 0 || getPatientInspection.size < 0)
            {
                throw new Exception($"Page or size cannot be negative");
            }

            // Получение всех инспекций для данного пациента
            IQueryable<Inspection> inspectionsQuery = _dbContext.Inspections.Where(x => x.PatientId == Id);

            // Если указано, что нужна группировка, добавляем условие
            if (getPatientInspection.grouped != null)
            {
                inspectionsQuery = inspectionsQuery.Where(x => x.PreviousInspectionId == Guid.Empty);
            }

            // Получение уникальных ID инспекций с диагнозами, соответствующими условиям
            var inspectionIdsWithDiagnoses = await _dbContext.DiagnosisModel
                .Where(diagnosis => inspectionsQuery.Any(i => i.Id == diagnosis.InspectionId))
                .Select(diagnosis => diagnosis.InspectionId)
                .Distinct()
                .ToListAsync();
            foreach (var i in inspectionIdsWithDiagnoses) {
                Console.WriteLine($"{i}");
                    }

            
            var filteredRecords = await _dbContextMKB.Records
                .Where(record => inspectionIdsWithDiagnoses.Contains(record.ID) && getPatientInspection.icdRoots.Contains(record.ROOT))
                .Select(record => record.ID)
                .ToListAsync();

            foreach (var i in filteredRecords)
            {
                Console.WriteLine($"Фильтрация записей по диагнозам: {i}");
            }

            // Получение всех диагнозов, соответствующих фильтру
            var diagnosisModels = await _dbContext.DiagnosisModel
                .Where(diagnosis => filteredRecords.Contains(diagnosis.IdDiagnosis))
                .ToListAsync();

            // Применение пагинации, если заданы параметры страницы и размера
            if (getPatientInspection.page.HasValue && getPatientInspection.size.HasValue)
            {
                inspectionsQuery = inspectionsQuery.Skip((getPatientInspection.page.Value - 1) * getPatientInspection.size.Value)
                                                   .Take(getPatientInspection.size.Value);
            }

            // Получение всех инспекций, удовлетворяющих условиям
            var inspections = await inspectionsQuery.ToListAsync();

            // Формирование DTO для инспекций
            var inspectionsDTO = inspections.Select(inspection =>
            {
                // Получение соответствующего диагноза для текущей инспекции
                var correspondingDiagnosis = diagnosisModels.FirstOrDefault(d => d.InspectionId == inspection.Id);

                return new getPatientInspectionsDTO
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
                    diagnosis = correspondingDiagnosis,
                    hasNasted = inspection.hasNested,
                    hasChain = inspection.hasChain
                };
            }).ToList();

            // Получение общего количества записей из базы данных
            var totalCountFromDatabase = await inspectionsQuery.CountAsync();

            // Формирование ответа в соответствии с DTO
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
                        Type = inspection.diagnosis.Type
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
*/

        public async Task<InspectionsResponseDTO> GetInspectionPatient(Guid Id, GetPatientInspection getPatientInspection)
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
            inspectionsQuery = ApplyPagination(inspectionsQuery, getPatientInspection);
            var inspections = await GetInspections(inspectionsQuery, diagnosisModels, getPatientInspection);

            var inspectionsDTO = MapInspectionsToDTO(inspections, diagnosisModels);
            var totalCountFromDatabase = await inspectionsQuery.CountAsync();

            var inspectionsResponse = CreateInspectionsResponse(inspectionsDTO, totalCountFromDatabase, getPatientInspection);

            return inspectionsResponse;
        }

        private void ValidatePatientAndParameters(Guid Id, GetPatientInspection getPatientInspection)
        {
            var checkPatient = _dbContext.patientModels.FirstOrDefault(x => x.Id == Id);
            if (checkPatient == null)
            {
                var exeption = new Exception();
                exeption.Data.Add(StatusCodes.Status404NotFound.ToString(), $"Не найден пациент с id {Id}");
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
            IQueryable<Inspection> inspectionsQuery = _dbContext.Inspections.Where(x => x.PatientId == Id).Include(x => x.Diagnoses);

            if (getPatientInspection.grouped != null)
            {
                inspectionsQuery = inspectionsQuery.Where(x => x.PreviousInspectionId == Guid.Empty);
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
            var filteredRecords = await _dbContextMKB.Records
                .Where(record => inspectionIdsWithDiagnoses.Contains(record.ID) && getPatientInspection.icdRoots.Contains(record.ROOT))
                .Select(record => record.ID)
                .ToListAsync();

            foreach (var i in filteredRecords)
            {
                Console.WriteLine($"filteredRecords:{i}");
            }

            return filteredRecords;
        }

        private async Task<List<DiagnosisModel>> GetDiagnosisModels(List<Guid> filteredRecords, Guid Id, IQueryable<Inspection> inspectionsQuery)
        {
            Console.WriteLine($"Id: {Id}");

            if (filteredRecords.Count == 0)
            {
                Console.WriteLine("Filtered Records: None (empty filter)");

                var inspectionIds = await inspectionsQuery.Select(i => i.Id).ToListAsync();
                var diagnosisModels = await _dbContext.DiagnosisModel
                    .Where(diagnosis => Id == diagnosis.IdPatient && diagnosis.Type == DiagnosisType.Main && inspectionIds.Contains(diagnosis.InspectionId))
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
                .Where(diagnosis => Id == diagnosis.IdPatient && filteredRecords.Contains(diagnosis.IdDiagnosis) && diagnosis.Type == DiagnosisType.Main)
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



        private void ValidateUserData(PatientDTO userRegistrationDTO)
        {

            if (!Enum.TryParse(userRegistrationDTO.gender, true, out Gender gender))
            {
                throw new ArgumentException("Invalid gender");
            }

            if (userRegistrationDTO.birthday > DateTime.UtcNow)
            {
                throw new ArgumentException("Birth date can't be in the future");
            }
        }

    }
}


