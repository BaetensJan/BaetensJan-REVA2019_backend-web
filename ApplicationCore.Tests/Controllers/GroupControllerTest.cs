using System.Collections.Generic;
using System.Linq;
using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Tests.Services.Data;
using Moq;
using Web.Controllers;
using Xunit;
using Web.DTOs;

namespace ApplicationCore.Tests.Controllers
{
    public class GroupControllerTest
    {
        private DummyApplicationDbContext _dummy;
        private Mock<IGroupRepository> _groupMock;
        private Mock<IAssignmentRepository> _assignmentMock;
        private Group group;
        private List<Assignment> assignments;
        private GroupController _controller;

        public GroupControllerTest()
        {
            _dummy = new DummyApplicationDbContext();
            _assignmentMock = new Mock<IAssignmentRepository>();
            _groupMock = new Mock<IGroupRepository>();
            group = _dummy.Groups.FirstOrDefault(x => x.Id == 1);
            assignments = _dummy.Assignments.ToList();
        }
/*
        #region Edit

        [Fact]
        public void EditHttpPost_ValidEdit_UpdatesAndPersistsGroup()
        {
            _groupMock.Setup(p => p.GetById(1)).Returns(group);
            _assignmentMock.Setup(c => c.All()).Returns(assignments);
            var assignments2 = assignments.FindAll(x => x.Id == 1 && x.Id == 2);
            var groupVm = new GroupDTO(group) 
            {
                Name = "Frieda",
                FinishedAssignments = assignments2
            };
            _controller = new GroupController(_groupMock.Object);
            _controller.Edit(1, groupVm);
            Assert.Equal("Frieda", @group.Name);
            Assert.Equal(2, @group.FinishedAssignments.Count());

        }
        [Fact]
        public void EditHttpPost_InValidEdit_DoesNotChangeNorPersistExhibitor()
        {
            var  GroupVm = new GroupDTO(_dummy.Groups.FirstOrDefault(x => x.Id == 1)) {Name = ""};
            _controller = new GroupController(_groupMock.Object);
            _controller.Edit(1, GroupVm);
            var exhibitor = _dummy.Exhibitors.FirstOrDefault(x => x.Id == 1);
            Assert.Equal("klas8b1", exhibitor.Name);
            _groupMock.Verify(m => m.SaveChanges(), Times.Never());
        }
        #endregion
        #region Create

        [Fact]
        public void CreateHttpPost_ValidExhibitor_AddsNewExhibitorToRepository()
        {
            _groupMock.Setup(c => c.Add(It.IsNotNull<Group>()));
            //categoryMock.Setup(p => p.GetById(It.IsAny<int>()))
              //  .Returns(_dummy.Categories.FirstOrDefault(x => x.Id == 1));
            var exhibitorVm = new GroupDTO(new Group()
            {
               Name = "Nieuwe groep",
               FinishedAssignments =  assignments
            });
            _controller = new GroupController(_groupMock.Object);
            _controller.CreateGroup(exhibitorVm);
            _groupMock.Verify(x => x.SaveChanges(), Times.Never());
            _groupMock.Verify(m => m.Add(It.IsAny<Group>()), Times.Never());
        }
        #endregion
        #region Delete

        [Fact]
        public void DeleteHttpPost_ExhibitorFoud_DeletesExhibitor()
        {
            Group groep = _dummy.Groups.FirstOrDefault(x => x.Id == 1);
            _groupMock.Setup(p => p.GetById(1)).Returns(group);
            _groupMock.Setup(p => p.Remove(groep));
            _controller = new GroupController(_groupMock.Object);
            _controller.Delete(1);
            _groupMock.Verify(m => m.Remove(groep), Times.Once());
            _groupMock.Verify(m => m.SaveChanges(), Times.Once);

        }
        #endregion*/
    }
}