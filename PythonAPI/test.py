import numpy as np
import sys
from neural import NeuralNet
from dataset import get_all_categories_shuffled,numpy_array_from_file,prepare_data

labels_dictionary = {
    'Circle':0,'L':1,'RightArrow':2
}
labels_map = ['Circle','L','RightArrow']

#Replace with your root folder that holds the dataset of images
all_bytes ,all_labels = prepare_data("Datasets/DatasetSample0/Images",labels_dictionary,56,56)

nn = NeuralNet(56,56)
nn.build_layers()
nn.fit_data(training_data= all_bytes, training_labels= all_labels, epochs=10, accuracy= 0.999)
nn.save_model('easter_egg_ahlabikyafraise_')
#nn.load_model('easter_egg_ahlabikyafraise_')


image_path= sys.argv[1]
test_image = numpy_array_from_file(image_path)
test_image = test_image/255

arg_max , prediction_level =  nn.predict_element(test_image)
print('arg_max : {} which is {} with prediction : {}'.format(arg_max,labels_map[arg_max] , prediction_level))
