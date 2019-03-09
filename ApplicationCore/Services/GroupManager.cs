using System;
using System.Collections.Generic;
using System.Linq;
using ApplicationCore.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace ApplicationCore.Services
{
    public class GroupManager
    {
        private readonly IConfiguration _configuration;

        public GroupManager(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public JsonResult GetGroupInfo(Group group)
        {
            if (group.Assignments == null) group.Assignments = new List<Assignment>();

            var previousExhibitorXCoordinate = 0.0;
            var previousExhibitorYCoordinate = 0.0;
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
                isCreatedExhibitor =
                    currentAssignment.WithCreatedExhibitor(
                        _configuration.GetValue<int>("CreatedExhibitorQuestionId"));
            }

            var hasNoAssignments = numberOfAssignments == 0;

            var startDate = _configuration.GetValue<DateTime>("StartDate");
            

            return new JsonResult(
                new
                {
                    startDate,
                    canStartTour = group.Name == "group1" || IsValidDate(startDate), // check if group can start Tour based on date.
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
        private static bool IsValidDate(DateTime startDate)
        {
            return DateTime.Compare(DateTime.Now, startDate) > 0; //DateTime.Now "is later than" startDate.
        }
    }
}