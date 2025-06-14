using Microsoft.AspNetCore.Mvc;
using Canela.Models;
using Canela.Services;

namespace Canela.Controllers
{
    public class SentimentController : Controller
    {
        private readonly SentimentService _sentimentService;

        public SentimentController(SentimentService sentimentService)
        {
            _sentimentService = sentimentService;
        }

        public IActionResult Index()
        {
            return View(new SentimentViewModel());
        }

        [HttpPost]
        public IActionResult Analyze(SentimentViewModel model)
        {
            if (ModelState.IsValid && !string.IsNullOrWhiteSpace(model.Comment))
            {
                var (isNegative, probability) = _sentimentService.Analyze(model.Comment);
                
                model.Result = isNegative ? 
                    "🔴 Comentario negativo detectado" : 
                    "🟢 Comentario positivo detectado";
                
                model.AlertClass = isNegative ? "danger" : "success";
                model.Probability = probability;

                // Ejemplos para probar:
                // Positivos: "Great job!", "I love this", "Excellent work"
                // Negativos: "You suck", "This is terrible", "I hate it"
            }
            else
            {
                model.Result = "Por favor ingresa un comentario válido";
                model.AlertClass = "warning";
            }

            return View("Index", model);
        }
    }
}