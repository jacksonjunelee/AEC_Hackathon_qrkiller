import cv2
import numpy as np
import matplotlib.pyplot as plt

border_colour = (255, 255, 255)

def display_images_grid(images, titles, rows, cols):
    plt.figure(figsize=(15, 10))
    for i, (img, title) in enumerate(zip(images, titles)):
        plt.subplot(rows, cols, i+1)
        if len(img.shape) == 2:
            plt.imshow(img, cmap='gray')
        else:
            plt.imshow(cv2.cvtColor(img, cv2.COLOR_BGR2RGB))
        plt.title(title)
        plt.axis('off')
    plt.tight_layout()
    plt.show()


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

    images = [img, gray, blurred, edges, aligned_image]
    titles = ["Original", "Gray", "Blurred", "Edges", "Aligned"]

    display_images_grid(images, titles, 2, 3)
    return aligned_image

def display(img, name):
    cv2.imshow(name, img)
    cv2.waitKey(0)
    cv2.destroyAllWindows()

def cropImg(img):
    gray = cv2.cvtColor(img, cv2.COLOR_BGR2GRAY)
    mask = np.all(img == border_colour, axis=2)
    mask = np.logical_not(mask)

    coords = np.argwhere(mask)
    x0, y0 = coords.min(axis=0)
    x1, y1 = coords.max(axis=0) + 1

    cropped = img[x0:x1, y0:y1]
    return cropped

if __name__ == "__main__":
    img = cv2.imread("./imageSeg/testImgs/phoneTest.jpg")
    orig = cv2.imread("./imageSeg/testImgs/borderTest.jpg")
    pre = cleanImg(orig)
    cv2.imwrite("./imageSeg/testImgs/borderTestCleaned.jpg", pre)
    final = cleanImg(img)
    cv2.imwrite("./imageSeg/testImgs/phoneTestCleaned.jpg", final)