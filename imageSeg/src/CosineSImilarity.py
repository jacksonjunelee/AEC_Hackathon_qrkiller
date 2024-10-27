from tensorflow.keras.applications import VGG16
from sklearn.metrics.pairwise import cosine_similarity
import numpy as np
import cv2

vgg = VGG16(weights='imagenet', include_top=False, input_shape=(224, 224, 3))

def get_embeddings(image):
    resized_image = cv2.resize(image, (224, 224))
    normalized_image = resized_image / 255.0
    expanded_image = np.expand_dims(normalized_image, axis=0)
    embedding = vgg.predict(expanded_image)
    return embedding.flatten()

db = cv2.imread("./imageSeg/testImgs/dbEdge.jpg")
test = cv2.imread("./imageSeg/testImgs/testEdge.jpg")
control = cv2.imread("./imageSeg/testImgs/controlEdge.jpg")

db_embedding = get_embeddings(db)
test_embedding = get_embeddings(test)
control_embedding = get_embeddings(control)

similarity = cosine_similarity([db_embedding], [control_embedding])[0][0]
print(f"Similarity: {similarity * 100:.2f}%")