﻿using System.Collections.Generic;
using TDSQASystemAPI.DAL.configs.dtos;

namespace TDSQASystemAPI.DAL.configs.daos
{
    /// <summary>
    /// A class for saving <code>TestGradeDTO</code>s to the <code>OSS_Configs..Client_TestGrade</code> table
    /// </summary>
    public class TestGradesDAO : TestPackageDaoBase<TestGradeDTO>
    {
        public TestGradesDAO()
        {
            DbConnectionStringName = DatabaseConnectionStringNames.CONFIGS;
            TvpType = "TestGradeTable";
            InsertSql =
                "INSERT \n" +
                "   dbo.Client_TestGrades (ClientName, TestId, Grade) \n" +
                "SELECT \n" +
                "   ClientName, \n" +
                "   TestId, \n" +
                "   Grade \n";
        }
    }
}
