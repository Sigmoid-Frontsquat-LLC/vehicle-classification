# import pandas as pd 
import numpy as np 
import sys
import os
import logging
import tensorflow as tf 
from PIL import Image, ImageEnhance


# output problems
logging.disable(logging.WARNING)
os.environ['TF_CPP_MIN_LOG_LEVEL'] = '3'


class_labels = ['coupe','motorcycle','sedan','suv','truck']
num_classes = 5
source = ''

# function for parsing the system argument....
def parse_source(flag,value):
    if flag[1] == '-s':
        return (True,value)
    else:
        return (False,None)


if len(sys.argv == 1) or (len(sys.argv) - 1) % 2:
    raise ValueError("Usage: [-s image]")
else:
    flag = sys.argv[1]
    value = sys.argv[2]
    isSource, src = parse_source(flag,value)
    if isSource:
        source = src


############## IMAGE PREPROCESSING ##################
img = Image.open(source)
width, height = img.size
# img = img.resize((256,256))
enhancer = ImageEnhance.Sharpness(img)
enhanced_img = enhancer.enhance(10.0)

# save to look what happened..
enhanced_img.save('enhanced_resized.jpg')
img_array = np.asarray(img)
img_array = img_array / 255
img_array = img_array.reshape((1,256,256,3))


############## LOADING THE MODEL ##################
# model128_path = '/dnn/model128_final.hdf5'
# model256_path = '/dnn/model256_final.hdf5'
# weights_128_path = '/dnn/weights_model128.hdf5'
# weights_256_path = '/dnn/weights_model256.hdf5'

model_path = '/dnn/model{}_final.hdf5'.format(width)
weight_path = '/dnn/weights_model{}.hdf5'.format(width)

# 
model = tf.keras.models.load_model(model_path)
model.load_weights(weights_path)
model.compile(loss='categorical_crossentropy',optimizer='adam')


############## CLASSIFICATION ##################
pred = model.predict(img_array)
# for some reason the prediction returns a 2D array with relevant info in position 0 
pred = pred[0]
predicted_class = class_labels[np.argmax(pred)]




############## OUTPUT ##################
classification = [
    {
        class_labels[0] : pred[0]
    },
    {
        class_labels[1] : pred[1]
    },
    {
        class_labels[2] : pred[2]
    },
    {
        class_labels[3] : pred[3]
    },
    {
        class_labels[4] : pred[4]
    }
]
print(classification)

