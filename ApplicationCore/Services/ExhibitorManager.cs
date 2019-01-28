using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using ApplicationCore.Interfaces;

namespace ApplicationCore.Services
{
    public class ExhibitorManager
    {
        private readonly IExhibitorRepository _exhibitorRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IQuestionRepository _questionRepository;

        public ExhibitorManager(IExhibitorRepository exhibitorRepository, ICategoryRepository categoryRepository,
            IQuestionRepository questionRepository)
        {
            _exhibitorRepository = exhibitorRepository;
            _categoryRepository = categoryRepository;
            _questionRepository = questionRepository;
        }

        public Task<List<Exhibitor>> Exhibitors()
        {
            return _exhibitorRepository.All();
        }

        public Task<IEnumerable<Exhibitor>> ExhibitorsLight()
        {
            return _exhibitorRepository.AllLight();
        }

        /**
         * Finds the "fittest" Exhibitor, holding in account how occupied with visitors and far the exhibitors are.
         */
        public async Task<Exhibitor> FindNextExhibitor(int exhibitorIdStart, int categoryId)
        {
            Exhibitor start = null;
            // The first time the tour starts, the exhibitorId will be -1
            if (exhibitorIdStart != -1)
                start = await _exhibitorRepository.GetById(exhibitorIdStart);

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

            /*Todo enkel categoryExhibitor objecten nemen waaraan een question verbonden is. en een methode in de repo daarvoor voorzien.*/
            var questions = await _questionRepository.GetAll();
            var potentialExhibitors =
                questions.Where(q => q.CategoryExhibitor.CategoryId == categoryId)
                    .Select(e => e.CategoryExhibitor.Exhibitor)
                    .ToList();
            var nextExhibitor = potentialExhibitors[0];
            potentialExhibitors.RemoveAt(0);
            var lowestWeight = GetWeight(nextExhibitor, startX, startY);

            potentialExhibitors.ForEach(e =>
            {
                var weight = GetWeight(e, startX, startY);

                if (weight < lowestWeight)
                {
                    lowestWeight = weight;
                    nextExhibitor = e;
                }
            });
            return nextExhibitor;
        }

        /**
         * value that reflects the weight, holding in account the distance and number of groups standing
         * at the exhibitor, as measure of potential.
         */
        private double GetWeight(Exhibitor exhibitor, double startX, double startY)
        {
            // Distance compared to current exhibitors' position.
            var distance = Math.Abs(startX - exhibitor.X) + Math.Abs(startY - exhibitor.Y);
            // value that reflects the weight, holding in account the distance and number of groups standing
            // at the exhibitor, as measure of potential.
            var weight = (int) (distance + Math.Pow(distance, exhibitor.GroupsAtExhibitor));
            return weight;
        }

        public Exhibitor AddExhibitor(Exhibitor exhibitor)
        {
            Exhibitor returnE = _exhibitorRepository.Add(exhibitor);
            _exhibitorRepository.SaveChanges();
            return returnE;
        }

        public async Task<Exhibitor> RemoveExhibitor(int id)
        {
            Exhibitor exhibitor = await _exhibitorRepository.GetById(id);
            _exhibitorRepository.Remove(exhibitor);
            _exhibitorRepository.SaveChanges();
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

            _exhibitorRepository.SaveChanges();
            return exhibitor;
        }

/*
        public Exhibitor ClosestExhibitor(Exhibitor start, Category category)
        {
            var previous = new Dictionary<Exhibitor, Exhibitor>();
            var distances = new Dictionary<Exhibitor, int>();
            var exhibitors = _exhibitorRepository.All();
            var nodes = new List<Exhibitor>();
            List<Exhibitor> path = null;

            var graph = FindNeighbouringExhibitors(exhibitors);

            foreach (var exhibitorPair in graph)
            {
                if (exhibitorPair.Key == start)
                {
                    distances[exhibitorPair.Key] = 0;
                }
                else
                {
                    distances[exhibitorPair.Key] = int.MaxValue;
                }

                nodes.Add(exhibitorPair.Key);
            }

            while (nodes.Count != 0)
            {
                nodes.Sort((x, y) => distances[x] - distances[y]);

                var smallest = nodes[0];
                nodes.Remove(smallest);

                if (smallest.Category == category)
                {
                    path = new List<Exhibitor>();
                    while (previous.ContainsKey(smallest))
                    {
                        path.Add(smallest);
                        smallest = previous[smallest];
                    }

                    break;
                }

                if (distances[smallest] == int.MaxValue)
                {
                    break;
                }

                foreach (var neighbor in graph[smallest])
                {
                    var alt = distances[smallest] + neighbor.Value;
                    if (alt < distances[neighbor.Key])
                    {
                        distances[neighbor.Key] = alt;
                        previous[neighbor.Key] = smallest;
                    }
                }
            }

            //searching closest in free flight
            return distances.Where(d => d.Key.Category.Name.Equals(category.Name)).OrderBy(x => x.Value)
                .Select(x => x.Key).FirstOrDefault();
        }

        private IDictionary<Exhibitor, IDictionary<Exhibitor, int>> FindNeighbouringExhibitors(
            IEnumerable<Exhibitor> nodes)
        {
            var neighbouringExhibitors = new Dictionary<Exhibitor, IDictionary<Exhibitor, int>>();

            foreach (var exhibitor in nodes)
            {
                //TODO: value of 300 can be configured in Configuration file

                var exhibitorsDistance = CalculateDistances(exhibitor).Where(x => x.Value <= 300)
                    .ToDictionary(t => t.Key, t => t.Value);
                neighbouringExhibitors.Add(exhibitor, exhibitorsDistance);
            }

            return neighbouringExhibitors;
        }

        private IDictionary<Exhibitor, int> CalculateDistances(Exhibitor start)
        {
            var exhibitors = _exhibitorRepository.All();
            var distances = new Dictionary<Exhibitor, int>();

            foreach (var exhibitor in exhibitors)
            {
                var x = (exhibitor.X > start.X ? exhibitor.X - start.X : start.X - exhibitor.X) +
                        exhibitor.GroupsAtExhibitor * 100;
                var y = (exhibitor.Y > start.Y ? exhibitor.Y - start.Y : start.Y - exhibitor.Y) +
                        exhibitor.GroupsAtExhibitor * 100;
                var distance = Convert.ToInt16(Math.Floor(Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2))));
                distances.Add(exhibitor, distance);
            }

            return distances;
        }

        
        public int FindClosestExhibitorsOfCategory(Exhibitor currentExhibitor, string chosenCategory)
        {
            var checkedExhibitors = new List<Exhibitor>();

            // list of potential exhibitors, close to where the group is standing, with the same category as chosen.
            var potentialExhibitors = new List<PotentialExhibitor>();

            // execute recursive method that will find the closest exhibitors with category equal to chosenCategory by group.
            CheckNeighbouringExhibitorsRecursive(currentExhibitor, chosenCategory, checkedExhibitors,
                potentialExhibitors, 0);

            // get exhibitorId of closest exhibitor to current position of the group.
            potentialExhibitors = potentialExhibitors.OrderByDescending(p => p.Counter).ToList();
            var exhibitorId = potentialExhibitors.ElementAt(0).ExhibitorId;

            // Exhibitor exhibitor = _exhibitorRepository.GetById(exhibitorId);

            // work with flags, if exhibitor is grabbed from db - increase his attribute 'inUse', if 'inUse' is different than 0 then 
            // get second elem of potentialExhibitors and try again at the end, when the Group is finished and submitted the assignment,
            // this 'inUse' attribute should be 0 (false) again.
            return exhibitorId;
        }

        private void CheckNeighbouringExhibitorsRecursive(Exhibitor exhibitor, string category,
            ICollection<Exhibitor> checkedExhibitors, ICollection<PotentialExhibitor> validExhibitors, int distance)
        {
            var distanceTo = distance; // not on reference

            // if exhibitor has already been checked.
            if (checkedExhibitors.Contains(exhibitor)) return;

            // add exhibitor to list of checked exhibitors.
            checkedExhibitors.Add(exhibitor);

            // check if exhibitor is of valid category.
            // increase distance after isValid check, in order to give a correct counter param to the recursive, neighbouring exhibitors.
            IsValidExhibitor(exhibitor, category, validExhibitors, distanceTo++);

            exhibitor.NeighbourExhibitors.ForEach(e =>
                CheckNeighbouringExhibitorsRecursive(e, category, checkedExhibitors, validExhibitors, distanceTo));
        }

        private void IsValidExhibitor(Exhibitor exhibitor, string chosenCategory,
            ICollection<PotentialExhibitor> validExhibitors, int counter)
        {
            if (exhibitor.Category.Equals(chosenCategory))
                validExhibitors.Add(new PotentialExhibitor(counter, exhibitor.Id));
        }*/
    }
}