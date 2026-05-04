from fastapi import FastAPI, UploadFile, File
from tensorflow.keras.models import load_model
from tensorflow.keras.applications.mobilenet_v2 import MobileNetV2, preprocess_input as mobilenet_preprocess
from tensorflow.keras.preprocessing import image as keras_image
from PIL import Image
import numpy as np 
import io
import uvicorn
import pickle
import time
import os

from fastapi.middleware.cors import CORSMiddleware

app = FastAPI()

BASE_DIR = os.path.dirname(os.path.abspath(__file__))
model_path = os.path.join(BASE_DIR, "..", "model", "model_From_Scratch.h5")
model = load_model(model_path)

feature_extractor = MobileNetV2(weights='imagenet', include_top=False, pooling='avg')

classes = ['Amenhotep III and Queen Tiye','Amun and Mut','Anubis Shrine',
            'Bust of the Roman Emperor Augustus','Bust of the Roman Emperor Hadrian','Bust of the god Serapis',
            'God Ptah, King Ramesses II and Goddess Sekhmet','Goddess Isis','Gold Funerary Mask of King Psusennes I',
            'Golden Throne of King Tutankhamun','Guardian Statues of Tutankhamun','Hanging Obelisk of Ramesses II',
            'Head of Queen Hatshepsut','Head of the Pharaoh Akhenaten','Innermost Coffin of King Tutankhamun',
            'Khafre Enthroned','King Djoser','King Khufu Solar Boat',
            'King Ptolemy XII Auletes','King Ramesses II','King Ramesses III between the gods Horus and Seth',
            'Kneeling Statue of Queen Hatshepsut','Mask of Tutankhamun','Menkaure Triad',
            'Meryre and Iniuia','Middle Coffin of King Tutankhamun','Mosaic of Queen Berenice II',
            'Narmer Palette','Other','Outer Coffin of King Tutankhamun',
            'Pharaoh Akhenaten','Prince Rahotep and his wife Nofret','Queen Hatshepsut obelisk',
            'Ramesses II as a child protected by the god Hauron','Roman Emperor Marcus Aurelius','Roman Emperor Septimius Severus',
            'Seated Scribe','Silver coffin of King Psusennes I','Sphinx of Queen Hatshepsut',
            'Sphinx of Ramses II and Merenptah','The sacred Apis Bull']

pkl_path = os.path.join(BASE_DIR, "reference_embeddings.pkl")
try:
    with open(pkl_path, "rb") as f:
        reference_embeddings = pickle.load(f)
except FileNotFoundError:
    reference_embeddings = {}

def prepare_image(image):
    prepared_image = Image.open(io.BytesIO(image)).convert('RGB')
    prepared_image = prepared_image.resize((256, 256))
    prepared_image = np.array(prepared_image) / 255.0 
    
    mean = np.array([0.485, 0.456, 0.406])
    std = np.array([0.229, 0.224, 0.225])
    
    prepared_image = (prepared_image - mean) / std
    
    return np.expand_dims(prepared_image, axis=0)

def extract_features(image_bytes):
    img = Image.open(io.BytesIO(image_bytes)).convert('RGB')
    img = img.resize((256, 256)) 
    x = keras_image.img_to_array(img)
    x = np.expand_dims(x, axis=0)
    x = mobilenet_preprocess(x)
    features = feature_extractor.predict(x, verbose=0)
    return features.flatten()

def cosine_similarity(vec1, vec2):
    if vec1 is None or vec2 is None: return 0.0
    return np.dot(vec1, vec2) / (np.linalg.norm(vec1) * np.linalg.norm(vec2))
    
@app.post("/predict")
async def predict(file: UploadFile = File(...)):
    start = time.perf_counter() 
    
    data = await file.read()

    prepared_image = prepare_image(data)
    prediction = model.predict(prepared_image, verbose=0)
    predicted_index = np.argmax(prediction, axis=1)[0]
    
    label_name = classes[predicted_index]
    confidence = float(np.max(prediction) * 100)
    
    response_data = {
        "label": "Unknown",
    }
    
    if confidence > 90 and label_name != 'Other':
        uploaded_features = extract_features(data)
        ref_features = reference_embeddings.get(label_name)
        
        if ref_features is not None:
            similarity_score = float(cosine_similarity(uploaded_features, ref_features) * 100)
            
            if similarity_score > 60.0:
                response_data = {
                    "label": label_name,                    
                }
            else:
                response_data = {
                    "label": "Unknown",
                }

    end = time.perf_counter()
    print(f"Total request time: {(end - start)*1000:.2f} ms")

    return response_data
    
app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"], 
    allow_methods=["*"],
    allow_headers=["*"],
)

if __name__ == "__main__":
    uvicorn.run(app, port=8000, host='0.0.0.0')