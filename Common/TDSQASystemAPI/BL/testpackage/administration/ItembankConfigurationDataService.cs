﻿using System;
using System.Collections.Generic;
using System.Linq;
using TDSQASystemAPI.DAL;
using TDSQASystemAPI.DAL.itembank.daos;
using TDSQASystemAPI.DAL.itembank.dtos;
using TDSQASystemAPI.TestPackage;

namespace TDSQASystemAPI.BL.testpackage.administration
{
    public class ItembankConfigurationDataService : IItembankConfigurationDataService
    {
        private readonly string[] CLAIM_AND_TARGET_TYPES = new string[] { "strand", "contentlevel", "claim" };
        private const string ITEM_FILE_NAME_PATTERN = "item-{0}-{1}.xml";
        private const string ITEM_FILE_PATH_PATTERN = "item-{0}-{1}/";
        private const string STIMULUS_FILE_NAME_PATTERN = "stim-{0}-{1}.xml";
        private const string STIMULUS_FILE_PATH_PATTERN = "stim-{0}-{1}/";

        private readonly ITestPackageDao<SubjectDTO> subjectDAO;
        private readonly ITestPackageDao<ClientDTO> clientDAO;
        private readonly ITestPackageDao<StrandDTO> strandDAO;
        private readonly ITestPackageDao<ItemDTO> itemDAO;
        private readonly ITestPackageDao<StimulusDTO> stimuliDAO;

        /// <summary>
        /// Default constructor to create new <code>ITestPackageDao<SubjectDTO></code> and
        /// <code>ITestPackageDao<ClientDTO></code> since a dependency injection container has not been 
        /// implemented.
        /// </summary>
        public ItembankConfigurationDataService()
        {
            subjectDAO = new SubjectDAO();
            clientDAO = new ClientDAO();
            strandDAO = new StrandDAO();
            itemDAO = new ItemDAO();
            stimuliDAO = new StimulusDAO();
        }

        public ItembankConfigurationDataService(ITestPackageDao<SubjectDTO> subjectDAO, 
                                                ITestPackageDao<ClientDTO> clientDAO,
                                                ITestPackageDao<StrandDTO> strandDAO,
                                                ITestPackageDao<ItemDTO> itemDAO,
                                                ITestPackageDao<StimulusDTO> stimuliDAO)
        {
            this.subjectDAO = subjectDAO;
            this.clientDAO = clientDAO;
            this.strandDAO = strandDAO;
            this.itemDAO = itemDAO;
            this.stimuliDAO = stimuliDAO;
        }

        public void CreateItems(TestPackage.TestPackage testPackage)
        {
            var allSegments = from a in testPackage.Assessment
                              from s in a.Segments
                              select s;

            // Collect all the items, regardless of their source (i.e. either a form or an item pool).
            List<ItemDTO> allItems = new List<ItemDTO>();
            foreach (var segment in allSegments)
            {
                if (segment.Item is AssessmentSegmentSegmentForms)
                {
                    foreach (var form in (segment.Item as AssessmentSegmentSegmentForms).SegmentForm)
                    {
                        var items = from ig in form.ItemGroup
                                    from item in ig.Item
                                    select new ItemDTO
                                    {
                                        ItemBankKey = testPackage.bankKey,
                                        ItemKey = long.Parse(item.id),
                                        ScorePoints = item.ItemScoreDimension.scorePoints,
                                        DateLastUpdated = DateTime.Now,
                                        FileName = string.Format(ITEM_FILE_NAME_PATTERN, testPackage.bankKey, item.id),
                                        FilePath = string.Format(ITEM_FILE_PATH_PATTERN, testPackage.bankKey, item.id),
                                        ItemType = item.type,
                                        Key = string.Format("{0}-{1}", testPackage.bankKey, item.id),
                                        TestVersion = (long)testPackage.version
                                    };
                        if (items.Any())
                        {
                            allItems.AddRange(items);
                        }
                    }
                }
                else
                {
                    var pool = segment.Item as AssessmentSegmentPool;
                    var items = from ig in pool.ItemGroup
                                from item in ig.Item
                                select new ItemDTO
                                {
                                    ItemBankKey = testPackage.bankKey,
                                    ItemKey = long.Parse(item.id),
                                    ScorePoints = item.ItemScoreDimension.scorePoints,
                                    DateLastUpdated = DateTime.Now,
                                    FileName = string.Format(ITEM_FILE_NAME_PATTERN, testPackage.bankKey, item.id),
                                    FilePath = string.Format(ITEM_FILE_PATH_PATTERN, testPackage.bankKey, item.id),
                                    ItemType = item.type,
                                    Key = string.Format("{0}-{1}", testPackage.bankKey, item.id),
                                    TestVersion = (long)testPackage.version
                                };
                    if (items.Any())
                    {
                        allItems.AddRange(items);
                    }
                }

                itemDAO.Insert(allItems);
            }
        }

        public void CreateStimuli(TestPackage.TestPackage testPackage)
        {
            var allSegments = from a in testPackage.Assessment
                              from s in a.Segments
                              select s;

            // Collect all the stimuli, regardless of their source (i.e. either a form or an item pool).
            List<StimulusDTO> allStimuli = new List<StimulusDTO>();
            foreach (var segment in allSegments)
            {
                if (segment.Item is AssessmentSegmentSegmentForms)
                {
                    foreach (var form in (segment.Item as AssessmentSegmentSegmentForms).SegmentForm)
                    {
                        var stimuli = from ig in form.ItemGroup
                                      where ig.Stimulus != null
                                      select new StimulusDTO
                                      {
                                          ItemBankKey = testPackage.bankKey,
                                          ItsKey = long.Parse(ig.Stimulus.id),
                                          TestVersion = (long)testPackage.version,
                                          FileName = string.Format(STIMULUS_FILE_NAME_PATTERN, testPackage.bankKey, ig.Stimulus.id),
                                          FilePath = string.Format(STIMULUS_FILE_PATH_PATTERN, testPackage.bankKey, ig.Stimulus.id),
                                          DateLastUpdated = DateTime.Now,
                                          StimulusKey = string.Format("{0}-{1}", testPackage.bankKey, ig.Stimulus.id)
                                      };
                        if (stimuli.Any())
                        { 
                            allStimuli.AddRange(stimuli);
                        }
                    }
                }
                else
                {
                    var pool = segment.Item as AssessmentSegmentPool;
                    var stimuli = from ig in pool.ItemGroup
                                  where ig.Stimulus != null
                                  select new StimulusDTO
                                  {
                                      ItemBankKey = testPackage.bankKey,
                                      ItsKey = long.Parse(ig.Stimulus.id),
                                      TestVersion = (long)testPackage.version,
                                      FileName = string.Format(STIMULUS_FILE_NAME_PATTERN, testPackage.bankKey, ig.Stimulus.id),
                                      FilePath = string.Format(STIMULUS_FILE_PATH_PATTERN, testPackage.bankKey, ig.Stimulus.id),
                                      DateLastUpdated = DateTime.Now,
                                      StimulusKey = string.Format("{0}-{1}", testPackage.bankKey, ig.Stimulus.id)
                                  };
                    if (stimuli.Any())
                    {
                        allStimuli.AddRange(stimuli);
                    }
                }
            }

            stimuliDAO.Insert(allStimuli);
        }

        public IDictionary<string, StrandDTO> CreateStrands(TestPackage.TestPackage testPackage)
        {
            var subjectKey = string.Format("{0}-{1}", testPackage.publisher, testPackage.subject);

            var subjectList = subjectDAO.Find(subjectKey);
            if (subjectList == null || !subjectList.Any())
            {
                throw new InvalidOperationException(string.Format("Could not find a subject for '{0}'", subjectKey));
            }

            var client = GetClient(testPackage.publisher);

            var blueprintElements = from bp in testPackage.Blueprint
                          select bp;

            var initialTreeLevel = 1;
            var initialParentKey = string.Empty;
            var newStrands = new List<StrandDTO>();

            BuildStrandsWithHierarchyFromBlueprintElements(blueprintElements,
                newStrands,
                client,
                initialParentKey,
                subjectKey,
                (long)testPackage.version,
                initialTreeLevel);

            strandDAO.Insert(newStrands);

            return newStrands.ToDictionary(s => s.Name, s => s);
        }

        public void CreateSubject(TestPackage.TestPackage testPackage)
        {
            var subjectKey = string.Format("{0}-{1}", testPackage.publisher, testPackage.subject);

            // If a subject already exists, exit; there's nothing to do
            var existingSubject = subjectDAO.Find(subjectKey);
            if (existingSubject != null && existingSubject.Any())
            {
                return;
            }

            var client = GetClient(testPackage.publisher);

            var newSubjectDto = new SubjectDTO
            {
                Name = testPackage.subject,
                Grade = string.Empty, // from line 25 of tp.spLoad_Subject
                SubjectKey = subjectKey,
                ClientKey = client.ClientKey, 
                TestVersion = (long)testPackage.version
            };

            subjectDAO.Insert(new List<SubjectDTO> { newSubjectDto });
        }

        /// <summary>
        /// Look up the <code>TestPackage</code>'s client.
        /// </summary>
        /// <param name="clientName">The name of the client to find.  This will stored in the <code>TestPackage.publisher</code> field.</param>
        /// <returns>The <code>ClientDTO</code> representing the client that "owns" this <code>TestPackage</code>.</returns>
        private ClientDTO GetClient(string clientName)
        {
            var clientList = clientDAO.Find(clientName);
            if (clientList == null || !clientList.Any())
            {
                throw new InvalidOperationException(string.Format("Could not find a client record with name '{0}'", clientName));
            }

            return clientList.First();
        }

        /// <summary>
        /// Create a collection of <code>StrandDTO</code>s from the <code>TestPackage</code>'s collection of <code>BlueprintElement</code>s.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method emulates the <code>AssessmentItemBankGenericDataLoaderServiceImpl#loadBlueprintElementsHelper</code> method
        /// in TDS.
        /// </para>
        /// </remarks>
        /// <see cref="https://github.com/SmarterApp/TDS_AssessmentService/blob/1dc03698a2a608437d63b491757d366b1c217abc/service/src/main/java/tds/assessment/services/impl/AssessmentItemBankGenericDataLoaderServiceImpl.java#L124"/>
        /// <param name="blueprintElements">The collection of <code>BlueprintElement</code>s from the test package</param>
        /// <param name="strands">The collection of <code>StrandDTO</code>s that will be built by this method</param>
        /// <param name="client">The <code>ClientDTO</code> representing the client that "owns" this test package</param>
        /// <param name="parentKey">The identifier of the parent strand</param>
        /// <param name="subjectKey">The identifier of the subject to which these <code>StrandDTO</code>s are assigned.  e.g. SBAC_PT-ELA or SBAC_PT-MATH</param>
        /// <param name="testVersion">The version number of the test package being loaded</param>
        /// <param name="treeLevel"></param>
        private void BuildStrandsWithHierarchyFromBlueprintElements(IEnumerable<BlueprintElement> blueprintElements, 
                                                                    IList<StrandDTO> strands, 
                                                                    ClientDTO client, 
                                                                    string parentKey, 
                                                                    string subjectKey, 
                                                                    long testVersion, 
                                                                    int treeLevel)
        {
            if (blueprintElements == null || !blueprintElements.Any())
            {
                return;
            }

            foreach (var blueprintElement in blueprintElements)
            {
                // For claims and targets, the convention is to prepend the client name to the id
                var key = CLAIM_AND_TARGET_TYPES.Contains(blueprintElement.type)
                    ? string.Format("{0}-{1}", client.Name, blueprintElement.id)
                    : blueprintElement.id;

                // If a blueprint element does not have any "child" blueprint elements, it should be marked as a
                // "leaf" node.  Otherwise, it should not be marked as a leaf node.  The HasLeafNode property is used
                // later in the loading process.
                // from https://github.com/SmarterApp/TDS_AssessmentService/blob/1dc03698a2a608437d63b491757d366b1c217abc/service/src/main/java/tds/assessment/services/impl/AssessmentItemBankGenericDataLoaderServiceImpl.java#L138
                var strand = new StrandDTO
                {
                    Name = blueprintElement.id,
                    ParentId = parentKey,
                    ClientKey = client.ClientKey,
                    TreeLevel = treeLevel,
                    TestVersion = testVersion,
                    BlueprintElementId = blueprintElement.id,
                    SubjectKey = subjectKey,
                    Key = key,
                    Type = blueprintElement.type,
                    IsLeafTarget = (blueprintElement.BlueprintElement1?.Length ?? 0) == 0
                };

                strands.Add(strand);

                // Build the blueprint hierarchy at the next tree level.
                BuildStrandsWithHierarchyFromBlueprintElements(blueprintElement.BlueprintElement1,
                    strands,
                    client,
                    key,
                    subjectKey,
                    testVersion,
                    treeLevel + 1);
            }
        }
    }
}
