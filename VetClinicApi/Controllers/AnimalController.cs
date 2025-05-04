using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using VetClinicApi.Models;

namespace VetClinicApi.Controllers
{
    [ApiController]
    [Route("api/animals")]
    public class AnimalsController : ControllerBase
    {
        private static List<Animal> _animals = new List<Animal>()
        {
            new Animal { Id = 1, Name = "Burek", Category = "Pies", Mass = 15.5, FurColor = "Brązowy" },
            new Animal { Id = 2, Name = "Mruczek", Category = "Kot", Mass = 4.2, FurColor = "Czarny" },
            new Animal { Id = 3, Name = "Azor", Category = "Pies", Mass = 22.1, FurColor = "Biały w łaty" },
            new Animal { Id = 4, Name = "Puszek", Category = "Kot", Mass = 5.0, FurColor = "Rudy" },
            new Animal { Id = 5, Name = "Burek", Category = "Pies", Mass = 12.0, FurColor = "Czarny" }
        };

        private static List<Visit> _visits = new List<Visit>()
        {
            new Visit { Id = 1, AnimalId = 1, VisitDate = new DateTime(2025, 5, 1, 10, 0, 0), Description = "Szczepienie", Price = 120.00m },
            new Visit { Id = 2, AnimalId = 2, VisitDate = new DateTime(2025, 5, 2, 11, 30, 0), Description = "Kontrola ogólna", Price = 80.00m },
            new Visit { Id = 3, AnimalId = 1, VisitDate = new DateTime(2025, 5, 3, 14, 0, 0), Description = "Opatrzenie rany", Price = 150.50m }
        };
        private static int _nextAnimalId = _animals.Count > 0 ? _animals.Max(a => a.Id) + 1 : 1;
        private static int _nextVisitId = _visits.Count > 0 ? _visits.Max(v => v.Id) + 1 : 1;

        [HttpGet]
        public ActionResult<IEnumerable<Animal>> GetAnimals([FromQuery] string? name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                var filteredAnimals = _animals.Where(a => a.Name.Equals(name, StringComparison.OrdinalIgnoreCase)).ToList();
                return Ok(filteredAnimals);
            }
            return Ok(_animals);
        }

        [HttpGet("{id:int}")]
        public ActionResult<Animal> GetAnimalById(int id)
        {
            var animal = _animals.FirstOrDefault(a => a.Id == id);
            if (animal == null)
            {
                return NotFound($"Nie znaleziono zwierzęcia o ID: {id}");
            }
            return Ok(animal);
        }

        [HttpPost]
        public ActionResult<Animal> AddAnimal([FromBody] Animal newAnimal)
        {
            if (newAnimal == null)
            {
                return BadRequest("Dane zwierzęcia nie mogą być puste.");
            }

            newAnimal.Id = _nextAnimalId++;
            _animals.Add(newAnimal);

            return CreatedAtAction(nameof(GetAnimalById), new { id = newAnimal.Id }, newAnimal);
        }

        [HttpPut("{id:int}")]
        public IActionResult UpdateAnimal(int id, [FromBody] Animal updatedAnimal)
        {
            if (updatedAnimal == null || id != updatedAnimal.Id)
            {
                return BadRequest("ID zwierzęcia w URL nie zgadza się z ID w ciele żądania lub dane są puste.");
            }

            var existingAnimal = _animals.FirstOrDefault(a => a.Id == id);
            if (existingAnimal == null)
            {
                return NotFound($"Nie znaleziono zwierzęcia o ID: {id} do aktualizacji.");
            }

            existingAnimal.Name = updatedAnimal.Name;
            existingAnimal.Category = updatedAnimal.Category;
            existingAnimal.Mass = updatedAnimal.Mass;
            existingAnimal.FurColor = updatedAnimal.FurColor;

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public IActionResult DeleteAnimal(int id)
        {
            var animalToRemove = _animals.FirstOrDefault(a => a.Id == id);
            if (animalToRemove == null)
            {
                return NotFound($"Nie znaleziono zwierzęcia o ID: {id} do usunięcia.");
            }

            _animals.Remove(animalToRemove);
            _visits.RemoveAll(v => v.AnimalId == id);

            return NoContent();
        }

        [HttpGet("{animalId:int}/visits")]
        public ActionResult<IEnumerable<Visit>> GetVisitsForAnimal(int animalId)
        {
            var animalExists = _animals.Any(a => a.Id == animalId);
            if (!animalExists)
            {
                return NotFound($"Nie znaleziono zwierzęcia o ID: {animalId}.");
            }

            var animalVisits = _visits.Where(v => v.AnimalId == animalId).ToList();
            return Ok(animalVisits);
        }

        [HttpPost("{animalId:int}/visits")]
        public ActionResult<Visit> AddVisitForAnimal(int animalId, [FromBody] Visit newVisit)
        {
             var animal = _animals.FirstOrDefault(a => a.Id == animalId);
             if (animal == null)
             {
                return NotFound($"Nie można dodać wizyty - nie znaleziono zwierzęcia o ID: {animalId}.");
            }

            if (newVisit == null)
            {
                return BadRequest("Dane wizyty nie mogą być puste.");
            }

            newVisit.AnimalId = animalId;
            newVisit.Id = _nextVisitId++;
            _visits.Add(newVisit);

             return Ok(newVisit);
        }
    }
}