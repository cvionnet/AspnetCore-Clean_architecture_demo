using Application.Contracts.Persistence;
using Domain.Entities;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UnitTests.Mocks;

public class MockCompanyRepository : Mock<IUnitOfWork> // Mock<ICompanyRepository>       //Mock<IAsyncRepository<Company>>
{
    // Task<IReadOnlyList<T>> ListAllAsync();
    public MockCompanyRepository MockListAllAsync()
    {
        Setup(p => p.Companies.ListAllAsync().Result)
            .Returns(MockFakeData.FakeCompanies);

        return this;
    }

    // Task<T> GetByIdAsync(int id);
    public MockCompanyRepository MockGetByIdAsync(int id)
    {
        Setup(p => p.Companies.GetByIdAsync(id).Result)
            .Returns(MockFakeData.FakeCompanies.FirstOrDefault(c => c.CompanyID == id));

        return this;
    }

    // Task<int> AddAsync(T entity);       // return the ID of the line inserted
    public MockCompanyRepository MockAddAsync(Company entity)
    {
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


        throw new NotImplementedException();
        return this;
    }

    // Task<int> UpdateAsync(T entity);    // return the number of lines updated
    public MockCompanyRepository MockUpdateAsync(Company entity)
    {
        throw new NotImplementedException();
        return this;
    }

    // Task<int> DeleteAsync(T entity);    // return the number of lines deleted
    public MockCompanyRepository MockDeleteAsync(Company entity)
    {
        var temp = Setup(p => p.Companies.ListAllAsync().Result)
            .Returns(MockFakeData.FakeCompanies);

        // If suppression in faked data successed, return 1 (fake an int)
        Setup(p => p.Companies.DeleteAsync(It.IsAny<Company>()).Result)
            .Returns(MockFakeData.FakeCompanies.Remove(entity) ? 1 : 0);


        // If suppression in faked data successed, return 1 (fake an int)
        //Setup(p => p.Companies.DeleteAsync(entity).Result)
        //    .Returns(MockFakeData.FakeCompanies.Remove(entity) ? 1 : 0);




        return this;
    }

    // Task<bool> ResetDBToDemo();
    public MockCompanyRepository MockResetDBToDemo()
    {
        throw new NotImplementedException();
        return this;
    }

}
