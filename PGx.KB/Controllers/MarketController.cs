using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PGx.Model.Entities;
using System.IO;
using System.Web.Script.Serialization;
using Microsoft.Office.Interop.Excel;

namespace PGx.KB.Controllers
{
    public class MarketController : BaseController
    {
        PGx_KBEntities context = new PGx_KBEntities();

        public string AbsolutePath(string relativePath)
        {
            return Server.MapPath(relativePath);
        }
        [HttpPost]
        public JsonResult UploadFile(RAW_DATA_FILE rawDataFile)
        {
            // save image, create image object
            //RAW_DATA_FILE file = null;
            var reObject = new Dictionary<string, RAW_DATA_FILE>();
            // RAW_DATA_FILE obj=new RAW_DATA_FILE();
            if (rawDataFile.ID == 0)
            {
                if (Request.Files.Count > 0)
                {
                    HttpPostedFileBase file = Request.Files[0];
                    //if (file.ContentLength > 0)                  
                    var fileName = Guid.NewGuid() + "-" + Path.GetFileName(file.FileName);
                    var folderPath = Server.MapPath("~/Upload/RawDataFile/");
                    Directory.CreateDirectory(folderPath);
                    //用绝对路径存储文件
                    var absolutePath = Path.Combine(folderPath, fileName);
                    file.SaveAs(absolutePath);
                    //数据库仅存储相对路径         
                    var relativePath = "/Upload/RawDataFile/" + fileName;
                    rawDataFile.FILE_PATH = relativePath;

                }
                //var count = context.RAW_DATA_FILE.Count()+1;
                //var med = new Random().Next(101, 1000);
                //var year = DateTime.Now.Year.ToString();
                //string head= year.Substring(2, 2);

                //if (count < 10)
                //{
                //    med = med * 100;
                //}
                //else if (10 < count && count < 100)
                //{
                //    med = med * 10;
                //}

                //var patientId = "Pa"+head + med.ToString()+count.ToString();
                //rawDataFile.PatientID = patientId;
                rawDataFile.TimeStamp = DateTime.Now;
                //DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                context.RAW_DATA_FILE.Add(rawDataFile);
                context.SaveChanges();
                reObject.Add("obj", rawDataFile);



            }
            else
            {
                var existFile = context.RAW_DATA_FILE.Where(x => x.ID == rawDataFile.ID).FirstOrDefault();
                //Replace file
                if (Request.Files.Count > 0)
                {
                    HttpPostedFileBase file = Request.Files[0];
                    //Create folder path
                    var folderPath = Server.MapPath("~/Upload/RawDataFile/");
                    //creat forder if it dosen't exist
                    Directory.CreateDirectory(folderPath);
                    //creat file name
                    var fileName = Guid.NewGuid() + "-" + Path.GetFileName(file.FileName);
                    //create file path=folder path + file name
                    var newFilePathAbs = Path.Combine(folderPath, fileName);
                    //Delete exist file
                    if (string.IsNullOrEmpty(existFile.FILE_PATH) == false)
                    {
                        FileInfo fi;
                        var fullPath = Server.MapPath(existFile.FILE_PATH);

                        fi = new System.IO.FileInfo(fullPath);

                        fi.Delete();
                    }
                    //Save new file
                    file.SaveAs(newFilePathAbs);
                    //save relative path in database
                    existFile.FILE_PATH = "/Upload/RawDataFile/" + fileName;

                }

                //existFile.PatientID = rawDataFile.PatientID;
                existFile.PatientName = rawDataFile.PatientName;
                existFile.Method = rawDataFile.Method;
                existFile.SampleCode = rawDataFile.SampleCode;

                existFile.Sex = rawDataFile.Sex;
                existFile.TYPE = rawDataFile.TYPE;
                existFile.Laboratory = rawDataFile.Laboratory;
                existFile.DESCRIPTION = rawDataFile.DESCRIPTION;
                existFile.TimeStamp = DateTime.Now;
                context.SaveChanges();
                reObject.Add("obj", null);

            }
            return Json(reObject, JsonRequestBehavior.AllowGet);
        }


        public void FrequencyExcelProcess(int id)
        {
            Application excel = new Application();
            var defFile = context.DefinitionFile.Where(x => x.ID == id).FirstOrDefault();
            var forderPath = Server.MapPath("~/Upload/RawDataFile/");
            var filePath = Path.GetFileName(defFile.AlleleFrequencyTable);
            filePath = Path.Combine(forderPath, filePath);
            Workbook wb = excel.Workbooks.Open(filePath);
            Worksheet ws = wb.Worksheets.get_Item(1);
            var rowsNum = ws.UsedRange.Cells.Rows.Count + 1;
            var columnNum = ws.UsedRange.Cells.Columns.Count + 1;

            //Remove existed frequency
            foreach (var namedAllele in defFile.NamedAllele)
            {
                var freList = namedAllele.PopulationFrequency.ToList();
                foreach (var fre in freList)
                {
                    context.PopulationFrequency.Remove(fre);
                    context.SaveChanges();
                }
            }

            for (int i = 3; i < rowsNum; i++)
            {

                string alleleName = ws.Cells[i, 1].Value2;
                if (string.IsNullOrEmpty(alleleName) == true)
                    break;
                alleleName = alleleName.Trim();
                var namedAllele = defFile.NamedAllele.Where(x => x.Name == alleleName.Trim()).FirstOrDefault();
                if (namedAllele == null)
                {
                    namedAllele = new NamedAllele()
                    {
                        DefinitionFile = defFile,
                        DefinitionFileID = defFile.ID,
                        Name = alleleName.Trim()
                    };

                    context.NamedAllele.Add(namedAllele);
                    context.SaveChanges();
                }
                for (int j = 2; j < columnNum; j++)
                {
                    string population = ws.Cells[2, j].Value2;

                    if (string.IsNullOrEmpty(population))
                        break;
                    population = population.Trim();
                    var fre = ws.Cells[i, j].Value2;
                    if (fre == null)
                        continue;
                    string freStr = fre.ToString();
                    if (string.IsNullOrEmpty(freStr) == true)
                        continue;
                    decimal freDec = 0;
                    var parseFlag = decimal.TryParse(freStr, out freDec);
                    if (parseFlag == false || freDec < 0.0001M)
                        continue;

                    var popfre = namedAllele.PopulationFrequency.Where(x => x.Population.Trim() == population).FirstOrDefault();
                    if (popfre == null)
                    {
                        popfre = new PopulationFrequency()
                          {
                              Population = population.Trim(),
                              Frequency = freDec,
                              NamedAllele = namedAllele
                          };
                        context.PopulationFrequency.Add(popfre);
                        context.SaveChanges();
                    }
                    else
                    {
                        popfre.Frequency = freDec;
                        context.SaveChanges();
                    }
                }
            }

            //calculate phenotype frequency
            //List<string> populationList =new List<string>() { "East Asian", "African", "African American", "Caucasian ", "Middle Eastern", "South/Central Asian", "Americas", "Oceanian" };
            List<string> populationList = new List<string>();
            if (string.IsNullOrEmpty(defFile.Populations) == false)
                populationList = defFile.Populations.Split(',').ToList();
            for (var k = 2; k < columnNum; k++)
            {
                string popName = ws.Cells[2, k].Value2;
                if (string.IsNullOrEmpty(popName) == true)
                    continue;
                populationList.Add(popName.Trim());
            }
            populationList = populationList.Distinct().ToList();
            defFile.Populations = string.Join(",", populationList);
            context.SaveChanges();
            var phenotypeList = defFile.PhenotypeMap.ToList();
            foreach (var phenotype in phenotypeList)
            {
                var phenoDefList = phenotype.PhenotypeDef.ToList();
                var existedFrequencyList = phenotype.PhenotypePopFreq.ToList();
                foreach (var existFre in existedFrequencyList)
                {
                    context.PhenotypePopFreq.Remove(existFre);
                    context.SaveChanges();
                }

                foreach (var population in populationList)
                {
                    decimal? phenotypeFrequency = 0;
                    foreach (var phenoDef in phenoDefList)
                    {
                        var funA = phenoDef.FunctionA.ToLower();
                        var funB = phenoDef.FunctionB.ToLower();
                        var alleleAlist = defFile.NamedAllele.Where(x => string.IsNullOrEmpty(x.M_Function) == false && x.M_Function.ToLower() == funA).ToList();
                        var alleleBlist = defFile.NamedAllele.Where(x => string.IsNullOrEmpty(x.M_Function) == false && x.M_Function.ToLower() == funB).ToList();
                        decimal? funAfre = 0, funBfre = 0, funcPairScore = 0;
                        if (funA != funB)
                        {
                            foreach (var alleleA in alleleAlist)
                            {
                                var alleleAfre = alleleA.PopulationFrequency.Where(x => x.Population.Trim() == population.Trim()).FirstOrDefault();
                                if (alleleAfre != null)
                                {
                                    if (alleleAfre.Frequency != null)
                                    {
                                        funAfre += alleleAfre.Frequency;
                                    }
                                }
                            }
                            foreach (var alleleB in alleleBlist)
                            {
                                var alleleBfre = alleleB.PopulationFrequency.Where(x => x.Population.Trim() == population.Trim()).FirstOrDefault();
                                if (alleleBfre != null)
                                {
                                    if (alleleBfre.Frequency != null)
                                    {
                                        funBfre += alleleBfre.Frequency;
                                    }
                                }
                            }
                            funcPairScore = 2 * funAfre * funBfre;

                            phenotypeFrequency += funcPairScore;
                        }
                        else
                        {
                            foreach (var alleleA in alleleAlist)
                            {
                                var alleleAfre = alleleA.PopulationFrequency.Where(x => x.Population.Trim() == population.Trim()).FirstOrDefault();
                                if (alleleAfre != null)
                                {
                                    if (alleleAfre.Frequency != null)
                                    {
                                        funAfre += alleleAfre.Frequency;
                                    }
                                }
                            }
                            funcPairScore = funAfre * funAfre;

                            phenotypeFrequency += funcPairScore;
                        }

                    }
                    if (phenotypeFrequency == 0)
                        //phenotypeFrequency = null;
                        continue;
                    else
                        phenotypeFrequency = (decimal?)Math.Round((decimal)phenotypeFrequency, 6);

                    //update and store calculated frequency
                    var popFre = phenotype.PhenotypePopFreq.Where(x => x.Population.ToLower() == population).FirstOrDefault();
                    if (popFre != null)
                    {
                        popFre.Frequency = (decimal)phenotypeFrequency;
                        context.SaveChanges();
                    }
                    else
                    {
                        var phenoPopFrequency = new PhenotypePopFreq()
                        {
                            Population = population,
                            Frequency = (decimal)phenotypeFrequency,
                            NamedPhenotype = phenotype
                        };
                        context.PhenotypePopFreq.Add(phenoPopFrequency);
                        context.SaveChanges();
                    }
                }
            }
            wb.Close();
            return;
        }

        public void AlleleDefinitionTableExtraction(int id)
        {
            Application excel = new Application();
            var defFile = context.DefinitionFile.Where(x => x.ID == id).FirstOrDefault();
            var forderPath = Server.MapPath("~/Upload/RawDataFile/");
            var filePath = Path.GetFileName(defFile.AlleleDefinitionTable);
            filePath = Path.Combine(forderPath, filePath);
            Workbook wb = excel.Workbooks.Open(filePath);
            Worksheet ws = wb.Worksheets.get_Item(1);
            var rowsNum = ws.UsedRange.Cells.Rows.Count + 1;
            var columnNum = ws.UsedRange.Cells.Columns.Count + 1;

            //set the row number of rsid as a marker;
            int marker = 6;
            for (var i = 1; i < rowsNum; i++)
            {

                string tex = ws.Cells[i, 1].Value2;
                tex = tex.ToLower().Trim();
                if (tex == "rsid")
                {
                    marker = i;
                    break;
                }
            }
            //remove exist
            var namedAlleleList = defFile.NamedAllele.ToList();
            foreach (var namedAllele in namedAlleleList)
            {
                var frequencyList = namedAllele.PopulationFrequency.ToList();
                foreach (var frequency in frequencyList)
                {
                    context.PopulationFrequency.Remove(frequency) ;
                    context.SaveChanges();
                }

                var alleleDefList = namedAllele.NamedAlleleDefinition.ToList();
                foreach (var alleleDef in alleleDefList)
                {
                    context.NamedAlleleDefinition.Remove(alleleDef);
                    context.SaveChanges();
                }

                context.NamedAllele.Remove(namedAllele);
                context.SaveChanges();
            }

            var variantList = defFile.VariantLocus.ToList();
            foreach (var variant in variantList)
            {
                context.VariantLocus.Remove(variant);
                context.SaveChanges();
            }


            //save namedAllele;
            for (int i = marker + 2; i < rowsNum; i++)
            {
                string starAllele = ws.Cells[i, 1].Value2;
                if (string.IsNullOrEmpty(starAllele))
                    break;
                starAllele = starAllele.Trim();
                var namedAllele = defFile.NamedAllele.Where(x => x.Name.Trim() == starAllele).FirstOrDefault();
                if (namedAllele == null)
                {
                    namedAllele = new NamedAllele();
                }

                namedAllele.Name = starAllele;
                namedAllele.IsRefAllele = i == marker + 2 ? true : false;
                if (namedAllele.ID == 0)
                {
                    namedAllele.DefinitionFile = defFile;
                    namedAllele.DefinitionFileID = defFile.ID;
                    context.NamedAllele.Add(namedAllele);
                }
                context.SaveChanges();
            }
            //save variants;
            int namedAlleleCount = defFile.NamedAllele.Count();

            for (int i = 2; i < columnNum; i++)
            {
                List<string> alleleList = new List<string>();
                string rsTxt = ws.Cells[marker, i].Value2;
                string geneName = ws.Cells[marker - 1, i].Value2;
                string chromoName = ws.Cells[marker - 2, i].Value2;
                string proteinName = ws.Cells[marker - 3, i].Value2;
                string transcriptName = ws.Cells[marker - 4, i].Value2;
                string refAllele = ws.Cells[marker + 2, i].Value2;
                if (string.IsNullOrEmpty(chromoName))
                    break;
                refAllele = refAllele.Trim();
                chromoName = chromoName.Trim();
                int endIndex = chromoName.IndexOf("_");
                if (endIndex < 0)
                {
                    endIndex = chromoName.ToLower().IndexOf("del");
                    if (endIndex < 0)
                    {
                        endIndex = chromoName.ToLower().IndexOf("ins");
                        if (endIndex < 0)
                        {
                            endIndex = chromoName.IndexOf(refAllele);
                        }
                    }
                }
                string positionStr = chromoName.Substring(2, endIndex - 2);
                int position = int.Parse(positionStr);

                string variantType = string.Empty;
                if (chromoName.ToLower().Contains("ins"))
                {
                    variantType = "INS";
                }
                else if (chromoName.ToLower().Contains("del"))
                {
                    variantType = "DEL";
                }
                else
                {
                    variantType = "SNP";
                }
                for (int j = marker + 2; j < marker + namedAlleleCount + 2; j++)
                {
                    string cellAllele = ws.Cells[j, i].Value2;
                    if (string.IsNullOrEmpty(cellAllele) == false && alleleList.Any(x => x == cellAllele) == false)
                    {
                        alleleList.Add(cellAllele.Trim());
                    }
                }
                VariantLocus variant = defFile.VariantLocus.Where(x => x.ChromosomeHgvsName.Trim() == chromoName.Trim()).FirstOrDefault();
                if (variant == null)
                {
                    variant = new VariantLocus();
                }
                variant.Rsid = string.IsNullOrEmpty(rsTxt) == false ? rsTxt.Trim() : rsTxt;
                variant.GeneHgvsName = string.IsNullOrEmpty(geneName) == false ? geneName.Trim() : geneName;
                variant.ChromosomeHgvsName = chromoName.Trim();
                variant.Alleles = string.Join(",", alleleList);
                variant.ProteinNote = string.IsNullOrEmpty(proteinName) == false ? proteinName.Trim() : proteinName;
                variant.TranscriptName = string.IsNullOrEmpty(transcriptName) == false ? transcriptName.Trim() : transcriptName;
                variant.Position = position;
                variant.Type = variantType;

                if (variant.ID == 0)
                {
                    variant.DefinitionFile = defFile;
                    variant.DefinitionFileID = defFile.ID;
                    context.VariantLocus.Add(variant);
                }
                context.SaveChanges();
            }
            //save baseAllele
            for (int i = marker + 2; i < rowsNum; i++)
            {
                string starAllele = ws.Cells[i, 1].Value2;
                if (string.IsNullOrEmpty(starAllele))
                    break;
                var namedAllele = defFile.NamedAllele.Where(x => x.Name == starAllele.Trim()).FirstOrDefault();
                for (int j = 2; j < columnNum; j++)
                {
                    string chromoName = ws.Cells[marker - 2, j].Value2;
                    if (string.IsNullOrEmpty(chromoName))
                        break;
                    VariantLocus variant = defFile.VariantLocus.Where(x => x.ChromosomeHgvsName == chromoName.Trim()).FirstOrDefault();
                    string allele = ws.Cells[i, j].Value2;
                    var namedAlleleDefinition = context.NamedAlleleDefinition.Where(x => x.NamedAlleleID == namedAllele.ID && x.VariantLocusID == variant.ID).FirstOrDefault();
                    if (namedAlleleDefinition == null)
                    {
                        namedAlleleDefinition = new NamedAlleleDefinition();

                    }
                    namedAlleleDefinition.Allele = string.IsNullOrEmpty(allele) == false ? allele.Trim() : null;

                    if (namedAlleleDefinition.ID == 0)
                    {
                        namedAlleleDefinition.NamedAllele = namedAllele;
                        namedAlleleDefinition.NamedAlleleID = namedAllele.ID;
                        namedAlleleDefinition.VariantLocus = variant;
                        namedAlleleDefinition.VariantLocusID = variant.ID;
                        context.NamedAlleleDefinition.Add(namedAlleleDefinition);
                    }
                    context.SaveChanges();
                }
            }

            wb.Close();
            return;
        }

        public void AlleleFunctionalityTableExtraction(int id)
        {
            Application excel = new Application();
            var defFile = context.DefinitionFile.Where(x => x.ID == id).FirstOrDefault();
            string forderPath = Server.MapPath("~/Upload/RawDataFile/");
            string filePath = Path.GetFileName(defFile.FunctionTable);
            filePath = Path.Combine(forderPath, filePath);
            Workbook wb = excel.Workbooks.Open(filePath);
            Worksheet ws = wb.Worksheets.get_Item(1);
            int rowsNum = ws.UsedRange.Cells.Rows.Count + 1;
            List<NamedAllele> alleleFunctionList = new List<NamedAllele>();
            List<string> functionList = new List<string>();
            for (int i = 3; i < rowsNum; i++)
            {
                string allele = ws.Cells[i, 1].Value2;
                if (string.IsNullOrEmpty(allele))
                    break;
                var activityValue = ws.Cells[i, 2].Value2;
                string function = ws.Cells[i, 4].Value2;
                allele = allele.Trim();
                function = function.Trim();
                NamedAllele alleleFunction = new NamedAllele()
                {
                    Name = allele,
                    M_Function = function
                };

                if (activityValue != null)
                {
                    string activityValueStr = activityValue.ToString();
                    double activityValueDouble;
                    if (double.TryParse(activityValueStr, out activityValueDouble))
                    {
                        activityValueStr = activityValueStr.Trim();
                        alleleFunction.ActivityValue = activityValueDouble;
                    }
                }
                alleleFunctionList.Add(alleleFunction);
            }
            if (string.IsNullOrEmpty(defFile.NamedFunctions) == false)
            {
                functionList = defFile.NamedFunctions.Split(',').ToList();
            }
            foreach (var alleleFunction in alleleFunctionList)
            {
                functionList.Add(alleleFunction.M_Function);
            }
            functionList = functionList.Distinct().ToList();
            functionList.Sort();
            defFile.NamedFunctions = string.Join(",", functionList);
            context.SaveChanges();

            foreach (var alleleFunction in alleleFunctionList)
            {
                var namedAllele = defFile.NamedAllele.Where(x => x.Name == alleleFunction.Name).FirstOrDefault();
                if (namedAllele == null)
                {
                    NamedAllele newNamedAllele = new NamedAllele()
                    {
                        Name = alleleFunction.Name,
                        M_Function = alleleFunction.M_Function,
                        ActivityValue = alleleFunction.ActivityValue,
                        DefinitionFileID = defFile.ID,
                        DefinitionFile = defFile
                    };
                    context.NamedAllele.Add(newNamedAllele);
                    context.SaveChanges();
                }
                else
                {
                    namedAllele.M_Function = alleleFunction.M_Function;
                    namedAllele.ActivityValue = alleleFunction.ActivityValue;
                    context.SaveChanges();
                }
            }
            wb.Close();
            return;
        }
    }
}


