﻿using System.Collections.Generic;
using System.Configuration;
using MySql.Data.MySqlClient;
using TDSQASystemAPI.DAL.itembank.dtos;

namespace TDSQASystemAPI.DAL.itembank.daos
{
    /// <summary>
    /// A class for saving <code>AdminStimulusDTO</code>s to the <code>OSS_Itembank..tblAdminStimulsu</code> table
    /// </summary>
    public class TestFormDAO : TestPackageDaoBase<TestFormDTO>
    {
        public TestFormDAO()
        {
            DbConnectionStringName = DatabaseConnectionStringNames.ITEMBANK;
            TvpType = "TestFormTable";
            InsertSql =
                "INSERT \n" +
                "   dbo.TestForm (_fk_AdminSubject, Cohort, Language, _Key, FormID, _efk_ITSBank, _efk_ITSKey, LoadConfig) \n" +
                "SELECT \n" +
                "   SegmentKey, \n" +
                "   Cohort, \n" +
                "   [Language], \n" +
                "   TestFormKey, \n" +
                "   FormId, \n" +
                "   ITSBankKey, \n" +
                "   ITSKey, \n" +
                "   TestVersion \n";
            SelectSql =
                "SELECT \n" +
                "    segments._key AS segmentKey, \n" +
                "    forms._efk_itsbank AS itsBankKey, \n" +
                "    forms._efk_itskey AS itsKey, \n" +
                "    forms._key AS `key`, \n" +
                "    forms.formid AS id, \n" +
                "    forms.language, \n" +
                "    forms.loadconfig AS loadVersion, \n" +
                "    forms.cohort \n" +
                "FROM \n" +
                "   itembank.tblsetofadminsubjects segments \n" +
                "JOIN \n" +
                "   itembank.testform forms \n" +
                "   ON segments._key = forms._fk_adminsubject \n" +
                "WHERE \n" +
                "   segments.virtualtest = @criteria \n" +
                "   OR segments._key = @criteria";
        }

        /// <summary>
        /// Collect the <code>TestFormDTO</code>s for an <code>Assessment</code> from TDS.
        /// </summary>
        /// <remarks>
        /// When TDS loads a test package, the identifier for test forms is generated by the server prior to persisting
        /// them to the database.  Since TIS also has to persist test forms to its database, it needs to know what
        /// identifiers were generated so TIS and TDS can be in synch (that is, the test forms in TDS and TIS have the
        /// same identifiers).  Since there is no publicly exposed HTTP endpoint to fetch this information, TIS must
        /// query TDS's <code>itembank</code> database to fetch the test form data.
        /// </remarks>
        /// <param name="criteria">The unique identifier of the <code>Assessment</code> or <code>AssessmentSegment</code>
        /// (in the case of multi-segmented assessments) that needs to have test forms collected.
        /// loaded.</param>
        /// <returns>A collection of <code>TestFormDTO</code> records from <code>itembank.testform</code> in TDS for the
        /// specified assessment/segment key.</returns>
        public override List<TestFormDTO> Find(object criteria)
        {
            var testForms = new List<TestFormDTO>();

            using (var connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["tds_itembank"].ConnectionString))
            {
                using (var command = new MySqlCommand(SelectSql, connection))
                {
                    command.CommandType = System.Data.CommandType.Text;
                    command.Parameters.AddWithValue("@criteria", criteria);

                    connection.Open();
                    using (var result = command.ExecuteReader())
                    {
                        while (result.Read())
                        {
                            testForms.Add(new TestFormDTO
                            {
                                SegmentKey = result.GetString("segmentKey"),
                                ITSBankKey = result.GetInt64("itsBankKey"),
                                ITSKey = result.GetInt64("itsKey"),
                                TestFormKey = result.GetString("key"),
                                FormId = result.GetString("id"),
                                Language = result.GetString("language"),
                                TestVersion = result.GetInt64("loadVersion"),
                                Cohort = result.GetString("cohort") 
                            });
                        }
                    }
                }  
            }

            return testForms;
        }
    }
}
