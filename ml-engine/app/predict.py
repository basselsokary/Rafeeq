from fastapi import FastAPI, UploadFile, File
from tensorflow.keras.models import load_model
from PIL import Image
import numpy as np 
import io
import uvicorn
import os
from fastapi.middleware.cors import CORSMiddleware

app = FastAPI()

model = load_model("../model/model.h5")
print("✅ Model loaded successfully!")

classes = [
    "Akhenaten",
    "AmenhotepIII",
    "Bent pyramid for senefru",
    "Colossoi of Memnon",
    "Goddess Isis",
    "Hatshepsut face",
    "Khafre Pyramid",
    "King Thutmose III",
    "Mask of Tutankhamun",
    "Nefertiti",
    "Pyramid_of_Djoser",
    "Ramesses II",
    "Ramessum",
    "Statue of King Zoser",
    "Statue of Tutankhamun with Ankhese", 
    "Temple_of_Hatshepsut",
    "Temple_of_Isis_in_Philae",
    "Temple_of_Kom_Ombo",
    "The Great Temple of Ramesses II",
    "menkaure pyramid",
    "sphinx"
]

def prepare_image(image):
    prepared_image = Image.open(io.BytesIO(image)).convert('RGB')
    prepared_image = prepared_image.resize((256, 256))
    prepared_image = np.array(prepared_image) / 255.0 
    
    mean = np.array([0.485, 0.456, 0.406])
    std = np.array([0.229, 0.224, 0.225])
    
    prepared_image = (prepared_image - mean) / std
    
    return np.expand_dims(prepared_image, axis=0)
    
@app.post("/predict")
async def predict(file: UploadFile = File(...)):
    data = await file.read()
    prepared_image = prepare_image(data)  
    
    prediction = model.predict(prepared_image)
    predicted_index = np.argmax(prediction, axis=1)[0]
    
    label_name = classes[predicted_index]
    
    confidence = float(np.max(prediction) * 100)
    
    return {
        "predicted_class": int(predicted_index),
        "label": label_name,
        "confidence": f"{confidence:.2f}%"
    }

app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"], 
    allow_methods=["*"],
    allow_headers=["*"],
)

if __name__ == "__main__":
    uvicorn.run(app, port=8000, host='0.0.0.0')