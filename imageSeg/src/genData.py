import cv2
import numpy as np
import os
import json
from tensorflow.keras.applications import VGG16
from getImg import cleanImg

# Load VGG16 model
vgg = VGG16(weights='imagenet', include_top=False, input_shape=(512, 512, 3))

# Function to generate embeddings from an image
def get_embeddings(image):
    resized_image = cv2.resize(image, (512, 512))
    normalized_image = resized_image / 255.0  # Normalize pixel values to [0, 1]
    expanded_image = np.expand_dims(normalized_image, axis=0)  # Add batch dimension
    embedding = vgg.predict(expanded_image)
    return embedding.flatten()

# Generate embeddings for all images in the train folder and store in a dictionary
train_path = "./imageSeg/data/train"
train_imgs = os.listdir(train_path)

embeddings_data = [] 
guids = [
    'ee908473-e374-4c88-837e-420c4bb0374b',
    '78c7ac50-dd8a-4151-871b-a599cbb649d0',
    '29c825e1-1a62-4ded-b087-fa860cfc5743',
    'b909a979-10ef-45d0-9739-ff2f51d09954',
    '311dbdad-6baf-417d-a43c-01fdee38f39e',
    '71805e4d-1579-4fce-80bc-dba31affdd41'
] 

for index, img_name in enumerate(train_imgs):
    img_path = os.path.join(train_path, img_name)
    image = cv2.imread(img_path)

    
    if image is None:
        print(f"Error loading image: {img_name}")
        continue
    image, edge = cleanImg(image)
    # Get the embedding for the current image
    embedding = get_embeddings(image)

    # Add data to the list as a dictionary
    embeddings_data.append({
        "data": guids[index],  # Placeholder for future metadata, currently using the index
        "embedding": embedding.tolist()  # Convert NumPy array to list for JSON serialization
    })

# Save the embeddings data to a JSON file
with open("image_embeddings.json", "w") as json_file:
    json.dump(embeddings_data, json_file)

print("Embeddings have been successfully saved to 'image_embeddings.json'")
