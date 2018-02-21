﻿USE OSS_TestScoringConfigs
GO
IF EXISTS(SELECT * FROM sys.types WHERE is_table_type = 1 AND name = 'TestTable')
BEGIN
	DROP TYPE dbo.TestTable
END
GO
CREATE TYPE dbo.TestTable AS TABLE
(
	ClientName	varchar(100) NOT NULL,
	TestId		varchar(150) NOT NULL,
	[Subject]	varchar(100)
)
GO
GRANT CONTROL ON TYPE::dbo.TestTable TO public
GO
IF EXISTS(SELECT * FROM sys.types WHERE is_table_type = 1 AND name = 'TestGradeTable')
BEGIN
	DROP TYPE dbo.TestGradeTable
END
GO
CREATE TYPE dbo.TestGradeTable AS TABLE
(
	ClientName		varchar(100) NOT NULL,
	TestId			varchar(150) NOT NULL,
	ReportingGrade	varchar(100) NOT NULL
)
GO
GRANT CONTROL ON TYPE::dbo.TestGradeTable TO public
GO
IF EXISTS(SELECT * FROM sys.types WHERE is_table_type = 1 AND name = 'TestScoreFeatureTable')
BEGIN
	DROP TYPE dbo.TestScoreFeatureTable
END
GO
CREATE TYPE dbo.TestScoreFeatureTable AS TABLE
(
	TestScoreFeatureKey	uniqueidentifier NOT NULL,
	ClientName			varchar(100) NOT NULL,
	TestId				varchar(150) NOT NULL,
	MeasureOf			varchar(150) NOT NULL,
	MeasureLabel		varchar(150) NOT NULL,
	IsScaled			bit NOT NULL,
	ComputationRule		varchar(255) NOT NULL,
	ComputationOrder	int NOT NULL
)
GO
GRANT CONTROL ON TYPE::dbo.TestScoreFeatureTable TO public
GO
IF EXISTS(SELECT * FROM sys.types WHERE is_table_type = 1 AND name = 'FeatureComputationLocationTable')
BEGIN
	DROP TYPE dbo.FeatureComputationLocationTable
END
GO
CREATE TYPE dbo.FeatureComputationLocationTable AS TABLE
(
	TestScoreFeatureKey	uniqueidentifier NOT NULL,
	[Location]			varchar(50) NOT NULL
)
GO
GRANT CONTROL ON TYPE::dbo.FeatureComputationLocationTable TO public
GO