import numpy as np
from os import listdir
from os.path import isfile, join
import random

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

def prepare_data(root_folder, labels_dictionary , width,height ):
    images,labels = get_all_categories_shuffled(root_folder)
    length = len(images)
    for i in range(0, length ):
        labels[i] = labels_dictionary[labels[i]]
    labels=np.array(labels)
    images = np.array(images).reshape(length,width,height,1)
    images=images/255

    return (images,labels)

