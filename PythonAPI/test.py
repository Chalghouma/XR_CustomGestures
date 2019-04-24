import numpy as np
import sys
from os import listdir
from os.path import isfile, join
import random
from neural import NeuralNet

def numpy_array_from_file(file_name):
    array = []
    bytes_read = open(file_name, "rb").read()
    count =-1
    row=[]
    for b in bytes_read:
        count = count +1
        if(((count % 56) == 0 )  and (count != 0 )):
            array.append(row)
            row = []
        row.append(b)
    array.append(row)
    return np.array(array)


def get_files(folderPath):
    onlyfiles = [f for f in listdir(folderPath) if isfile(join(folderPath, f))]
    result_list = []
    for file in onlyfiles:
        result_list .append(join(folderPath,file))
    return result_list

def get_all_categories_shuffled(root_folder):
    l_folder = join(root_folder, "L")
    circle_folder =join(root_folder,"Circle")
    arrow_folder =join(root_folder,"RightArrow")

    all_files = []
    for f in get_files(l_folder):
        all_files.append(f)
    for f in get_files(circle_folder):
        all_files.append(f)
    for f in get_files(arrow_folder):
        all_files.append(f)
    
    random.shuffle(all_files)
    all_bytes=[]
    for ff in all_files:
        all_bytes.append(numpy_array_from_file(ff))
    return (np.array(all_bytes) , [ fName.split("\\")[-2]  for fName in all_files ])

labels_dictionary = {
    'Circle':0,'L':1,'RightArrow':2
}
labels_map = ['Circle','L','RightArrow']

#Replace with your root folder that holds the dataset of images
all_bytes ,all_labels = get_all_categories_shuffled("<<Your RootFolder>>")
print(all_labels)
data_length = len(all_labels)
for i in range(0, data_length ):
    all_labels[i] = labels_dictionary[all_labels[i]]
all_labels=np.array(all_labels)

print(all_labels)

width = 56 
height = 56
all_bytes=all_bytes.reshape(data_length,width,height,1)
print(all_labels.shape)
print(all_bytes.shape)
all_bytes = all_bytes/255

nn = NeuralNet(width,height)
nn.fit_data(training_data= all_bytes, training_labels= all_labels, epochs=10, accuracy= 0.999)
nn.save_model('easter_egg_ahlabikyafraise_')
#nn.load_model('easter_egg_ahlabikyafraise_')


image_path= sys.argv[1]
test_image = numpy_array_from_file(image_path)
test_image = test_image/255

arg_max , prediction_level =  nn.predict_element(test_image)
print('arg_max : {} which is {} with prediction : {}'.format(arg_max,labels_map[arg_max] , prediction_level))
