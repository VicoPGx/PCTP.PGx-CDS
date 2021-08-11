
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, and Azure
-- --------------------------------------------------
-- Date Created: 08/09/2021 20:46:54
-- Generated from EDMX file: D:\PGx.KB\PGx.KB 005\PGx.Model\Entities\PGx_KB.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [PGx_KB];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_DefinitionFileVariantLocus]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[VariantLocus] DROP CONSTRAINT [FK_DefinitionFileVariantLocus];
GO
IF OBJECT_ID(N'[dbo].[FK_VariantLocusAlleleDefinition]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[NamedAlleleDefinition] DROP CONSTRAINT [FK_VariantLocusAlleleDefinition];
GO
IF OBJECT_ID(N'[dbo].[FK_DefinitionFileNamedAllele]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[NamedAllele] DROP CONSTRAINT [FK_DefinitionFileNamedAllele];
GO
IF OBJECT_ID(N'[dbo].[FK_NamedAlleleNamedAlleleDefinition]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[NamedAlleleDefinition] DROP CONSTRAINT [FK_NamedAlleleNamedAlleleDefinition];
GO
IF OBJECT_ID(N'[dbo].[FK_DefinitionExemptionVariantLocus]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[VariantLocus] DROP CONSTRAINT [FK_DefinitionExemptionVariantLocus];
GO
IF OBJECT_ID(N'[dbo].[FK_NamedAllelePopulationFrequency]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[PopulationFrequency] DROP CONSTRAINT [FK_NamedAllelePopulationFrequency];
GO
IF OBJECT_ID(N'[dbo].[FK_GeneCallDiplotypeMatch]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[DiplotypeMatch] DROP CONSTRAINT [FK_GeneCallDiplotypeMatch];
GO
IF OBJECT_ID(N'[dbo].[FK_GeneCallHaplotypeMatch]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[HaplotypeMatch] DROP CONSTRAINT [FK_GeneCallHaplotypeMatch];
GO
IF OBJECT_ID(N'[dbo].[FK_GeneCallVariant1]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Variant] DROP CONSTRAINT [FK_GeneCallVariant1];
GO
IF OBJECT_ID(N'[dbo].[FK_GeneCallVarOfInterest]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[VarOfInterest] DROP CONSTRAINT [FK_GeneCallVarOfInterest];
GO
IF OBJECT_ID(N'[dbo].[FK_PGxGuidelineDosingGuidence]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[DosingGuidence] DROP CONSTRAINT [FK_PGxGuidelineDosingGuidence];
GO
IF OBJECT_ID(N'[dbo].[FK_PGxGuidelineLiterature]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Literature] DROP CONSTRAINT [FK_PGxGuidelineLiterature];
GO
IF OBJECT_ID(N'[dbo].[FK_PGxGuidelineRelatedGene_PGxGuideline]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[PGxGuidelineRelatedGene] DROP CONSTRAINT [FK_PGxGuidelineRelatedGene_PGxGuideline];
GO
IF OBJECT_ID(N'[dbo].[FK_PGxGuidelineRelatedGene_RelatedGene]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[PGxGuidelineRelatedGene] DROP CONSTRAINT [FK_PGxGuidelineRelatedGene_RelatedGene];
GO
IF OBJECT_ID(N'[dbo].[FK_DefinitionFilePhenotypeMap]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[NamedPhenotype] DROP CONSTRAINT [FK_DefinitionFilePhenotypeMap];
GO
IF OBJECT_ID(N'[dbo].[FK_NamedPhenotypePhenotypeDef]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[PhenotypeDef] DROP CONSTRAINT [FK_NamedPhenotypePhenotypeDef];
GO
IF OBJECT_ID(N'[dbo].[FK_PhenotypePopFreqNamedPhenotype]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[PhenotypePopFreq] DROP CONSTRAINT [FK_PhenotypePopFreqNamedPhenotype];
GO
IF OBJECT_ID(N'[dbo].[FK_PGxGuidelineDefinitionFile_PGxGuideline]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[PGxGuidelineDefinitionFile] DROP CONSTRAINT [FK_PGxGuidelineDefinitionFile_PGxGuideline];
GO
IF OBJECT_ID(N'[dbo].[FK_PGxGuidelineDefinitionFile_DefinitionFile]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[PGxGuidelineDefinitionFile] DROP CONSTRAINT [FK_PGxGuidelineDefinitionFile_DefinitionFile];
GO
IF OBJECT_ID(N'[dbo].[FK_RAW_DATA_FILEDiplotypeResult]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[DiplotypeResult] DROP CONSTRAINT [FK_RAW_DATA_FILEDiplotypeResult];
GO
IF OBJECT_ID(N'[dbo].[FK_RAW_DATA_FILEGeneCall]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[GeneCall] DROP CONSTRAINT [FK_RAW_DATA_FILEGeneCall];
GO
IF OBJECT_ID(N'[dbo].[FK_DosingGuidenceRecommendationPhenotype]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RecommendationPhenotype] DROP CONSTRAINT [FK_DosingGuidenceRecommendationPhenotype];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[RAW_DATA_FILE]', 'U') IS NOT NULL
    DROP TABLE [dbo].[RAW_DATA_FILE];
GO
IF OBJECT_ID(N'[dbo].[DefinitionFile]', 'U') IS NOT NULL
    DROP TABLE [dbo].[DefinitionFile];
GO
IF OBJECT_ID(N'[dbo].[VariantLocus]', 'U') IS NOT NULL
    DROP TABLE [dbo].[VariantLocus];
GO
IF OBJECT_ID(N'[dbo].[NamedAllele]', 'U') IS NOT NULL
    DROP TABLE [dbo].[NamedAllele];
GO
IF OBJECT_ID(N'[dbo].[NamedAlleleDefinition]', 'U') IS NOT NULL
    DROP TABLE [dbo].[NamedAlleleDefinition];
GO
IF OBJECT_ID(N'[dbo].[DefinitionExemption]', 'U') IS NOT NULL
    DROP TABLE [dbo].[DefinitionExemption];
GO
IF OBJECT_ID(N'[dbo].[PopulationFrequency]', 'U') IS NOT NULL
    DROP TABLE [dbo].[PopulationFrequency];
GO
IF OBJECT_ID(N'[dbo].[PGxGuideline]', 'U') IS NOT NULL
    DROP TABLE [dbo].[PGxGuideline];
GO
IF OBJECT_ID(N'[dbo].[RelatedGene]', 'U') IS NOT NULL
    DROP TABLE [dbo].[RelatedGene];
GO
IF OBJECT_ID(N'[dbo].[DosingGuidence]', 'U') IS NOT NULL
    DROP TABLE [dbo].[DosingGuidence];
GO
IF OBJECT_ID(N'[dbo].[Literature]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Literature];
GO
IF OBJECT_ID(N'[dbo].[GeneCall]', 'U') IS NOT NULL
    DROP TABLE [dbo].[GeneCall];
GO
IF OBJECT_ID(N'[dbo].[DiplotypeMatch]', 'U') IS NOT NULL
    DROP TABLE [dbo].[DiplotypeMatch];
GO
IF OBJECT_ID(N'[dbo].[HaplotypeMatch]', 'U') IS NOT NULL
    DROP TABLE [dbo].[HaplotypeMatch];
GO
IF OBJECT_ID(N'[dbo].[Variant]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Variant];
GO
IF OBJECT_ID(N'[dbo].[VarOfInterest]', 'U') IS NOT NULL
    DROP TABLE [dbo].[VarOfInterest];
GO
IF OBJECT_ID(N'[dbo].[NamedPhenotype]', 'U') IS NOT NULL
    DROP TABLE [dbo].[NamedPhenotype];
GO
IF OBJECT_ID(N'[dbo].[PhenotypeDef]', 'U') IS NOT NULL
    DROP TABLE [dbo].[PhenotypeDef];
GO
IF OBJECT_ID(N'[dbo].[PhenotypePopFreq]', 'U') IS NOT NULL
    DROP TABLE [dbo].[PhenotypePopFreq];
GO
IF OBJECT_ID(N'[dbo].[Patient]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Patient];
GO
IF OBJECT_ID(N'[dbo].[DiplotypeResult]', 'U') IS NOT NULL
    DROP TABLE [dbo].[DiplotypeResult];
GO
IF OBJECT_ID(N'[dbo].[ClinicalRelevance]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ClinicalRelevance];
GO
IF OBJECT_ID(N'[dbo].[RecommendationPhenotype]', 'U') IS NOT NULL
    DROP TABLE [dbo].[RecommendationPhenotype];
GO
IF OBJECT_ID(N'[dbo].[EFTest]', 'U') IS NOT NULL
    DROP TABLE [dbo].[EFTest];
GO
IF OBJECT_ID(N'[dbo].[PGxGuidelineRelatedGene]', 'U') IS NOT NULL
    DROP TABLE [dbo].[PGxGuidelineRelatedGene];
GO
IF OBJECT_ID(N'[dbo].[PGxGuidelineDefinitionFile]', 'U') IS NOT NULL
    DROP TABLE [dbo].[PGxGuidelineDefinitionFile];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'RAW_DATA_FILE'
CREATE TABLE [dbo].[RAW_DATA_FILE] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [PatientName] nvarchar(max)  NULL,
    [SampleCode] nvarchar(max)  NULL,
    [FILE_PATH] nvarchar(max)  NULL,
    [TYPE] nvarchar(max)  NULL,
    [DESCRIPTION] nvarchar(max)  NULL,
    [TimeStamp] datetime  NULL,
    [Method] nvarchar(max)  NULL,
    [Laboratory] nvarchar(max)  NULL,
    [Sex] bit  NULL,
    [PatientID] nvarchar(max)  NULL,
    [FileType] nvarchar(max)  NULL
);
GO

-- Creating table 'DefinitionFile'
CREATE TABLE [dbo].[DefinitionFile] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [FormatVersion] nvarchar(max)  NULL,
    [ModificationDate] datetime  NOT NULL,
    [GeneSymbol] nvarchar(max)  NULL,
    [Orientation] nvarchar(max)  NULL,
    [Chromosome] nvarchar(max)  NULL,
    [GenomeBuild] nvarchar(max)  NULL,
    [RefSeqChromosome] nvarchar(max)  NULL,
    [RefSeqGene] nvarchar(max)  NULL,
    [RefSeqProtein] nvarchar(max)  NULL,
    [Populations] nvarchar(max)  NULL,
    [GeneName] nvarchar(max)  NULL,
    [NamedFunctions] nvarchar(max)  NULL,
    [AlleleFrequencyTable] nvarchar(max)  NULL,
    [FunctionTable] nvarchar(max)  NULL,
    [AlleleDefinitionTable] nvarchar(max)  NULL,
    [DiplotypePhenotypeTable] nvarchar(max)  NULL,
    [IsAvailable] nvarchar(max)  NULL
);
GO

-- Creating table 'VariantLocus'
CREATE TABLE [dbo].[VariantLocus] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [Chromosome] nvarchar(max)  NULL,
    [Position] int  NOT NULL,
    [Rsid] nvarchar(max)  NULL,
    [ChromosomeHgvsName] nvarchar(max)  NULL,
    [GeneHgvsName] nvarchar(max)  NULL,
    [ProteinNote] nvarchar(max)  NULL,
    [ResourceNote] nvarchar(max)  NULL,
    [Type] nvarchar(max)  NULL,
    [ReferenceRepeat] nvarchar(max)  NULL,
    [DefinitionFileID] int  NULL,
    [Alleles] nvarchar(max)  NULL,
    [DefinitionExemptionID] int  NULL,
    [TranscriptName] nvarchar(max)  NULL
);
GO

-- Creating table 'NamedAllele'
CREATE TABLE [dbo].[NamedAllele] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NULL,
    [M_Id] nvarchar(max)  NULL,
    [M_Function] nvarchar(max)  NULL,
    [DefinitionFileID] int  NULL,
    [IsRefAllele] bit  NULL,
    [ActivityValue] float  NULL
);
GO

-- Creating table 'NamedAlleleDefinition'
CREATE TABLE [dbo].[NamedAlleleDefinition] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [Allele] nvarchar(max)  NULL,
    [VariantLocusID] int  NULL,
    [NamedAlleleID] int  NULL
);
GO

-- Creating table 'DefinitionExemption'
CREATE TABLE [dbo].[DefinitionExemption] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [Gene] nvarchar(max)  NOT NULL,
    [AllHits] bit  NOT NULL,
    [AssumeReference] bit  NOT NULL,
    [IgnoredAlleles] nvarchar(max)  NULL,
    [IgnoredAllelesLc] nvarchar(max)  NULL,
    [ModificationTime] datetime  NULL
);
GO

-- Creating table 'PopulationFrequency'
CREATE TABLE [dbo].[PopulationFrequency] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [Population] nvarchar(max)  NOT NULL,
    [NamedAlleleID] int  NOT NULL,
    [Frequency] decimal(18,0)  NULL
);
GO

-- Creating table 'PGxGuideline'
CREATE TABLE [dbo].[PGxGuideline] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [M_id] nvarchar(max)  NULL,
    [Source] nvarchar(max)  NULL,
    [Summary] nvarchar(max)  NULL,
    [GenesInStr] nvarchar(max)  NULL,
    [Chemical] nvarchar(max)  NULL,
    [ChemicalDictID] int  NULL,
    [ClinicalImplication] nvarchar(max)  NULL,
    [IsAvailable] nvarchar(max)  NULL
);
GO

-- Creating table 'RelatedGene'
CREATE TABLE [dbo].[RelatedGene] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [M_id] nvarchar(max)  NULL,
    [Symbol] nvarchar(max)  NULL,
    [Name] nvarchar(max)  NULL
);
GO

-- Creating table 'DosingGuidence'
CREATE TABLE [dbo].[DosingGuidence] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NULL,
    [Phenotype] nvarchar(max)  NULL,
    [Strength] nvarchar(max)  NULL,
    [RxChange] nvarchar(max)  NULL,
    [Implication] nvarchar(max)  NULL,
    [Recommendation] nvarchar(max)  NULL,
    [PGxGuidelineID] int  NOT NULL,
    [Proportion] int  NULL,
    [Additional] nvarchar(max)  NULL,
    [Alternative] nvarchar(max)  NULL,
    [Contraindicated] bit  NULL,
    [ProportionMax] int  NULL,
    [Frequency] nvarchar(max)  NULL,
    [ImpactLevel] nvarchar(max)  NULL
);
GO

-- Creating table 'Literature'
CREATE TABLE [dbo].[Literature] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [M_title] nvarchar(max)  NULL,
    [M_journal] nvarchar(max)  NULL,
    [M_year] int  NULL,
    [M_pmid] nvarchar(max)  NULL,
    [M_authors] nvarchar(max)  NULL,
    [PGxGuidelineID] int  NOT NULL
);
GO

-- Creating table 'GeneCall'
CREATE TABLE [dbo].[GeneCall] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [AlleleDefinitionVersion] nvarchar(max)  NULL,
    [Chromosome] nvarchar(max)  NULL,
    [Gene] nvarchar(max)  NULL,
    [IsPhased] bit  NULL,
    [UncallableHaplotypes] nvarchar(max)  NULL,
    [IgnoredHaplotypes] nvarchar(max)  NULL,
    [FileId] int  NULL,
    [TimeStamp] nvarchar(max)  NULL,
    [IsVcfCall] bit  NULL,
    [RAW_DATA_FILEID] int  NOT NULL,
    [HaplotypeA] nvarchar(max)  NULL,
    [HaplotypeB] nvarchar(max)  NULL,
    [Diplotype] nvarchar(max)  NULL,
    [Phenotype] nvarchar(max)  NULL
);
GO

-- Creating table 'DiplotypeMatch'
CREATE TABLE [dbo].[DiplotypeMatch] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NULL,
    [Score] int  NULL,
    [GeneCallID] int  NOT NULL,
    [Phenotype] nvarchar(max)  NULL
);
GO

-- Creating table 'HaplotypeMatch'
CREATE TABLE [dbo].[HaplotypeMatch] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NULL,
    [M_function] nvarchar(max)  NULL,
    [Sequences] nvarchar(max)  NULL,
    [GeneCallID] int  NOT NULL
);
GO

-- Creating table 'Variant'
CREATE TABLE [dbo].[Variant] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [Position] int  NOT NULL,
    [Rsid] nvarchar(max)  NULL,
    [VcfCall] nvarchar(max)  NULL,
    [IsPhased] bit  NULL,
    [VcfPosition] int  NULL,
    [VcfAlleles] nvarchar(max)  NULL,
    [GeneCallID] int  NOT NULL
);
GO

-- Creating table 'VarOfInterest'
CREATE TABLE [dbo].[VarOfInterest] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [Position] int  NOT NULL,
    [Rsid] nvarchar(max)  NULL,
    [VcfCall] nvarchar(max)  NULL,
    [IsPhased] bit  NULL,
    [VcfPosition] int  NULL,
    [VcfAlleles] nvarchar(max)  NULL,
    [GeneCallID] int  NOT NULL
);
GO

-- Creating table 'NamedPhenotype'
CREATE TABLE [dbo].[NamedPhenotype] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [Symbol] nvarchar(max)  NULL,
    [Phenotype] nvarchar(max)  NULL,
    [Description] nvarchar(max)  NULL,
    [DefinitionFileID] int  NOT NULL,
    [GenotypeList] nvarchar(max)  NULL,
    [ActivityScore] nvarchar(max)  NULL
);
GO

-- Creating table 'PhenotypeDef'
CREATE TABLE [dbo].[PhenotypeDef] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [NamedPhenotypeID] int  NOT NULL,
    [Genotype] nvarchar(max)  NULL,
    [FunctionA] nvarchar(max)  NULL,
    [FunctionB] nvarchar(max)  NULL,
    [GeneSymbol] nvarchar(max)  NULL,
    [Phenotype] nvarchar(max)  NULL
);
GO

-- Creating table 'PhenotypePopFreq'
CREATE TABLE [dbo].[PhenotypePopFreq] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [Population] nvarchar(max)  NULL,
    [Frequency] decimal(18,3)  NULL,
    [NamedPhenotypeID] int  NOT NULL
);
GO

-- Creating table 'Patient'
CREATE TABLE [dbo].[Patient] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [PatientID] nvarchar(max)  NULL,
    [Name] nvarchar(max)  NULL,
    [Sex] bit  NULL,
    [Age] int  NULL,
    [Race] nvarchar(max)  NULL,
    [Weight] decimal(18,1)  NULL,
    [Height] int  NULL,
    [BirthDay] datetime  NULL,
    [Nation] nvarchar(max)  NULL,
    [TimeStamp] datetime  NULL
);
GO

-- Creating table 'DiplotypeResult'
CREATE TABLE [dbo].[DiplotypeResult] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [GeneSymbol] nvarchar(max)  NULL,
    [Diplotype] nvarchar(max)  NULL,
    [PHenotype] nvarchar(max)  NULL,
    [AlleleA] nvarchar(max)  NULL,
    [AlleleB] nvarchar(max)  NULL,
    [RAW_DATA_FILEID] int  NOT NULL
);
GO

-- Creating table 'ClinicalRelevance'
CREATE TABLE [dbo].[ClinicalRelevance] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [Level] nvarchar(max)  NULL,
    [Description] nvarchar(max)  NULL,
    [Example] nvarchar(max)  NULL,
    [Combination] nvarchar(max)  NULL
);
GO

-- Creating table 'RecommendationPhenotype'
CREATE TABLE [dbo].[RecommendationPhenotype] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [Gene] nvarchar(max)  NULL,
    [Phenotype] nvarchar(max)  NULL,
    [DosingGuidenceID] int  NOT NULL
);
GO

-- Creating table 'EFTest'
CREATE TABLE [dbo].[EFTest] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [Property1] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'PGxGuidelineRelatedGene'
CREATE TABLE [dbo].[PGxGuidelineRelatedGene] (
    [PGxGuideline_ID] int  NOT NULL,
    [RelatedGene_ID] int  NOT NULL
);
GO

-- Creating table 'PGxGuidelineDefinitionFile'
CREATE TABLE [dbo].[PGxGuidelineDefinitionFile] (
    [PGxGuideline_ID] int  NOT NULL,
    [DefinitionFile_ID] int  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [ID] in table 'RAW_DATA_FILE'
ALTER TABLE [dbo].[RAW_DATA_FILE]
ADD CONSTRAINT [PK_RAW_DATA_FILE]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'DefinitionFile'
ALTER TABLE [dbo].[DefinitionFile]
ADD CONSTRAINT [PK_DefinitionFile]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'VariantLocus'
ALTER TABLE [dbo].[VariantLocus]
ADD CONSTRAINT [PK_VariantLocus]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'NamedAllele'
ALTER TABLE [dbo].[NamedAllele]
ADD CONSTRAINT [PK_NamedAllele]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'NamedAlleleDefinition'
ALTER TABLE [dbo].[NamedAlleleDefinition]
ADD CONSTRAINT [PK_NamedAlleleDefinition]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'DefinitionExemption'
ALTER TABLE [dbo].[DefinitionExemption]
ADD CONSTRAINT [PK_DefinitionExemption]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'PopulationFrequency'
ALTER TABLE [dbo].[PopulationFrequency]
ADD CONSTRAINT [PK_PopulationFrequency]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'PGxGuideline'
ALTER TABLE [dbo].[PGxGuideline]
ADD CONSTRAINT [PK_PGxGuideline]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'RelatedGene'
ALTER TABLE [dbo].[RelatedGene]
ADD CONSTRAINT [PK_RelatedGene]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'DosingGuidence'
ALTER TABLE [dbo].[DosingGuidence]
ADD CONSTRAINT [PK_DosingGuidence]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'Literature'
ALTER TABLE [dbo].[Literature]
ADD CONSTRAINT [PK_Literature]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'GeneCall'
ALTER TABLE [dbo].[GeneCall]
ADD CONSTRAINT [PK_GeneCall]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'DiplotypeMatch'
ALTER TABLE [dbo].[DiplotypeMatch]
ADD CONSTRAINT [PK_DiplotypeMatch]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'HaplotypeMatch'
ALTER TABLE [dbo].[HaplotypeMatch]
ADD CONSTRAINT [PK_HaplotypeMatch]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'Variant'
ALTER TABLE [dbo].[Variant]
ADD CONSTRAINT [PK_Variant]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'VarOfInterest'
ALTER TABLE [dbo].[VarOfInterest]
ADD CONSTRAINT [PK_VarOfInterest]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'NamedPhenotype'
ALTER TABLE [dbo].[NamedPhenotype]
ADD CONSTRAINT [PK_NamedPhenotype]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'PhenotypeDef'
ALTER TABLE [dbo].[PhenotypeDef]
ADD CONSTRAINT [PK_PhenotypeDef]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'PhenotypePopFreq'
ALTER TABLE [dbo].[PhenotypePopFreq]
ADD CONSTRAINT [PK_PhenotypePopFreq]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'Patient'
ALTER TABLE [dbo].[Patient]
ADD CONSTRAINT [PK_Patient]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'DiplotypeResult'
ALTER TABLE [dbo].[DiplotypeResult]
ADD CONSTRAINT [PK_DiplotypeResult]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'ClinicalRelevance'
ALTER TABLE [dbo].[ClinicalRelevance]
ADD CONSTRAINT [PK_ClinicalRelevance]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'RecommendationPhenotype'
ALTER TABLE [dbo].[RecommendationPhenotype]
ADD CONSTRAINT [PK_RecommendationPhenotype]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'EFTest'
ALTER TABLE [dbo].[EFTest]
ADD CONSTRAINT [PK_EFTest]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [PGxGuideline_ID], [RelatedGene_ID] in table 'PGxGuidelineRelatedGene'
ALTER TABLE [dbo].[PGxGuidelineRelatedGene]
ADD CONSTRAINT [PK_PGxGuidelineRelatedGene]
    PRIMARY KEY NONCLUSTERED ([PGxGuideline_ID], [RelatedGene_ID] ASC);
GO

-- Creating primary key on [PGxGuideline_ID], [DefinitionFile_ID] in table 'PGxGuidelineDefinitionFile'
ALTER TABLE [dbo].[PGxGuidelineDefinitionFile]
ADD CONSTRAINT [PK_PGxGuidelineDefinitionFile]
    PRIMARY KEY NONCLUSTERED ([PGxGuideline_ID], [DefinitionFile_ID] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [DefinitionFileID] in table 'VariantLocus'
ALTER TABLE [dbo].[VariantLocus]
ADD CONSTRAINT [FK_DefinitionFileVariantLocus]
    FOREIGN KEY ([DefinitionFileID])
    REFERENCES [dbo].[DefinitionFile]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_DefinitionFileVariantLocus'
CREATE INDEX [IX_FK_DefinitionFileVariantLocus]
ON [dbo].[VariantLocus]
    ([DefinitionFileID]);
GO

-- Creating foreign key on [VariantLocusID] in table 'NamedAlleleDefinition'
ALTER TABLE [dbo].[NamedAlleleDefinition]
ADD CONSTRAINT [FK_VariantLocusAlleleDefinition]
    FOREIGN KEY ([VariantLocusID])
    REFERENCES [dbo].[VariantLocus]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_VariantLocusAlleleDefinition'
CREATE INDEX [IX_FK_VariantLocusAlleleDefinition]
ON [dbo].[NamedAlleleDefinition]
    ([VariantLocusID]);
GO

-- Creating foreign key on [DefinitionFileID] in table 'NamedAllele'
ALTER TABLE [dbo].[NamedAllele]
ADD CONSTRAINT [FK_DefinitionFileNamedAllele]
    FOREIGN KEY ([DefinitionFileID])
    REFERENCES [dbo].[DefinitionFile]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_DefinitionFileNamedAllele'
CREATE INDEX [IX_FK_DefinitionFileNamedAllele]
ON [dbo].[NamedAllele]
    ([DefinitionFileID]);
GO

-- Creating foreign key on [NamedAlleleID] in table 'NamedAlleleDefinition'
ALTER TABLE [dbo].[NamedAlleleDefinition]
ADD CONSTRAINT [FK_NamedAlleleNamedAlleleDefinition]
    FOREIGN KEY ([NamedAlleleID])
    REFERENCES [dbo].[NamedAllele]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_NamedAlleleNamedAlleleDefinition'
CREATE INDEX [IX_FK_NamedAlleleNamedAlleleDefinition]
ON [dbo].[NamedAlleleDefinition]
    ([NamedAlleleID]);
GO

-- Creating foreign key on [DefinitionExemptionID] in table 'VariantLocus'
ALTER TABLE [dbo].[VariantLocus]
ADD CONSTRAINT [FK_DefinitionExemptionVariantLocus]
    FOREIGN KEY ([DefinitionExemptionID])
    REFERENCES [dbo].[DefinitionExemption]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_DefinitionExemptionVariantLocus'
CREATE INDEX [IX_FK_DefinitionExemptionVariantLocus]
ON [dbo].[VariantLocus]
    ([DefinitionExemptionID]);
GO

-- Creating foreign key on [NamedAlleleID] in table 'PopulationFrequency'
ALTER TABLE [dbo].[PopulationFrequency]
ADD CONSTRAINT [FK_NamedAllelePopulationFrequency]
    FOREIGN KEY ([NamedAlleleID])
    REFERENCES [dbo].[NamedAllele]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_NamedAllelePopulationFrequency'
CREATE INDEX [IX_FK_NamedAllelePopulationFrequency]
ON [dbo].[PopulationFrequency]
    ([NamedAlleleID]);
GO

-- Creating foreign key on [GeneCallID] in table 'DiplotypeMatch'
ALTER TABLE [dbo].[DiplotypeMatch]
ADD CONSTRAINT [FK_GeneCallDiplotypeMatch]
    FOREIGN KEY ([GeneCallID])
    REFERENCES [dbo].[GeneCall]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_GeneCallDiplotypeMatch'
CREATE INDEX [IX_FK_GeneCallDiplotypeMatch]
ON [dbo].[DiplotypeMatch]
    ([GeneCallID]);
GO

-- Creating foreign key on [GeneCallID] in table 'HaplotypeMatch'
ALTER TABLE [dbo].[HaplotypeMatch]
ADD CONSTRAINT [FK_GeneCallHaplotypeMatch]
    FOREIGN KEY ([GeneCallID])
    REFERENCES [dbo].[GeneCall]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_GeneCallHaplotypeMatch'
CREATE INDEX [IX_FK_GeneCallHaplotypeMatch]
ON [dbo].[HaplotypeMatch]
    ([GeneCallID]);
GO

-- Creating foreign key on [GeneCallID] in table 'Variant'
ALTER TABLE [dbo].[Variant]
ADD CONSTRAINT [FK_GeneCallVariant1]
    FOREIGN KEY ([GeneCallID])
    REFERENCES [dbo].[GeneCall]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_GeneCallVariant1'
CREATE INDEX [IX_FK_GeneCallVariant1]
ON [dbo].[Variant]
    ([GeneCallID]);
GO

-- Creating foreign key on [GeneCallID] in table 'VarOfInterest'
ALTER TABLE [dbo].[VarOfInterest]
ADD CONSTRAINT [FK_GeneCallVarOfInterest]
    FOREIGN KEY ([GeneCallID])
    REFERENCES [dbo].[GeneCall]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_GeneCallVarOfInterest'
CREATE INDEX [IX_FK_GeneCallVarOfInterest]
ON [dbo].[VarOfInterest]
    ([GeneCallID]);
GO

-- Creating foreign key on [PGxGuidelineID] in table 'DosingGuidence'
ALTER TABLE [dbo].[DosingGuidence]
ADD CONSTRAINT [FK_PGxGuidelineDosingGuidence]
    FOREIGN KEY ([PGxGuidelineID])
    REFERENCES [dbo].[PGxGuideline]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_PGxGuidelineDosingGuidence'
CREATE INDEX [IX_FK_PGxGuidelineDosingGuidence]
ON [dbo].[DosingGuidence]
    ([PGxGuidelineID]);
GO

-- Creating foreign key on [PGxGuidelineID] in table 'Literature'
ALTER TABLE [dbo].[Literature]
ADD CONSTRAINT [FK_PGxGuidelineLiterature]
    FOREIGN KEY ([PGxGuidelineID])
    REFERENCES [dbo].[PGxGuideline]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_PGxGuidelineLiterature'
CREATE INDEX [IX_FK_PGxGuidelineLiterature]
ON [dbo].[Literature]
    ([PGxGuidelineID]);
GO

-- Creating foreign key on [PGxGuideline_ID] in table 'PGxGuidelineRelatedGene'
ALTER TABLE [dbo].[PGxGuidelineRelatedGene]
ADD CONSTRAINT [FK_PGxGuidelineRelatedGene_PGxGuideline]
    FOREIGN KEY ([PGxGuideline_ID])
    REFERENCES [dbo].[PGxGuideline]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [RelatedGene_ID] in table 'PGxGuidelineRelatedGene'
ALTER TABLE [dbo].[PGxGuidelineRelatedGene]
ADD CONSTRAINT [FK_PGxGuidelineRelatedGene_RelatedGene]
    FOREIGN KEY ([RelatedGene_ID])
    REFERENCES [dbo].[RelatedGene]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_PGxGuidelineRelatedGene_RelatedGene'
CREATE INDEX [IX_FK_PGxGuidelineRelatedGene_RelatedGene]
ON [dbo].[PGxGuidelineRelatedGene]
    ([RelatedGene_ID]);
GO

-- Creating foreign key on [DefinitionFileID] in table 'NamedPhenotype'
ALTER TABLE [dbo].[NamedPhenotype]
ADD CONSTRAINT [FK_DefinitionFilePhenotypeMap]
    FOREIGN KEY ([DefinitionFileID])
    REFERENCES [dbo].[DefinitionFile]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_DefinitionFilePhenotypeMap'
CREATE INDEX [IX_FK_DefinitionFilePhenotypeMap]
ON [dbo].[NamedPhenotype]
    ([DefinitionFileID]);
GO

-- Creating foreign key on [NamedPhenotypeID] in table 'PhenotypeDef'
ALTER TABLE [dbo].[PhenotypeDef]
ADD CONSTRAINT [FK_NamedPhenotypePhenotypeDef]
    FOREIGN KEY ([NamedPhenotypeID])
    REFERENCES [dbo].[NamedPhenotype]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_NamedPhenotypePhenotypeDef'
CREATE INDEX [IX_FK_NamedPhenotypePhenotypeDef]
ON [dbo].[PhenotypeDef]
    ([NamedPhenotypeID]);
GO

-- Creating foreign key on [NamedPhenotypeID] in table 'PhenotypePopFreq'
ALTER TABLE [dbo].[PhenotypePopFreq]
ADD CONSTRAINT [FK_PhenotypePopFreqNamedPhenotype]
    FOREIGN KEY ([NamedPhenotypeID])
    REFERENCES [dbo].[NamedPhenotype]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_PhenotypePopFreqNamedPhenotype'
CREATE INDEX [IX_FK_PhenotypePopFreqNamedPhenotype]
ON [dbo].[PhenotypePopFreq]
    ([NamedPhenotypeID]);
GO

-- Creating foreign key on [PGxGuideline_ID] in table 'PGxGuidelineDefinitionFile'
ALTER TABLE [dbo].[PGxGuidelineDefinitionFile]
ADD CONSTRAINT [FK_PGxGuidelineDefinitionFile_PGxGuideline]
    FOREIGN KEY ([PGxGuideline_ID])
    REFERENCES [dbo].[PGxGuideline]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [DefinitionFile_ID] in table 'PGxGuidelineDefinitionFile'
ALTER TABLE [dbo].[PGxGuidelineDefinitionFile]
ADD CONSTRAINT [FK_PGxGuidelineDefinitionFile_DefinitionFile]
    FOREIGN KEY ([DefinitionFile_ID])
    REFERENCES [dbo].[DefinitionFile]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_PGxGuidelineDefinitionFile_DefinitionFile'
CREATE INDEX [IX_FK_PGxGuidelineDefinitionFile_DefinitionFile]
ON [dbo].[PGxGuidelineDefinitionFile]
    ([DefinitionFile_ID]);
GO

-- Creating foreign key on [RAW_DATA_FILEID] in table 'DiplotypeResult'
ALTER TABLE [dbo].[DiplotypeResult]
ADD CONSTRAINT [FK_RAW_DATA_FILEDiplotypeResult]
    FOREIGN KEY ([RAW_DATA_FILEID])
    REFERENCES [dbo].[RAW_DATA_FILE]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_RAW_DATA_FILEDiplotypeResult'
CREATE INDEX [IX_FK_RAW_DATA_FILEDiplotypeResult]
ON [dbo].[DiplotypeResult]
    ([RAW_DATA_FILEID]);
GO

-- Creating foreign key on [RAW_DATA_FILEID] in table 'GeneCall'
ALTER TABLE [dbo].[GeneCall]
ADD CONSTRAINT [FK_RAW_DATA_FILEGeneCall]
    FOREIGN KEY ([RAW_DATA_FILEID])
    REFERENCES [dbo].[RAW_DATA_FILE]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_RAW_DATA_FILEGeneCall'
CREATE INDEX [IX_FK_RAW_DATA_FILEGeneCall]
ON [dbo].[GeneCall]
    ([RAW_DATA_FILEID]);
GO

-- Creating foreign key on [DosingGuidenceID] in table 'RecommendationPhenotype'
ALTER TABLE [dbo].[RecommendationPhenotype]
ADD CONSTRAINT [FK_DosingGuidenceRecommendationPhenotype]
    FOREIGN KEY ([DosingGuidenceID])
    REFERENCES [dbo].[DosingGuidence]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_DosingGuidenceRecommendationPhenotype'
CREATE INDEX [IX_FK_DosingGuidenceRecommendationPhenotype]
ON [dbo].[RecommendationPhenotype]
    ([DosingGuidenceID]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------