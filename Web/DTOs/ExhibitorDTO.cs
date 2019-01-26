using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography.X509Certificates;
using ApplicationCore.Entities;

namespace Web.DTOs
{
    public class ExhibitorDTO
    {
        [Required] public int Id { get; set; }
        [Required] public string Name { get; set; }
        public List<int> CategoryIds { get; set; } //selected category
        [Required] public double X { get; set; }

        [Required] public double Y { get; set; }

//        [Required] public int GroupsAtExhibitor { get; set; }
        [Required] public string ExhibitorNumber { get; set; } //booth number

        //public List<Exhibitor> NeighbourExhibitors { get; set; } // adjacent, neighbouring exhibitor of this Exhibitor. <= a lot of work and memory heavy
        public ExhibitorDTO(Exhibitor ex)
        {
        }
    }
}