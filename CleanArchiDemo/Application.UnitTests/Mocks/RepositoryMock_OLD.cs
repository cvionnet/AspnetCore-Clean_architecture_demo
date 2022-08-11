using Application.Contracts.Persistence;
using Domain.Entities;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UnitTests.Mocks;

public class RepositoryMock_OLD
{
    public static Mock<IAsyncRepository<Company>> GetCompanyRepository()
    {
        // Mock initialization
        var companyRepositoryMock = new Mock<IAsyncRepository<Company>>();

        // Mock methods from the interface IAsyncRepository
        // ListAllAsync() - return the list of all companies
        companyRepositoryMock.Setup(p => p.ListAllAsync().Result).Returns(MockFakeData.FakeCompanies);

        // Task<T> GetByIdAsync(int id);

        // Task<int> AddAsync(T entity);       // return the ID of the line inserted

        //It = helper to give arguments to a method (here AddAsync())
        //It.IsAny = any values of a certain data type
        //It.IsAny<Category>() = setting this function for any passed value of type Category as parameter
        //ReturnsAsync() : lambda return of whatever value we need to return (here a new Category object)
        /*
                    mockCategoryRepository.Setup(repo => repo.AddAsync(It.IsAny<Category>())).ReturnsAsync(
                        (Category category) =>
                        {
                            categories.Add(category);
                            return category;
                        });
        */


        // Task<int> UpdateAsync(T entity);    // return the number of lines updated
        // Task<int> DeleteAsync(T entity);    // return the number of lines deleted


        return companyRepositoryMock;
    }
}
