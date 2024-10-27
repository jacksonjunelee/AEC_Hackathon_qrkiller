from tensorflow.keras.applications.vgg16 import VGG16
from sklearn.metrics.pairwise import cosine_similarity
import numpy as np
import json
import cv2
from getImg import cleanImg

vgg = VGG16(weights='imagenet', include_top=False, input_shape=(512, 512, 3))

def get_embeddings(image):
    resized_image = cv2.resize(image, (512, 512))
    normalized_image = resized_image / 255.0  # Normalize pixel values to [0, 1]
    expanded_image = np.expand_dims(normalized_image, axis=0)  # Add batch dimension
    embedding = vgg.predict(expanded_image)
    return embedding.flatten()

# Load the embeddings from the JSON file
with open("image_embeddings.json", "r") as json_file:
    stored_embeddings = json.load(json_file)

def find_closest_match(input_image_path):
    input_image = cv2.imread(input_image_path)
    if input_image is None:
        raise ValueError(f"Error loading input image: {input_image_path}")

    input_image, edge = cleanImg(input_image)
    input_embedding = get_embeddings(input_image)

    best_match_data = None
    highest_similarity = -1

    for entry in stored_embeddings:
        stored_embedding = np.array(entry["embedding"])
        similarity = cosine_similarity([input_embedding], [stored_embedding])[0][0]

        print(f"Similarity with data {entry['data']}: {similarity:.2f}")
        if similarity > highest_similarity:
            highest_similarity = similarity
            best_match_data = entry["data"]

    return best_match_data, highest_similarity

# Example usage: Compare an input image with stored embeddings
input_image_path = "./imageSeg/data/test/test.jpg"
match_data, similarity_score = find_closest_match(input_image_path)

print(f"Best match data: {match_data} with similarity score: {similarity_score:.2f}")
