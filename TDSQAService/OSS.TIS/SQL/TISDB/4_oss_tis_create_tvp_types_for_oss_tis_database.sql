﻿USE OSS_TIS
GO
IF EXISTS(SELECT * FROM sys.types WHERE is_table_type = 1 AND name = 'TestNameLookupTable')
BEGIN
	DROP TYPE dbo.TestNameLookupTable;
END
GO
CREATE TYPE dbo.TestNameLookupTable AS TABLE
(
	InstanceName	varchar(50) NOT NULL,
	TestName		varchar(50) NOT NULL
)
GO
GRANT CONTROL ON TYPE::dbo.TestNameLookupTable TO public
GO