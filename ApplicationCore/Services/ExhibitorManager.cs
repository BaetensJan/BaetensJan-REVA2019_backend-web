using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using ApplicationCore.Interfaces;

namespace ApplicationCore.Services
{
    public class ExhibitorManager
    {
        private readonly IExhibitorRepository _exhibitorRepository;

        public ExhibitorManager(IExhibitorRepository exhibitorRepository)
        {
            _exhibitorRepository = exhibitorRepository;
        }

        public Task<IEnumerable<Exhibitor>> Exhibitors()
        {
            return _exhibitorRepository.All();
        }

        public Task<IEnumerable<Exhibitor>> ExhibitorsLight()
        {
            return _exhibitorRepository.AllLight();
        }

        /**
         * Finds the "fittest" Exhibitor, holding in account how occupied with visitors and far the exhibitors are.
         *
         * Always check that potentialExhibitors doesn't contain Exhibitor with id equal to startExhibitorId
         */
        public async Task<Exhibitor> FindNextExhibitor(int startExhibitorId, List<Exhibitor> potentialExhibitors)
        {
            Exhibitor start = null;
            // The first time the tour starts, the exhibitorId will be -1
            if (startExhibitorId != -1)
                start = await _exhibitorRepository.GetById(startExhibitorId);

            double startX;
            double startY;

            if (start == null)
            {
                startX = new Random().NextDouble();
                startY = new Random().NextDouble();
            }
            else
            {
                startX = start.X;
                startY = start.Y;
            }

            var maxDistance = 0.0;
            // contains the distances per Exhibitor to the current Exhibitor.
            var distances = new List<double>();
            var maxVisitorsAtExhibitor = 0;
            var maxTotalVisitors = 0;

            potentialExhibitors.ForEach(exh =>
            {
                // Distance compared to current exhibitors' position.
                var distance = Math.Abs(startX - exh.X) + Math.Abs(startY - exh.Y);

                distances.Add(distance);

                if (distance > maxDistance) maxDistance = distance;

                if (exh.GroupsAtExhibitor > maxVisitorsAtExhibitor) maxVisitorsAtExhibitor = exh.GroupsAtExhibitor;

                if (exh.TotalNumberOfVisits > maxTotalVisitors) maxTotalVisitors = exh.TotalNumberOfVisits;
            });

            var nextExhibitor = potentialExhibitors[0];
            var highestWeight = GetWeight(nextExhibitor, distances[0], maxDistance, maxVisitorsAtExhibitor,
                maxTotalVisitors);

            for (var i = 1; i < potentialExhibitors.Count; i++)
            {
                var exhibitor = potentialExhibitors[i];
                var weight = GetWeight(exhibitor, distances[i], maxDistance, maxVisitorsAtExhibitor, maxTotalVisitors);

                if (weight < highestWeight)
                {
                    nextExhibitor = exhibitor;
                    highestWeight = weight;
                }
            }

            return nextExhibitor;
        }


        /**
         * value that reflects the weight, holding in account the distance and number of groups standing
         * at the exhibitor, as measure of potential.
         */
        private double GetWeight(Exhibitor exhibitor, double distance, double maxDistance,
            double maxVisitorsAtExhibitor,
            double maxTotalVisitors)
        {
            // every weight is a number between 0 and 1.
            var distanceWeight = distance / maxDistance;
            var visitorsAtExhibitorWeight = maxVisitorsAtExhibitor == 0
                ? maxVisitorsAtExhibitor
                : exhibitor.GroupsAtExhibitor / maxVisitorsAtExhibitor;
            var totalVisitorsWeight = maxTotalVisitors == 0
                ? maxTotalVisitors
                : exhibitor.TotalNumberOfVisits / maxTotalVisitors;

            // We multiply each weight-attribute with a vector that represents its procentual (total is 100) rate of
            // importance.
            var weight = distanceWeight * 0.25 + visitorsAtExhibitorWeight * 0.5 + totalVisitorsWeight * 0.25;

            // the weight of the Exhibitor is a number between 0 and 1.
            return weight;
        }

        public async Task<Exhibitor> AddExhibitor(Exhibitor exhibitor)
        {
            await _exhibitorRepository.Add(exhibitor);
            await _exhibitorRepository.SaveChanges();
            return exhibitor;
        }

        public async Task<Exhibitor> UpdateExhibitor(int id, Exhibitor exhibitorLast)
        {
            Exhibitor exhibitor = await _exhibitorRepository.GetById(id);
            exhibitor.Name = exhibitorLast.Name;
            exhibitor.X = exhibitorLast.X;
            exhibitor.Y = exhibitorLast.Y;
            exhibitor.ExhibitorNumber = exhibitorLast.ExhibitorNumber;
            exhibitor.GroupsAtExhibitor = exhibitorLast.GroupsAtExhibitor;
            exhibitor.Categories = exhibitorLast.Categories;
            _exhibitorRepository.Update(exhibitor);
            await _exhibitorRepository.SaveChanges();
            return exhibitor;
        }

        public async Task<Exhibitor> RemoveExhibitor(int id)
        {
            Exhibitor exhibitor = await _exhibitorRepository.GetById(id);
            _exhibitorRepository.Remove(exhibitor);
            await _exhibitorRepository.SaveChanges();
            return exhibitor;
        }

        public async Task<Exhibitor> UpdateExhibitor(Exhibitor exhibitor)
        {
            Exhibitor e = await _exhibitorRepository.GetById(exhibitor.Id);

            e.Name = exhibitor.Name;
            e.X = exhibitor.X;
            e.Y = exhibitor.Y;
            e.Categories = exhibitor.Categories;
            e.ExhibitorNumber = exhibitor.ExhibitorNumber;

            await _exhibitorRepository.SaveChanges();
            return exhibitor;
        }
    }
}