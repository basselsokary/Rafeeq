from fastapi import FastAPI, UploadFile, File
from tensorflow.keras.models import load_model
from PIL import Image
import numpy as np 
import io
import uvicorn
import os
from fastapi.middleware.cors import CORSMiddleware

app = FastAPI()

model = load_model("D:\\I will prepare my paper to be the best in this world\\Gp\\Rafeeq\\ml-engine\\model\\model.h5")
print("✅ Model loaded successfully!")

classes = ['Amenhotep III and Queen Tiye',
            'Amun and Mut',
            'Anubis Shrine',
            'Bust of the Roman Emperor Augustus',
            'Bust of the Roman Emperor Hadrian',
            'Bust of the god Serapis',
            'God Ptah, King Ramesses II and Goddess Sekhmet',
            'Goddess Isis',
            'Gold Funerary Mask of King Psusennes I',
            'Golden Throne of King Tutankhamun',
            'Guardian Statues of Tutankhamun',
            'Hanging Obelisk of Ramesses II',
            'Head of Queen Hatshepsut',
            'Head of the Pharaoh Akhenaten',
            'Innermost Coffin of King Tutankhamun',
            'Khafre Enthroned',
            'King Djoser',
            'King Khufu Solar Boat',
            'King Ptolemy XII Auletes',
            'King Ramesses II',
            'King Ramesses III between the gods Horus and Seth',
            'Kneeling Statue of Queen Hatshepsut',
            'Mask of Tutankhamun',
            'Menkaure Triad',
            'Meryre and Iniuia',
            'Middle Coffin of King Tutankhamun',
            'Mosaic of Queen Berenice II',
            'Narmer Palette',
            'Other',
            'Outer Coffin of King Tutankhamun',
            'Pharaoh Akhenaten',
            'Prince Rahotep and his wife Nofret',
            'Queen Hatshepsut obelisk',
            'Ramesses II as a child protected by the god Hauron',
            'Roman Emperor Marcus Aurelius',
            'Roman Emperor Septimius Severus',
            'Seated Scribe',
            'Silver coffin of King Psusennes I',
            'Sphinx of Queen Hatshepsut',
            'Sphinx of Ramses II and Merenptah',
            'The sacred Apis Bull']

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
        "confidence": confidence
    }

app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"], 
    allow_methods=["*"],
    allow_headers=["*"],
)

if __name__ == "__main__":
    uvicorn.run(app, port=8000, host='0.0.0.0')