using System;
using System.Collections.Generic;
using System.Linq;
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
    public class CategoryManagerTest
    {
        private List<Category> _categories;
        private List<Exhibitor> _exhibitors;
        private Mock<IExhibitorRepository> mock;
        private DummyApplicationDbContext _dummy = new DummyApplicationDbContext();
        private Mock<ICategoryRepository> categoryMock = new Mock<ICategoryRepository>();
        private ExhibitorController _controller;

        public CategoryManagerTest()
        {
            /*
            mock = new Mock<IExhibitorRepository>();

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
            _exhibitors = new List<Exhibitor>()
            {
                new Exhibitor()
                {
                    Categories = _categories,
                    Id = 1,
                    Name = "This is exhibitor 1",
                    X = 20,
                    Y = 20,
                    GroupsAtExhibitor = 0
                },
                new Exhibitor()
                {
                    Categories = _categories,
                    Id = 2,
                    Name = "This is exhibitor 2",
                    X = 200,
                    Y = 20,
                    GroupsAtExhibitor = 1
                },
                new Exhibitor()
                {
                    Category = _categories[0],
                    Id = 3,
                    Name = "This is exhibitor 3",
                    X= 20,
                    Y= 150,
                    GroupsAtExhibitor = 2
                },
                new Exhibitor()
                {
                    Category = _categories[1],
                    Id = 4,
                    Name = "This is exhibitor 4",
                    X= 150,
                    Y= 20,
                    GroupsAtExhibitor = 0
                },
                new Exhibitor()
                {
                    Category = _categories[0],
                    Id = 5,
                    Name = "This is exhibitor 5",
                    X= 400,
                    Y= 400,
                    GroupsAtExhibitor = 0
                }
        };
    }

    
    [Fact]
    public void ClosestExhibitorOfCategoryTest()
    {
        Mock<IExhibitorRepository> mock = new Mock<IExhibitorRepository>();
        mock.Setup(t => t.All()).Returns(_exhibitors);
        
        ExhibitorManager exhibitorManager = new ExhibitorManager(mock.Object);
        Exhibitor exhibitor = exhibitorManager.ClosestExhibitor(_exhibitors[0], _categories[0]);
        Assert.True(exhibitor.Id == 2);
    }*/
            /*
            [Fact]
            public void FindNextExhibitorTest()
            {
                mock = new Mock<IExhibitorRepository>();
                mock.Setup(t => t.All()).Returns(_exhibitors);
                mock.Setup(t => t.GetById(It.IsAny<int>())).Returns(_exhibitors[0]);
                categoryMock.Setup(t => t.GetById(It.IsAny<int>())).Returns(_categories[0]);
        
                ExhibitorManager exhibitorManager = new ExhibitorManager(mock.Object, categoryMock.Object);
                Exhibitor exhibitor = exhibitorManager.FindNextExhibitor(_exhibitors[0].Id, _categories[0].Id);
                Assert.True(exhibitor.Id == 2);
            }
        
            #region Edit
        
            [Fact]
            public void Edit_validEdit_ChangesAndPersistsExhibitor()
            {
                Exhibitor ex = new Exhibitor();
                ex = _dummy.Exhibitors.FirstOrDefault();
                mock.Setup(t => t.GetById(1)).Returns(ex);
                categoryMock.Setup(t => t.GetById(1))
                    .Returns(_dummy.Categories.FirstOrDefault());
                categoryMock.Setup(t => t.GetById(2))
                    .Returns(_dummy.Categories.SingleOrDefault(c => c.Id == 2));
                Exhibitor exhibitor2 = new Exhibitor()
                {
                    Id=1,
                    Categories = _dummy.Categories.ToList(),//.SingleOrDefault(x => x.Id == 2),
                    Name = "exhibitor gewijzigd",
                    X = 500,
                    Y = 200,
                    Question = "Wat is de functie van een rolstoel?"
                };
                _controller = new ExhibitorController(mock.Object, categoryMock.Object);
                _controller.UpdateExhibitor(exhibitor2);
                Assert.Equal("exhibitor gewijzigd", ex.Name);
        //            Assert.Equal(_dummy.Categories.SingleOrDefault(x => x.Id == 2), ex.Category);
                Assert.Equal(_dummy.Categories.ToList(), ex.Categories);
                Assert.Equal(500, ex.X);
                Assert.Equal(200, ex.Y);
                mock.Verify(m => m.SaveChanges(), Times.Once);
                
                
            }
        
            [Fact]
            public void EditHttpPost_InValidEdit_DoesNotChangeNorPersistExhibitor()
            {
                var exhibitorVm = new ExhibitorDTO(_dummy.Exhibitors.FirstOrDefault(x => x.Id == 1)) {Name = ""};
                categoryMock.Setup(c => c.GetById(1)).Returns(_dummy.Categories.FirstOrDefault(x => x.Id == 1));
                _controller = new ExhibitorController(mock.Object, categoryMock.Object);
                Exhibitor exhibitor = new Exhibitor();
                _controller.UpdateExhibitor(exhibitor);
                Assert.Equal("Jos Van Elst", exhibitor.Name);
                mock.Verify(m => m.SaveChanges(), Times.Never());
            }
            #endregion
        
            #region Create
        
            [Fact]
            public void CreateHttpPost_ValidExhibitor_AddsNewExhibitorToRepository()
            {
                mock.Setup(c => c.Add(It.IsNotNull<Exhibitor>()));
                categoryMock.Setup(p => p.GetById(It.IsAny<int>()))
                    .Returns(_dummy.Categories.FirstOrDefault(x => x.Id == 1));
                var exhibitorVm = new ExhibitorDTO(new Exhibitor()
                {
                    Categories = _dummy.Categories.ToList(),//.FirstOrDefault(x => x.Id == 1),
                    GroupsAtExhibitor = 20,
                    Id = 4,
                    Name = "Joris",
                    Question = "Vraag?", 
                    X = 9,
                    Y = 9
                });
                _controller = new ExhibitorController(mock.Object, categoryMock.Object);
               // _controller.Create(exhibitorVm);
                mock.Verify(x => x.SaveChanges(), Times.Never());
                mock.Verify(m => m.Add(It.IsAny<Exhibitor>()), Times.Never());
            }
            #endregion
            #region Delete
        
            [Fact]
            public void DeleteHttpPost_ExhibitorFound_DeletesExhibitor()
            {
                Exhibitor ex = _dummy.Exhibitors.FirstOrDefault(x => x.Id == 1); // exhibitor met id 1
                mock.Setup(p => p.GetById(1)).Returns(ex); // zorgt dat mock exhibitor teruggeeft met id 1
                categoryMock.Setup(p => p.GetById(1)).Returns(_dummy.Categories.FirstOrDefault(x => x.Id == 1)); 
                //mock.Setup(p => p.Remove(ex));
                ExhibitorManager exManager = new ExhibitorManager(mock.Object, categoryMock.Object);
                exManager.RemoveExhibitor(1);
                mock.Verify(m => m.Remove(ex), Times.Once());
                mock.Verify(m => m.SaveChanges(), Times.Once);
        
            }
            #endregion
            */
        }
    }
}