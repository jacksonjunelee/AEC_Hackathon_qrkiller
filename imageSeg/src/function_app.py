import azure.functions as func
import logging
import json
import numpy as np
import cv2
from tensorflow.keras.applications.vgg16 import VGG16
from sklearn.metrics.pairwise import cosine_similarity
from getImg import cleanImg

app = func.FunctionApp(http_auth_level=func.AuthLevel.FUNCTION)
# Load the VGG16 model once when the function is initialized
vgg = VGG16(weights='imagenet', include_top=False, input_shape=(512, 512, 3))


def get_embeddings(image):
    resized_image = cv2.resize(image, (512, 512))
    normalized_image = resized_image / 255.0  # Normalize pixel values to [0, 1]
    expanded_image = np.expand_dims(normalized_image, axis=0)  # Add batch dimension
    embedding = vgg.predict(expanded_image)
    return embedding.flatten()

def find_closest_match(input_image):
    input_embedding = get_embeddings(input_image)

    best_match_data = None
    highest_similarity = -1

    for entry in stored_embeddings:
        stored_embedding = np.array(entry["embedding"])
        similarity = cosine_similarity([input_embedding], [stored_embedding])[0][0]
        logging.info(f"Similarity with data {entry['data']}: {similarity:.2f}")

        if similarity > highest_similarity:
            highest_similarity = similarity
            best_match_data = entry["data"]

    return best_match_data, highest_similarity

@app.route(route="test")
def test(req: func.HttpRequest) -> func.HttpResponse:
    return func.HttpResponse(f"You connected:" , status_code=200)

@app.route(route="http_trigger")
def http_trigger(req: func.HttpRequest) -> func.HttpResponse:
    logging.info('Python HTTP trigger function processed a request.')
    

    try:
        # Get the image from the request
        image_file = req.files.get('image')
        if image_file is None:
            return func.HttpResponse("No image file provided.", status_code=400)

        # Read the image
        input_image = cv2.imdecode(np.frombuffer(image_file.read(), np.uint8), cv2.IMREAD_COLOR)
        if input_image is None:
            return func.HttpResponse("Error loading input image.", status_code=400)

        input_image, _ = cleanImg(input_image)  # Clean the image if needed

        # Find the closest match
        match_data, similarity_score = find_closest_match(input_image)

        return func.HttpResponse(
            json.dumps({"match_data": match_data, "similarity_score": similarity_score}),
            mimetype="application/json",
            status_code=200
        )

    except Exception as e:
        logging.error(f"An error occurred: {str(e)}")
        return func.HttpResponse(f"An error occurred: {str(e)}", status_code=500)