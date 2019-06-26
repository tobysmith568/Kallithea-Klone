using NUnit.Framework;
using KallitheaKlone.WPF.Models.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using KallitheaKlone.Models.Dialogs.MessagePrompts;
using KallitheaKlone.Models.JSONConverter;
using System.Reflection;
using System.IO;
using KallitheaKlone.Models.Repositories;

namespace KallitheaKlone.WPF.Models.Repositories.Tests
{
    [TestFixture, IntegrationTests]
    public class RepositoryManagerTests
    {
        private RepositoryManager subject;

        private Mock<IMessagePrompt> messagePrompt;
        private Mock<IJSONConverter> jsonConverter;

        private static readonly string testDirectory = Path.Combine(TestContext.CurrentContext.TestDirectory, "testDir");
        private static readonly string testFile = Path.Combine(testDirectory, "KallitheaRepositories.dat");

        [SetUp]
        public void SetUp()
        {
            messagePrompt = new Mock<IMessagePrompt>();
            jsonConverter = new Mock<IJSONConverter>();

            subject = new RepositoryManager(messagePrompt.Object, jsonConverter.Object);
        }

        /// <exception cref="Exception"></exception>
        [TearDown]
        public void TearDown()
        {
            Given_FolderDoesNotExist(testDirectory);
        }


        #region GetAllRepositories


        /// <exception cref="Exception"></exception>
        [Test]
        public async Task GetAllRepositories_CreatesAppDataFolderIfItDoesNotExist()
        {
            Given_subject_appDataFolder_Is(testDirectory);
            Given_FolderDoesNotExist(testDirectory);

            await subject.GetAllRepositories();

            Assert.IsTrue(Directory.Exists(testDirectory));
        }

        /// <exception cref="Exception"></exception>
        [Test]
        public async Task GetAllRepositories_CreatesEmptyFileIfItDoesNotExist()
        {
            Given_subject_appDataFolder_Is(testDirectory);
            Given_subject_allRepositoriesFile_Is(testFile);
            Given_FolderDoesNotExist(testDirectory);

            await subject.GetAllRepositories();

            Assert.IsTrue(File.Exists(testFile));
            Assert.AreEqual(string.Empty, File.ReadAllText(testFile));
        }

        /// <exception cref="Exception"></exception>
        [Test]
        public async Task GetAllRepositories_ReturnsEmptyFolderIfFileDoesNotExist()
        {
            Given_subject_appDataFolder_Is(testDirectory);
            Given_subject_allRepositoriesFile_Is(testFile);
            Given_FolderDoesNotExist(testDirectory);

            IRepositoryFolder<RepositoryFolder, Repository> result = await subject.GetAllRepositories();

            Assert.NotNull(result);
            Assert.NotNull(result.ChildFolders);
            Assert.NotNull(result.ChildRepositories);
            Assert.AreEqual(string.Empty, result.Name);
            Assert.AreEqual(0, result.ChildFolders.Count);
            Assert.AreEqual(0, result.ChildRepositories.Count);
        }

        /// <exception cref="Exception"></exception>
        [Test]
        public async Task GetAllRepositories_ReturnsDeserializedJSONIfFileDoesExist()
        {
            string testFileData = "testString";
            RepositoryFolder repositoryFolder = new RepositoryFolder();

            Given_subject_appDataFolder_Is(testDirectory);
            Given_subject_allRepositoriesFile_Is(testFile);
            Given_FolderDoesExist(testDirectory);
            Given_FileDoesExistWithData(testFile, testFileData);
            Given_jsonConverter_FromJson_ReturnsWhenGiven(repositoryFolder, testFileData);

            IRepositoryFolder<RepositoryFolder, Repository> result = await subject.GetAllRepositories();

            Assert.AreSame(repositoryFolder, result);
        }

        /// <exception cref="Exception"></exception>
        [Test]
        public async Task GetAllRepositories_ReturnsEmptyFolderOnException()
        {
            string testFileData = "testString";

            Given_subject_appDataFolder_Is(testDirectory);
            Given_subject_allRepositoriesFile_Is(testFile);
            Given_FolderDoesExist(testDirectory);
            Given_FileDoesExistWithData(testFile, testFileData);
            Given_jsonConverter_FromJson_ThrowsAnException();
            Given_messagePrompt_PromptOK_IsStubbed();

            IRepositoryFolder<RepositoryFolder, Repository> result = await subject.GetAllRepositories();

            Assert.NotNull(result);
            Assert.NotNull(result.ChildFolders);
            Assert.NotNull(result.ChildRepositories);
            Assert.AreEqual(string.Empty, result.Name);
            Assert.AreEqual(0, result.ChildFolders.Count);
            Assert.AreEqual(0, result.ChildRepositories.Count);

            messagePrompt.Verify(m => m.PromptOK($"Unable to read repositories from storage!{Environment.NewLine}" +
                                           $"Please reload your repositories.", "Error", MessageType.Error), Times.Once);
        }


        #endregion
        #region OverwriteAllRespositories


        /// <exception cref="Exception"></exception>
        [Test]
        public async Task OverwriteAllRespositories_CreatesAppDataFolderIfItDoesNotExist()
        {
            Given_subject_appDataFolder_Is(testDirectory);
            Given_FolderDoesNotExist(testDirectory);

            await subject.OverwriteAllRespositories(null);

            Assert.IsTrue(Directory.Exists(testDirectory));
        }

        /// <exception cref="Exception"></exception>
        [Test]
        public async Task OverwriteAllRepositories_SavesResultOfJSONConverter()
        {
            RepositoryFolder repositoryFolder = new RepositoryFolder();
            string testFileData = "testString";

            Given_subject_appDataFolder_Is(testDirectory);
            Given_subject_allRepositoriesFile_Is(testFile);
            Given_FolderDoesExist(testDirectory);
            Given_jsonConverter_ToJson_ReturnsWhenGiven(testFileData, repositoryFolder);

            bool result = await subject.OverwriteAllRespositories(repositoryFolder);

            Assert.True(result);
            Assert.AreEqual(testFileData, File.ReadAllText(testFile));
        }

        /// <exception cref="Exception"></exception>
        [Test]
        public async Task OverwriteAllRepositories_ReturnsFalseOnException()
        {
            RepositoryFolder repositoryFolder = new RepositoryFolder();
            string testFileData = "testString";

            Given_subject_appDataFolder_Is(testDirectory);
            Given_subject_allRepositoriesFile_Is(testFile);
            Given_FolderDoesExist(testDirectory);
            Given_FileDoesExistWithData(testFile, string.Empty);
            Given_jsonConverter_ToJson_ReturnsWhenGiven(testFileData, repositoryFolder);

            bool result;

            using (File.Open(testFile, FileMode.Open))
            {
                result = await subject.OverwriteAllRespositories(repositoryFolder);
            }

            Assert.False(result);

            messagePrompt.Verify(m => m.PromptOK($"Unable to save repositories to storage!{Environment.NewLine}" +
                                           $"Please reopen the program and try again.", "Error", MessageType.Error), Times.Once);
        }


        #endregion


        /// <exception cref="Exception"></exception>
        private void Given_subject_appDataFolder_Is(string value)
        {
            subject.GetType()
                .GetField("appDataFolder", BindingFlags.Static | BindingFlags.NonPublic)
                .SetValue(subject, value);
        }

        /// <exception cref="Exception"></exception>
        private void Given_subject_allRepositoriesFile_Is(string value)
        {
            subject.GetType()
                .GetField("allRepositoriesFile", BindingFlags.Static | BindingFlags.NonPublic)
                .SetValue(subject, value);
        }

        /// <exception cref="Exception"></exception>
        private void Given_FolderDoesNotExist(string folderName)
        {
            if (Directory.Exists(folderName))
            {
                Directory.Delete(folderName, true);
            }
        }

        /// <exception cref="Exception"></exception>
        private void Given_FolderDoesExist(string folderName)
        {
            if (!Directory.Exists(folderName))
            {
                Directory.CreateDirectory(folderName);
            }
        }

        /// <exception cref="Exception"></exception>
        private void Given_FileDoesExistWithData(string fileName, string content)
        {
            if (!File.Exists(fileName))
            {
                File.WriteAllText(fileName, content);
            }
        }

        private void Given_jsonConverter_FromJson_ReturnsWhenGiven(RepositoryFolder repositoryFolder, string testFileData)
        {
            jsonConverter.Setup(j => j.FromJson<RepositoryFolder>(testFileData)).Returns(repositoryFolder);
        }

        private void Given_jsonConverter_FromJson_ThrowsAnException()
        {
            jsonConverter.Setup(j => j.FromJson<RepositoryFolder>(It.IsAny<string>())).Throws(new Exception());
        }

        private void Given_jsonConverter_ToJson_ReturnsWhenGiven(string testFileData, IRepositoryFolder<RepositoryFolder, Repository> repositoryFolder)
        {
            jsonConverter.Setup(j => j.ToJson(repositoryFolder)).Returns(testFileData);
        }

        private void Given_messagePrompt_PromptOK_IsStubbed()
        {
            messagePrompt.Setup(m => m.PromptOK(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<MessageType>())).Verifiable();
        }
    }
}