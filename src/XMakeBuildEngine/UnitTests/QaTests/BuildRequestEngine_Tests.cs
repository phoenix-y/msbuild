﻿using System;
using System.Xml;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Build.BackEnd;
using Microsoft.Build.Framework;
using Microsoft.Build.Shared;

using NodeLoggingContext = Microsoft.Build.BackEnd.Logging.NodeLoggingContext;

namespace Microsoft.Build.UnitTests.QA
{
    /// <summary>
    /// Steps to write the tests
    /// 1) Create a TestProjectDefinition object for each project file or a build request that you will submit
    /// 2) Call Build() on the object to submit the build request
    /// 3) Call ValidateResults() on the object to wait till the build completes and the results sent were what we expected
    /// </summary>
    [TestClass]
    [Ignore] // "Tests are out of sync with BuildRequestEngine/Scheduler changes"
    public class BuildRequestEngine_Tests
    {
        #region Data members

        private Common_Tests commonTests;
        private QAResultsCache resultsCache;

        #endregion

        #region Constructor

        /// <summary>
        /// Setup the object to be used
        /// </summary>
        public BuildRequestEngine_Tests()
        {
            this.commonTests = new Common_Tests(this.GetComponent, false);
            this.resultsCache = null;
        }

        #endregion

        #region Common

        /// <summary>
        /// Delegate to common test setup
        /// </summary>
        [TestInitialize]
        public void Setup()
        {
            this.resultsCache = new QAResultsCache();
            this.commonTests.Setup();
        }

        /// <summary>
        /// Delegate to common test teardown
        /// </summary>
        [TestCleanup]
        public void TearDown()
        {
            this.resultsCache = null;
            this.commonTests.TearDown();
        }

        #endregion

        #region GetComponent delegate

        /// <summary>
        /// Provides the components required by the tests
        /// </summary>
        internal IBuildComponent GetComponent(BuildComponentType type)
        {
            switch (type)
            {
                case BuildComponentType.RequestBuilder:
                    QARequestBuilder requestBuilder = new QARequestBuilder();
                    return (IBuildComponent)requestBuilder;

                case BuildComponentType.TaskBuilder:
                    TaskBuilder taskBuilder = new TaskBuilder();
                    return (IBuildComponent)taskBuilder;

                case BuildComponentType.TargetBuilder:
                    TargetBuilder targetBuilder = new TargetBuilder();
                    return (IBuildComponent)targetBuilder;

                case BuildComponentType.ResultsCache:
                    return (IBuildComponent)this.resultsCache;

                default:
                    throw new ArgumentException("Unexpected type requested. Type = " + type.ToString());
            }
        }

        #endregion

        #region Common Tests

        /// <summary>
        /// Delegate to common tests
        /// </summary>
        [TestMethod]
        public void BuildOneProject()
        {
            this.commonTests.BuildOneProject();
            Assert.AreEqual(this.resultsCache.CacheCount, 1, "Cache should only have 1 entry");
        }

        /// <summary>
        /// Delegate to common tests
        /// </summary>
        [TestMethod]
        public void Build4DifferentProjects()
        {
            this.commonTests.Build4DifferentProjects();
            Assert.AreEqual(this.resultsCache.CacheCount, 4, "Cache should have 4 entries");
        }

        /// <summary>
        /// Delegate to common tests
        /// </summary>
        [TestMethod]
        public void BuildingTheSameProjectTwiceWithDifferentToolsVersion()
        {
            this.commonTests.BuildingTheSameProjectTwiceWithDifferentToolsVersion();
            Assert.AreEqual(((QAResultsCache)this.resultsCache).CacheCount, 2);
        }

        /// <summary>
        /// Delegate to common tests
        /// </summary>
        [TestMethod]
        public void BuildingTheSameProjectTwiceWithDifferentToolsGlobalProperties()
        {
            this.commonTests.BuildingTheSameProjectTwiceWithDifferentGlobalProperties();
            Assert.AreEqual(((QAResultsCache)this.resultsCache).CacheCount, 2);
        }

        /// <summary>
        /// Delegate to common tests
        /// </summary>
        [TestMethod]
        public void ReferenceAProjectAlreadyBuiltInTheNode()
        {
            this.commonTests.ReferenceAProjectAlreadyBuiltInTheNode();
            Assert.AreEqual(((QAResultsCache)this.resultsCache).CacheCount, 2);
        }

        /// <summary>
        /// Delegate to common tests
        /// </summary>
        [TestMethod]
        public void ReferenceAProjectAlreadyBuiltInTheNodeButWithDifferentToolsVersion()
        {
            this.commonTests.ReferenceAProjectAlreadyBuiltInTheNodeButWithDifferentToolsVersion();
            Assert.AreEqual(((QAResultsCache)this.resultsCache).CacheCount, 3);
        }

        /// <summary>
        /// Delegate to common tests
        /// </summary>
        [TestMethod]
        public void ReferenceAProjectAlreadyBuiltInTheNodeButWithDifferentGlobalProperties()
        {
            this.commonTests.ReferenceAProjectAlreadyBuiltInTheNodeButWithDifferentGlobalProperties();
        }

        /// <summary>
        /// Delegate to common tests
        /// </summary>
        [TestMethod]
        public void BuildOneProjectWith1Reference()
        {
            this.commonTests.BuildOneProjectWith1Reference();
            Assert.AreEqual(((QAResultsCache)this.resultsCache).CacheCount, 2);
        }

        /// <summary>
        /// Delegate to common tests
        /// </summary>
        [TestMethod]
        public void BuildOneProjectWith3Reference()
        {
            this.commonTests.BuildOneProjectWith3Reference();
            Assert.AreEqual(((QAResultsCache)this.resultsCache).CacheCount, 4);
        }

        /// <summary>
        /// Delegate to common tests
        /// </summary>
        [TestMethod]
        public void BuildOneProjectWith3ReferenceWhere2AreTheSame()
        {
            this.commonTests.BuildOneProjectWith3ReferenceWhere2AreTheSame();
            Assert.AreEqual(((QAResultsCache)this.resultsCache).CacheCount, 3);
        }

        /// <summary>
        /// Delegate to common tests
        /// </summary>
        [TestMethod]
        public void BuildMultipleProjectsWithMiddleProjectHavingReferenceToANewProject()
        {
            this.commonTests.BuildMultipleProjectsWithMiddleProjectHavingReferenceToANewProject();
        }

        /// <summary>
        /// Delegate to common tests
        /// </summary>
        [TestMethod]
        public void BuildMultipleProjectsWithTheFirstProjectHavingReferenceToANewProject()
        {
            this.commonTests.BuildMultipleProjectsWithTheFirstProjectHavingReferenceToANewProject();
        }

        /// <summary>
        /// Delegate to common tests
        /// </summary>
        [TestMethod]
        public void BuildMultipleProjectsWithTheLastProjectHavingReferenceToANewProject()
        {
            this.commonTests.BuildMultipleProjectsWithTheLastProjectHavingReferenceToANewProject();
        }

        /// <summary>
        /// Delegate to common tests
        /// </summary>
        [TestMethod]
        public void BuildMultipleProjectsWithEachReferencingANewProject()
        {
            this.commonTests.BuildMultipleProjectsWithEachReferencingANewProject();
        }

        /// <summary>
        /// Delegate to common tests
        /// </summary>
        [TestMethod]
        public void BuildMultipleProjectsWhereFirstReferencesMultipleNewProjects()
        {
            this.commonTests.BuildMultipleProjectsWhereFirstReferencesMultipleNewProjects();
        }

        /// <summary>
        /// Delegate to common tests
        /// </summary>
        [TestMethod]
        public void BuildMultipleProjectsWhereFirstAndLastReferencesMultipleNewProjects()
        {
            this.commonTests.BuildMultipleProjectsWhereFirstAndLastReferencesMultipleNewProjects();
        }

        /// <summary>
        /// Delegate to common tests
        /// </summary>
        [TestMethod]
        public void BuildMultipleProjectsWithReferencesWhereSomeReferencesAreAlreadyBuilt()
        {
            this.commonTests.BuildMultipleProjectsWithReferencesWhereSomeReferencesAreAlreadyBuilt();
        }

        [TestMethod]
        public void BuildMultipleProjectsWithReferencesAndDifferentGlobalProperties()
        {
            this.commonTests.BuildMultipleProjectsWithReferencesAndDifferentGlobalProperties();
        }

        /// <summary>
        /// Delegate to common tests
        /// </summary>
        [TestMethod]
        public void BuildMultipleProjectsWithReferencesAndDifferentToolsVersion()
        {
            this.commonTests.BuildMultipleProjectsWithReferencesAndDifferentToolsVersion();
        }

        /// <summary>
        /// Delegate to common tests
        /// </summary>
        [TestMethod]
        public void Build3ProjectsWhere1HasAReferenceTo3()
        {
            this.commonTests.Build3ProjectsWhere1HasAReferenceTo3();
        }

        /// <summary>
        /// Delegate to common tests
        /// </summary>
        [TestMethod]
        public void Build3ProjectsWhere2HasAReferenceTo3()
        {
            this.commonTests.Build3ProjectsWhere2HasAReferenceTo3();
        }

        /// <summary>
        /// Delegate to common tests
        /// </summary>
        [TestMethod]
        public void Build3ProjectsWhere3HasAReferenceTo1()
        {
            this.commonTests.Build3ProjectsWhere3HasAReferenceTo1();
        }

        #endregion
    }
}