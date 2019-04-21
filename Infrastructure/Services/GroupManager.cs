using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

//todo add interface of GroupManager to ApplicationCore/Interfaces
namespace Infrastructure.Services
{
    public class GroupManager
    {
        private readonly IConfiguration _configuration;

        private readonly IGroupRepository _groupRepository;

        public GroupManager(
            IConfiguration configuration,
            IGroupRepository groupRepository
//            UserManager<ApplicationUser> userManager
        )
        {
            _configuration = configuration;
            _groupRepository = groupRepository;
//            _userManager = userManager;
        }
        
        public GroupManager(IGroupRepository groupRepository)
        {
            _groupRepository = groupRepository;
        }

        public JsonResult GetGroupInfo(Group group)
        {
            if (group.Assignments == null) group.Assignments = new List<Assignment>();

            // Coordinates of entrance of exposition.
            var previousExhibitorXCoordinate = 0.0966804979253112;
            var previousExhibitorYCoordinate = 0.612603305785124;
            var numberOfAssignments = group.Assignments.Count;

            if (numberOfAssignments > 1)
            {
                group.Assignments.Sort((ass1, ass2) => ass1.CreationDate.CompareTo(ass2.CreationDate));

                previousExhibitorXCoordinate =
                    group.Assignments[numberOfAssignments - 2].Question.CategoryExhibitor.Exhibitor.X;
                previousExhibitorYCoordinate =
                    group.Assignments[numberOfAssignments - 2].Question.CategoryExhibitor.Exhibitor.Y;
            }

            var currentAssignment = group.Assignments.LastOrDefault();
            var isCreatedExhibitor = false;
            var numberOfSubmittedAssignments = numberOfAssignments;

            if (currentAssignment != null && !currentAssignment.Submitted)
            {
                numberOfSubmittedAssignments = numberOfAssignments - 1;
                
//                /**
//                 * Check if Exhibitor created by Group.
//                 */
//                if (currentAssignment.Question == null)
//                {
//                    isCreatedExhibitor = true;
//                    currentAssignment.Answer = ""; // we have to temporarily remove the 
//                    // by the group created exhibitor information that we stored in the answer.
//                }
//                    currentAssignment.WithCreatedExhibitor(
//                        _configuration.GetValue<int>("CreatedExhibitorQuestionId"));
            }
            
            var hasNoAssignments = numberOfAssignments == 0;

            var startDate = _configuration.GetValue<DateTime>("StartDate");


            return new JsonResult(
                new
                {
                    startDate,
                    canStartTour =
                        group.Name == "groep9000" ||
                        IsValidDate(startDate, DateTime.Now), // check if group can start Tour based on date.
                    hasNoAssignments, // we need this attribute, because numberOfAssignments != numberOfSubmittedAssignments
                    // (and the app only knows the latter) 
                    numberOfSubmittedAssignments,
                    maxNumberOfAssignments = _configuration.GetValue<int>("AmountOfQuestions"),
                    currentAssignment, // last assignment
                    isCreatedExhibitor,
                    previousExhibitorXCoordinate,
                    previousExhibitorYCoordinate
                });
        }

        /**
         * Check if current date is passed the startDate, which means the Group may start a Tour.
         */
        public static bool IsValidDate(DateTime startDate, DateTime dateTimeNow)
        {
            return dateTimeNow.Hour > 8 // Check if after 8 o'clock. 
                   && DateTime.Compare(dateTimeNow, startDate) > 0; // DateTime.Now "is later than" startDate.
        }

        public async Task<Group> GetGroup(IEnumerable<Claim> claims)
        {
            var groupSidClaim = claims.SingleOrDefault(c => c.Type == ClaimTypes.GroupSid);

            if (groupSidClaim == null)
            {
                return null;
            }

            var groupId = groupSidClaim.Value;

            return await _groupRepository.GetById(Convert.ToInt32(groupId));
        }
    }
}