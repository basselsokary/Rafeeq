import os
import pickle
import numpy as np
from tensorflow.keras.applications.mobilenet_v2 import MobileNetV2, preprocess_input
from tensorflow.keras.preprocessing import image as keras_image

print("Loading MobileNetV2 model...")
feature_extractor = MobileNetV2(weights='imagenet', include_top=False, pooling='avg')

DATASET_PATH = "C:\\Users\\Abdulla\\OneDrive - Faculty of Computers and Information\\Desktop\\Data" 

embeddings_dict = {}

print("Starting feature extraction...")

for class_name in os.listdir(DATASET_PATH):
    class_dir = os.path.join(DATASET_PATH, class_name)
    
    if not os.path.isdir(class_dir):
        continue
        
    class_vectors = []
    
    for img_name in os.listdir(class_dir):
        img_path = os.path.join(class_dir, img_name)
        try:
            img = keras_image.load_img(img_path, target_size=(224, 224))
            x = keras_image.img_to_array(img)
            x = np.expand_dims(x, axis=0)
            x = preprocess_input(x)
            
            features = feature_extractor.predict(x, verbose=0).flatten()
            class_vectors.append(features)
        except Exception as e:
            print(f"⚠️ Error loading image {img_path}: {e}")
            
    if class_vectors:
        class_mean_vector = np.mean(class_vectors, axis=0)
        embeddings_dict[class_name] = class_mean_vector
        print(f"✅ Processed '{class_name}' ({len(class_vectors)} images)")
    else:
        print(f"⚠️ Warning: No valid images found in '{class_name}'")

OUTPUT_FILE = "reference_embeddings.pkl"
with open(OUTPUT_FILE, "wb") as f:
    pickle.dump(embeddings_dict, f)

print(f"\n🎉 Done! Embeddings saved successfully to '{OUTPUT_FILE}'.")
print(f"Total classes processed: {len(embeddings_dict)}")