import viktor as vkt
import requests
from pathlib import Path


class Parametrization(vkt.Parametrization):
    btn = vkt.SetParamsButton('execute this', 'endpoint1')
    match_data = vkt.HiddenField('yesyes')


class Controller(vkt.Controller):
    parametrization = Parametrization

    def endpoint1(self, params, **kwargs):


        try:

            print('running endpoint 1')

            save_path = Path(__file__).parent / 'downloaded_image.jpg'

            if params.get('file_url'):
                # The local path where the image will be saved

                # Download the image
                response = requests.get(params.get('file_url'))

                # Check if the request was successful
                if response.status_code == 200:
                    with open(save_path, 'wb') as file:
                        file.write(response.content)


            print('here1')

            def cleanImg(img):

                gray = cv2.cvtColor(img, cv2.COLOR_BGR2GRAY)
                blurred = cv2.GaussianBlur(gray, (5, 5), 0)
                edges = cv2.Canny(blurred, 50, 150)
                contours, _ = cv2.findContours(edges, cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE)
                contour = max(contours, key=cv2.contourArea)
                epsilon = 0.02 * cv2.arcLength(contour, True)
                approx = cv2.approxPolyDP(contour, epsilon, True)

                aligned_image = None
                if (len(approx) == 4):
                    points = approx.reshape(4,2)

                    def sort_points(pts):
                        sorted_pts = pts[np.argsort(pts[:, 1])]
                        top_pts = sorted_pts[:2][np.argsort(sorted_pts[:2,0])]
                        bottom_pts = sorted_pts[2:][np.argsort(sorted_pts[2:,0])]
                        return np.array([top_pts[0], top_pts[1], bottom_pts[1], bottom_pts[0]])
                
                    sorted_points = sort_points(points)

                    width = max(np.linalg.norm(sorted_points[2] - sorted_points[3]), np.linalg.norm(sorted_points[1] - sorted_points[0]))
                    height = max(np.linalg.norm(sorted_points[1] - sorted_points[2]), np.linalg.norm(sorted_points[0] - sorted_points[3]))
                    dst_points = np.array([[0, 0], [width, 0], [width, height], [0, height]], dtype="float32")

                    M = cv2.getPerspectiveTransform(sorted_points.astype('float32'), dst_points)
                    aligned_image = cv2.warpPerspective(img, M, (int(width), int(height)))

                    M = cv2.getPerspectiveTransform(sorted_points.astype('float32'), dst_points)
                    aligned_image = cv2.warpPerspective(img, M, (int(width), int(height)))

                return aligned_image, edges


            print('here2')


            from tensorflow.keras.applications.vgg16 import VGG16
            from sklearn.metrics.pairwise import cosine_similarity
            import numpy as np
            import json
            import cv2

            vgg = VGG16(weights='imagenet', include_top=False, input_shape=(512, 512, 3))

            def get_embeddings(image):
                print('image.shape', image.shape)
                resized_image = cv2.resize(image, (512, 512))
                normalized_image = resized_image / 255.0  # Normalize pixel values to [0, 1]
                expanded_image = np.expand_dims(normalized_image, axis=0)  # Add batch dimension
                embedding = vgg.predict(expanded_image)
                return embedding.flatten()

            # Load the embeddings from the JSON file
            embedding_path = Path(__file__).parent / "image_embeddings.json"
            with open(embedding_path, "r") as json_file:
                stored_embeddings = json.load(json_file)

            print('here2')


            def find_closest_match(input_image_path):
                print('input_image_path', input_image_path)
                
                import os

                print('os.listdir', os.listdir())

                input_image = cv2.imread(input_image_path)
                if input_image is None:
                    raise ValueError(f"Error loading input image: {input_image_path}")

                print('shape1', input_image.shape)

                input_image, edge = cleanImg(input_image)

                print('shape2', input_image.shape)

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

            print('here3')

            # Example usage: Compare an input image with stored embeddings
            match_data, similarity_score = find_closest_match(save_path)

            print(f"Best match data: {match_data} with similarity score: {similarity_score:.2f}")

        except Exception as e:

            match_data = str(e)

        return vkt.SetParamsResult({'match_data': match_data})
