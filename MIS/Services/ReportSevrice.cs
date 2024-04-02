using Microsoft.EntityFrameworkCore;
using MIS.Data;
using MIS.Data.DTO;
using MIS.Data.Models;
using MIS.Services.Interfaces;

namespace MIS.Services
{
    public class ReportSevrice : IReportService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ApplicationDbContextForMIS _dbContextMKB;


        public ReportSevrice(ApplicationDbContext context, ApplicationDbContextForMIS dbContextMKB)
        {
            _dbContext = context;
            _dbContextMKB = dbContextMKB;
        }



        public async Task<ResponseDTO> GetReport(DTOGetReport dTOGetReport)
        {
            if (dTOGetReport.Start > dTOGetReport.End)
            {
                var ex = new Exception();
                ex.Data.Add(StatusCodes.Status400BadRequest.ToString(), $"Время start не долно быть больше end");
                throw ex;
            }

            var getRootIcd = _dbContextMKB.Records.Where(x => x.ID_PARENT == null).Select(x => x.ID).ToList();
            List<string> icdRootsToUse = new List<string>();
            var getRootCode = _dbContextMKB.Records.Where(x => x.ID_PARENT == null).Select(x => x.REC_CODE).ToList();
            if (dTOGetReport.icdRoot != null && dTOGetReport.icdRoot.Any())
            {
                foreach (var i in dTOGetReport.icdRoot)
                {
                    if (!getRootIcd.Contains(i))
                    {
                        var ex = new Exception();
                        ex.Data.Add(StatusCodes.Status404NotFound.ToString(), $"Данный root - {i} не найден.");
                        throw ex;
                    }
                    else
                    {
                        icdRootsToUse.Add(i.ToString());
                    }
                }
            }
            else
            {
                icdRootsToUse = getRootCode;
            }
            var icdCode = new List<string>();
            foreach (var i in icdRootsToUse)
            {
                var record = _dbContextMKB.Records.FirstOrDefault(x => x.ID.ToString() == i);
                if (record != null)
                {
                    icdCode.Add(record.MKB_CODE);
                }
            }

            var Filters = new FilterData()
            {
                Start = dTOGetReport.Start,
                End = dTOGetReport.End,
                IcdRoots = icdCode
            };

            var getInspection = _dbContext.Inspections.Where(x => x.Date >= dTOGetReport.Start && x.Date <= dTOGetReport.End).Select(x => x.Id).ToList();
            var getConsultation = new List<DiagnosisModel>();
            var infoUser = new List<PatientModel>();
            var GuidIdPatient = new List<Guid>();

            getConsultation = _dbContext.DiagnosisModel.Where(x => getInspection.Contains(x.InspectionId)).ToList();
            GuidIdPatient = getConsultation.Select(x => x.IdPatient).ToList();

            var listDiagnosFromRecord = new List<Record>();

            foreach (var consultation in getConsultation)
            {
                var record = _dbContextMKB.Records.FirstOrDefault(x => x.ID == consultation.IdDiagnosis && icdRootsToUse.Contains(x.ROOT.ToString()));
                if (record != null)
                {
                    listDiagnosFromRecord.Add(record);
                }

            }

            listDiagnosFromRecord = listDiagnosFromRecord.GroupBy(record => record.ID)
                                                         .Select(group => group.First())
                                                         .ToList();

            var newConsultationAfterCheck = new List<DiagnosisModel>();
            foreach (var i in listDiagnosFromRecord)
            {
                var consultations = _dbContext.DiagnosisModel.Where(x => x.IdDiagnosis == i.ID).ToList();
                newConsultationAfterCheck.AddRange(consultations);
            }
            infoUser = _dbContext.patientModels.Where(x => GuidIdPatient.Contains(x.Id)).ToList();


            var rootRecords = _dbContextMKB.Records.Where(x => x.ID_PARENT == null).ToList();
            var allRecords = _dbContextMKB.Records.ToList();

            // Создание словаря для соотнесения MKB_CODE с корневым кодом
            var mkbCodeToRootCodeMap = new Dictionary<string, string>();
            foreach (var record in allRecords)
            {
                // Находим корневую запись для данного MKB_CODE в загруженных данных
                var rootRecord = allRecords.FirstOrDefault(r => r.ID == record.ROOT);
                if (rootRecord != null)
                {
                    mkbCodeToRootCodeMap[record.MKB_CODE] = rootRecord.MKB_CODE;
                }
            }
            var patientDiagnosesCount = new Dictionary<Guid, Dictionary<string, int>>();


            var updatedPatientDiagnosesCount = new Dictionary<Guid, Dictionary<string, int>>();
            foreach (var diagnosis in newConsultationAfterCheck)
            {
                if (!updatedPatientDiagnosesCount.ContainsKey(diagnosis.IdPatient))
                {
                    updatedPatientDiagnosesCount[diagnosis.IdPatient] = new Dictionary<string, int>();
                }

                string rootCode = mkbCodeToRootCodeMap.ContainsKey(diagnosis.Code) ? mkbCodeToRootCodeMap[diagnosis.Code] : diagnosis.Code;

                if (!updatedPatientDiagnosesCount[diagnosis.IdPatient].ContainsKey(rootCode))
                {
                    updatedPatientDiagnosesCount[diagnosis.IdPatient][rootCode] = 0;
                }

                updatedPatientDiagnosesCount[diagnosis.IdPatient][rootCode]++;
            }

            // Создание записей RecordData
            var records = new List<RecordData>();
            foreach (var patientId in updatedPatientDiagnosesCount.Keys)
            {
                var patient = _dbContext.patientModels.FirstOrDefault(p => p.Id == patientId);
                if (patient != null)
                {
                    var record = new RecordData
                    {
                        PatientName = patient.Name,
                        PatientBirthdate = patient.BirthDate,
                        Gender = patient.Gender,
                        VisitsByRoot = updatedPatientDiagnosesCount[patientId]
                    };

                    records.Add(record);
                }
            }

            var summaryByRoot = new Dictionary<string, int>();
            foreach (var patientVisit in updatedPatientDiagnosesCount)
            {
                foreach (var visit in patientVisit.Value)
                {
                    if (!summaryByRoot.ContainsKey(visit.Key))
                    {
                        summaryByRoot[visit.Key] = 0;
                    }
                    summaryByRoot[visit.Key] += visit.Value;
                }
            }

            // Формирование и возврат ResponseDTO
            var responseDto = new ResponseDTO
            {
                Filters = Filters,
                Records = records,
                SummaryByRoot = summaryByRoot
            };

            return responseDto;

        }



    }
}
