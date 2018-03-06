'''
@version: 1.0
@author: royran
@contact: iranpeng@gmail.com
@file: data_preprocessing.py
@time: 2018/3/3 20:19
'''

import tensorflow as tf
import cv2
import numpy as np
from game_config import GameConfig

class DataPrerocessing(object):
    def __init__(self, gray_scale=False):
        with tf.name_scope("ImagePreporcess"):
            self.input_x = tf.placeholder(dtype=tf.int32, shape=(None, GameConfig.image_height * GameConfig.image_width * 3))
            self.x_reshape = tf.reshape(self.input_x, shape=(-1, GameConfig.image_height, GameConfig.image_width, 3))
            self.x_reshape = tf.cast(self.x_reshape, tf.uint8)
            self.resized_x = tf.image.resize_bilinear(self.x_reshape, (GameConfig.target_size, GameConfig.target_size))
            self.x_gray = tf.image.rgb_to_grayscale(self.resized_x)
            if gray_scale:
                self.normalized_x = tf.div(self.x_gray, 255.0)
            else:
                self.normalized_x = tf.div(self.resized_x, 255.0)

    def run(self, session, data):
        return session.run(self.normalized_x, feed_dict={self.input_x: [data]})[0]

    def resize_and_to_gray(self, session, data):
        return session.run(self.x_gray, feed_dict={self.input_x: [data]})[0]

    def resize_and_normalize(self, session, data):
        return session.run(self.normalized_x, feed_dict={self.input_x: [data]})[0]

    def resize(self, session, data):
        return session.run(self.resized_x, feed_dict={self.input_x: [data]})[0]

    def reshape(self, session, data):
        return session.run(self.x_reshape, feed_dict={self.input_x: [data]})[0]

    def resize_and_threshold(self, session, data):
        image = self.reshape(session, data)
        gray_image = cv2.cvtColor(image, cv2.COLOR_RGB2GRAY)
        _, im_th = cv2.threshold(gray_image, 1, 255, cv2.THRESH_BINARY)
        gray = cv2.resize(im_th, (GameConfig.target_size, GameConfig.target_size), interpolation=cv2.INTER_AREA)
        return gray
