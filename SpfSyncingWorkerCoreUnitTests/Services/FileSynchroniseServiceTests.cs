using Ardalis.Result;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFTP.Wrapper;
using SFTP.Wrapper.Requests;
using SFTP.Wrapper.Responses;
using SpfSyncingWorkerCore.Interfaces;
using SpfSyncingWorkerCore.Services;

namespace SpfSyncingWorkerCoreUnitTests.Services;

[TestFixture]
public class FileSynchroniseServiceTests
{

    private Mock<ISftpManager> _sftpManagerMock = new();
    private Mock<ISyncedFileRepository> _syncedFileRepositoryMock = new();
    private Mock<IFileService> _fileServiceMock = new();
    private Mock<ILogger<FileSynchroniseService>> _loggerMock = new();

    [SetUp]
    public void SetUp()
    {
        _sftpManagerMock = new();
        _syncedFileRepositoryMock = new();
        _fileServiceMock = new();
        _loggerMock = new();
    }

    /*
     *
     //Arrange
   
     //Act  

     //Assert
     *
     *
     */

    [Test]
    public async Task SynchroniseFileAsync_ShouldReturnResultSucess_WhenNoFilesInSftp()
    {
        //Arrange
        var request = new GetAllFilesRequest("SomePath");
        var response = ResultStatus<GetAllFilesResponse>.Error("");
        _sftpManagerMock.Setup(
            _ => _.GetAllFilesAsync(
                request))
            .ReturnsAsync(response);

        var service = new FileSynchroniseService(
            _syncedFileRepositoryMock.Object, 
            _sftpManagerMock.Object, 
            _loggerMock.Object, 
            _fileServiceMock.Object );

        //Act

        var result = await service.SynchronizeAsync("", "");
        //Assert

        Assert.That(result.Status, Is.EqualTo(ResultStatus.Ok));
    }
}
