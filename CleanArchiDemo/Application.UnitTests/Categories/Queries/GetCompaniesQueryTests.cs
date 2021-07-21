using Application.Features._ViewModels;
using Application.Features.Companies;
using Application.Profiles;
using Application.Responses;
using Application.UnitTests.Mocks;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Application.UnitTests.Categories.Queries
{
    public class GetCompaniesQueryTests
    {
        private readonly IMapper _mapper;
        private readonly ILogger<GetCompaniesQueryHandler> _logger;
        private readonly MockCompanyRepository _mockCompanyRepository;

        public GetCompaniesQueryTests()
        {
            _mockCompanyRepository = new MockCompanyRepository();

            _logger = Mock.Of<ILogger<GetCompaniesQueryHandler>>();     // mock a fake ILogger

            var configurationProvider = new MapperConfiguration(cfg => { cfg.AddProfile<MappingProfile>(); });
                _mapper = configurationProvider.CreateMapper();
        }

        [Fact]
        public async Task GetCompanies_WhenGettingAll_ShouldBeTypeResponse_ListCompanyVM()
        {
            //Arrange
            var mockObject = _mockCompanyRepository.MockListAllAsync();
            var handler = new GetCompaniesQueryHandler(_mapper, _logger, mockObject.Object);

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
            var handler = new GetCompaniesQueryHandler(_mapper, _logger, mockObject.Object);

            //Act
            var result = await handler.Handle(new GetCompaniesQuery(), CancellationToken.None);

            //Assert
            result.Company.Count.ShouldBe(3);
        }


        //TODO: sur les ADD/UPDATE, tester si Email valide

    }
}
