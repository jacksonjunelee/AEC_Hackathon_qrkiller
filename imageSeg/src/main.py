import cv2
import numpy as np 
from flask import Flask, request, jsonify

from convImgToBin import convImgToBin


app = Flask(__name__)