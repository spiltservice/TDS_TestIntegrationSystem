﻿using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace TDSQASystemAPI.TestPackage.utils
{
    /// <summary>
    /// A class for building up a complete <code>TestPackage</code> after the XML has been deserialized.
    /// </summary>
    public class TestPackageMapper
    {
        private static readonly XmlSerializer xmlSerializer = new XmlSerializer(typeof(TestPackage));

        /// <summary>
        /// Create a <code>TestPackage</code> instance from XML.  The returned <code>TestPackage</code> will
        /// have the required parent -> child relationships established between the elements that need them.
        /// </summary>
        /// <param name="reader">An <code>XmlReader</code> containing the test package's XML.</param>
        /// <returns>A <code>TestPackage</code> with the parent -> child relationships established.</returns>
        public static TestPackage FromXml(XmlReader reader)
        {
            // The test package XML is loaded into an XDocument to preserve the Tools configuration XML that was
            // provided in the test package XML.  The XDocument is used to build up the tools & honor the tool
            // configuration that was provided in the test package XML.  Because of the way the TestPackage classes
            // were generated by the XSD, some of the tool defaults might be incorrectly set.
            var testPackageXDocument = XDocument.Load(reader);
            var testPackage = xmlSerializer.Deserialize(testPackageXDocument.CreateReader()) as TestPackage;
      
            // WIRE UP ASSESSMENTS
            // 1.  Set the assesment's parent test package property
            // 2.  Update each segment's Assessment property to denote its parent assessment
            // 3.  Build up the Assessment-wide Tools
            testPackage.Assessment.ForEach(a => 
            {
                a.TestPackage = testPackage;

                a.Segments.ForEach(s =>
                {
                    s.Assessment = a;
                });

                var assessmentElement = testPackageXDocument.Root.Elements("Assessment")
                    .Where(el => el.Attribute("id").Value.Equals(a.id))
                    .First();

                a.Tools = AssembleTools(assessmentElement);
            });

            // WIRE UP SEGMENTS            
            var allSegments = from a in testPackage.Assessment
                              from s in a.Segments
                              select s;

            // The segment's Item property can be a form (for fixed-form assessments) or a pool of items (for an 
            // adaptive assessment).  Set the appropriate properties based on the type that's stored in the
            // AssessmentSegment's Item property.
            foreach (var segment in allSegments)
            {
                if (segment.Item is AssessmentSegmentSegmentForms)
                {
                    foreach (var form in (segment.Item as AssessmentSegmentSegmentForms).SegmentForm)
                    { 
                        form.AssessmentSegment = segment;
                        form.ItemGroup.ForEach(ig => AssembleItemGroup(ig, testPackage, segment, form));
                    }
                }
                else
                {
                    var pool = segment.Item as AssessmentSegmentPool;
                    pool.ItemGroup.ForEach(ig => AssembleItemGroup(ig, testPackage, segment));
                }

                // Get the XML for the segment-specific tools and build them.
                var segmentElement = testPackageXDocument.Descendants("Segment")
                    .Where(s => s.Attribute("id").Value.Equals(segment.id))
                    .First();

                // Build up the segment-specific tools
                segment.Tools = AssembleTools(segmentElement);
            }

            return testPackage;
        }

        /// <summary>
        /// Set up the parent -> child associations for the data stored within an <code>ItemGroup</code>.
        /// </summary>
        /// <param name="itemGroup">The <code>ItemGroup</code> to assemble</param>
        /// <param name="testPackage">The <code>TestPackage</code> that ultimately owns this <code>ItemGroup</code></param>
        /// <param name="segment">The <code>AssessmentSegment</code> that owns this <code>ItemGroup</code></param>
        /// <param name="form">The <code>AssessmentSegmentSegmentFormsSegmentForm</code> to which the item is associated.
        /// Will be null for items associated to adaptive assessments; adaptive assessments do not have forms.</param>
        private static void AssembleItemGroup(ItemGroup itemGroup, 
            TestPackage testPackage, 
            AssessmentSegment segment, 
            AssessmentSegmentSegmentFormsSegmentForm form = null)
        {
            itemGroup.AssessmentSegment = segment;
            foreach (var item in itemGroup.Item)
            {
                item.TestPackage = testPackage;
                item.AssessmentSegment = segment;
                item.SegmentForm = form;
                item.ItemGroup = itemGroup;
                if (item.TeacherHandScoring != null)
                {
                    item.TeacherHandScoring.TestPackage = testPackage;
                    item.TeacherHandScoring.ItemGroupItem = item;
                }
            }

            if (itemGroup.Stimulus != null)
            {
                itemGroup.Stimulus.TestPackage = testPackage;
                itemGroup.Stimulus.AssessmentSegment = segment;
                itemGroup.Stimulus.ItemGroup = itemGroup;
            }
        }

        /// <summary>
        /// Build a <code>ToolsTool</code> collection from the Tools elements contained in an <code>XElement</code>.
        /// </summary>
        /// <remarks>
        /// Typically Tool elements can be found at two levels:
        /// 
        /// 1.  Assessment-wide:  These are Tool elements that are stored within the Assessment XML.  These tools are
        /// available in every segment of the assessment.
        /// 2.  Segment-specific:  These Tool elements are stored within the Segment XML.  These tools are only available
        /// in the segment in which they are defined. 
        /// </remarks>
        /// <param name="elementWithTools">The <code>XElement</code> containing "Tool" element(s) that need to be converted
        /// to <code>ToolsTool</code>s.</param>
        /// <returns>An array of <code>ToolsTool</code>s built from the <code>XElement</code> fragment.</returns>
        private static ToolsTool[] AssembleTools(XElement elementWithTools)
        {
            var toolElements = from e in elementWithTools.Descendants("Tool")
                               select e;

            var tools = new List<ToolsTool>();
            toolElements.ForEach(el =>
            {
                var tool = ToolMapper.FromXml(el);
                tools.Add(tool);
            });

            return tools.ToArray();
        }
    }
}
