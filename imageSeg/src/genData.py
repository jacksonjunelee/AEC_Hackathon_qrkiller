import cv2
import numpy as np
import os
import json
from tensorflow.keras.applications import VGG16

# Load VGG16 model
vgg = VGG16(weights='imagenet', include_top=False, input_shape=(224, 224, 3))

# Function to generate embeddings from an image
def get_embeddings(image):
    resized_image = cv2.resize(image, (224, 224))
    normalized_image = resized_image / 255.0  # Normalize pixel values to [0, 1]
    expanded_image = np.expand_dims(normalized_image, axis=0)  # Add batch dimension
    embedding = vgg.predict(expanded_image)
    return embedding.flatten()

# Generate embeddings for all images in the train folder and store in a dictionary
train_path = "./imageSeg/data/train"
train_imgs = os.listdir(train_path)

embeddings_data = []  # Store all embeddings with their metadata

for index, img_name in enumerate(train_imgs):
    img_path = os.path.join(train_path, img_name)
    image = cv2.imread(img_path)

    # Skip if image is not found or unreadable
    if image is None:
        print(f"Error loading image: {img_name}")
        continue

    # Get the embedding for the current image
    embedding = get_embeddings(image)

    # Add data to the list as a dictionary
    embeddings_data.append({
        "data": index+1,  # Placeholder for future metadata, currently using the index
        "embedding": embedding.tolist()  # Convert NumPy array to list for JSON serialization
    })

# Save the embeddings data to a JSON file
with open("image_embeddings.json", "w") as json_file:
    json.dump(embeddings_data, json_file)

print("Embeddings have been successfully saved to 'image_embeddings.json'")
