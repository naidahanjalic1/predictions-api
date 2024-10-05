# predictions-api

## Regression Forecasting API

This API endpoint allows you to retrieve regression forecasts based on the provided input features.

### Endpoint

**GET** `/forecast/regression`

### Query Parameters

- **inputFeatures**: (required) An array of double values representing the input features for prediction. You can specify multiple values by repeating the parameter:
  - Example: `inputFeatures=45141.01042&inputFeatures=150&inputFeatures=8&inputFeatures=3&inputFeatures=2023&inputFeatures=4&inputFeatures=1&inputFeatures=0&inputFeatures=0&inputFeatures=0&inputFeatures=15&inputFeatures=0&inputFeatures=4806.7&inputFeatures=50.06979167&inputFeatures=9301.12&inputFeatures=48.44333333&inputFeatures=0&inputFeatures=0&inputFeatures=0&inputFeatures=0&inputFeatures=0&inputFeatures=0&inputFeatures=0&inputFeatures=0&inputFeatures=0&inputFeatures=0&inputFeatures=0&inputFeatures=0`

### Example Request

```plaintext
GET /forecast/regression?inputFeatures=45141.01042&inputFeatures=150&inputFeatures=8&inputFeatures=3&inputFeatures=2023&inputFeatures=4&inputFeatures=1&inputFeatures=0&inputFeatures=0&inputFeatures=0&inputFeatures=15&inputFeatures=0&inputFeatures=4806.7&inputFeatures=50.06979167&inputFeatures=9301.12&inputFeatures=48.44333333&inputFeatures=0&inputFeatures=0&inputFeatures=0&inputFeatures=0&inputFeatures=0&inputFeatures=0&inputFeatures=0&inputFeatures=0&inputFeatures=0&inputFeatures=0&inputFeatures=0&inputFeatures=0

```

## Time-Series Forecasting API

This API endpoint allows you to retrieve time-series forecasts based on provided label and timestamp data.

### Endpoint

**GET** `/foreast/time-series`

### Query Parameters

- **labels**: (required) An array of float values representing the last 10 labels for prediction. You can specify multiple values by repeating the parameter:
  - Example: `labels=46.38&labels=52.94&labels=36.16&labels=48.38&labels=119.1&labels=115.82&labels=117.74&labels=94.74&labels=102.42&labels=89.18`

- **timestamps**: (required) An array of float values representing the last 10 timestamps for prediction. Similar to labels, you can specify multiple values by repeating the parameter:
  - Example: `timestamps=45141.63542&timestamps=45141.64583&timestamps=45141.65625&timestamps=45141.66667&timestamps=45141.67708&timestamps=45141.6875&timestamps=45141.69792&timestamps=45141.70833&timestamps=45141.71875&timestamps=45141.72917`

### Example Request

```plaintext
GET /forecast/time-series?labels=46.38&labels=52.94&labels=36.16&labels=48.38&labels=119.1&labels=115.82&labels=117.74&labels=94.74&labels=102.42&labels=89.18&timestamps=45141.63542&timestamps=45141.64583&timestamps=45141.65625&timestamps=45141.66667&timestamps=45141.67708&timestamps=45141.6875&timestamps=45141.69792&timestamps=45141.70833&timestamps=45141.71875&timestamps=45141.72917

```

## Discussion

#### Supporting large datasets (range of tens of GBs) for training
To effectively support large datasets for training, it is essential to store them in cloud storage rather than locally. This ensures accessibility and scalability. Model training should also be conducted on a cloud service, allowing for the use of powerful computational resources. Once trained, models should be stored in the cloud to facilitate easy access by the application.

#### Supporting large datasets (range of tens of GBs) through APIs
When designing the API for predictions that supports large datasets, it should implement batch processing to allow clients to send multiple data points in a single request. This optimization enhances performance and minimizes overhead by reducing the number of individual API calls.
Additionally, the API should utilize pagination or chunking for data retrieval to avoid overwhelming clients with large responses. 
Data compression techniques should be leveraged to reduce the size of data being transferred over the network. This enhancement not only improves response times but also reduces bandwidth consumption.

#### Supporting large number of API calls
To efficiently support a high volume of API calls, it is crucial to implement a scalable architecture. This includes using load balancers to distribute incoming requests across multiple instances of the application. It is acceptable to have multiple instances of the app, as they can share the same model stored in the cloud, which simplifies management and ensures consistency. 


#### Analysis of Results on Test Data

Regression Model:
- **Mean Absolute Error**: 4.49
- **R² Score**: 0.96

LSTM Model for Time-Series:
- **Mean Absolute Error**: 0.01
- **R² Score**: 0.96

It is evident that the time-series prediction significantly outperforms the regression model, as indicated by the smaller error. Observing the data reveals a recurring trend associated with the timestamps, suggesting that the other input features have minimal impact on the predictions.

### Running the application
To be able to run the application, go to appsettings.json and edit variables:
  - PythonPath - your local python path
  -  PythonDll - your local pythonDLL
Also, python 3 should be installed locally, it should not be higher than 13.10.x and Visual Studio that supports .NET 6 applications.
On application start, both of the models are trained and output can be seen in the console. After training completion, they are stored in Models folder in the App.

