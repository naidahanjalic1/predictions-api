using Python.Runtime;
using System.Runtime.Intrinsics.X86;

namespace PreditionModels.Helpers
{
    public static class ModelTrainer
    {
        public static void TrainXGBoostModel()
        {
            using (Py.GIL())
            {
                var csvFileName = "Regression_dataset.csv";
                string csvFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", csvFileName);
                var modelSavePath = Path.Combine(Directory.GetCurrentDirectory(), "Models", "xgboost_model.joblib"); ;
                try
                {

                    string pythonCode = $@"
import pandas as pd
import xgboost as xgb
from sklearn.model_selection import train_test_split
from sklearn.preprocessing import LabelEncoder
from sklearn.metrics import mean_absolute_error, r2_score
import joblib

# Load the data from a CSV file
df = pd.read_csv(r'{csvFilePath}')

# Convert columns to numeric, forcing errors to NaN if there are non-numeric values
df['V14'] = pd.to_numeric(df['V14'], errors='coerce')
df['V16'] = pd.to_numeric(df['V16'], errors='coerce')

# Handle missing values (e.g., fill with mean)
df['V14'].fillna(df['V14'].mean(), inplace=True)
df['V16'].fillna(df['V16'].mean(), inplace=True)

# Define the label column
label_column = 'Label'

# Check for NaN values in the label column
if df[label_column].isnull().any():
    df = df.dropna(subset=[label_column])  # Drop rows with NaN in label column

# Select all columns except for the label column for features
X = df.drop(columns=[label_column])  # Drop the label column
y = df[label_column]  # Assign the label column

X_train, X_test, y_train, y_test = train_test_split(X, y, test_size=0.2)

# Train the XGBoost model
model = xgb.XGBRegressor()
model.fit(X_train, y_train)

# Save the trained model
joblib.dump(model, r'{modelSavePath}')
y_pred = model.predict(X_test)

# Evaluate the model's performance
mae = mean_absolute_error(y_test, y_pred)
r2 = r2_score(y_test, y_pred)

print(f'Mean Absolute Error: {{mae}}')
print(f'R^2 Score: {{r2}}')
";
                    PythonEngine.Exec(pythonCode);

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred while training the model: {ex.Message}");
                }

            }
        }

        public static void TrainTimeseriesModel()
        {
            using (Py.GIL())
            {
                var csvFileName = "Timeseries_dataset.csv";
                string csvFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", csvFileName);
                var modelSavePath = Path.Combine(Directory.GetCurrentDirectory(), "Models", "timeseries_model.joblib");
                var scalerSavePath = Path.Combine(Directory.GetCurrentDirectory(), "Models", "scaler.joblib");
                try
                {

                    string pythonCode = $@"
import pandas as pd
import numpy as np
from sklearn.preprocessing import MinMaxScaler
from keras.models import Sequential
from keras.layers import LSTM, Dense, Dropout
from keras.callbacks import EarlyStopping
from sklearn.metrics import mean_absolute_error, r2_score  # Importing metrics
import joblib

# Load the dataset
data = pd.read_csv(r'{csvFilePath}')

# Preprocess the data
# Assuming the target variable is in a column named 'target'
target_column = 'Label'
data[target_column] = pd.to_numeric(data[target_column], errors='coerce')
data = data.dropna(subset=[target_column])  # Remove rows with NaN in target column

# Scale the data
scaler = MinMaxScaler(feature_range=(0, 1))
scaled_data = scaler.fit_transform(data)

# Save the scaler
scaler_path = r'{scalerSavePath}'
joblib.dump(scaler, scaler_path)

# Create sequences for LSTM
def create_sequences(data, seq_length):
    X, y = [], []
    for i in range(len(data) - seq_length):
        X.append(data[i:i + seq_length])
        y.append(data[i + seq_length, -1])  # Target is the last column
    return np.array(X), np.array(y)

seq_length = 10  # Adjust this based on your dataset
X, y = create_sequences(scaled_data, seq_length)

# Split the data into training and testing sets
split = int(0.8 * len(X))
X_train, X_test = X[:split], X[split:]
y_train, y_test = y[:split], y[split:]

# Build the LSTM model
model = Sequential()
model.add(LSTM(50, return_sequences=True, input_shape=(X_train.shape[1], X_train.shape[2])))
model.add(Dropout(0.2))
model.add(LSTM(50, return_sequences=False))
model.add(Dropout(0.2))
model.add(Dense(25))
model.add(Dense(1))

# Compile the model
model.compile(optimizer='adam', loss='mean_absolute_error')

# Fit the model with early stopping
early_stopping = EarlyStopping(monitor='loss', patience=10)
model.fit(X_train, y_train, batch_size=128, epochs=5, callbacks=[early_stopping])

# Make predictions on the test set
print(X_test)
print(X_test.shape)
y_pred = model.predict(X_test)

mae = mean_absolute_error(y_test, y_pred)
r2 = r2_score(y_test, y_pred)

print(f'Mean Absolute Error: {{mae}}')
print(f'R^2 Score: {{r2}}')

# Save the trained model
joblib.dump(model, r'{modelSavePath}')
";
                    PythonEngine.Exec(pythonCode);

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred while training the model: {ex.Message}");
                }

            }
        }

    }
}
