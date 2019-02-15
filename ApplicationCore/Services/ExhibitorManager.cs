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
         */
        public async Task<Exhibitor> FindNextExhibitor(int exhibitorIdStart, int categoryId, List<Exhibitor> potentialExhibitors)
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