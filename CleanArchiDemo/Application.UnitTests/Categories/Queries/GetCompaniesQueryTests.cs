using Application.Features._ViewModels;
using Application.Features.Companies;
using Application.Profiles;
using Application.UnitTests.Mocks;
using AutoMapper;
using Domain.Entities;
using Shouldly;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Application.UnitTests.Categories.Queries;

public class GetCompaniesQueryTests
{
    private readonly IMapper _mapper;
    private readonly MockCompanyRepository _mockCompanyRepository;

    public GetCompaniesQueryTests()
    {
        _mockCompanyRepository = new MockCompanyRepository();

        var configurationProvider = new MapperConfiguration(cfg => { cfg.AddProfile<MappingProfile>(); });
            _mapper = configurationProvider.CreateMapper();
    }

    [Fact]
    public async Task GetCompanies_WhenGettingAll_ShouldBeTypeResponse_ListCompanyVM()
    {
        //Arrange
        var mockObject = _mockCompanyRepository.MockListAllAsync();
        var handler = new GetCompaniesQueryHandler(_mapper, MockFakeLogger<GetCompaniesQueryHandler>.FakeLogger(), mockObject.Object);
        //var handler = new GetCompaniesQueryHandler(_mapper, _logger, mockObject.Object);

        //Act
        var result = await handler.Handle(new GetCompaniesQuery(), CancellationToken.None);

        //Assert
        result.ShouldBeOfType<GetCompaniesQueryResponse>();
        result.Company.ShouldBeOfType<List<CompanyVM>>();
    }

    [Fact]
    public async Task GetCompanies_WhenGettingAll_ShouldReturn3()
    {
        //Arrange
        var mockObject = _mockCompanyRepository.MockListAllAsync();
        var handler = new GetCompaniesQueryHandler(_mapper, MockFakeLogger<GetCompaniesQueryHandler>.FakeLogger(), mockObject.Object);

        //Act
        var result = await handler.Handle(new GetCompaniesQuery(), CancellationToken.None);

        //Assert
        result.Company.Count.ShouldBe(3);
    }

    [Fact]
    public async Task GetCompanyById_WhenId2_ShouldBeTypeResponse_CompanyVM()
    {
        //Arrange
        int id = 2;
        var mockObject = _mockCompanyRepository.MockGetByIdAsync(id);
        var handler = new GetCompanyByIdQueryHandler(_mapper, MockFakeLogger<GetCompanyByIdQueryHandler>.FakeLogger(), mockObject.Object);

        //Act
        var result = await handler.Handle(new GetCompanyByIdQuery() { CompanyId = id }, CancellationToken.None);

        //Assert
        result.ShouldBeOfType<GetCompanyByIdQueryResponse>();
        result.Company.ShouldBeOfType<CompanyVM>();
    }

    [Fact]
    public async Task GetCompanyById_WhenId2_ShouldReturnCompany2()
    {
        //Arrange
        int id = 2;
        var mockObject = _mockCompanyRepository.MockGetByIdAsync(id);
        var handler = new GetCompanyByIdQueryHandler(_mapper, MockFakeLogger<GetCompanyByIdQueryHandler>.FakeLogger(), mockObject.Object);

        //Act
        var result = await handler.Handle(new GetCompanyByIdQuery() { CompanyId = id }, CancellationToken.None);

        //Assert
        result.Company.Name.ShouldBe("Company 2");
    }

    [Fact]
    public async Task GetCompanyById_WhenId4_ShouldReturnNull()
    {
        //Arrange
        int id = 4;
        var mockObject = _mockCompanyRepository.MockGetByIdAsync(id);
        var handler = new GetCompanyByIdQueryHandler(_mapper, MockFakeLogger<GetCompanyByIdQueryHandler>.FakeLogger(), mockObject.Object);

        //Act
        var result = await handler.Handle(new GetCompanyByIdQuery() { CompanyId = id }, CancellationToken.None);

        //Assert
        result.Company.ShouldBeNull();
    }

    [Fact]
    public async Task DeleteCompany_WhenId2_ShouldBeTypeResponse_Int()
    {
        //Arrange
        Company deleteCompany = MockFakeData.FakeCompanies.FirstOrDefault(c => c.CompanyID == 2);
        var mockObject = _mockCompanyRepository.MockDeleteAsync(deleteCompany);
        var handler = new DeleteCompanyCommandHandler (_mapper, MockFakeLogger<DeleteCompanyCommandHandler>.FakeLogger(), mockObject.Object);

        //Act
        var result = await handler.Handle(new DeleteCompanyCommand() { CompanyId = deleteCompany.CompanyID }, CancellationToken.None);

        //Assert
        result.ShouldBeOfType<DeleteCompanyCommandResponse>();
        result.DeletedLinesCount.ShouldBeOfType<int>();
    }

    [Fact]
    public async Task DeleteCompany_WhenId2_ShouldReturn1()
    {
        //Arrange
        Company deleteCompany = MockFakeData.FakeCompanies.FirstOrDefault(c => c.CompanyID == 2);
        var mockObject = _mockCompanyRepository.MockDeleteAsync(deleteCompany);
        var handler = new DeleteCompanyCommandHandler(_mapper, MockFakeLogger<DeleteCompanyCommandHandler>.FakeLogger(), mockObject.Object);

        //Act
        var result = await handler.Handle(new DeleteCompanyCommand() { CompanyId = deleteCompany.CompanyID }, CancellationToken.None);

        //Assert
        result.DeletedLinesCount.ShouldBe(1);
    }

    [Fact]
    public async Task DeleteCompany_WhenId4_ShouldReturnNull()
    {
        //Arrange
        Company deleteCompany = MockFakeData.FakeCompanies.FirstOrDefault(c => c.CompanyID == 4);
        var mockObject = _mockCompanyRepository.MockDeleteAsync(deleteCompany);
        var handler = new DeleteCompanyCommandHandler(_mapper, MockFakeLogger<DeleteCompanyCommandHandler>.FakeLogger(), mockObject.Object);

        //Act
        var result = await handler.Handle(new DeleteCompanyCommand() { CompanyId = deleteCompany.CompanyID }, CancellationToken.None);

        //Assert
        result.ShouldBeNull();
    }

    //TODO: sur les ADD/UPDATE, tester si Email valide

}
