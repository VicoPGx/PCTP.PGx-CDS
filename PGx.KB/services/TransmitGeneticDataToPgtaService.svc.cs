using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using PGx.Model.Entities;
using System.IO;

namespace PGx.KB.services
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码、svc 和配置文件中的类名“TransmitGeneticDataToPgtaService”。
    // 注意: 为了启动 WCF 测试客户端以测试此服务，请在解决方案资源管理器中选择 TransmitGeneticDataToPgtaService.svc 或 TransmitGeneticDataToPgtaService.svc.cs，然后开始调试。
    public class TransmitGeneticDataToPgtaService : ITransmitGeneticDataToPgtaService
    {
        PGx_KBEntities context = new PGx_KBEntities();
        public void TransmitVcfData(string patientId, string patientName, string vcfData)
        {
            var patient = context.RAW_DATA_FILE.Where(x => x.PatientID == patientId).FirstOrDefault();
            if (patient == null)
            {
                patient = new RAW_DATA_FILE() { PatientID = patientId, PatientName = patientName };
            }
            string fileName = Guid.NewGuid() +"-"+ patientId + ".vcf";
            String basePath = System.AppDomain.CurrentDomain.BaseDirectory;
          

            var filePath = "Upload/RawDataFile/"+fileName;
            var absoluteFilePath= Path.Combine(basePath, filePath);
           
            System.IO.File.WriteAllText(absoluteFilePath, vcfData);
            if (string.IsNullOrEmpty(patient.FILE_PATH)==false)
            {
                FileInfo fi;
                var fullPath = Path.Combine(basePath, patient.FILE_PATH.Substring(1));
                fi = new System.IO.FileInfo(fullPath);
                fi.Delete();
            }
            patient.FILE_PATH = "/Upload/RawDataFile/"+fileName;
            if (patient.ID == 0)
            {
                context.RAW_DATA_FILE.Add(patient);
                context.SaveChanges();
            }
            else
                context.SaveChanges();
            return;
        }

        public void TransmitDiplotypes(string patientId, string patientName, string geneDiplotypes)
        {
            var patient = context.RAW_DATA_FILE.Where(x => x.PatientID == patientId).FirstOrDefault();
            if (patient == null)
            {
                patient = new RAW_DATA_FILE() { PatientID = patientId, PatientName = patientName };
            }

            List<string> geneDiplotypeList = geneDiplotypes.Split(',').ToList();
            if (patient.ID == 0)
            {
                context.RAW_DATA_FILE.Add(patient);
                context.SaveChanges();
                foreach (var geneDiplotype in geneDiplotypeList)
                {

                    var gene = geneDiplotype.Split(':')[0];
                    var diplotype = geneDiplotype.Split(':')[1];
                    var geneCall = new GeneCall()
                    {
                        Gene = gene,
                        RAW_DATA_FILE = patient,
                        TimeStamp = DateTime.Now.ToShortTimeString()
                    };
                    context.GeneCall.Add(geneCall);
                    context.SaveChanges();
                    DiplotypeMatch diplotypeMatch = new DiplotypeMatch()
                    {
                        GeneCall = geneCall,
                        Name = diplotype
                    };
                    context.DiplotypeMatch.Add(diplotypeMatch);
                    context.SaveChanges();
                }
            }
            else {
                foreach (var geneDiplotype in geneDiplotypeList)
                {
                    var gene = geneDiplotype.Split(':')[0];
                    var diplotype = geneDiplotype.Split(':')[1];
                    var geneCall=patient.GeneCall.Where(x => x.Gene == gene).FirstOrDefault();
                    if (geneCall != null)
                    {
                        geneCall.Gene = gene;
                        geneCall.TimeStamp = DateTime.Now.ToShortTimeString();
                        geneCall.DiplotypeMatch.FirstOrDefault().Name = diplotype;
                        context.SaveChanges();
                    }
                    else
                    {
                        geneCall = new GeneCall()
                        {
                            Gene = gene,
                            RAW_DATA_FILE = patient,
                            TimeStamp = DateTime.Now.ToShortTimeString()
                        };
                        context.GeneCall.Add(geneCall);
                        context.SaveChanges();
                        DiplotypeMatch diplotypeMatch = new DiplotypeMatch()
                        {
                            GeneCall = geneCall,
                            Name = diplotype
                        };
                        context.DiplotypeMatch.Add(diplotypeMatch);
                        context.SaveChanges();
 
                    }
                }            
            }
            return;
        }
    }
}
