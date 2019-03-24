using System;
using Infrastructure.Services;
using Xunit;


namespace ApplicationCore.Tests.Services
//todo deze Manager tests moeten prolly in Infrastructure.
{
    public class GroupManagerTest
    {
        private const string Title = "GroupManagerTest";
        private readonly DateTime _startDate;

        public GroupManagerTest()
        {
            _startDate = Convert.ToDateTime("2019-04-25T08:08:00+01:00");
        }

        /**
         * Test if the app can be used 1 day after the StartDate.
         */
        [Fact]
        public void DateTest1DayAfterStartDate()
        {
            Assert.True(GroupManager.IsValidDate(_startDate, _startDate.AddDays(1)));
        }

        /**
         * Test if the app can be used 1 day before the StartDate.
         */
        [Fact]
        public void DateTest1DayBeforeStartDate()
        {
            Assert.False(GroupManager.IsValidDate(_startDate, _startDate.AddDays(-1)));
        }

        /**
         * Test if the app can be used 1 hour after the StartDate.
         */
        [Fact]
        public void DateTest1HourAfterStartDate()
        {
            Assert.True(GroupManager.IsValidDate(_startDate, _startDate.AddHours(1)));
        }

        /**
         * Test if the app can be used 1 hour before the StartDate.
         */
        [Fact]
        public void DateTest1HourBeforeStartDate()
        {
            Assert.False(GroupManager.IsValidDate(_startDate, _startDate.AddHours(-1)));
        }

        /**
         * Test if the app can be used before 8 o'clock (start of the exposition).
         */
        [Fact]
        public void StartDateTestBefore8()
        {
            var before8 = new DateTime(_startDate.Year, _startDate.Month, _startDate.Day, 7, 59, 59);
            Assert.False(GroupManager.IsValidDate(_startDate, before8));
        }

        /**
         * Test if the app can be used before 8 o'clock (start of the exposition) on wrong date.
         */
        [Fact]
        public void StartDateTestBefore8WrongDate()
        {
            var wrongDate = new DateTime(2019, 4, 24, 7, 0, 0);
            Assert.False(GroupManager.IsValidDate(_startDate, wrongDate));
        }

        /**
        * Test if the app can be used before 8 o'clock (start of the exposition) on wrong date.
        */
        [Fact]
        public void StartDateTestAfter8WrongDate()
        {
            var wrongDate = new DateTime(2019, 4, 24, 8, 0, 1);
            Assert.False(GroupManager.IsValidDate(_startDate, wrongDate));
        }

        /**
         * Test if the app can be used after 8 o'clock (start of the exposition).
         */
        [Fact]
        public void StartDateTestAfter8()
        {
            Assert.True(GroupManager.IsValidDate(_startDate, _startDate.AddMinutes(1)));
        }

        /**
         * This is a test written to be sure that the datetime string representation, saved in
         * appsettings ("StartDate") is correct with the current timezone.
         */
        [Fact]
        public void TestCurrentTimeWithStringInput()
        {
            var dateTimeNow = DateTime.Now;
            
            // 1.
            // To test the current GMT +1 time, fill in the current date and time,
            // uncomment the lines beneath and debug to get a string representation of the current datetime.

            var startDateManually = new DateTime(dateTimeNow.Year, dateTimeNow.Month, dateTimeNow.Day, dateTimeNow.Hour,
                dateTimeNow.Minute, dateTimeNow.Second, dateTimeNow.Millisecond,  DateTimeKind.Local);

            // 2.
            // Fill the string representation in this and add some minutes, in order for you to run
            // the test at correct time
            // Or edit the string to the current date and time and add some extra minutes.
            var dateTimeString = startDateManually.ToString("o");

            System.Diagnostics.Debug.WriteLine($"{Title} - String representation of" +
                                               $" current DateTime with Timezone: {dateTimeString}");

            var startDate = Convert.ToDateTime(dateTimeString);

            // 3.
            // Run the test 1 minute BEFORE the time of the string representation you made before, where you
            // added some extra minutes.
            
            // We add the minutes & milliseconds of the startDate as both dateTime objects were not created on the
            // same minute, second and millisecond.
            dateTimeNow = new DateTime(dateTimeNow.Year, dateTimeNow.Month, dateTimeNow.Day, dateTimeNow.Hour, startDate.Minute, startDate.Second, startDate.Millisecond);
            
            // 4.
            // current time is the same dateTime you created with the string representation.
            Assert.True(DateTime.Compare(dateTimeNow, startDate) == 0);
            
            Assert.Equal(dateTimeNow.Year, startDate.Year);
            Assert.Equal(dateTimeNow.Month, startDate.Month);
            Assert.Equal(dateTimeNow.Day, startDate.Day);
            Assert.Equal(dateTimeNow.Hour, startDate.Hour);
            Assert.Equal(dateTimeNow.Minute, startDate.Minute);
            Assert.Equal(dateTimeNow.Second, startDate.Second);
            Assert.Equal(dateTimeNow.Millisecond, startDate.Millisecond);

            // 5.
            // Added 1 hour to the datetimeNow.
            dateTimeNow = dateTimeNow.AddHours(1);

            // current time is AFTER the time you created with the string representation.
            Assert.True(DateTime.Compare(dateTimeNow, startDate) > 0);

            // 6.
            // Removed 1 hour to the datetimeNow.
            dateTimeNow = dateTimeNow.AddHours(-1);

            // current time is EQUAL to the time you created with the string representation.
            Assert.True(DateTime.Compare(dateTimeNow, startDate) == 0);
        }
    }
}