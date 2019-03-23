using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Services;
using ApplicationCore.Tests.Services.Data;
using Infrastructure;
using Infrastructure.Repositories;
using Xunit;
using Moq;
using Newtonsoft.Json;
using Web.Controllers;
using Web.DTOs;


namespace ApplicationCore.Tests.Services
{
    public class ExhibitorManagerTest
    {
        private List<Category> _categories;
        private List<Exhibitor> _exhibitors;
        private DummyApplicationDbContext _dummy = new DummyApplicationDbContext();
//        private Mock<ICategoryRepository> categoryMock = new Mock<ICategoryRepository>();

        public ExhibitorManagerTest()
        {
            #region MockCategories

            _categories = new List<Category>()
            {
                new Category()
                {
                    Name = "Rolstoelen",
                    Id = 0
                },
                new Category()
                {
                    Name = "Mobile",
                    Id = 1
                }
            };

            #endregion

            #region MockExhibitors          
            _exhibitors = new List<Exhibitor>
            {
                new Exhibitor()
                {
                    Id = 1,
                    Name = "This is exhibitor 1",
                    X = 0.2,
                    Y = 0.2,
                    GroupsAtExhibitor = 0,
                    TotalNumberOfVisits = 9000
                },
                new Exhibitor()
                {
                    Id = 2,
                    Name = "This is exhibitor 2",
                    X = 1,
                    Y = 0.2,
                    GroupsAtExhibitor = 1,
                    TotalNumberOfVisits = 99
                },
                new Exhibitor()
                {
                    Id = 3,
                    Name = "This is exhibitor 3",
                    X = 0.2,
                    Y = 0.15,
                    GroupsAtExhibitor = 2,
                    TotalNumberOfVisits = 80
                },
                new Exhibitor()
                {
//                    Category = _categories[1],
                    Id = 4,
                    Name = "This is exhibitor 4",
                    X = 0.15,
                    Y = 0.2,
                    GroupsAtExhibitor = 0,
                    TotalNumberOfVisits = 120
                },
                new Exhibitor()
                {
//                    Category = _categories[0],
                    Id = 5,
                    Name = "This is exhibitor 5",
                    X = 0.4,
                    Y = 0.4,
                    GroupsAtExhibitor = 0,
                    TotalNumberOfVisits = 61
                },
                new Exhibitor()
                {
                    Id = 6,
                    Name = "This is exhibitor 6",
                    X = 0.41,
                    Y = 0.41,
                    GroupsAtExhibitor = 0,
                    TotalNumberOfVisits = 60
                }
            };
            #endregion
        }
        
        [Theory]
        [InlineData(6, 5)] // Exhibitor with id 5 is neighbouring Exhibitor with id 6.
        [InlineData(5, 6)] // neighbours Exhibitors
        public async Task FindNextExhibitorTest(int correctExhibitorId, int startExhibitorId)
        {
            // the exhibitor where a group is standing at atm.
            var startExhibitor = _exhibitors.SingleOrDefault(ex => ex.Id == startExhibitorId);
            //Always check that _exhibitors doesn't contain Exhibitor with id equal to exhibitorIdStart
            _exhibitors.Remove(startExhibitor);
            
            var mock = new Mock<IExhibitorRepository>();  
            mock.Setup(t => t.GetById(It.IsAny<int>())).Returns(Task.FromResult(startExhibitor));

            var exhibitor = await new ExhibitorManager(mock.Object).FindNextExhibitor(startExhibitorId, _exhibitors);
            Assert.Equal(correctExhibitorId, exhibitor.Id);
        } 
        
        [Fact]
        public async Task FindNextExhibitorTotalVisitsTest()
        {
            var exhibitors = new List<Exhibitor>
            {
                new Exhibitor
                {
                    Id = 1,
                    TotalNumberOfVisits = 1,
                    GroupsAtExhibitor = 1,
                    X = 1,
                    Y = 1,
                },
                new Exhibitor
                {
                    Id = 2,
                    TotalNumberOfVisits = 1,
                    GroupsAtExhibitor = 1,
                    X = 1,
                    Y = 1,
                },
                new Exhibitor
                {
                    Id = 3,
                    TotalNumberOfVisits = 2,
                    GroupsAtExhibitor = 1,
                    X = 1,
                    Y = 1,
                }
            };
            
            // the exhibitor where a group is standing at atm.
            var startExhibitor = exhibitors[0];
            //Always check that _exhibitors doesn't contain Exhibitor with id equal to exhibitorIdStart
            exhibitors.Remove(startExhibitor);
            
            var mock = new Mock<IExhibitorRepository>();  
            mock.Setup(t => t.GetById(It.IsAny<int>())).Returns(Task.FromResult(startExhibitor));

            var exhibitor = await new ExhibitorManager(mock.Object).FindNextExhibitor(startExhibitor.Id, exhibitors);
            Assert.Equal(2, exhibitor.Id);
        }  
        
        [Fact]
        public async Task FindNextExhibitorGroupsAtTest()
        {
            var exhibitors = new List<Exhibitor>
            {
                new Exhibitor
                {
                    Id = 1,
                    TotalNumberOfVisits = 1,
                    GroupsAtExhibitor = 1,
                    X = 1,
                    Y = 1,
                },
                new Exhibitor
                {
                    Id = 2,
                    TotalNumberOfVisits = 1,
                    GroupsAtExhibitor = 1,
                    X = 1,
                    Y = 1,
                },
                new Exhibitor
                {
                    Id = 3,
                    TotalNumberOfVisits = 1,
                    GroupsAtExhibitor = 2,
                    X = 1,
                    Y = 1,
                }
            };
            
            // the exhibitor where a group is standing at atm.
            var startExhibitor = exhibitors[0];
            //Always check that _exhibitors doesn't contain Exhibitor with id equal to exhibitorIdStart
            exhibitors.Remove(startExhibitor);
            
            var mock = new Mock<IExhibitorRepository>();  
            mock.Setup(t => t.GetById(It.IsAny<int>())).Returns(Task.FromResult(startExhibitor));

            var exhibitor = await new ExhibitorManager(mock.Object).FindNextExhibitor(startExhibitor.Id, exhibitors);
            Assert.Equal(2, exhibitor.Id);
        }
        
        [Fact]
        public async Task FindNextExhibitorFullTest()
        {
            var exhibitors = new List<Exhibitor>
            {
                new Exhibitor
                {
                    Name = "startExhibitor",
                    Id = 1,
                    TotalNumberOfVisits = 1,
                    GroupsAtExhibitor = 1,
                    X = 0.1,
                    Y = 0.1,
                },
                new Exhibitor // very far to startExhibitor, but only 1 visitor standing at booth.
                {
                    Name = "potentialNextExhibitor1",
                    Id = 2,
                    TotalNumberOfVisits = 15,
                    GroupsAtExhibitor = 1,
                    X = 1,
                    Y = 1,
                },
                
                new Exhibitor // very close to startExhibitor, but many visitors standing at booth.
                {
                    Name = "potentialNextExhibitor2",
                    Id = 3,
                    TotalNumberOfVisits = 15,
                    GroupsAtExhibitor = 5,
                    X = 0.1,
                    Y = 0.2,
                }
            };
            
            // the exhibitor where a group is standing at atm.
            var startExhibitor = exhibitors[0];
            //Always check that _exhibitors doesn't contain Exhibitor with id equal to exhibitorIdStart
            exhibitors.Remove(startExhibitor);
            
            var mock = new Mock<IExhibitorRepository>();  
            mock.Setup(t => t.GetById(It.IsAny<int>())).Returns(Task.FromResult(startExhibitor));

            var exhibitor = await new ExhibitorManager(mock.Object).FindNextExhibitor(startExhibitor.Id, exhibitors);
            
            // Exhibitor with id 2 should be chosen as the weight of visitors at booth,
            // which is low at that Exhibitor, is more important.
            Assert.Equal(2, exhibitor.Id);
        }
    }
}

