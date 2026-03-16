from fastapi import FastAPI
import uvicorn

app = FastAPI()

@app.get("/predict")
def predict():
    # Placeholder for prediction logic
    return {"prediction": "This is a mock prediction."}

if __name__ == "__main__":
    uvicorn.run(app,port=8000,host="0.0.0.0")