import PIL as pil
from PIL import Image

def convImgToBin(imgPath):
    img = Image.open(imgPath)
    img = img.convert('1')
    
    binary_data = ""
    width, height = img.size

    print("starting converstion")
    for y in range(height):
        for x in range(width):
            if img.getpixel((x, y)) == 0:
                binary_data += "1"
            else:
                binary_data += "0"
    
    return binary_data

if __name__ == "__main__":
    imgPath = "./testImgs/test1.jpg"
    binary_data = convImgToBin(imgPath)
    print(binary_data)

