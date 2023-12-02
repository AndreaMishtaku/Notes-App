using AutoMapper;
using IRepository;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Service;
using Shared.Configuration;
using Shared.DTO;

namespace UnitTesting;

[TestClass]
public class TestNote
{
    [TestMethod]
    public async Task NoteServiceCreationTest()
    {
        var noteRepositoryMock = new Mock<INoteRepository>();
        var mapperMock = new Mock<IMapper>();
        var claimsUtilityMock = new Mock<IClaimsUtility>();
        var loggerMock = new Mock<ILogger<NoteService>>();

        claimsUtilityMock.Setup(utility => utility.ReadCurrentUserId()).Returns(1);

        var noteService = new NoteService(
            mapperMock.Object,
            noteRepositoryMock.Object,
            claimsUtilityMock.Object,
            loggerMock.Object
        );

        var noteRequestDto = new NoteRequestDTO
        {
            Title="Test for creation",
            Content="abc"
        };

        var result = await noteService.CreateNote(noteRequestDto);

        // Assert
        Assert.IsNotNull(result);
        //Assert.AreEqual("dfafas", result.Message);
        Assert.AreEqual(200, result.StatusCode);
    }
}
