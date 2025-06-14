using Microsoft.ML;
using Microsoft.ML.Data;
using System.IO;
using System.Linq;

namespace Canela.Services
{
    public class SentimentService
    {
        private readonly MLContext _mlContext;
        private readonly PredictionEngine<ModelInput, ModelOutput> _predictionEngine;
        private readonly string _modelPath;

        public class ModelInput
        {
            public string Comment { get; set; }
            public bool Label { get; set; }
        }

        public class ModelOutput
        {
            [ColumnName("PredictedLabel")] public bool Prediction { get; set; }
            public float Probability { get; set; }
            public float Score { get; set; }
        }

        public SentimentService()
        {
            _mlContext = new MLContext();
            _modelPath = Path.Combine(Directory.GetCurrentDirectory(), "ML", "SentimentModel.zip");
            
            ITransformer model = File.Exists(_modelPath) 
                ? _mlContext.Model.Load(_modelPath, out _) 
                : TrainModel();
            
            _predictionEngine = _mlContext.Model.CreatePredictionEngine<ModelInput, ModelOutput>(model);
        }

        private ITransformer TrainModel()
        {
            // 1. Cargar y limpiar datos manualmente
            var dataPath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "wikiDetoxAnnotated40kRows.tsv");
            
            var cleanData = File.ReadAllLines(dataPath)
                .Skip(1) // Saltar encabezado
                .Select(line => line.Split('\t'))
                .Where(parts => parts.Length >= 8)
                .Select(parts => new ModelInput
                {
                    Comment = parts[1],
                    Label = parts[7] == "1" // Solo aceptar 1/0 como valores válidos
                })
                .Where(x => !string.IsNullOrWhiteSpace(x.Comment))
                .ToList();

            // 2. Convertir a IDataView
            var dataView = _mlContext.Data.LoadFromEnumerable(cleanData);

            // 3. Configurar pipeline
            var pipeline = _mlContext.Transforms.Text
                .FeaturizeText(outputColumnName: "Features", inputColumnName: nameof(ModelInput.Comment))
                .Append(_mlContext.BinaryClassification.Trainers.SdcaLogisticRegression(
                    labelColumnName: nameof(ModelInput.Label),
                    featureColumnName: "Features"));

            // 4. Entrenar y guardar modelo
            var model = pipeline.Fit(dataView);
            
            Directory.CreateDirectory(Path.GetDirectoryName(_modelPath));
            _mlContext.Model.Save(model, dataView.Schema, _modelPath);

            return model;
        }

        public (bool IsNegative, float Probability) Analyze(string comment)
        {
            var input = new ModelInput { Comment = comment };
            var prediction = _predictionEngine.Predict(input);
            return (prediction.Prediction, prediction.Probability);
        }
    }
}