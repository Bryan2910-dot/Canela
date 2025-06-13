using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers;
using Canela.Data;
using Canela.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Canela.Services
{
    public class ProductRecommender
    {
        private readonly ApplicationDbContext _context;
        private MLContext _mlContext;
        private ITransformer _model;

        public ProductRecommender(ApplicationDbContext context)
        {
            _context = context;
            _mlContext = new MLContext();
            TrainModel();
        }

        public void TrainModel()
        {
            // Cargar datos primero a memoria antes de operaciones complejas
            var preOrdenes = _context.DbSetPreOrden
                .Include(p => p.Producto)
                .Where(p => p.Producto != null)
                .AsEnumerable() // Esto fuerza la evaluación en el cliente
                .GroupBy(p => p.UserId)
                .ToList();

            var trainingData = preOrdenes
                .SelectMany(g => g.SelectMany(p1 => g.Select(p2 => new ProductEntry
                {
                    ProductoId = (uint)p1.Producto.Id,
                    CoProductoId = (uint)p2.Producto.Id,
                    Label = (p1.Producto.Id == p2.Producto.Id) ? 0 : 1
                })))
                .ToList();

            // Si no hay datos, usar combinaciones de productos existentes
            if (!trainingData.Any())
            {
                var productos = _context.DbSetProducto
                    .Take(5)
                    .ToList();

                trainingData = productos
                    .SelectMany(p1 => productos.Select(p2 => new ProductEntry
                    {
                        ProductoId = (uint)p1.Id,
                        CoProductoId = (uint)p2.Id,
                        Label = (p1.Id == p2.Id) ? 0 : 1
                    }))
                    .ToList();
            }

            var dataView = _mlContext.Data.LoadFromEnumerable(trainingData);
            
            var options = new MatrixFactorizationTrainer.Options
            {
                MatrixColumnIndexColumnName = nameof(ProductEntry.ProductoId),
                MatrixRowIndexColumnName = nameof(ProductEntry.CoProductoId),
                LabelColumnName = "Label",
                NumberOfIterations = 20,
                ApproximationRank = 100
            };

            var pipeline = _mlContext.Recommendation().Trainers.MatrixFactorization(options);
            _model = pipeline.Fit(dataView);
        }

        public List<Producto> RecommendProducts(int productoId, int count = 3)
        {
            var predictionEngine = _mlContext.Model.CreatePredictionEngine<ProductEntry, ProductPrediction>(_model);
            var allProducts = _context.DbSetProducto.ToList();
            
            var recommendations = allProducts
                .Where(p => p.Id != productoId)
                .Select(p => new
                {
                    Product = p,
                    Score = predictionEngine.Predict(new ProductEntry
                    {
                        ProductoId = (uint)productoId,
                        CoProductoId = (uint)p.Id
                    }).Score
                })
                .OrderByDescending(r => r.Score)
                .Take(count)
                .Select(r => r.Product)
                .ToList();

            // Completar con productos aleatorios si no hay suficientes recomendaciones
            if (recommendations.Count < count)
            {
                var randomProducts = allProducts
                    .Where(p => p.Id != productoId && !recommendations.Any(r => r.Id == p.Id))
                    .OrderBy(p => Guid.NewGuid())
                    .Take(count - recommendations.Count);
                
                recommendations.AddRange(randomProducts);
            }

            return recommendations;
        }
    }

    public class ProductEntry
    {
        [KeyType(count: 1000)]
        public uint ProductoId { get; set; }
        
        [KeyType(count: 1000)]
        public uint CoProductoId { get; set; }
        
        public float Label { get; set; }
    }

    public class ProductPrediction
    {
        public float Score { get; set; }
    }
}