using Python.Runtime;

namespace PredictionModels.Service
{
    public class ForecastService
    {
        private dynamic regressionModel;
        private dynamic timeseriesModel;

        public ForecastService()
        {
            regressionModel = LoadModel("xgboost_model.joblib");
            timeseriesModel = LoadModel("timeseries_model.joblib");
        }

        private dynamic LoadModel(string modelFile)
        {
            // Initialize the Python runtime
            //PythonEngine.Initialize();

            using (Py.GIL()) // Acquire the Python GIL
            {
                try
                {
                    // Import required modules
                    dynamic joblib = Py.Import("joblib");
                    dynamic np = Py.Import("numpy");

                    // Load the model
                    var modelPath = Path.Combine(Directory.GetCurrentDirectory(), "Models", modelFile) ;
                    return joblib.load(modelPath);
                }
                catch (Exception ex)
                {
                    throw new Exception($"An error occurred while loading the model: {ex.Message}");
                }
            }
        }

        public float PredictXGBoostModel(double[] inputData)
        {
            return GetForcast(regressionModel, inputData);
        }

        public float PredictTimeseriesModel(float[] previousTimestamps, float[] previousLabels )
        {
            // Step 1: Combine the features into a single 2D array
            float[,] inputData2 = new float[10, 2];
            for (int i = 0; i < 10; i++)
            {
                inputData2[i, 0] = previousTimestamps[i];
                inputData2[i, 1] = previousLabels[i]; 
            }
            using (Py.GIL())
            {
                try
                {

                    // Import the required Python modules
                    dynamic joblib = Py.Import("joblib");
                    dynamic np = Py.Import("numpy");
                    string scalerPath = Path.Combine(Directory.GetCurrentDirectory(), "Models", "scaler.joblib");
                    dynamic scaler = joblib.load(scalerPath);

                    // Load the scaler
                    //string scalerPath = Path.Combine(Directory.GetCurrentDirectory(), "Models", "scaler.pkl");
                    //dynamic scaler = joblib.load(scalerPath);

                    // Prepare input data (convert to numpy array and reshape)
                    dynamic inputArray = np.array(inputData2).reshape(1, 10, 2); // Assuming one feature

                    // Scale the input data
                    dynamic reshapedInput = inputArray.reshape(-1, 2);
                    dynamic scaledInput = scaler.fit_transform(reshapedInput);  // Scale to (10, 2)

                    // Reshape back to (1, 10, 2)
                    inputArray = scaledInput.reshape(1, 10, 2);

                    // Make the prediction
                    dynamic prediction = timeseriesModel.predict(inputArray);
                   
                    // Return the prediction
                    return (float)prediction[0][0];
                }
                catch (Exception ex)
                {
                    throw new Exception($"An error occurred while making a prediction: {ex.Message}");
                }
            }
        }

            private float GetForcast(dynamic model, double[] inputData)
        {
            using (Py.GIL())
            {
                try
                {

                    // Import required modules
                    dynamic joblib = Py.Import("joblib");
                    dynamic np = Py.Import("numpy");

                    // Prepare input data (convert to numpy array)
                    dynamic inputArray = np.array(inputData).reshape(1, -1);

                    // Make the prediction
                    dynamic prediction = model.predict(inputArray);

                    // Return the prediction as a float value
                    return (float)prediction[0];
                }
                catch (Exception ex)
                {
                    throw new Exception($"An error occurred while making a prediction: {ex.Message}");
                }
            }
        }

    }
}