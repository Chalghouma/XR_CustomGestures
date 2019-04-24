import tensorflow as tf
import numpy as np 

class myCallback(tf.keras.callbacks.Callback):
  def set_accuracy(self,acc):
    self.limit=acc
  def on_epoch_end(self, epoch, logs={}):
    if(logs.get('acc')>self.limit):
      print("\nReached {} accuracy so cancelling training!".format(self.limit))
      self.model.stop_training = True

class NeuralNet():
  model=None
  width=0
  height=0

  def __init__(self,width,height):
    self.width=width
    self.height=height

  def build_layers(self):
    self.model = tf.keras.models.Sequential([
        tf.keras.layers.Conv2D(32, (3,3), activation='relu', input_shape=(self.width,self.height, 1)),
        tf.keras.layers.MaxPooling2D(2, 2),
        tf.keras.layers.Flatten(),
        tf.keras.layers.Dense(128, activation='relu'),
        tf.keras.layers.Dense(10, activation='softmax')
        ])
    self.model.compile(optimizer='adam', loss='sparse_categorical_crossentropy', metrics=['accuracy'])
        
        
  def save_model(self,path):
    self.model.save(path+'.modelconfig')

  def load_model(self,path):
    self.build_layers()
    self.model = tf.keras.models.load_model(path)

  def fit_data(self,training_data,training_labels,epochs,accuracy):
    cb = myCallback()
    cb.set_accuracy(accuracy)
    self.model.fit(training_data,training_labels,epochs=epochs,callbacks= [cb])

  def predict_element(self,element):
    predictions = self.model.predict(np.array([element]).reshape(1,self.width,self.height,1) )
    pred = predictions[0]
    return ( np.argmax(pred)    ,  np.max(pred)  )
    
