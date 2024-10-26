import cv2
import numpy as np

border_colour = (255, 255, 255)

def display(img, name):
    cv2.imshow(name, img)
    cv2.waitKey(0)
    cv2.destroyAllWindows()

def cleanImg(img):
    gray = cv2.cvtColor(img, cv2.COLOR_BGR2GRAY)
    _, binary = cv2.threshold(gray, 150, 255, cv2.THRESH_BINARY)
    contours, _  = cv2.findContours(binary, cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE)
    contour = max(contours, key=cv2.contourArea)
    x, y, w, h = cv2.boundingRect(contour)
    cropped = img[y:y+h, x:x+w]
    return cropped

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
    img = cv2.imread("./imageSeg/testImgs/borderTest.jpg")
    display(img, "original")
    display(cropImg(img), "cropped")